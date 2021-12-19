using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ArticleService.Commons;
using ArticleService.Data.Dtos;
using ArticleService.Data.Models;
using ArticleService.Data.Repositories.Implementations;
using ArticleService.Data.Repositories.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ArticleService.Controllers
{
    [Route("api/v1/[controller]")]
    public class ArticleController : ControllerBase
    {
        private readonly IArticleRepository _articleRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly int _perPage;

        public ArticleController(IArticleRepository articleRepository, IMapper mapper, IConfiguration configuration,
                                 ICommentRepository commentRepository, IUserRepository userRepository)
        {
            _articleRepository = articleRepository;
            _commentRepository = commentRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _perPage = Convert.ToInt32(configuration.GetSection("PaginationSettings:PerPage").Value);
        }


        [HttpPost()]
        [Authorize(Roles = "Editor, Regular")]
        public async Task<IActionResult> AddArticle([FromBody] AddArticleDto article)
        {

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("Model error", "Model state not valid");
                return BadRequest(Utilities.CreateResponse<string>("Model error", ModelState, ""));
            }

            var entity = _mapper.Map<Article>(article);
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

            var newArticle = await _articleRepository.AddArticleAsync(entity);

            if (!newArticle)
            {
                ModelState.AddModelError("Failed", "Article was not created successfully");
                return BadRequest(Utilities.CreateResponse<string>("Failed to add", ModelState, ""));
            }

            AddArticleDto articleToReturn = _mapper.Map<AddArticleDto>(entity);
            return Created("", Utilities.CreateResponse("Article Was Created Successfully", ModelState, articleToReturn));
        }


        [HttpGet("{articleId}")]
        [Authorize]
        public async Task<IActionResult> GetArticle([FromRoute] string articleId)
        {
            if (string.IsNullOrEmpty(articleId))
            {
                ModelState.AddModelError("Null Id", "No Id Was provided");
                return BadRequest(Utilities.CreateResponse<string>("No Id Was provided", ModelState, ""));
            }


            var articleEntity = await _articleRepository.GetArticleByIdAsync(articleId);

            if (articleEntity == null)
            {
                ModelState.AddModelError("Not found", "The Requested Article was not found");
                return NotFound(Utilities.CreateResponse<string>("Not found", ModelState, ""));
            }

            ArticleToReturnDto articleToReturn = _mapper.Map<ArticleToReturnDto>(articleEntity);
            return Ok(Utilities.CreateResponse<ArticleToReturnDto>("Article Retrieved Successfully", ModelState, articleToReturn));
        }

        [HttpGet("get-articles-by-category")]
        [Authorize]
        public async Task<IActionResult> GetArticlesByCategory([FromQuery] int page, string categoryId)
        {
            page = page <= 0 ? 1 : page;

            var articles = await _articleRepository.GetArticlesByCategoryAsync(categoryId, _perPage, page);

            if (articles.Count() == 0)
            {
                ModelState.AddModelError("Article", "No Article was found for the Requested Category");
                return NotFound(Utilities.CreateResponse(message: "No Article was found for the Requested Category", errs: ModelState, ""));
            }
            var mappedArticles = _mapper.Map<ICollection<ArticleCardDto>>(articles);
            var response = AddPaginationToResult(mappedArticles, page);

            return Ok(Utilities.CreateResponse(message: "List of Artcles", errs: null, data: response));
        }

        [HttpPatch("{articleId}")]
        [Authorize(Roles ="Editor, Regular")]
        public async Task<IActionResult> UpdateArticle([FromBody] UpdateArticleDto model , [FromRoute]string articleId)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("Model error", "Model state not valid");
                return BadRequest(Utilities.CreateResponse<string>("Model error", ModelState, ""));
            }

            var articleToUpdate = await _articleRepository.GetArticleByIdAsync(articleId);

            if (articleToUpdate == null)
            {
                ModelState.AddModelError("Not found", "The Requested Article was not found");
                return NotFound(Utilities.CreateResponse<string>("Not found", ModelState, ""));
            }

            if (articleToUpdate.UserId != User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value)
            {
                ModelState.AddModelError("Unauthorized", "You are not authorized to update this article");
                return Unauthorized(Utilities.CreateResponse<string>("Unauthorized", ModelState, ""));
            }

            var getUserDetails = await _userRepository.GetUserByIdAsync(articleToUpdate.UserId);

            var firstName = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value;
            var lastName = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname)?.Value;

            if (getUserDetails.FirstName != firstName || getUserDetails.LastName != lastName)
            {
                getUserDetails.FirstName = firstName ?? getUserDetails.FirstName;
                getUserDetails.LastName = lastName ?? getUserDetails.LastName;

                await _userRepository.UpdateUserAsync(getUserDetails);
            }

            articleToUpdate.Title = model.Title;
            articleToUpdate.Content = model.Content;
            articleToUpdate.Stack = model.Stack;
            articleToUpdate.StackId = articleToUpdate.StackId;
            articleToUpdate.Tag = model.Tag;
            articleToUpdate.CategoryId = model.CategoryId;
            articleToUpdate.Approved = model.Approved;
            articleToUpdate.ApprovedBy = model.ApprovedBy;
            articleToUpdate.UpdatedAt = DateTime.UtcNow;

            var response = await _articleRepository.UpdateArticleAsync(articleToUpdate);

            if (!response)
            {
                ModelState.AddModelError("Update error", "there was an error updating the article");
                return BadRequest(Utilities.CreateResponse<string>("Update error", ModelState, ""));
            }

            UpdateArticleDto articleToReturn = _mapper.Map<UpdateArticleDto>(articleToUpdate);
            return Ok(Utilities.CreateResponse(message: "Article Updated successfully", null, data: articleToReturn));
        }


        [HttpDelete("{articleId}")]
        [Authorize]
        public async Task<IActionResult> DeleteArticleAsync([FromRoute] string articleId)
        {
            if (String.IsNullOrEmpty(articleId))
            {
                ModelState.AddModelError("Bad request", "No article id was provided");
                return BadRequest(Utilities.CreateResponse<string>("No article id", ModelState, ""));
            }

            var articleToDelete = await _articleRepository.GetArticleByIdAsync(articleId);

            if (articleToDelete == null)
            {
                ModelState.AddModelError("Bad request", "the article you attempted to delete does not exist");
                return BadRequest(Utilities.CreateResponse<string>("the article you attempted to delete does not exist", ModelState, ""));
            }


            var userRole = String.Empty;
            if (User.Identity is ClaimsIdentity identity)
            {
                userRole = identity.FindFirst(ClaimTypes.Role).Value;
            }

            if (userRole == "Regular" && articleToDelete.UserId != User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value)
            {
                ModelState.AddModelError("Unauthorized", "You are not authorized to update this article");
                return Unauthorized(Utilities.CreateResponse<string>("Unauthorized", ModelState, ""));
            }

            var respond = await _articleRepository.DeletedArticleAsync(articleToDelete);
            if (!respond)
            {
                ModelState.AddModelError("Delete Error", "the article you was not deleted due to some internal error");
                return BadRequest(Utilities.CreateResponse<string>("the article you was not deleted due to some internal error", ModelState, ""));
            }

            return Ok(Utilities.CreateResponse(message: "Article Deleted successfully", errs: null, data: ""));
        }

        [HttpPost("like-article/{articleId}")]
        [Authorize(Roles = "SuperAdmin, Admin, Editor, Regular")]
        public async Task<IActionResult> AddArticleLike(string articleId)
        {
            if (string.IsNullOrWhiteSpace(articleId))
            {
                ModelState.AddModelError("ArticleId", "No article id was provided");
                return BadRequest(Utilities.CreateResponse<string>("No article id", ModelState, ""));
            }

            string userId = string.Empty;
            string userName = string.Empty;

            if (User.Identity is ClaimsIdentity identity)
            {
                userId = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
                userName = identity.FindFirst(ClaimTypes.Name).Value;
            }

            if(_articleRepository.GetArticleLikeByArticleIdAndLikerAsync(articleId, userId) != null)
            {
                ModelState.AddModelError("Article", $"User already liked this article with id = {articleId}");
                return BadRequest(Utilities.CreateResponse("Article already liked", ModelState, string.Empty));
            }

            ArticleLike articleLike = new ArticleLike
            {
                ArticleId = articleId,
                Liker = userName,
                LikerId = userId
            };

            var addArticleLikeResponse = await _articleRepository.AddArticleLikeAsync(articleLike);

            if (!addArticleLikeResponse)
            {
                ModelState.AddModelError("ArticleLike", $"Could not successfully like the article with id {articleId}");
                return UnprocessableEntity(Utilities.CreateResponse("Liking article was not successful", ModelState, string.Empty));
            }

            return Created("", Utilities.CreateResponse($"Successfully added a like for article id = {articleId}", null, string.Empty));
        }

        [HttpPost("like-comment/{commentId}")]
        [Authorize(Roles = "SuperAdmin, Admin, Editor, Regular")]
        public async Task<IActionResult> AddCommentLike(string commentId)
        {
            if (string.IsNullOrWhiteSpace(commentId))
            {
                ModelState.AddModelError("CommentId", "No comment id was provided");
                return BadRequest(Utilities.CreateResponse("No comment id", ModelState, ""));
            }

            string userId = string.Empty;
            string userName = string.Empty;

            if (User.Identity is ClaimsIdentity identity)
            {
                userId = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
                userName = identity.FindFirst(ClaimTypes.Name).Value;
            }

            if (_articleRepository.GetCommentLikeByCommentIdAndLikerAsync(commentId, userId) != null)
            {
                ModelState.AddModelError("Comment", $"User already liked this comment with id = {commentId}");
                return BadRequest(Utilities.CreateResponse("Comment already liked", ModelState, string.Empty));
            }

            CommentLike commentLike = new CommentLike
            {
                CommentId = commentId,
                Liker = userName,
                LikerId = userId
            };

            var addCommentLikeResponse = await _articleRepository.AddCommentLikeAsync(commentLike);

            if (!addCommentLikeResponse)
            {
                ModelState.AddModelError("ArticleLike", $"Could not successfully like the comment with id {commentId}");
                return UnprocessableEntity(Utilities.CreateResponse("Liking comment was not successful", ModelState, string.Empty));
            }

            return Created("", Utilities.CreateResponse($"Successfully added a like for comment id = {commentId}", null, string.Empty));
        }


        private PagenatedResultDto<T> AddPaginationToResult<T>(IEnumerable<T> items, int page)
        {
            var pageMetaData = Utilities.Paginate(page, _perPage, _articleRepository.GetCount());
            var pagedItems = new PagenatedResultDto<T> { PageMetaData = pageMetaData, ResponseData = items };
            return pagedItems;
        }

        [HttpDelete("dislike-article/{articleId}")]
        [Authorize(Roles = "SuperAdmin, Admin, Editor, Regular")]
        public async Task<IActionResult> DislikeArticleLike(string articleId)
        {
            if (string.IsNullOrWhiteSpace(articleId))
            {
                ModelState.AddModelError("ArticleId", "No article id was provided");
                return BadRequest(Utilities.CreateResponse("No article id", ModelState, ""));
            }

            string userId = string.Empty;

            if (User.Identity is ClaimsIdentity identity)
            {
                userId = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
            }

            var getArticleLike = await _articleRepository.GetArticleLikeByArticleIdAndLikerAsync(articleId, userId);

            var deleteArticleLikeResponse = await _articleRepository.DeleteArticleLikeAsync(getArticleLike);

            if (!deleteArticleLikeResponse)
            {
                ModelState.AddModelError("ArticleLike", $"Could not successfully dislike the article with id {articleId}");
                return UnprocessableEntity(Utilities.CreateResponse("Disliking article was not successful", ModelState, string.Empty));
            }

            return Created("", Utilities.CreateResponse($"Successfully disliked article with id = {articleId}", null, string.Empty));
        }

        [HttpDelete("dislike-comment/{commentId}")]
        [Authorize(Roles = "SuperAdmin, Admin, Editor, Regular")]
        public async Task<IActionResult> DislikeCommentLike(string commentId)
        {
            if (string.IsNullOrWhiteSpace(commentId))
            {
                ModelState.AddModelError("CommentId", "No comment id was provided");
                return BadRequest(Utilities.CreateResponse("No comment id", ModelState, ""));
            }

            string userId = string.Empty;

            if (User.Identity is ClaimsIdentity identity)
            {
                userId = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
            }

            var getCommentLike = await _articleRepository.GetCommentLikeByCommentIdAndLikerAsync(commentId, userId);

            var deleteCommentLikeResponse = await _articleRepository.DeleteCommentLikeAsync(getCommentLike);

            if (!deleteCommentLikeResponse)
            {
                ModelState.AddModelError("ArticleLike", $"Could not successfully dislike the comment with id {commentId}");
                return UnprocessableEntity(Utilities.CreateResponse("Disliking comment was not successful", ModelState, string.Empty));
            }

            return Created("", Utilities.CreateResponse($"Successfully disliked comment with id = {commentId}", null, string.Empty));
        }

        [HttpGet("get-comment/{commentId}")]
        [Authorize(Roles = "SuperAdmin, Admin, Editor, Regular")]
        public async Task<IActionResult> GetCommentByUniqueId(string commentId)
        {
            if (string.IsNullOrWhiteSpace(commentId))
            {
                ModelState.AddModelError("CommentId", "No comment id was provided");
                return BadRequest(Utilities.CreateResponse("No comment id", ModelState, ""));
            }

            var fetchedComment = await _commentRepository.GetCommentByIdAsync(commentId);

            if (fetchedComment == null)
            {
                ModelState.AddModelError("Comment Error", "Could not fetch details about comment");
                return BadRequest(Utilities.CreateResponse("Comment could not load", ModelState, ""));
            }
            var commentResponse = _mapper.Map<CommentToReturnDto>(fetchedComment);
            return Ok(Utilities.CreateResponse("Successfully retrieved comment", errs: null, data: commentResponse));
        }

        [HttpPatch("approve/{articleId}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<IActionResult> ApproveArticle(string articleId)
        {
            var article = await _articleRepository.GetArticleByIdAsync(articleId);
            if (article == null)
            {
                ModelState.AddModelError("Article", "Article was not found");
                return BadRequest(Utilities.CreateResponse("Not found", ModelState, ""));
            }

            if(article.Approved == true)
            {
                ModelState.AddModelError("Article", "Article already approved");
                return BadRequest(Utilities.CreateResponse("Already approved", ModelState, ""));
            }

            var approverId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            var approver = await _userRepository.GetUserByIdAsync(approverId);

            if (approver == null)
            {
                var firstName = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value;
                var lastName = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname)?.Value;

                var userToAdd = new User
                {
                    Id = approverId,
                    FirstName = firstName,
                    LastName = lastName
                };

                await _userRepository.AddUserAsync(userToAdd);
            }            

            article.Approved = true;
            article.ApprovedBy = approverId;
            await _articleRepository.UpdateArticleAsync(article);

            return Ok(Utilities.CreateResponse<string>("Article successfully approved", null, ""));
        }

        [HttpPatch("disapprove/{articleId}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<IActionResult> DisapproveArticles(string articleId)
        {
            var article = await _articleRepository.GetArticleByIdAsync(articleId);
            if (article == null)
            {
                ModelState.AddModelError("Article", "Article was not found");
                return BadRequest(Utilities.CreateResponse("Not found", ModelState, ""));
            }

            if (article.Approved == false)
            {
                ModelState.AddModelError("Article", "Article has not been approved");
                return BadRequest(Utilities.CreateResponse("Article not approved", ModelState, ""));
            }

            article.Approved = false;
            article.ApprovedBy = null;
            await _articleRepository.UpdateArticleAsync(article);

            return Ok(Utilities.CreateResponse<string>("Article has been disapproved", null, ""));
        }

        [HttpGet("unapproved")]
        [Authorize(Roles = "SuperAdmin, Admin, Editor")]
        public async Task<IActionResult> GetUnapprovedArticle([FromQuery] int page)
        {
            page = page <= 0 ? 1 : page;

            var articles = await _articleRepository.GetUnapprovedArticles(_perPage, page);

            if (articles.Count() == 0)
            {
                ModelState.AddModelError("Article", "No Article was found for the Requested Category");
                return NotFound(Utilities.CreateResponse(message: "No Article was found for the Requested Category", errs: ModelState, ""));
            }
            var mappedArticles = _mapper.Map<ICollection<ArticleCardDto>>(articles);
            var response = AddPaginationToResult(mappedArticles, page);

            return Ok(Utilities.CreateResponse(message: "Artcles", errs: null, data: response));
        }

        [HttpGet("recent")]
        [Authorize]
        public async Task<IActionResult> GetRecentArticles([FromQuery] int page)
        {
            page = page <= 0 ? 1 : page;

            var articles = await _articleRepository.GetRecentArticlesAsync(_perPage, page);

            if (articles.Count() == 0)
            {
                ModelState.AddModelError("Articles", "No Articles");
                return NotFound(Utilities.CreateResponse(message: "Articles", errs: ModelState, ""));
            }
            var mappedArticles = _mapper.Map<ICollection<ArticleCardDto>>(articles);
            var response = AddPaginationToResult(mappedArticles, page);

            return Ok(Utilities.CreateResponse(message: "Artcles", errs: null, data: response));
        }

        [HttpGet("search")]
        [Authorize]
        public async Task<IActionResult> SearchArticles([FromQuery]string query, [FromQuery] int page)
        {
            page = page <= 0 ? 1 : page;

            var articles = await _articleRepository.SearchArticles(query, _perPage, page);

            if (articles.Count() == 0)
            {
                ModelState.AddModelError("Articles", "No Articles matched your search");
                return NotFound(Utilities.CreateResponse(message: "Not found", errs: ModelState, ""));
            }
            var mappedArticles = _mapper.Map<ICollection<ArticleCardDto>>(articles);
            var response = AddPaginationToResult(mappedArticles, page);

            return Ok(Utilities.CreateResponse(message: "Artcles", errs: null, data: response));
        }
    }
}