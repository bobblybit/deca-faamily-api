using System.Threading.Tasks;

using AccountManagement.Data.Models;

namespace AccountManagement.Data.Repositories.Interfaces
{
    public interface ICompanyRepository
    {
        Task<Company> FindCompanyByDepartmentUserId(string userId);
    }
}