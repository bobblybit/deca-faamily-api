using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ArticleService.Data.Database;
using ArticleService.Data.Models;
using ArticleService.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ArticleService.Data.Repositories.Implementations
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DataContext _context;
        public CategoryRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<bool> AddCategoryAsync(Category category)
        {
            _context.Categories.Add(category);
            int rowsAffected = await _context.SaveChangesAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteCategoryAsync(Category category)
        {
            _context.Categories.Remove(category);
            int rowsAffected = await _context.SaveChangesAsync();
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category> GetCategoryByIdAsync(string categoryId)
        {
            return await _context.Categories.FindAsync(categoryId);
        }

        public async Task<bool> UpdateCategoryAsync(Category category)
        {
            _context.Categories.Update(category);
            int rowsAffected =  await _context.SaveChangesAsync();
            return rowsAffected > 0;
        }
    }
}
