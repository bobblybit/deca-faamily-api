using ArticleService.Data.Dtos;
using ArticleService.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArticleService.Controllers.CategoryHelper
{
    public class CategoryToReturnObjectMapper
    {
        public static IEnumerable<CategoryToReturnDto> MapCategories(IEnumerable<Category> categories)
        {
            var categoriesToReturn = new List<CategoryToReturnDto>();

            foreach (var category in categories)
            {
                var newCategory = new CategoryToReturnDto
                {
                    Id = category.Id,
                    CategoryName = category.CategoryName,
                    Description = category.Description,
                    CreatedAt = category.CreatedAt,
                    UpdatedAt = category.UpdatedAt
                };

                categoriesToReturn.Add(newCategory);
            }
            
            return categoriesToReturn;
        }
    }
}