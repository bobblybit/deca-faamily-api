using ArticleService.Data.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArticleService.Data.Database
{
    public class Preseeder
    {
        public async static Task EnsurePopulated(IApplicationBuilder app)
        {

            //Get db context
            var _context = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<DataContext>();

            if (_context.Database.GetPendingMigrations().Any())
            {
                _context.Database.Migrate();
            }

            if (_context.Articles.Any())
            {
                return;
            }
            else
            {
                //Seed Database with articles and comments
                await _context.AddRangeAsync(GetSampleData());
            }
            await _context.SaveChangesAsync();
        }

        //Create sample users
        private static List<Article> GetSampleData()
        {
            var techCat = new Category { CategoryName = "Technology", Description = "Technological discussions" };
            var financeCat = new Category { CategoryName = "Finance", Description = "Financial discussions" };

            return new List<Article>
            {
               new Article{User = new User{ FirstName = "James", LastName = "Brown" }, Category = techCat, Content = "This is a sample content on technological discussions", Title="ASP.Net in the wild",
                                    Comments = new List<Comment>
                                    {
                                        new Comment{User = new User { FirstName = "William", LastName = "Shaw" }, Content = "Nice Article."},
                                        new Comment{User = new User { FirstName = "John", LastName = "Steven" }, Content = "Great work sir."},
                                        new Comment{User = new User { FirstName = "Tej", LastName = "Johnson" }, Content = "Talk more on life."},
                                    }},
               new Article{User = new User { FirstName = "Andrew", LastName = "Wiggins" }, Category = techCat, Content = "This is a sample content on technological discussions", Title="Microservices in Action",
                                    Comments = new List<Comment>
                                    {
                                        new Comment{User = new User { FirstName = "Andrew", LastName = "Johnson" }, Content = "Nicely written"},
                                        new Comment{User = new User { FirstName = "Steve", LastName = "Ardalis" }, Content = "More content required"},
                                        new Comment{User = new User { FirstName = "Mike", LastName = "Spence" }, Content = "Stay winning."},
                                    }},
               new Article{User = new User { FirstName = "Nathan", LastName = "Lively" }, Category = financeCat, Content = "This is a sample content on financial discussions", Title="Save money when young.",
                                    Comments = new List<Comment>
                                    {
                                        new Comment{User = new User { FirstName = "Lilly", LastName = "Straus" }, Content = "Good one."},
                                        new Comment{User = new User { FirstName = "Fletcher", LastName = "Green" }, Content = "Never in doubt."},
                                        new Comment{User = new User { FirstName = "Nia", LastName = "Long" }, Content = "More of this."},
                                    }}
            };
        }
    }
}
