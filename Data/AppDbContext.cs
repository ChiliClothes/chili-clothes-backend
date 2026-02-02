using Microsoft.EntityFrameworkCore;
using ChiliClothes.Models;

namespace ChiliClothes.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User config
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasDefaultValue("USER");

            // Order config
            modelBuilder.Entity<Order>()
                .Property(o => o.Status)
                .HasDefaultValue("PENDING");
                
             modelBuilder.Entity<Product>()
                .Property(p => p.IsActive)
                .HasDefaultValue(true);

             modelBuilder.Entity<Product>()
                .Property(p => p.Stock)
                .HasDefaultValue(0);

            // Relationships
             modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId);

            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId);
        }
    }
}
