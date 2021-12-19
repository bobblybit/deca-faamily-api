using AccountManagement.Commons;
using AccountManagement.Controllers.Helper;
using AccountManagement.Data.Dtos;
using AccountManagement.Data.Models;
using AccountManagement.Data.Repositories.Interfaces;
using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MqDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace AccountManagement.Controllers
{
    [Route("api/v1/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IPublishEndpoint _publishendpoint;
        private readonly ICompanyRepository _companyRepository;
        private readonly int _perPage;
        private readonly string _clientUrl;

        public UserController(UserManager<AppUser> userManager, IMapper mapper,
            RoleManager<IdentityRole> roleManager, IConfiguration config, IPublishEndpoint publishendpoint, ICompanyRepository companyRepository)
        {
            _userManager = userManager;
            _mapper = mapper;
            _roleManager = roleManager;
            _publishendpoint = publishendpoint;
            _perPage = Convert.ToInt32(config.GetSection("PaginationSettings:perPage").Value);
            _clientUrl = config.GetSection("Client:ClientURI").Value;
            _companyRepository = companyRepository;
        }

        /// <summary>
        /// Provides and endpoint for registering a user
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] UserToAddDto model)
        {

            
            //Return badrequest with errors if the model is not valid
            if (!ModelState.IsValid)
            {
                return BadRequest(Utilities.CreateResponse<string>("Validation Errors", ModelState, ""));
            }

            //Check if a user with same email already exists in the database
            var userExist = await _userManager.FindByEmailAsync(model.Email);

            //return badrequest with email exists error if a user with same email already exists
            if (userExist != null)
            {
                ModelState.AddModelError("Email", "User with this email already exists");
                return BadRequest(Utilities.CreateResponse(message: "Email exists", ModelState, data: ""));
            }

            var entity = _mapper.Map<AppUser>(model);
            entity.UserName = model.Email;

            //create a new user in the database
            var createdResult = await _userManager.CreateAsync(entity, model.Password);

            //Return a badrequest with errors if creating the user was not successful
            if (!createdResult.Succeeded)
            {
                foreach (var error in createdResult.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return BadRequest(Utilities.CreateResponse<string>("", ModelState, ""));
            }

            //create role if the user role does not exist
            if(!await _roleManager.RoleExistsAsync("Regular"))
            {
                await _roleManager.CreateAsync(new IdentityRole { Name = "Regular" });
            }

            //assign "Regular" role the created user
            await _userManager.AddToRoleAsync(entity, "Regular");

            /*
             *  Broadcast messges and events here.
             *  Send email
             *  publish event to authentication service
             */

            //Generate email token and send confirmation token to user
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(entity);

            //get the base address
            //get the email link address
            var queryParams = new Dictionary<string, string>
            {
                ["Email"] = model.Email,
                ["Token"] = token
            };

            var link = QueryHelpers.AddQueryString(_clientUrl + "emailconfirmation", queryParams);
            //return Ok if everything goes fine.
            var newUsertoSend = new NewUserMqDto
            {
                FirstName = model.FirstName, LastName = model.LastName, Email = model.Email, Link=link, BaseUrl = _clientUrl
            };

            await _publishendpoint.Publish<NewUserMqDto>(newUsertoSend);
            return Ok(Utilities.CreateResponse<object>("Registration successful", null, new { Id = entity.Id }));
        }

        /// <summary>
        /// Provides end endpoint for fetching a registered user
        /// </summary>
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUser(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                ModelState.AddModelError("Id", "User id is not provided");
                return BadRequest(Utilities.CreateResponse("No id supplied", ModelState, ""));
            }

            //fetch the registered user from the database
            var returneduser = await _userManager.Users.Include(a => a.Address)
                                               .Include(d => d.Department)
                                               .ThenInclude(c => c.Company)
                                               .Include(m => m.MySquad)
                                               .Include(s => s.MyStack)
                                               .Include(p => p.Photos)
                                               .Include(h => h.SocialHandles)
                                               .FirstOrDefaultAsync(x => x.Id == userId);

            if (returneduser != null)
            {
                //get the role of the user
                var userRoles = await _userManager.GetRolesAsync(returneduser);

                var userToReturn = Helper.Mapper.MapUser(returneduser);
                userToReturn.Roles = userRoles.ToList();

                /*
                 *  Broadcast messges and events here.
                 *  Publish request to client
                 */

                //return ok upon successful mapping of user model
                return Ok(Utilities.CreateResponse("User successfully retrieved", null, userToReturn));
            }

            ModelState.AddModelError("User", "User was not found!");
            return NotFound(Utilities.CreateResponse("Not found", ModelState, ""));
        }

        /*activate user route */
       [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPatch("{userId}")]
        public async Task<IActionResult> ActivateUser([FromRoute] string userId)
        {
            if(String.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError("ID", "User Id cannot be empty");
                return BadRequest(Utilities.CreateResponse(message: "User id is empty", ModelState, data: ""));
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ModelState.AddModelError("User", "User does not exist");
                return BadRequest(Utilities.CreateResponse(message: "User does not exist", ModelState, data: ""));
            }

            if (user.IsActive)
            {
                ModelState.AddModelError("User", "User is already active");
                return BadRequest(Utilities.CreateResponse(message: "User is already active", ModelState, data: ""));

            }

            user.IsActive = true;
            await _userManager.UpdateAsync(user);

            return Ok(Utilities.CreateResponse<object>(message:"User successfully activated", null, data:""));
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            var confirmEmailModel = new ConfirmEmailDto { Email = email, Token = token };

            if (!ModelState.IsValid)
            {
                return BadRequest(Utilities.CreateResponse<object>(message: "Model Error", ModelState, null));
            }

            var user = await _userManager.FindByEmailAsync(confirmEmailModel.Email);

            if (user == null)
            {
                ModelState.AddModelError("Invalid Email", "Email does not exist");
                return BadRequest(Utilities.CreateResponse<object>(message: "Email does not exist", ModelState, data: ""));
            }

            var emailConfirmed = await _userManager.ConfirmEmailAsync(user, confirmEmailModel.Token);

            if (!emailConfirmed.Succeeded)
            {
                foreach (var err in emailConfirmed.Errors)
                {
                    ModelState.AddModelError(err.Code, err.Description);
                }
                return BadRequest(Utilities.CreateResponse<string>("Token", ModelState, null));
            }

            return Ok(Utilities.CreateResponse<object>(message: "Email confirmation was successful", null, data: ""));

        }


        /// <summary>
        /// provides an endpoint for deactivating a user
        /// </summary>
        /// <param name="id">represents the id of the user to be deactivated</param>
        [HttpPatch("deactivate-user/{userId}")]
        [Authorize(Roles ="Admin, Super Admin")]
        public async Task<ActionResult> DeactivateUser(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                ModelState.AddModelError("Id", "User Id was not provided");
                return BadRequest(Utilities.CreateResponse<string>("No Id", ModelState, ""));
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ModelState.AddModelError("User", "User not found");
                return NotFound(Utilities.CreateResponse("Not found", ModelState, ""));
            }

            user.IsActive = false;
            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                ModelState.AddModelError("Update error", "An error occured while deactivating the user");
                return BadRequest(Utilities.CreateResponse<string>("Update error", ModelState, ""));
            }

            return Ok(Utilities.CreateResponse<object>("User deactivated successfully", null, ""));
        }


        /// <summary>
        /// Provides end endpoint for fetching all registered users
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet("get-all-users")]
        public async Task<IActionResult> GetAllUsers([FromQuery] int page)
        {
            page = page <= 0 ? 1 : page;

            //fetch the registered users from the database in batches based on the _perPage settings
            var allUsers = _userManager.Users.Skip((page - 1) * _perPage).Take(_perPage);
            var usersToReturn = new List<UsersToReturnDto>();

            if (allUsers != null || allUsers.ToList().Count != 0)
            {
                foreach (var user in allUsers)
                {
                    var userToReturn = Helper.Mapper.MapUsers(user);

                    //add each user to the list of users to be returned
                    usersToReturn.Add(userToReturn);
                }

                //get the page metaData for the results fetched
                var pageMetaData = Utilities.Paginate(page, _perPage, _userManager.Users.Count());

                var result = new PaginatedResultDto<UsersToReturnDto>
                {
                    PageMetaData = pageMetaData,
                    ResponseData = usersToReturn
                };

                /*
                 *  Broadcast messges and events here.
                 *  Publish request to client
                 */

                //return OK upon successful mapping of user model
                return Ok(Utilities.CreateResponse("Users successfully retrieved", null, result));
            }

            ModelState.AddModelError("No users found", "There are no users in the database");
            return NotFound(Utilities.CreateResponse("Not found", ModelState, ""));
        }

        [HttpPut("{userId}")]
        //[Authorize(Roles = "Regular")]
        public async Task<ActionResult> EditUser([FromBody] UserToUpdateDto model, string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError("Id", "User id was not provided");
                return BadRequest(Utilities.CreateResponse("No id supplied", ModelState, ""));
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(Utilities.CreateResponse("Validation errors", ModelState, ""));
            }

            AppUser user = await _userManager.Users
                            .Include(x=>x.Address)
                            .Include(d => d.Department)
                            .Include(p => p.Photos)
                            .Include(h => h.SocialHandles)
                            .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
            {
                ModelState.AddModelError("User", "User does not exist");
                return BadRequest(Utilities.CreateResponse<string>("User does not exist", ModelState, ""));
            }

            AppUser entity = _mapper.Map(model, user);
            if(!string.IsNullOrWhiteSpace(model.Department?.CompanyName) &&
                !string.IsNullOrWhiteSpace(model.Department?.Name) &&
                !string.IsNullOrWhiteSpace(model.Department?.CompanyName))
            {
                Company company = company = await _companyRepository.FindCompanyByDepartmentUserId(userId);

                company = company == null ? new Company() : company;
                if(company.Departments.Count == 0)
                {
                    var department = new Department { Name = model.Department.Name, Position = model.Department.Position };
                    company.Departments.Add(department);
                }
                else
                {
                    company.Departments.ElementAt(0).Name = model.Department.Name;
                    company.Departments.ElementAt(0).Position = model.Department.Position;
                }
                company.Name = model.Department.CompanyName;
                entity.Department = company.Departments.ElementAt(0);
            }
            IdentityResult updateResult = await _userManager.UpdateAsync(entity);

            if (!updateResult.Succeeded)
            {
                foreach(var error in updateResult.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }

                return BadRequest(Utilities.CreateResponse<string>("Update errors", ModelState, ""));
            }

            return Ok(Utilities.CreateResponse<string>("User updated successfully", null, ""));
        }

        /// <summary>
        /// Provides end endpoint for fetching a registered user by email
        /// </summary>
        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError("Email", "User email is not provided");
                return BadRequest(Utilities.CreateResponse("No email is supplied", ModelState, ""));
            }
            //fetch the registered user from the database
            var returneduser = await _userManager.Users.Include(a => a.Address)
                                               .Include(d => d.Department)
                                               .ThenInclude(c => c.Company)
                                               .Include(m => m.MySquad)
                                               .Include(s => s.MyStack)
                                               .Include(p => p.Photos)
                                               .Include(h => h.SocialHandles)
                                               .FirstOrDefaultAsync(x => x.Email == email);
            if (returneduser != null)
            {
                //get the role of the user
                var userRoles = await _userManager.GetRolesAsync(returneduser);
                var userToReturn = Helper.Mapper.MapUser(returneduser);
                userToReturn.Roles = userRoles.ToList();
                /*
                 *  Broadcast messges and events here.
                 *  Publish request to client
                 */
                //return ok upon successful mapping of user model
                return Ok(Utilities.CreateResponse("User successfully retrieved", null, userToReturn));
            }
            ModelState.AddModelError("User", "User was not found!");
            return NotFound(Utilities.CreateResponse("Not found", ModelState, ""));
        }

    }
}