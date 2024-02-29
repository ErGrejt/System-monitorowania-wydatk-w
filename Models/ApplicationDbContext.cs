using Microsoft.EntityFrameworkCore;
using System;

namespace WebApplication1.Models
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }
            public DbSet<Jedzenie> Jedzenie { get; set; }

            public DbSet<Zdrowie> Zdrowie { get; set; }

            public DbSet<Zachcianki> Zachcianki { get; set; }

            public DbSet<Przelewy> Przelewy { get; set; }

            public DbSet<Saldo> Saldo { get; set; }
        
    }
}
