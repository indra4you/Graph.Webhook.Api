using System;
using System.Text.Json.Serialization;

namespace Graph.Webhook.Services.Models;

public sealed class Notification
{
    [JsonPropertyName("subscriptionId")]
    public string? SubscriptionId { get; set; }

    [JsonPropertyName("subscriptionExpirationDateTime")]
    public DateTimeOffset? SubscriptionExpirationDateTime { get; set; }

    [JsonPropertyName("tenantId")]
    public string? TenantId { get; set; }

    [JsonPropertyName("changeType")]
    public string? ChangeType { get; set; }

    [JsonPropertyName("clientState")]
    public string? ClientState { get; set; }

    [JsonPropertyName("resource")]
    public string? Resource { get; set; }

    [JsonPropertyName("resourceData")]
    public NotificationResourceData? ResourceData { get; set; }

    [JsonPropertyName("lifecycleEvent")]
    public string? LifecycleEvent { get; set; }
}