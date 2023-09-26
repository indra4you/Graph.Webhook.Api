using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Graph.Webhook.Services;

public sealed class GraphService
{
    private readonly Configuration.Settings _settings;
    private readonly GraphServiceClient _graphServiceClient;
    private readonly ILogger<GraphService> _logger;

    public GraphService(
        Configuration.Settings settings,
        GraphServiceClient graphServiceClient,
        ILogger<GraphService> logger
    )
    {
        this._settings = settings;
        this._graphServiceClient = graphServiceClient;
        this._logger = logger;
    }

    public async Task<IList<Subscription>> GetSubscriptionsByChangeType(
        string changeType
    )
    {
        var subscriptionCollection = await _graphServiceClient
            .Subscriptions
            .GetAsync();
        if (null == subscriptionCollection ||
            null == subscriptionCollection.Value ||
            0 == subscriptionCollection.Value.Count)
        {
            this._logger.LogInformation("There are no subscriptions!");

            return new List<Subscription>();
        }

        this._logger.LogInformation("Subscriptions Total# {count}", subscriptionCollection.Value.Count);

        var response = subscriptionCollection
            .Value
            .Where(c => c.ApplicationId == this._settings.AzureAdSettings.ClientId && c.ChangeType == changeType)
            .ToList();

        this._logger.LogInformation("Subscriptions Application# {count}", response.Count);

        return response;
    }

    public async Task SubscribeToUserChange(
        string notificationActionUri,
        string? lifecycleNotificationUri = null
    )
    {
        await _graphServiceClient
            .Subscriptions
            .PostAsync(
                new Subscription
                {
                    Resource = "/users",
                    ChangeType = "updated",
                    NotificationUrl = $"{this._settings.SubscriptionSettings.NotificationHost!.TrimEnd('/')}/{notificationActionUri.TrimStart('/')}",
                    LifecycleNotificationUrl = null != lifecycleNotificationUri
                        ? $"{this._settings.SubscriptionSettings.NotificationHost!.TrimEnd('/')}/{lifecycleNotificationUri.TrimStart('/')}"
                        : null,
                    ExpirationDateTime = DateTimeOffset.UtcNow.AddMinutes(this._settings.SubscriptionSettings.ExpirationInMinutes!.Value),
                    ClientState = this._settings.SubscriptionSettings.ClientState
                }
            );

        this._logger.LogInformation("Successfully subscribed to 'users.updated' notification");
    }

    public async Task ReauthorizeSubscription(
        Models.Notification notification
    )
        => await this._graphServiceClient
            .Subscriptions[notification.SubscriptionId]
            .Reauthorize
            .PostAsync();

    public bool IsSubscriptionRequiredToRenew(
        DateTimeOffset? subscriptionExpirationDateTime
    )
    {
        if (!this._settings.SubscriptionSettings.RenewThresholdInMinutes.HasValue)
            return false;

        if (!subscriptionExpirationDateTime.HasValue)
            return false;

        var renewByDateTime = DateTimeOffset
            .UtcNow
            .AddMinutes(
                this._settings.SubscriptionSettings.RenewThresholdInMinutes!.Value
            );
        var supposedToRenewByDateTime = subscriptionExpirationDateTime
            .Value
            .AddMinutes(
                -1 * this._settings.SubscriptionSettings.RenewThresholdInMinutes!.Value
            );

        return renewByDateTime >= supposedToRenewByDateTime;
    }

    public async Task RenewSubscriptionIfRequired(
        Subscription subscription
    )
    {
        if (string.IsNullOrEmpty(subscription.Id))
            return;

        if (!this.IsSubscriptionRequiredToRenew(subscription.ExpirationDateTime))
            return;

        await this._graphServiceClient
            .Subscriptions[subscription.Id]
            .PatchAsync(
                new Subscription
                {
                    ExpirationDateTime = DateTimeOffset.UtcNow.AddMinutes(this._settings.SubscriptionSettings.ExpirationInMinutes!.Value)
                }
            );
    }

    public async Task RenewSubscriptionsIfRequired(
        IEnumerable<Subscription> subscriptions
    )
    {
        if (!subscriptions.Any())
            return;

        foreach (var subscription in subscriptions)
            await this.RenewSubscriptionIfRequired(subscription);
    }

    public async Task RenewSubscriptionIfRequired(
        Models.Notification notification
    )
    {
        if (string.IsNullOrEmpty(notification.SubscriptionId))
            return;

        if (!this.IsSubscriptionRequiredToRenew(notification.SubscriptionExpirationDateTime))
            return;

        await this._graphServiceClient
            .Subscriptions[notification.SubscriptionId]
            .PatchAsync(
                new Subscription
                {
                    ExpirationDateTime = DateTimeOffset.UtcNow.AddMinutes(this._settings.SubscriptionSettings.ExpirationInMinutes!.Value)
                }
            );
    }

    public async Task RenewSubscriptionsIfRequired(
        Models.NotificationCollection notificationCollection
    )
    {
        if (!notificationCollection.Value!.Any())
            return;

        foreach (var notification in notificationCollection.Value!)
            await this.RenewSubscriptionIfRequired(notification);
    }
}