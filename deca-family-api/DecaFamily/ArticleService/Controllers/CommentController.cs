using ArticleService.Commons;
using ArticleService.Data.Dtos;
using ArticleService.Data.Models;
using ArticleService.Data.Repositories.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ArticleService.Controllers
{
    [Route("api/v1/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public CommentController(ICommentRepository commentRepository, IUserRepository userRepository, IMapper mapper)
        {
            _commentRepository = commentRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpGet("{articleId}")]
        [Authorize]
        public async Task<IActionResult> GetCommentsByArticleId( [FromRoute]string articleId)
        {
            if (string.IsNullOrWhiteSpace(articleId))
            {
                ModelState.AddModelError("Id", "No Id was provided");
                return BadRequest(Utilities.CreateResponse<string>("No Id", ModelState, ""));
            }

            var comments = await _commentRepository.GetCommentByArticleId(articleId);

            if (comments == null || comments.Count() < 1)
            {
                ModelState.AddModelError("Comments", "No comment yet for this article");
                return NotFound(Utilities.CreateResponse(message: "comments not found", errs: ModelState, ""));
            }


            var response = _mapper.Map<IEnumerable<CommentResponseDto>>(comments);

            return Ok(Utilities.CreateResponse(message: "Comments retrieved successfully", errs: null, data: response));

        }

        [HttpPost("{articleId}")]
        [Authorize]
        public async Task<IActionResult> AddComment([FromBody] CommentToAddDto model)
        {
            if (model == null)
            {
                ModelState.AddModelError("Model error", "No model was provided");
                return BadRequest(Utilities.CreateResponse<string>("Model error", ModelState, ""));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(Utilities.CreateResponse<string>("Validation error", ModelState, ""));
            }


            var entity = _mapper.Map<Comment>(model);
            entity.UserId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            if (await _userRepository.GetUserByIdAsync(entity.UserId) == null)
            {
                var firstName = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value;
                var lastName = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname)?.Value;

                var userToAdd = new User
                {
                    Id = entity.UserId,
                    FirstName = firstName,
                    LastName = lastName
                };

                await _userRepository.AddUserAsync(userToAdd);
            }

            var created = await _commentRepository.AddCommentAsync(entity);

            if (!created)
            {
                ModelState.AddModelError("Creation error", "An error occurred while posting the comment");
                return BadRequest(Utilities.CreateResponse<string>("Creation error", ModelState, ""));
            }

            return Ok(Utilities.CreateResponse<Object>(message: "Comment posted successfully", null, data:new { CommentId= entity.Id}));

        }

        [HttpPatch("{commentId}")]
        [Authorize]
        public async Task<IActionResult> EditComment([FromBody] CommentToEditDto model, string commentId)
        {
            if (string.IsNullOrWhiteSpace(commentId))
            {
                ModelState.AddModelError("Id", "No Id was provided");
                return BadRequest(Utilities.CreateResponse<string>("No Id", ModelState, ""));
            }

            if (model == null)
            {
                ModelState.AddModelError("Model error", "No model was provided");
                return BadRequest(Utilities.CreateResponse<string>("Model error", ModelState, ""));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(Utilities.CreateResponse<string>("Validation error", ModelState, ""));
            }

            var entity = await _commentRepository.GetCommentByIdAsync(commentId);

            if(entity == null)
            {
                ModelState.AddModelError("Query error", "No comment was found for this id");
                return NotFound(Utilities.CreateResponse<string>("Comment not found", ModelState, ""));
            }

            if(entity.UserId != User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value)
            {
                ModelState.AddModelError("Model error", "You are not authorized to edit this comment");
                return Unauthorized(Utilities.CreateResponse<string>("Model error", ModelState, ""));
            }

            var getUserDetails = await _userRepository.GetUserByIdAsync(entity.UserId);

            var firstName = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value;
            var lastName = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname)?.Value;

            if (getUserDetails.FirstName != firstName || getUserDetails.LastName != lastName)
            {
                getUserDetails.FirstName = firstName ?? getUserDetails.FirstName;
                getUserDetails.LastName = lastName ?? getUserDetails.LastName;

                await _userRepository.UpdateUserAsync(getUserDetails);
            }

            entity.Content = model.Content;
            entity.UpdatedAt = DateTime.UtcNow;

            var created = await _commentRepository.UpdateCommentAsync(entity);

            if (!created)
            {
                ModelState.AddModelError("Update error", "An error occurred while updating the comment");
                return BadRequest(Utilities.CreateResponse<string>("Update error", ModelState, ""));
            }

            return Ok(Utilities.CreateResponse(message: "Comment Updated successfully", null, data: ""));

        }

        [HttpDelete("{commentId}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<IActionResult> DeletComment(string commentId)
        {
            if (string.IsNullOrWhiteSpace(commentId))
            {
                ModelState.AddModelError("Id", "No Id was provided");
                return BadRequest(Utilities.CreateResponse<string>("No Id", ModelState, ""));
            }


            var commentToDelete = await _commentRepository.GetCommentByIdAsync(commentId);

            if (commentToDelete == null)
            {
                ModelState.AddModelError("Id", "Id does not exist");
                return NotFound(Utilities.CreateResponse("No record found", ModelState, ""));
            }

            if (commentToDelete.UserId != User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value)
            {
                ModelState.AddModelError("Model error", "You are not authorized to delete this comment");
                return Unauthorized(Utilities.CreateResponse<string>("Model error", ModelState, ""));
            }

            var response = await _commentRepository.DeleteCommentAsync(commentToDelete);
            if (!response)
            {
                ModelState.AddModelError("Comment", "Could not delete comment");
                return BadRequest(Utilities.CreateResponse(message: "comment not deleted", errs: ModelState, data: ""));
            }

            return Ok(Utilities.CreateResponse(message: "Comment Deleted sucessfully", errs: null, data: ""));

        }
    }
}
