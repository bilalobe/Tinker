using Tinker.Infrastructure.Configuration.Groups.Services;
using Tinker.Infrastructure.Integration.Messaging.Services;
using Tinker.Infrastructure.Integration.Messaging.Services.Interfaces;
using Tinker.Infrastructure.Integration.Notifications.Persistence;
using EmailService = Tinker.Infrastructure.Integration.Email.Services.EmailService;

namespace Tinker.Server.Extensions;

public static class NotificationServiceExtensions
{
    public static IServiceCollection AddNotificationServices(
        this IServiceCollection services,
        IConfiguration          configuration)
    {
        services.Configure<EmailSettings>(
            configuration.GetSection("EmailSettings"));

        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<INotificationStore, NotificationStore>();
        services.AddScoped<INotificationService, NotificationService>();

        return services;
    }
}