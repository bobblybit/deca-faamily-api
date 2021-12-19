using AccountManagement.Commons;
using AccountManagement.Controllers.Helper;
using AccountManagement.Data.Dtos;
using AccountManagement.Data.Models;
using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MqDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountManagement.Controllers
{
    [Route("api/v1/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _rolemanager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly int _perPage;
        private readonly IPublishEndpoint _publishEndpoint;
        public RoleController(RoleManager<IdentityRole> rolemanager, UserManager<AppUser> userManager, IMapper mapper, IConfiguration config, IPublishEndpoint publishEndpoint)
        {
            _rolemanager = rolemanager;
            _userManager = userManager;
            _mapper = mapper;
            _perPage = Convert.ToInt32(config.GetSection("PaginationSettings:perPage").Value);
            _publishEndpoint = publishEndpoint;
        }

        [Authorize(Roles ="SuperAdmin, Admin")]
        [HttpGet("{roleName}")]
        public async Task<IActionResult> GetRoleByName([FromRoute] string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                ModelState.AddModelError("Role Error", "The role name cannot be empty");
                var errMsg = Utilities.CreateResponse(message: "The role name cannot be empty", errs: ModelState, data: "");
                return BadRequest(errMsg);
            }

            var roleEntity = await _rolemanager.FindByNameAsync(roleName);

            if (roleEntity == null)
             {
                ModelState.AddModelError("Not Found", "The requested role was not found");
                var errMsg = Utilities.CreateResponse(message: "The requested role was not found", errs: ModelState, data:"");
                return NotFound(errMsg);
            }

            RoleToReturnDto roleToReturn = Helper.Mapper.MapRole(roleEntity);

            return Ok(Utilities.CreateResponse(message:"Role was retrieved Successfully", errs:null, data:roleToReturn));
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllRolesPaginated([FromQuery] int page)
        {
            page = page <= 0 ? 1 : page;

            var allRoles =  _rolemanager.Roles.Skip((page - 1) * _perPage).Take(_perPage);
            var rolesToReturn = new List<RoleToReturnDto>();

            if (allRoles.ToList().Count == 0)
            {
                ModelState.AddModelError("Not found", "No role was found");
                return NotFound(Utilities.CreateResponse("No role was found", ModelState, ""));
            }

            foreach (var role in allRoles)
            {
                var roleToReturn = Helper.Mapper.MapRole(role);
                rolesToReturn.Add(roleToReturn);
            }

            var pageMetaData = Utilities.Paginate(page, _perPage, _rolemanager.Roles.Count());

            var result = new PaginatedResultDto<RoleToReturnDto>
            {
                PageMetaData = pageMetaData,
                ResponseData = rolesToReturn
            };

            return Ok(Utilities.CreateResponse(message: "Roles was successfully retrieved", errs:null, data: result));
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpPost()]
        public async Task<IActionResult> CreateRolesAsync([FromBody] AddRoleDto newRole)
        {
            if (newRole == null)
            {
                ModelState.AddModelError("Role Error", "The New Role cannot be empty");
                var errMsg = Utilities.CreateResponse(message: "The New Role cannot be empty", errs: ModelState, data: "");
                return BadRequest(errMsg);

            }

            var role = await _rolemanager.FindByNameAsync(newRole.RoleName);
            if (role != null)
            {
                ModelState.AddModelError("Role Error", "Role Already Exists");
                var errMsg = Utilities.CreateResponse(message: "Role Already Exists", errs: ModelState, data: "");
                return BadRequest(errMsg);
            }

            var response = await _rolemanager.CreateAsync(new IdentityRole { Name = newRole.RoleName });

            if (!response.Succeeded)
            {
                ModelState.AddModelError("Creation error", "Role could not be created ");
                var errMsg = Utilities.CreateResponse(message: "Role could not be created", errs: ModelState, data: "");
                return UnprocessableEntity(errMsg);
            }
            return Ok(Utilities.CreateResponse(message: "The new role was created successfully", errs: null, data: response));
        }


        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpDelete("{roleId}")]
        public async Task<IActionResult> DeleteRoleAsync([FromRoute] string roleId)
        {

            if (string.IsNullOrWhiteSpace(roleId))
            {
                ModelState.AddModelError("Delete Error", "the role ID is empty");
                var errmsg = Utilities.CreateResponse(message: "the role ID is empty", errs: ModelState, data: "");
                return BadRequest(errmsg);
            }

            var role = await _rolemanager.FindByIdAsync(roleId);

            if (role == null)
            {
                ModelState.AddModelError("Delete Error", "the role you attempted to delete does not exist");
                var errmsg = Utilities.CreateResponse(message: "the role you attempted to delete does not exist", errs: ModelState, data: "");
                return BadRequest(errmsg);
            }

            var response = await _rolemanager.DeleteAsync(role);

            if (!response.Succeeded)
            {
                ModelState.AddModelError("Delete Error", "deleting the role was not successful");
                var errmsg = Utilities.CreateResponse(message: "role was not successfully deleted", errs: ModelState, data: "");
            }

            return Ok(Utilities.CreateResponse(message: "Role was successfully deleted", errs: null, data: ""));
        }

        [Authorize(Roles = "SuperAdmin , Admin")]
        [HttpPost("add-user-role/{userId}")]
        public async Task<IActionResult> AddUserRoleAsync([FromBody]AddUserRoleDto newRole ,[FromRoute] string userId)
        {

            if (string.IsNullOrWhiteSpace(userId))
            {
                ModelState.AddModelError("Update Error", "the user ID cannot be empty");
                var errmsg = Utilities.CreateResponse(message: "the user ID cannot be empty", errs: ModelState, data: "");
                return BadRequest(errmsg);
            }


            if (newRole == null)
            {
                ModelState.AddModelError("Update Error", "the role cannot be empty");
                var errmsg = Utilities.CreateResponse(message: "the role cannot be empty", errs: ModelState, data: "");
                return BadRequest(errmsg);
            }


            //check role exist
            var role = await _rolemanager.FindByNameAsync(newRole.NewUserRole);
            if (role == null)
            {
                ModelState.AddModelError("User Role Update error", "the role you want to assign to user does not exist");
                var errmsg = Utilities.CreateResponse(message: "the role you want to assign to user does not exist", errs: ModelState, data: "");
                return BadRequest(errmsg);
            }


            //check if user exist
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ModelState.AddModelError("User Role Update Error", "The user does not exist");
                var errmsg = Utilities.CreateResponse(message: "The user does not exist", errs: ModelState, data: "");
                return BadRequest(errmsg);
            }

            // check if user has role already
            var userRole = await _userManager.GetRolesAsync(user);

            if (userRole.Contains(newRole.NewUserRole))
            {
                ModelState.AddModelError("User Role Update error", "The User Already has this role");
                var errmsg = Utilities.CreateResponse(message: "The User Already has this role", errs: ModelState, data: "");
                return BadRequest(errmsg);
            }

            //add new role to user
            var response =await  _userManager.AddToRoleAsync(user ,newRole.NewUserRole);
            if (!response.Succeeded)
            {
                ModelState.AddModelError("Update user role error", "Role was not added successfully");
                var errmsg = Utilities.CreateResponse(message: "Role was not added successfully", errs: ModelState, data: "");
            }


            var assignRoleData = new RoleNotificationDto {
                                                          Email = user.Email,
                                                          FirstName = user.FirstName,
                                                          LastName = user.LastName,
                                                          Role = newRole.NewUserRole ,
                                                          EmailTemplateName = "AssignRoleEmailTemplate.html"
            };

             await _publishEndpoint.Publish<RoleNotificationDto>(assignRoleData);

            return Ok(Utilities.CreateResponse(message:"Role was successfully assigned to user", errs:null, data:newRole));
        }

       [Authorize(Roles = "SuperAdmin , Admin")]
       [HttpDelete("remove-user-role/{userId}")]
        public async Task<IActionResult> RemoveUserRoleAsync( [FromRoute] string userId, [FromBody] RoleToRemoveDto roleToRemove)
        {

            if (string.IsNullOrWhiteSpace(userId))
            {
                ModelState.AddModelError("Update Error", "the user ID is empty");
                var errmsg = Utilities.CreateResponse(message: "the user ID is empty", errs: ModelState, data: "");
                return BadRequest(errmsg);
            }


            if (string.IsNullOrWhiteSpace(roleToRemove.RoleName))
            {
                ModelState.AddModelError("Update Error", "the role cannot be empty");
                var errmsg = Utilities.CreateResponse(message: "the role cannot be empty", errs: ModelState, data: "");
                return BadRequest(errmsg);
            }



            //check user exist
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ModelState.AddModelError("User Error", "The user does not exist");
                var errmsg = Utilities.CreateResponse(message: "The user does not exist", errs: ModelState, data: "");
                return BadRequest(errmsg);
            }

            // check if user has role
            var userRole = await _userManager.GetRolesAsync(user);

            if (!userRole.Contains(roleToRemove.RoleName))
            {
                ModelState.AddModelError("Role error", "The User does not have the role you want to remove");
                var errmsg = Utilities.CreateResponse(message: "The User does not have the role you want to remove", errs: ModelState, data: "");
                return BadRequest(errmsg);
            }

            var response = await _userManager.RemoveFromRoleAsync(user, roleToRemove.RoleName);
            if (!response.Succeeded)
            {
                ModelState.AddModelError("Role error", "Role was not successfully removed from user");
                var errmsg = Utilities.CreateResponse(message: "Role was not successfully removed from user", errs: ModelState, data: "");
                return BadRequest(errmsg);
            }

            var removeRoleData = new RoleNotificationDto { Email = user.Email,
                                                           FirstName = user.FirstName,
                                                           LastName = user.LastName,
                                                           Role = roleToRemove.RoleName,
                                                           EmailTemplateName = "RemoveRoleEmaIlTemplate.html" };

            await _publishEndpoint.Publish<RoleNotificationDto>(removeRoleData);

            return Ok(Utilities.CreateResponse(message: "Role was successfully removed from user", errs: null, data: roleToRemove.RoleName));
        }

    }
}
