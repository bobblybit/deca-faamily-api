using System.Collections.Generic;
using System.Threading.Tasks;

using ArticleService.Data.Models;

namespace ArticleService.Data.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<bool> AddCategoryAsync(Category category);
        Task<bool> UpdateCategoryAsync(Category category);
        Task<bool> DeleteCategoryAsync(Category category);
        Task<Category> GetCategoryByIdAsync(string categoryId);
        Task<IEnumerable<Category>> GetAllCategories();
    }
}