using System.Threading.Tasks;

using AccountManagement.Data.Dtos;
using AccountManagement.Data.Models;

namespace AccountManagement.Services.Interfaces
{
    public interface IStacksAndSquadsRepository
    {
        public Task<StacksAndSquadsToReturnDto> GetStacksAndSquadsAsync();
        public Task<bool> AddStackAsync(MyStack stack);
        public Task<bool> AddsquadAsync(MySquad squad);
        public Task<bool> UpdateStackAsync(MyStack stack);
        public Task<bool> UpdateSquadAsync(MySquad squad);
        public Task<MyStack> GetStackByIdAsync(string id);
        public Task<MySquad> GetSquadByIdAsync(string id);
    }
}
