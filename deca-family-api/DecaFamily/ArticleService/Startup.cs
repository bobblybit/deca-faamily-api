using ArticleService.Consumers;
using ArticleService.Data.Database;
using ArticleService.Data.Repositories.Implementations;
using ArticleService.Data.Repositories.Interfaces;
using ArticleService.Extensions;
using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Linq;

namespace ArticleService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMassTransit(x => {
                x.AddConsumer<DbSeedConsumer>();
                x.UsingRabbitMq((ctx, config) =>
                {
                    config.Host("amqp://guest:guest@localhost:5672");
                    config.ReceiveEndpoint("dbseed-queue", x => {
                        x.ConfigureConsumer<DbSeedConsumer>(ctx);
                    });
                });
            });
            services.AddMassTransitHostedService();

            services.AddControllers();
            services.AddDbContext<DataContext>(options =>
                {
                    options.UseSqlServer(Configuration.GetConnectionString("DbConnection"));
                }
            );

            // Enable Swagger   
            services.AddSwaggerGen(swagger =>
            {
                //This is to generate the Default UI of Swagger Documentation  
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Deca Family Article Service API",
                    Description = "ASP.NET Core 3.1 Web API For DecaFamily"
                });
                // To Enable authorization using Swagger (JWT)  
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                });
                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}

                    }
                });
                swagger.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            });

            var key = Configuration.GetSection("JWTConfigurations:SecretKey").Value;
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(key)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddAutoMapper(typeof(Startup));
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IArticleRepository, ArticleRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.ConfigureExceptionHandler();

            app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Article Service API V1");
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            //Preseeder.EnsurePopulated(app).Wait();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
