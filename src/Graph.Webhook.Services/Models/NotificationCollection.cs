using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Graph.Webhook.Services.Models;

public sealed class NotificationCollection
{
    /*
    [JsonPropertyName("validationTokens")]
    public IEnumerable<string>? ValidationTokens { get; set; }
    */

    [JsonPropertyName("value")]
    public IEnumerable<Notification>? Value { get; set; }
}