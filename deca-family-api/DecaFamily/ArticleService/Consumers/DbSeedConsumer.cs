using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using ArticleService.Data.Database;
using ArticleService.Data.Models;

using MassTransit;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using MqDtos;

namespace ArticleService.Consumers
{
    public class DbSeedConsumer : IConsumer<DbSeedMqDto>
    {
        public readonly IServiceProvider _serviceProvider;
        private readonly string path = Path.GetFullPath(@"Data/json-data/");
        public DbSeedConsumer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        Task IConsumer<DbSeedMqDto>.Consume(ConsumeContext<DbSeedMqDto> context)
        {
            var _context = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<DataContext>();

            if (_context.Database.GetPendingMigrations().Any())
            {
                _context.Database.Migrate();
            }

            if (_context.Users.Any())
            {
                return Task.CompletedTask;
            }
            else
            {
                var articles = GetSampleData<Article>(File.ReadAllText(path + "articles.json"));
                var comments = GetSampleData<Comment>(File.ReadAllText(path + "comments.json"));
                var regularUsers = context.Message.Users.Where(x => x.Role.ToLower() == "regular").ToList();
                var editors = context.Message.Users.Where(x => x.Role.ToLower() == "editor").ToList();
                var superAdmin = context.Message.Users.FirstOrDefault(x => x.Role.ToLower() == "superadmin");
                var admins = context.Message.Users.Where(x => x.Role.ToLower() == "admin").ToList();
                var categoryNames = new List<string> { "Technology", "Web Development", "Desktop", "Database", "Front End", "Back End" };
                var categories = categoryNames.Select(x => new Category { CategoryName = x }).ToList();


                //add editors, admins and super admin to DbContext
                foreach(var editor in editors)
                {
                    _context.Users.Add(new User { FirstName = editor.FirstName, LastName = editor.LastName, Id = editor.Id });
                }

                foreach (var admin in admins)
                {
                    _context.Users.Add(new User { FirstName = admin.FirstName, LastName = admin.LastName, Id = admin.Id });
                }

                _context.Users.Add(new User { FirstName = superAdmin.FirstName, LastName = superAdmin.LastName, Id = superAdmin.Id });

                //add article categories to DbContext
                _context.Categories.AddRange(categories);
                _context.SaveChanges();

                //seed regular users, articles and comments
                var usersToAdd = new List<User>();
                var articlesCount = 0;
                var rand = new Random();
                foreach(var user in regularUsers)
                {
                    var userToAdd = new User { FirstName = user.FirstName, LastName = user.LastName, Id = user.Id };
                    var userArticles = new List<Article>();
                    for(int i = articlesCount; i< articlesCount + 4; i++)
                    {
                        var stack = context.Message.Stacks[rand.Next(0, context.Message.Stacks.Count() - 1)];

                        var article = new Article
                        {
                            UserId = user.Id,
                            ApprovedBy = editors[rand.Next(0, editors.Count() - 1)].Id,
                            Title = articles[i].Title,
                            Tag = articles[i].Tag,
                            Content = articles[i].Content,
                            CategoryId = categories[rand.Next(0, categories.Count() - 1)].Id,
                            Stack = stack.StackName,
                            StackId = stack.StackId,
                            Approved = true,
                        };

                        userArticles.Add(article);
                    }
                    userToAdd.Articles = userArticles;
                    usersToAdd.Add(userToAdd);
                }

                _context.Users.AddRange(usersToAdd);
                _context.SaveChanges();

                //seed comments and likes
                var allArticles = _context.Articles.ToList();
                foreach(var article in allArticles)
                {
                    var articleLikes = new List<ArticleLike>();
                    var commentersCount = rand.Next(0, regularUsers.Count() - 1);
                    for(int i = 0; i<= commentersCount; i++)
                    {
                        articleLikes.Add(new ArticleLike { LikerId = regularUsers[i].Id, Liker = $"{regularUsers[i].FirstName} {regularUsers[i].LastName}" });
                    }

                    article.ArticleLikes = articleLikes;

                    var articleComments = new List<Comment>();
                    var commenters = regularUsers.Where(x => x.Id != article.UserId).Take(10);
                    foreach(var user in commenters)
                    {
                        var articleComment = new Comment { UserId = user.Id };
                        articleComment.Content = comments[rand.Next(0, comments.Count() - 1)].Content;
                        var commentLikes = new List<CommentLike>();
                        var commentLikersCount = rand.Next(0, regularUsers.Count() - 1);
                        for (int i = 0; i <= commentLikersCount; i++)
                        {
                            commentLikes.Add(new CommentLike { LikerId = regularUsers[i].Id, Liker = $"{regularUsers[i].FirstName} {regularUsers[i].LastName}" });
                        }
                        articleComment.CommentLikes = commentLikes;
                        articleComments.Add(articleComment);
                    }
                    article.Comments = articleComments;
                }

                _context.Articles.UpdateRange(allArticles);
                _context.SaveChangesAsync().Wait();

            }

            return Task.CompletedTask;
        }

        private List<T> GetSampleData<T>(string jsonString)
        {
            var output = JsonSerializer.Deserialize<List<T>>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return output;
        }
    }
}
