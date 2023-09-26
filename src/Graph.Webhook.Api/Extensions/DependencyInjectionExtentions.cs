using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;
using Microsoft.Kiota.Abstractions.Authentication;
using System;

namespace Graph.Webhook.Api;

internal static class DependencyInjectionExtentions
{
    internal static IServiceCollection ConfigrationServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var azureAdSettings = configuration
            .GetSection("AzureAd")
            .Get<Services.Configuration.AzureAdSettings>();
        if (null == azureAdSettings)
            throw new InvalidOperationException("'AzureAd' settings missing!");

        var subscriptionSettings = configuration
            .GetSection("Subscription")
            .Get<Services.Configuration.SubscriptionSettings>();
        if (null == subscriptionSettings)
            throw new InvalidOperationException("'Subscription' settings missing!");

        services.AddSingleton(
            new Services.Configuration.Settings()
            {
                AzureAdSettings = azureAdSettings,
                SubscriptionSettings = subscriptionSettings
            }
        );

        return services;
    }

    internal static IServiceCollection ConfigureServices(
        this IServiceCollection services
    )
    {
        services
            .AddTransient<IAccessTokenProvider, Services.AccessTokenProvider>()
            .AddTransient<IAuthenticationProvider, BaseBearerTokenAuthenticationProvider>()
            .AddTransient<GraphServiceClient>()
            .AddTransient<Services.ScopedProcessHelper>()
            .AddTransient<Services.GraphService>();

        services
            .AddAuthorization()
            .AddControllers();
        
        return services;
    }

    internal static WebApplication UseApplications(
        this WebApplication application
    )
    {
        application
            .MapControllers();
            
        application
            .UseHttpsRedirection()
            .UseAuthorization();

        return application;
    }
}