using AccountManagement.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountManagement.Data.Repositories.Interfaces
{
    public interface IStackRepository
    {
        Task<bool> AddStackAsync(MyStack stack);
        Task<MyStack> GetStackByIdAsync(string id);
        Task<bool> UpdateStackAsync(MyStack stack);
        Task<IEnumerable<MyStack>> GetRecentStacksAsync(int page = 6);
        Task<IEnumerable<MyStack>> GetStacksAsync();
    }
}
