using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph.Models.ODataErrors;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Graph.Webhook.Api;

internal static class SubscriptionExtensions
{
    internal static async Task SubscribeToNotifications(
        this WebApplication application
    )
    {
        var graphService = application
            .Services
            .GetRequiredService<Services.GraphService>();

        var subscriptions = await graphService.GetSubscriptionsByChangeType("updated");

        try
        {
            if (!subscriptions.Any())
                await graphService
                    .SubscribeToUserChange(
                        "api/v1/listen/user.updated",
                        "api/v1/listen/lifecycle"
                    );
            else
                await graphService
                    .RenewSubscriptionsIfRequired(
                        subscriptions
                    );
        }
        catch (ODataError ex)
        {
            // Do NOT throw exception here, throwing exception here will stop further processing
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ERROR - Create Subscription Failed! - {0}", ex.Message);
            Console.ResetColor();
        }
    }
}