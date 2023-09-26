using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using System;
using System.Threading.Tasks;

namespace Graph.Webhook.Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public sealed class ListenController : ControllerBase
{
    [HttpPost("lifecycle")]
    [AllowAnonymous()]
    public async Task<IActionResult> Lifecycle(
        [FromServices] ILogger<ListenController> logger,
        [FromServices] Services.Configuration.Settings settings,
        [FromServices] Services.ScopedProcessHelper scopedProcessHelper,
        [FromQuery] string? validationToken = null
    )
    {
        if (!string.IsNullOrEmpty(validationToken))
        {
            logger.LogInformation("Listen::Lifecycle => Validation Token received, responsed with 'Ok'");

            return Ok(validationToken);
        }

        var notificationCollection = await base.Request
            .Body
            .ToJson<Services.Models.NotificationCollection>(
                logger
            );
        if (!notificationCollection.IsValid(settings))
        {
            logger.LogInformation("Listen::Lifecycle => Notification is NOT valid, responsed with 'Ok'");

            return Ok();
        }

        scopedProcessHelper
            .RunWithinScope(
                notificationCollection!,
                (_, notificationCollection) =>
                {
                    Console.WriteLine("Processing Lifecycle Notification...");

                    foreach (var notificationValue in notificationCollection.Value!)
                    {
                        Console.WriteLine();
                        Console.WriteLine("ClientState: {0}", notificationValue.ClientState);
                        Console.WriteLine("LifecycleEvent: {0}", notificationValue.LifecycleEvent);

                        /*
                         * For more details: https://learn.microsoft.com/en-us/graph/webhooks-lifecycle
                         * 
                        switch (notificationValue.LifecycleEvent)
                        {
                            case "reauthorizationRequired":
                                break;
                            case "missed":
                                break;
                            case "subscriptionRemoved":
                                break;
                        }
                        */
                    }
                }
            );

        logger.LogInformation("Listen::Lifecycle => Notification is valid, responsed with 'Accepted'");

        return base.Accepted();
    }

    [HttpPost("user.updated")]
    [AllowAnonymous()]
    public async Task<IActionResult> UserUpdated(
        [FromServices] ILogger<ListenController> logger,
        [FromServices] Services.Configuration.Settings settings,
        [FromServices] Services.ScopedProcessHelper scopedProcessHelper,
        [FromQuery] string? validationToken = null
    )
    {
        if (!string.IsNullOrEmpty(validationToken))
        {
            logger.LogInformation("Listen::UserUpdated => Validation Token received, responsed with 'Ok'");

            return Ok(validationToken);
        }

        var notificationCollection = await base.Request
            .Body
            .ToJson<Services.Models.NotificationCollection>(
                logger
            );
        if (!notificationCollection.IsValid(settings))
        {
            logger.LogInformation("Listen::UserUpdated => Notification is NOT valid, responsed with 'Ok'");

            return Ok();
        }

        scopedProcessHelper
            .RunWithinScope(
                notificationCollection!,
                async (scope, notificationCollection) =>
                {
                    var graphServiceClient = scope.ServiceProvider.GetRequiredService<GraphServiceClient>();

                    Console.WriteLine("Processing User Updated Notifications...");

                    foreach (var notificationValue in notificationCollection.Value!)
                    {
                        Console.WriteLine();
                        Console.WriteLine("ChangeType: {0}", notificationValue.ChangeType);
                        Console.WriteLine("ClientState: {0}", notificationValue.ClientState);
                        Console.WriteLine("Resource: {0}", notificationValue.Resource);
                        Console.WriteLine("SubscriptionId: {0}", notificationValue.SubscriptionId);

                        if (null != notificationValue.ResourceData)
                        {
                            Console.WriteLine("ResourceData.Id: {0}", notificationValue.ResourceData.Id);
                        }

                        var user = await graphServiceClient.Users[notificationValue.ResourceData!.Id]
                            .GetAsync();
                        Console.WriteLine("User Display Name: {0}", user!.DisplayName);
                    }
                }
            );

        logger.LogInformation("Listen::UserUpdated => Notification is valid and processed, responsed with 'Accepted'");

        return base.Accepted();
    }
}