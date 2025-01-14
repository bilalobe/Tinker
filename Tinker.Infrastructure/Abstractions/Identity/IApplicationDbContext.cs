using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace Tinker.Infrastructure.Core.Data.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Customer> Customers { get; set; }
    DbSet<Product> Products { get; set; }
    DbSet<Order> Orders { get; set; }
    DbSet<OrderItem> OrderItems { get; set; }
    DbSet<Supplier> Suppliers { get; set; }
    DbSet<Prescription> Prescriptions { get; set; }
    DbSet<ComplianceLog> ComplianceLogs { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}