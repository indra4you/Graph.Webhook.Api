using System.Text.Json.Serialization;

namespace Graph.Webhook.Services.Models;

public sealed class NotificationResourceData
{
    [JsonPropertyName("organizationId")]
    public string? OrganizationId { get; set; }

    public string? TenantId
        => this.OrganizationId;

    [JsonPropertyName("@odata.type")]
    public string? ODataType { get; set; }

    [JsonPropertyName("@odata.id")]
    public string? ODataId { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }
}