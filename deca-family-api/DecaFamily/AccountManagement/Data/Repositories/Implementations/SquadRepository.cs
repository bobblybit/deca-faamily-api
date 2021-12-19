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
    public class SquadRepository : ISquadRepository
    {
        private readonly DataContext _context;
        public SquadRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> AddSquadAsync(MySquad squad)
        {
            _context.MySquads.Add(squad);
            return ((await _context.SaveChangesAsync()) > 0);
        }

        public async Task<MySquad> GetSquadByIdAsync(string id) => await _context.MySquads.FindAsync(id);

        public async Task<bool> UpdateSquadAsync(MySquad squad)
        {
            _context.MySquads.Update(squad);
            return ((await _context.SaveChangesAsync()) > 0);
        }

        public async Task<IEnumerable<MySquad>> GetSquadsAsync()
        {
            return await _context.MySquads.ToListAsync();
        }
    }
}
