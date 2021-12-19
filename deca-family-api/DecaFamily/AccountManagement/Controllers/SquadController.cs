using AccountManagement.Commons;
using AccountManagement.Data.Dtos;
using AccountManagement.Data.Models;
using AccountManagement.Data.Repositories.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountManagement.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class SquadController : ControllerBase
    {
        private readonly ISquadRepository _squadRepository;
        private readonly IMapper _mapper;

        public SquadController(ISquadRepository squadRepository, IMapper mapper)
        {
            _squadRepository = squadRepository;
            _mapper = mapper;
        }

        [HttpGet("get-squad/{id}", Name = "GetSquadById")]
        public async Task<IActionResult> GetSquadById(string id)
        {
            var squad = await _squadRepository.GetSquadByIdAsync(id);
            if (squad == null)
            {
                ModelState.AddModelError("Squad", "The squad was not found");
                return NotFound(Utilities.CreateResponse<string>("Not found", ModelState, null));
            }
            return Ok(
                Utilities.CreateResponse("Squad", null, _mapper.Map<SquadToReturnDto>(squad)));
        }

        [HttpPost("add-squad")]
        public async Task<IActionResult> AddSquad([FromBody] SquadToAddDto squad)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(Utilities.CreateResponse<string>("Validation error", ModelState, ""));
            }

            var entity = _mapper.Map<MySquad>(squad);
            if (await _squadRepository.AddSquadAsync(entity))
            {
                return CreatedAtAction("GetSquadById", new { id = entity.Id }, _mapper.Map<SquadToReturnDto>(entity));
            }

            ModelState.AddModelError("error", "An error occured while createing the squad, please try again");
            return BadRequest(Utilities.CreateResponse<string>("error", ModelState, ""));
        }

        [HttpPut("update-squad/{id}")]
        public async Task<IActionResult> UpdateSquad([FromBody] SquadToUpdateDto squad, string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(Utilities.CreateResponse<string>("Validation error", ModelState, null));
            }

            var entity = await _squadRepository.GetSquadByIdAsync(id);
            if (entity == null)
            {
                ModelState.AddModelError("Squad", "Squad to update was not found");
                return NotFound(Utilities.CreateResponse<string>("Not found", ModelState, null));
            }

            _mapper.Map(squad, entity);
            if (await _squadRepository.UpdateSquadAsync(entity))
            {
                return Ok(Utilities.CreateResponse<string>("Squad updated successfully", null, null));
            }

            ModelState.AddModelError("error", "There was an error updating the squad please try again");
            return BadRequest(Utilities.CreateResponse<string>("Update error", ModelState, null));
        }

        [AllowAnonymous]
        [HttpGet("get-all-squads")]
        public async Task<IActionResult> GetAllSquads()
        {
            var getAllStacks = await _squadRepository.GetSquadsAsync();
            var result = Helper.Mapper.MapSquads(getAllStacks);
            return Ok(Utilities.CreateResponse("Squads", null, result));
        }
    }
}
