using Microsoft.EntityFrameworkCore;

namespace RestaurantAPI.Models;

public class RestaurantDbContext : DbContext
{
    public RestaurantDbContext(DbContextOptions<RestaurantDbContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<FoodItem> FoodItems { get; set; }
    public DbSet<OrderMaster> OrderMasters { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure relationships explicitly
        modelBuilder.Entity<OrderMaster>()
            .HasOne(om => om.Customer)
            .WithMany(c => c.Orders) // tie inverse navigation to prevent duplicate FK
            .HasForeignKey(om => om.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OrderDetail>()
            .HasOne(od => od.FoodItem)
            .WithMany()
            .HasForeignKey(od => od.FoodItemId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OrderDetail>()
            .HasOne<OrderMaster>()
            .WithMany(om => om.OrderDetails)
            .HasForeignKey(od => od.OrderMasterId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure decimal precision
        modelBuilder.Entity<FoodItem>()
            .Property(f => f.Price)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<OrderMaster>()
            .Property(om => om.GTotal)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<OrderDetail>()
            .Property(od => od.FoodItemPrice)
            .HasColumnType("decimal(18,2)");

        base.OnModelCreating(modelBuilder);
    }
}
