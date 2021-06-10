using System;
using Microsoft.EntityFrameworkCore;

namespace BankAccountAPI.Models
{
    public class BankContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<User> Users { get; set; }

        //todo: move connection string to appsettings
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite(@"Data Source=C:\Users\ear36\bank.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Account>(entity => { entity.HasIndex(e => e.AccountNumber).IsUnique(); });

            modelBuilder.Entity<User>().HasData(new User
            {
                Username = "Admin",
                FirstName = "First",
                LastName = "Last",
                Address = "Addr",
                PostCode = 2000,
                State = "NSW",
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            });
        }
    }
}