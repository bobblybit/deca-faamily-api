using AccountManagement.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountManagement.Data.Repositories.Interfaces
{
    public interface ISquadRepository
    {
        Task<bool> AddSquadAsync(MySquad squad);
        Task<MySquad> GetSquadByIdAsync(string id);
        Task<bool> UpdateSquadAsync(MySquad squad);
        Task<IEnumerable<MySquad>> GetSquadsAsync();
    }
}
