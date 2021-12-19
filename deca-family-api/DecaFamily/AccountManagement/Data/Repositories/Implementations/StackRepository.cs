using AccountManagement.Data.Database;
using AccountManagement.Data.Models;
using AccountManagement.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountManagement.Data.Repositories.Implementations
{
    public class StackRepository : IStackRepository
    {
        private readonly DataContext _context;
        public StackRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> AddStackAsync(MyStack stack)
        {
            _context.MyStacks.Add(stack);
            return ((await _context.SaveChangesAsync()) > 0);
        }

        public async Task<MyStack> GetStackByIdAsync(string id) => await _context.MyStacks.FindAsync(id);

        public async Task<bool> UpdateStackAsync(MyStack stack)
        {
            _context.MyStacks.Update(stack);
            return ((await _context.SaveChangesAsync()) > 0);
        }

        public async Task<IEnumerable<MyStack>> GetRecentStacksAsync(int page)
        {
            return await _context.MyStacks.OrderByDescending(x => x.CreatedAt).Include(x => x.AppUsers).Take(page).ToListAsync();
        }

        public async Task<IEnumerable<MyStack>> GetStacksAsync()
        {
            return await _context.MyStacks.ToListAsync();
        }
    }
}
