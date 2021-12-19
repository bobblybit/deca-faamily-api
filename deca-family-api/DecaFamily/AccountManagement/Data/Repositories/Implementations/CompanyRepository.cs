using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AccountManagement.Data.Database;
using AccountManagement.Data.Models;
using AccountManagement.Data.Repositories.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace AccountManagement.Data.Repositories.Implementations
{
    public class CompanyRepository : ICompanyRepository
    {
        public readonly DataContext _context;

        public CompanyRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<Company> FindCompanyByDepartmentUserId(string userId)
        {
            var com = await _context.Companies.Include(x => x.Departments)
                        .Where(x => x.Departments.Any(x => x.AppUserId == userId)).FirstOrDefaultAsync();
            return com;
        }
    }
}
