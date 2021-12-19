using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ArticleService.Commons;
using ArticleService.Controllers.CategoryHelper;
using ArticleService.Data.Dtos;
using ArticleService.Data.Models;
using ArticleService.Data.Repositories.Interfaces;

using AutoMapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ArticleService.Controllers
{
    [Route("api/v1/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _repository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private int _perPage;

        public CategoryController(ICategoryRepository repository, IMapper mapper, IConfiguration config)
        {
            _repository = repository;
            _mapper = mapper;
            _config = config;
            _perPage = Convert.ToInt32(_config.GetSection("PaginationSettings:defaultPage").Value);
        }

        [HttpGet("get-all-categories")]
        [Authorize(Roles = "SuperAdmin, Admin, Editor")]
        public async Task<IActionResult> GetAllCategories([FromQuery] int page, int perPage)
        {
            page = page <= 0 ? 1 : page;
            _perPage = perPage > _perPage ? perPage : _perPage;

            IEnumerable<Category> categories = await _repository.GetAllCategories();

            if (categories.Count() < 0 || categories == null)
            {
                ModelState.AddModelError("Not found", "There are no categories");
                return NotFound(Utilities.CreateResponse<string>("Not found", ModelState, ""));
            }

            IEnumerable<CategoryToReturnDto> categoriesToReturn = CategoryToReturnObjectMapper.MapCategories(categories);

            //add MetaData pagination
            var pageMetaData = Utilities.Paginate(page, _perPage, categories.Count());

            var result = new PaginatedResultDto<CategoryToReturnDto>
            {
                PageMetaData = pageMetaData,
                ResponseData = categoriesToReturn
            };

            return Ok(Utilities.CreateResponse("Success", null, result));
        }

        [HttpGet("{categoryId}")]
        [Authorize(Roles ="SuperAdmin, Admin, Editor")]
        public async Task<IActionResult> GetCategoryById(string categoryId)
        {
            if (string.IsNullOrWhiteSpace(categoryId))
            {
                ModelState.AddModelError("Id", "No Id was provided");
                return BadRequest(Utilities.CreateResponse<string>("No Id", ModelState, ""));
            }

            Category category = await _repository.GetCategoryByIdAsync(categoryId);

            if(category == null)
            {
                ModelState.AddModelError("Not found", "The category was not found");
                return NotFound(Utilities.CreateResponse<string>("Not found", ModelState, ""));
            }

            CategoryToReturnDto categoryToReturn = _mapper.Map<CategoryToReturnDto>(category);
            return Ok(Utilities.CreateResponse<CategoryToReturnDto>("Success", null, categoryToReturn));
        }

        [HttpPost()]
        [Authorize(Roles ="SuperAdmin, Admin, Editor")]
        public async Task<IActionResult> AddCategory([FromBody] CategoryToAddDto model)
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

            var entity = _mapper.Map<Category>(model);
            var created = await _repository.AddCategoryAsync(entity);

            if (!created)
            {
                ModelState.AddModelError("Createion error", "An error occurred while creating the category");
                return BadRequest(Utilities.CreateResponse<string>("Creation error", ModelState, ""));
            }

            CategoryToReturnDto categoryToReturn = _mapper.Map<CategoryToReturnDto>(entity);
            return Ok(Utilities.CreateResponse<CategoryToReturnDto>("Category created successfully", ModelState, categoryToReturn));
        }

        [HttpPut("{categoryId}")]
        [Authorize(Roles = "SuperAdmin, Admin, Editor")]
        public async Task<IActionResult> EditCategory([FromBody] CategoryToUpdateDto model, string categoryId)
        {
            if (string.IsNullOrWhiteSpace(categoryId))
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

            Category entity = await _repository.GetCategoryByIdAsync(categoryId);

            if (entity == null)
            {
                ModelState.AddModelError("Not found", "The category was not found");
                return NotFound(Utilities.CreateResponse<string>("Not found", ModelState, ""));
            }

            entity = _mapper.Map(model, entity);
            var updated = await _repository.UpdateCategoryAsync(entity);

            if (!updated)
            {
                ModelState.AddModelError("Update error", "An error occurred while updating the category");
                return BadRequest(Utilities.CreateResponse<string>("update error", ModelState, ""));
            }

            return Ok(Utilities.CreateResponse<string>("Category was updated successfully", null, ""));
        }

        [HttpDelete("{categoryId}")]
        [Authorize(Roles = "SuperAdmin, Admin, Editor")]
        public async Task<IActionResult> DeleteCategory(string categoryId)
        {
            if (string.IsNullOrWhiteSpace(categoryId))
            {
                ModelState.AddModelError("Id", "Category Id was no provided");
                return BadRequest(Utilities.CreateResponse<string>("Id", ModelState, ""));
            }

            var entity = await _repository.GetCategoryByIdAsync(categoryId);
            if(entity == null)
            {
                ModelState.AddModelError("Not found", "The category was not found");
                return NotFound(Utilities.CreateResponse<string>("Category not found", ModelState, ""));
            }

            var deleted = await _repository.DeleteCategoryAsync(entity);
            if (!deleted)
            {
                ModelState.AddModelError("Deletion error", "There was an error deleting the category");
                return BadRequest(Utilities.CreateResponse<string>("Deletion Error", ModelState, ""));
            }

            return Ok(Utilities.CreateResponse<string>("Successfully deleted", null, ""));
        }
    }
}
