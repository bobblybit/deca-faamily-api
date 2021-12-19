using AccountManagement.Commons;
using AccountManagement.Data.Dtos;
using AccountManagement.Data.Models;
using AccountManagement.Data.Repositories.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AccountManagement.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize(Roles = "Admin, SuperAdmin")]
    public class StackController : ControllerBase
    {
        private readonly IStackRepository _stackRepository;
        private readonly IMapper _mapper;

        public StackController(IStackRepository stackRepository, IMapper mapper)
        {
            _stackRepository = stackRepository;
            _mapper = mapper;
        }

        [HttpGet("get-stack/{id}", Name = "GetStackById")]
        public async Task<IActionResult> GetStackById(string id)
        {
            var stack = await _stackRepository.GetStackByIdAsync(id);
            if (stack == null)
            {
                ModelState.AddModelError("Stack", "The stack was not found");
                return NotFound(Utilities.CreateResponse<string>("Not found", ModelState, null));
            }
            return Ok(Utilities.CreateResponse("Stack", null, _mapper.Map<StackToReturnDto>(stack)));
        }

        [HttpPost("add-stack")]
        public async Task<IActionResult> AddStack([FromBody] StackToAddDto stack)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(Utilities.CreateResponse("Validation error", ModelState, ""));
            }

            var entity = _mapper.Map<MyStack>(stack);
            if (await _stackRepository.AddStackAsync(entity))
            {
                return CreatedAtAction("GetStackById", new { id = entity.Id }, _mapper.Map<StackToReturnDto>(entity));
            }

            ModelState.AddModelError("Create error", "An error occured while createing the stack, please try again");
            return BadRequest(Utilities.CreateResponse<string>("error", ModelState, ""));
        }

        [HttpPut("update-stack/{id}")]
        public async Task<IActionResult> UpdateStack([FromBody] StackToUpdateDto stack, string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(Utilities.CreateResponse<string>("Validation error", ModelState, null));
            }

            var entity = await _stackRepository.GetStackByIdAsync(id);
            if (entity == null)
            {
                ModelState.AddModelError("Stack", "Stack to update was not found");
                return NotFound(Utilities.CreateResponse<string>("Not found", ModelState, null));
            }

            _mapper.Map(stack, entity);
            if (await _stackRepository.UpdateStackAsync(entity))
            {
                return Ok(Utilities.CreateResponse<string>("Stack updated successfully", null, null));
            }

            ModelState.AddModelError("error", "There was an error updating the stack please try again");
            return BadRequest(Utilities.CreateResponse<string>("Update error", ModelState, null));
        }

        [HttpGet("get-recent-stacks")]
        public async Task<IActionResult> GetRecentStacks()
        {
            var entity = await _stackRepository.GetRecentStacksAsync();

            var shapedStacks = Helper.Mapper.MapGetRecentStacks(entity);

            return Ok(Utilities.CreateResponse("Recent stacks successfully retrieved", null, shapedStacks));
        }

        [AllowAnonymous]
        [HttpGet("get-all-stacks")]
        public async Task<IActionResult> GetAllStacks() 
        {
            var getAllStacks = await _stackRepository.GetStacksAsync();
            var result = Helper.Mapper.MapStacks(getAllStacks);
            return Ok(Utilities.CreateResponse("Stacks", null, result));
        }
    }
}
