using AccountManagement.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountManagement.Data.Database
{
    public class DataContext : IdentityDbContext<AppUser>
    {
        public DataContext(DbContextOptions<DataContext> options): base(options){}

        public DbSet<AppUserAddress> Addresses { get; set; }
        public DbSet<MySquad> MySquads { get; set; }
        public DbSet<MyStack> MyStacks { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Company> Companies { get; set; }

    }
}
