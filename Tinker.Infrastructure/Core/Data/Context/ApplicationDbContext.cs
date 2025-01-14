using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Tinker.Infrastructure.Core.Data.Interfaces;

namespace Tinker.Infrastructure.Core.Data.Context;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public required DbSet<Customer> Customers { get; set; }
    public required DbSet<Product> Products { get; set; }
    public required DbSet<Order> Orders { get; set; }
    public required DbSet<OrderItem> OrderItems { get; set; }
    public required DbSet<Supplier> Suppliers { get; set; }
    public required DbSet<Prescription> Prescriptions { get; set; }
    public required DbSet<ComplianceLog> ComplianceLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Customer>().ToTable("Customers");
        modelBuilder.Entity<Product>().ToTable("Products");
        modelBuilder.Entity<Order>().ToTable("Orders");
        modelBuilder.Entity<OrderItem>().ToTable("OrderItems");
        modelBuilder.Entity<Supplier>().ToTable("Suppliers");
        modelBuilder.Entity<Prescription>().ToTable("Prescriptions");
        modelBuilder.Entity<ComplianceLog>().ToTable("ComplianceLogs");
    }
}