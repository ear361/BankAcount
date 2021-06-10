using System;
using Microsoft.EntityFrameworkCore;

namespace BankAccountAPI.Models
{
    public class BankContext : DbContext
    {        
        public BankContext(DbContextOptions<BankContext> options) : base(options)
        {

        }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<User> Users { get; set; }

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