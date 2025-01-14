using Blazored.Modal;
using Blazored.Toast;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Tinker.Client.Infrastructure.Auth;
using Tinker.Client.Infrastructure.Http.Handlers;
using Tinker.Client.Infrastructure.Http.Handlers.Implementation;
using Tinker.Client.Infrastructure.State.Models;
using Tinker.Client.Shared.Components;
using Tinker.Core.Security.Interfaces;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Core Services
builder.Services
    .AddScoped<IStateContainer<InventoryState>, StateContainer<InventoryState>>()
    .AddScoped<IStateContainer<OrderState>, StateContainer<OrderState>>()
    .AddScoped<IStateContainer<CartStateModel>, CartState>();

// HTTP Services  
builder.Services
    .AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) })
    .AddScoped<IInventoryHttpClient, InventoryHttpClient>()
    .AddScoped<IOrderHttpClient, OrderHttpClient>()
    .AddScoped<IAuthHttpClient, AuthHttpClient>();

builder.Services
    .AddScoped<AuthenticationHeaderHandler>()
    .AddScoped<RetryHandler>()
    .AddScoped<CacheHandler>()
    .AddScoped<ErrorHandler>();

builder.Services
    .AddHttpClient("API", client => client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"]))
    .AddHttpMessageHandler<ErrorHandler>()
    .AddHttpMessageHandler<CacheHandler>()
    .AddHttpMessageHandler<RetryHandler>();

builder.Services
    .AddHttpClient("AuthenticatedClient", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<AuthenticationHeaderHandler>();

// Security Services
builder.Services
    .AddBlazoredLocalStorage()
    .AddAuthorizationCore(options =>
    {
        options.AddPolicy("AdminOnly", policy => policy.RequireClaim("role", "admin"));
        options.AddPolicy("UserOnly", policy => policy.RequireClaim("role", "user")); 
    })
    .AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>()
    .AddScoped<IAuthenticationService, AuthenticationService>();

// UI Services
builder.Services
    .AddBlazoredModal()
    .AddBlazoredToast()
    .AddBlazoredLocalStorage();

// Infrastructure Services
builder.Services
    .AddScoped<CustomErrorBoundary>()
    .AddScoped<IErrorHandler, ClientErrorHandler>()
    .AddScoped<ILoadingService, LoadingService>();

// Components
builder.Services
    .AddScoped<GlobalSeoMetadata>()
    .AddScoped<SchemaOrg>();

// Configuration 
builder.Services.Configure<ClientSettings>(
    builder.Configuration.GetSection("ClientSettings").Bind);

// PWA Support
builder.Services.AddPWA(options =>
{
    options.CacheFiles = new List<string>
    {
        "/css/app.css",
        "/js/app.js", 
        "/images/*"
    };
});

await builder.Build().RunAsync();