using System;
using Final.Models;
using Microsoft.EntityFrameworkCore;

namespace Final.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<Role> Roles { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Admin", CodeName = "ADMIN" },
            new Role { Id = 2, Name = "User", CodeName = "USER" }
        );

        modelBuilder.Entity<Account>()
        .HasOne(a => a.Role)
        .WithMany() // Assuming Role can have many Accounts
        .HasForeignKey(a => a.RoleId)
        .OnDelete(DeleteBehavior.Cascade);

    }
}

