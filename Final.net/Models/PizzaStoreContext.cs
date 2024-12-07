using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore;

namespace Final.net.Models;

public partial class PizzaStoreContext : DbContext
{
    public PizzaStoreContext()
    {
    }

    public PizzaStoreContext(DbContextOptions<PizzaStoreContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Crust> Crusts { get; set; }

    public virtual DbSet<Delivery> Deliveries { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Size> Sizes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Blogs> Blogs { get; set; }

    public virtual DbSet<CartItem> CartItems { get; set; }




    //     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    //         => optionsBuilder.UseSqlServer("Server=KHANGMINH;Database=PizzaStore;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Delivery>(entity =>
        {
            entity.Property(e => e.DeliveryStatus).HasMaxLength(20);
        });


        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(ci => ci.CartItemId); // Định nghĩa khóa chính

            entity.HasOne(ci => ci.Size)
                .WithMany(s => s.CartItems)
                .HasForeignKey(ci => ci.SizeId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ci => ci.Crust)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CrustId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(ci => ci.Product)
                .WithMany(p => p.CartItems)
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });


        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasOne(o => o.Payment)
                  .WithMany(p => p.Orders)
                  .HasForeignKey(o => o.PaymentId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(o => o.Delivery)
                  .WithMany(d => d.Orders)
                  .HasForeignKey(o => o.DeliveryId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(p => p.CreatedAt)
                  .HasDefaultValueSql("GETUTCDATE()");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.Property(e => e.Method).HasMaxLength(20);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasOne(d => d.Category)
                  .WithMany(p => p.Products)
                  .HasForeignKey(d => d.CategoryId);

            entity.Property(p => p.CreatedAt)
                  .HasDefaultValueSql("GETUTCDATE()");
        });

        modelBuilder.Entity<Size>(entity =>
        {
            entity.Property(e => e.SizeName).HasMaxLength(50);
        });

        base.OnModelCreating(modelBuilder);

        // Seed data for Role
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Admin" },
            new Role { Id = 2, Name = "User" },
            new Role { Id = 3, Name = "Staff" }
        );

        modelBuilder.Entity<User>()
        .HasOne(u => u.Role)
        .WithMany(r => r.Users)
        .HasForeignKey(u => u.RoleId)
        .OnDelete(DeleteBehavior.Restrict);

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
