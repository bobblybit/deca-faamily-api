using System.Linq;
using System.Threading.Tasks;

using AccountManagement.Data.Database;
using AccountManagement.Data.Dtos;
using AccountManagement.Data.Models;
using AccountManagement.Services.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace AccountManagement.Services.Implementation
{
    public class StacksAndSquadsRepository : IStacksAndSquadsRepository
    {
        private readonly DataContext _context;

        public StacksAndSquadsRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> AddsquadAsync(MySquad squad)
        {
            _context.MySquads.Add(squad);
            return ((await _context.SaveChangesAsync()) > 0);
        }

        public async Task<bool> AddStackAsync(MyStack stack)
        {
            _context.MyStacks.Add(stack);
            return ((await _context.SaveChangesAsync()) > 0);
        }

        public async Task<MySquad> GetSquadByIdAsync(string id) => await _context.MySquads.FindAsync(id);

        public async Task<MyStack> GetStackByIdAsync(string id) => await _context.MyStacks.FindAsync(id);

        public async Task<StacksAndSquadsToReturnDto> GetStacksAndSquadsAsync()
        {
            return new StacksAndSquadsToReturnDto
            {
                Stacks = await _context.MyStacks.Select(x => new StackToReturnDto { Id = x.Id, Name = x.Name }).ToListAsync(),
                Squads = await _context.MySquads.Select(x => new SquadToReturnDto { Id = x.Id, Name = x.Name}).ToListAsync()
            };
        }

        public async Task<bool> UpdateSquadAsync(MySquad squad)
        {
            _context.MySquads.Update(squad);
            return ((await _context.SaveChangesAsync()) > 0);
        }

        public async Task<bool> UpdateStackAsync(MyStack stack)
        {
            _context.MyStacks.Update(stack);
            return ((await _context.SaveChangesAsync()) > 0);
        }
    }
}
