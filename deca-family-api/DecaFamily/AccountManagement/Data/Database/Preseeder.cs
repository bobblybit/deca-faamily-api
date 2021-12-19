using AccountManagement.Data.Models;

using MassTransit;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using MqDtos;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace AccountManagement.Data.Database
{
    public class Preseeder
    {
        private const string adminPassword = "Secret@123";
        private const string regularPassword = "P@ssw0rd";
        private static string path = Path.GetFullPath(@"Data/json-data/");

        public async static Task EnsurePopulated(IApplicationBuilder app)
        {
            //Get db context
            var _context = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<DataContext>();

            if (_context.Database.GetPendingMigrations().Any())
            {
                _context.Database.Migrate();
            }

            if (_context.Users.Any())
            {
                return;
            }
            else
            {
                //Get Usermanager and rolemanager from IoC container
                var userManager = app.ApplicationServices.CreateScope()
                                              .ServiceProvider.GetRequiredService<UserManager<AppUser>>();

                var roleManager = app.ApplicationServices.CreateScope()
                                                .ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var publishEndpoint = app.ApplicationServices.CreateScope()
                                                 .ServiceProvider.GetRequiredService<IPublishEndpoint>();


                var roles = new string[] { "SuperAdmin", "Admin", "Editor", "Regular" };
                var stacks = GetSampleData<MyStack>(File.ReadAllText(path + "stacks.json"));
                var squads = GetSampleData<MySquad>(File.ReadAllText(path + "squads.json"));
                var companies = GetSampleData<Company>(File.ReadAllText(path + "companies.json"));
                var photos = GetSampleData<Photo>(File.ReadAllText(path + "photos.json"));
                var appUsers = GetSampleData<AppUser>(File.ReadAllText(path + "users.json"));
                var departmentNames = new List<string> { "Program", "Facility", "Engineering", "Marketing", "DevOps" };
                var positionNames = new List<string> { "DecaDev", "Stack Lead (C#)", "Stack Lead (Java)", "Secretary", "Stack Associate" };
                var seededUsers = new DbSeedMqDto();

                //Create role if it doesn't exists
                foreach (var role in roles)
                {
                    var roleExist = await roleManager.RoleExistsAsync(role);
                    if (!roleExist)
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }

                //seed companies
                _context.Companies.AddRange(companies);
                await _context.SaveChangesAsync();

                //Seed Users with 1(one) Admin User
                var adminCount = 0;
                var stacksCount = 0;
                var squadsCount = 0;
                var photosCount = 0;

                //seed users
                foreach (var user in appUsers)
                {
                    if (adminCount == 0)
                    {
                        user.IsActive = true;
                        user.EmailConfirmed = true;
                        user.UserName = user.Email;
                        user.MyStack = stacks[stacksCount];
                        user.MySquad = squads[squadsCount];
                        user.Photos.Add(photos[photosCount]);
                        await userManager.CreateAsync(user, adminPassword);
                        await userManager.AddToRoleAsync(user, roles[0]);
                        seededUsers.Users.Add(new SeededUserMqDto { FirstName = user.FirstName, LastName = user.LastName, Id = user.Id, Role = roles[0] });

                        ++adminCount;
                        ++photosCount;
                        ++stacksCount;
                        ++squadsCount;
                        if (squadsCount > squads.Count() - 1)
                            squadsCount = 0;

                        if (stacksCount > stacks.Count() - 1)
                            stacksCount = 0;
                    }
                    else if (adminCount <= 2)
                    {
                        user.IsActive = true;
                        user.EmailConfirmed = true;
                        user.UserName = user.Email;
                        user.MyStack = stacks[stacksCount];
                        user.MySquad = squads[squadsCount];
                        user.Photos.Add(photos[photosCount]);
                        await userManager.CreateAsync(user, adminPassword);
                        await userManager.AddToRoleAsync(user, roles[1]);
                        seededUsers.Users.Add(new SeededUserMqDto { FirstName = user.FirstName, LastName = user.LastName, Id = user.Id, Role = roles[1] });
                        ++adminCount;
                        ++photosCount;
                        ++stacksCount;
                        ++squadsCount;
                        if (squadsCount > squads.Count() - 1)
                            squadsCount = 0;

                        if (stacksCount > stacks.Count() - 1)
                            stacksCount = 0;
                    }
                    else if (adminCount <= 4)
                    {
                        user.IsActive = true;
                        user.EmailConfirmed = true;
                        user.UserName = user.Email;
                        user.MyStack = stacks[stacksCount];
                        user.MySquad = squads[squadsCount];
                        user.Photos.Add(photos[photosCount]);
                        await userManager.CreateAsync(user, adminPassword);
                        await userManager.AddToRoleAsync(user, roles[2]);
                        seededUsers.Users.Add(new SeededUserMqDto { FirstName = user.FirstName, LastName = user.LastName, Id = user.Id, Role = roles[2] });

                        ++adminCount;
                        ++photosCount;
                        ++stacksCount;
                        ++squadsCount;
                        if (squadsCount > squads.Count() - 1)
                            squadsCount = 0;

                        if (stacksCount > stacks.Count() - 1)
                            stacksCount = 0;
                    }
                    else
                    {

                        var rand = new Random();
                        var departmentIdx = rand.Next(0, 3);
                        var companyIdx = rand.Next(0, companies.Count() - 1);
                        user.IsActive = true;
                        user.EmailConfirmed = true;
                        user.UserName = user.Email;
                        user.Photos.Add(photos[photosCount]);
                        user.MyStack = stacks[stacksCount];
                        user.MySquad = squads[squadsCount];
                        await userManager.CreateAsync(user, regularPassword);
                        await userManager.AddToRoleAsync(user, roles[3]);
                        seededUsers.Users.Add(new SeededUserMqDto { FirstName = user.FirstName, LastName = user.LastName, Id = user.Id, Role = roles[3] });
                        companies[companyIdx].Departments.Add(new Department { AppUserId = user.Id,
                            Name = departmentNames[rand.Next(0, departmentNames.Count() - 1)],
                            Position = positionNames[rand.Next(0, positionNames.Count() - 1)]
                        });
                        ++photosCount;
                        ++stacksCount;
                        ++squadsCount;
                        if (squadsCount > squads.Count() - 1)
                            squadsCount = 0;

                        if (stacksCount > stacks.Count() - 1)
                            stacksCount = 0;

                    }

                }
                _context.Companies.UpdateRange(companies);
                _context.SaveChanges();
                seededUsers.Stacks = await _context.MyStacks.Select(x => new SeededStackMqDto { StackName = x.Name, StackId = x.Id }).ToListAsync();
                await publishEndpoint.Publish<DbSeedMqDto>(seededUsers);
            }



        }

        private static List<T> GetSampleData<T>(string jsonString)
        {
            var output = JsonSerializer.Deserialize<List<T>>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return output;
        }
    }
}
