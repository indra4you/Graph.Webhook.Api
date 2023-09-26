using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace Graph.Webhook.Api;

public static class Program
{
    public static async Task Main(
        string[] args
    )
    {
        var builder = WebApplication
            .CreateBuilder(
                args
            );

        builder
            .Services
            .ConfigrationServices(
                builder.Configuration
            )
            .ConfigureServices(
            );

        var application = builder.Build();
        application
            .UseApplications(
            );

        await application
            .StartAsync(
            );

        // Run once when application starts
        await application
            .SubscribeToNotifications(
            );
        
        await application
            .WaitForShutdownAsync(
            );
    }
}