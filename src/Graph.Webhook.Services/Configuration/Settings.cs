namespace Graph.Webhook.Services.Configuration;

public sealed class Settings
{
    public AzureAdSettings AzureAdSettings { get; set; } = new AzureAdSettings();

    public SubscriptionSettings SubscriptionSettings { get; set; } = new SubscriptionSettings();
}