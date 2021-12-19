using ArticleService.Data.Database;
using ArticleService.Data.Models;
using ArticleService.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArticleService.Data.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _ctx;

        public UserRepository(DataContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<bool> AddUserAsync(User user)
        {
            await _ctx.Users.AddAsync(user);
            return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            _ctx.Users.Update(user);
            return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<User> GetUserByIdAsync(string userId)
        {
            return await _ctx.Users.SingleOrDefaultAsync(x => x.Id == userId);
        }
    }
}
