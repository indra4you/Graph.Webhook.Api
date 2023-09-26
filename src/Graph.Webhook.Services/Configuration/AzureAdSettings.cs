namespace Graph.Webhook.Services.Configuration;

public sealed class AzureAdSettings
{
    public string? TenantId { get; set; }

    public string? ClientId { get; set; }

    public string? ClientSecret { get; set; }
}