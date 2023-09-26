namespace Graph.Webhook.Services.Configuration;

public sealed class SubscriptionSettings
{
    public string? ClientState { get; set; }

    public double? ExpirationInMinutes { get; set; }

    public double? RenewThresholdInMinutes { get; set; }

    public string? NotificationHost { get; set; }
}