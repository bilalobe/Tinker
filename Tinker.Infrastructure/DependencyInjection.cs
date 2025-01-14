using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tinker.Infrastructure.Configuration.Groups.Auth;
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
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Tinker.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration          configuration)
    {
        // Database configuration moved here
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Other infrastructure services
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICacheService, CacheService>();

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IPaginationService, PaginationService>();

        // Add caching
        services.AddMemoryCache();
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
            options.InstanceName = "Tinker_";
        });

        services.Configure<DomainSettings>(
            configuration.GetSection("DomainSettings"));
        services.Configure<IntegrationSettings>(
            configuration.GetSection("IntegrationSettings"));
        services.Configure<AuthSettings>(
            configuration.GetSection("Auth"));
        // Register repositories
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ISupplierRepository, SupplierRepository>();
        services.AddScoped<IBatchRepository, BatchRepository>();

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICheckoutService, CheckoutService>();
        services.AddScoped<IEventDispatcher, EventDispatcher>();
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<ISupplierService, SupplierService>();
        services.AddScoped<IComplianceService, ComplianceService>();
        services.AddScoped<IBatchService, BatchService>();
        services.AddScoped<ILoyaltyService, LoyaltyService>();

        // Register validators
        services.AddTransient<IValidator<UpdateStockCommand>, UpdateStockCommandValidator>();
        services.AddTransient<IValidator<CreateOrderCommand>, CreateOrderCommandValidator>();
        services.AddTransient<IValidator<CreateCustomerCommand>, CreateCustomerCommandValidator>();
        services.AddTransient<IValidator<CreateSupplierCommand>, CreateSupplierCommandValidator>();
        services.AddTransient<IValidator<CreateComplianceLogCommand>, CreateComplianceLogCommandValidator>();
        services.AddTransient<IValidator<CreateBatchCommand>, CreateBatchCommandValidator>();

        // Register domain validators
        services.AddTransient<IValidator<Batch>, BatchValidator>();
        services.AddTransient<IValidator<Product>, ProductValidator>();
        services.AddTransient<IValidator<Customer>, CustomerValidator>();

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddMediatR(typeof(UpdateStockCommandHandler).Assembly);
        services.AddMediatR(typeof(CreateOrderCommandHandler).Assembly);
        services.AddMediatR(typeof(CreateCustomerCommandHandler).Assembly);
        services.AddMediatR(typeof(CreateSupplierCommandHandler).Assembly);
        services.AddMediatR(typeof(CreateComplianceLogCommandHandler).Assembly);
        services.AddMediatR(typeof(CreateBatchCommandHandler).Assembly);
        services.AddMediatR(typeof(UpdateLoyaltyPointsCommandHandler).Assembly);

        // Register other services here
        return services;
    }
}