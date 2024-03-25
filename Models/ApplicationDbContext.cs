using Microsoft.EntityFrameworkCore;
using System;

namespace WebApplication1.Models
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }
            public DbSet<Food> Food { get; set; }

            public DbSet<Health> Health { get; set; }

            public DbSet<Others> Others { get; set; }

            public DbSet<Transfers> Transfers { get; set; }

            public DbSet<Balance> Balance { get; set; }

            public DbSet<Users> Users { get; set; }
        
    }
}
