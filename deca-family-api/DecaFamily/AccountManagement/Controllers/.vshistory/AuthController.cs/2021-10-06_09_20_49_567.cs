using AccountManagement.Commons;
using AccountManagement.Data.Dtos;
using AccountManagement.Data.Models;
using AccountManagement.Security;
using AccountManagement.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AccountManagement.Controllers.Helper;
using MassTransit;
using MqDtos;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace AccountManagement.Controllers
{
    [Route("api/v1/[Controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IOptions<JWTData> _JWTData;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IConfiguration _config;

        public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IOptions<JWTData> jWTData, IPublishEndpoint publishEndpoint, IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _JWTData = jWTData;
            _publishEndpoint = publishEndpoint;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            
            if (!ModelState.IsValid)
            {
                var msg = Utilities.CreateResponse(
                                message: "Incomplete model", errs: ModelState, data: "");
                return BadRequest(msg);
            }

            var user = await _userManager.Users
                                .Include(x => x.Photos)
                                .FirstOrDefaultAsync(x => x.Email == model.Email);

            if (user == null)
            {
                ModelState.AddModelError("Email", "Invalid Credential");
                var errMsg = Utilities.CreateResponse(message: "Invalid Credentials", errs: ModelState, data: "");
                return BadRequest(errMsg);
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError("Email", "Email not confirmed yet");
                var errMsg = Utilities.CreateResponse(message: "Email not confirmed", errs: ModelState, data: "");
                return BadRequest(errMsg);
            }

            var checkPassword = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
            if (!checkPassword.Succeeded)
            {
                ModelState.AddModelError("Password", "Invalid Credential");
                var errMsg = Utilities.CreateResponse(message: "Invalid Credentials", errs: ModelState, data: "");
                return BadRequest(errMsg);
            }

            var userRoles = await _userManager.GetRolesAsync(user) as List<string>;
            var token = JWTService.GenerateToken(user, userRoles, _JWTData);

            LoginResponseDto res = new LoginResponseDto
            {
                Token = token,
                Role = string.Join(",", await _userManager.GetRolesAsync(user)),
                UserId = user.Id,
                Fullname = $"{user.FirstName} {user.LastName}",
                PhotoUrl = user.Photos.Where(x => x.IsMain == true).Select(x => x.PhotoUrl).FirstOrDefault()
            };

            return Ok(Utilities.CreateResponse("Login Successful", null, res));
        }

        [HttpPost("signin-google")]
        public async Task<IActionResult> ExternalLogin([FromBody]ExternalAuthDto externalAuth)
        {
            var payload = await JWTService.VerifyGoogleToken(externalAuth, _config);

            if (payload == null)
                return BadRequest("Invalid External Authentication.");

            var info = new UserLoginInfo(externalAuth.Provider, payload.Subject, externalAuth.Provider);

            var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            if (user == null)
            {
                user = await _userManager.Users
                        .Include(x => x.Photos)
                        .Where(x => x.Email == payload.Email)
                        .FirstOrDefaultAsync();
                if (user == null)
                {
                    user = new AppUser { Email = payload.Email, UserName = payload.Email, FirstName = payload.GivenName, LastName = payload.FamilyName };
                    await _userManager.CreateAsync(user);
                    //prepare and send an email for the email confirmation
                    await _userManager.AddToRoleAsync(user, "Regular");
                    var confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    await _userManager.ConfirmEmailAsync(user, confirmToken);
                    await _userManager.AddLoginAsync(user, info);
                }
                else
                {
                    await _userManager.AddLoginAsync(user, info);
                }
            }
            if (user == null)
                return BadRequest("Invalid External Authentication.");


            var userRoles = await _userManager.GetRolesAsync(user) as List<string>;
            var token = JWTService.GenerateToken(user, userRoles, _JWTData);

            var res = new LoginResponseDto
            {
                Token = token,
                Role = string.Join(",", await _userManager.GetRolesAsync(user)),
                UserId = user.Id,
                Fullname = $"{user.FirstName} {user.LastName}",
                PhotoUrl = user.Photos.Where(x => x.IsMain == true).Select(x => x.PhotoUrl).FirstOrDefault()
            };

            return Ok(Utilities.CreateResponse("Login Successful", null, res));
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                var responseObj = Utilities.CreateResponse<string>("Model errors", ModelState, "");
                return BadRequest(responseObj);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("Email", "Email does not exist");
                var responseObj = Utilities.CreateResponse<string>($"Invalid Email", ModelState, "");

                return BadRequest(responseObj);
            }

            //Get the password reset token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var queryParams = new Dictionary<string, string>()
            {
                ["email"] = user.Email,
                ["token"] = token
            };

            var baseUrl = UrlHelper.BaseAddress(HttpContext);

            if (string.IsNullOrEmpty(token))
            {
                ModelState.AddModelError("EmailService", "There was an error sending the password reset link. Please try again");
                return BadRequest(Utilities.CreateResponse<string>("Service not available", ModelState, ""));
            }

            var link = Url.Action("ResetPassword", queryParams);

            var resetPasswordMqDto = new ResetPasswordMqDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = model.Email,
                Link = link,
                BaseAddress = baseUrl
            };

            await _publishEndpoint.Publish<ResetPasswordMqDto>(resetPasswordMqDto);

            return Ok(Utilities.CreateResponse<string>($"Successfully sent forgot password mail", null, ""));

        }

        [HttpPatch("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                var responseObj = Utilities.CreateResponse<string>("Model errors", ModelState, null);
                return BadRequest(responseObj);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("Email", "Email does not exist");
                var responseObj = Utilities.CreateResponse<string>("Model errors", ModelState, null);
                return BadRequest(responseObj);
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                return Ok(Utilities.CreateResponse<string>("Password reset was successful", null, null));
            }

            foreach (var err in result.Errors)
            {
                ModelState.AddModelError("", err.Description);
            }
            return BadRequest(Utilities.CreateResponse<string>("Token", ModelState, null));
        }

        [Authorize]
        [HttpPatch("{userId}")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model, string userId)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (!currentUser.IsActive)
            {
                ModelState.AddModelError("Access denied", "In-active user");
                var responseObj = Utilities.CreateResponse("Access denied for in-active user", ModelState, "");
                return BadRequest(responseObj);
            }

            if (currentUser.Id != userId)
            {
                ModelState.AddModelError("Email", "Access is denied");
                return BadRequest(Utilities.CreateResponse("Access denied", ModelState, ""));
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ModelState.AddModelError("Id", "Id does not exist");
                return NotFound(Utilities.CreateResponse("Required user not found", ModelState, ""));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(Utilities.CreateResponse("Model state error", ModelState, ""));
            }

            if (model.OldPassword == model.NewPassword)
            {
                return BadRequest(Utilities.CreateResponse("Old and new password cannot be the same", ModelState, ""));
            }

            if (!await _userManager.CheckPasswordAsync(user, model.OldPassword))
            {
                var msg = Utilities.CreateResponse("Password does not match\nPlease enter your current password", null, "");
                return NotFound(msg);
            }
            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (result.Succeeded) return Ok(Utilities.CreateResponse("Password successfully changed", null, ""));
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
                var msg = Utilities.CreateResponse("Password does not match\nPlease enter your current password", ModelState, "");
                return BadRequest(msg);
            }

        }
    }


}
