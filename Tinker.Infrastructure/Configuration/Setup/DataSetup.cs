using Tinker.Infrastructure.Core.Caching.Compression;
using Tinker.Infrastructure.Core.Caching.Interfaces;
using Tinker.Infrastructure.Core.Caching.Services;
using Tinker.Infrastructure.Core.Data;
using Tinker.Infrastructure.Core.Data.Base;
using Tinker.Infrastructure.Core.Data.Context;
using Tinker.Infrastructure.Core.Data.Interfaces;
using Tinker.Infrastructure.Core.Data.Repositories.Compliance;
using Tinker.Infrastructure.Core.Data.Repositories.Customers;
using Tinker.Infrastructure.Core.Data.Repositories.Orders;
using Tinker.Infrastructure.Core.Data.Repositories.Products;

namespace Tinker.Infrastructure.Configuration.Setup;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Tinker.Infrastructure.Configuration.Groups.Data;

public static class DataSetup
{
    public static IServiceCollection AddDataServices(
        this IServiceCollection services,
        DataSettings settings)
    {
        settings.Validate();

        AddDatabaseServices(services, settings.Database);
        AddCacheServices(services, settings.Cache);
        AddRepositories(services);

        services.AddDbContext<ApplicationDbContext>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPaginationService, PaginationService>();

        return services;
    }

    private static void AddDatabaseServices(
        IServiceCollection services,
        DatabaseSettings settings)
    {
        services.AddDbContext<ApplicationDbContext>((_, options) =>
        {
            options.UseSqlServer(settings.ConnectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: settings.MaxRetryCount,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
                sqlOptions.CommandTimeout(settings.CommandTimeout);
            });

            if (settings.EnableDetailedErrors)
                options.EnableDetailedErrors();
            if (settings.EnableSensitiveDataLogging)
                options.EnableSensitiveDataLogging();
        });

        services.AddDbContext<AuthDbContext>((_, options) =>
        {
            var authConnString = settings.ConnectionString.Replace("Database=Tinker", "Database=TinkerAuth");
            options.UseSqlServer(authConnString, 
                sqlOptions => sqlOptions.EnableRetryOnFailure(settings.MaxRetryCount));
        });

        services.AddScoped<IApplicationDbContext>(
            provider => provider.GetRequiredService<ApplicationDbContext>());
    }

    private static void AddCacheServices(
        IServiceCollection services,
        CacheSettings settings)
    {
        if (!settings.Enabled) 
            return;

        services.AddMemoryCache();
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = settings.ConnectionString;
            options.InstanceName = settings.InstanceName;
        });

        services.Configure<CacheOptions>(options =>
        {
            options.Expiry = settings.DefaultExpiry;
            options.CompressionThreshold = settings.CompressionThreshold;
        });

        services.AddSingleton<ICompressionService, CompressionService>();
        services.AddSingleton<ICacheService, CacheService>();
    }

    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ISupplierRepository, SupplierRepository>();
        services.AddScoped<IBatchRepository, BatchRepository>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();
    }
}
