namespace Graph.Webhook;

public static class NotificationExtensions
{
    public static bool IsValid(
        this Services.Models.NotificationCollection? notificationCollection,
        Services.Configuration.Settings settings
    )
    {
        if (null == notificationCollection ||
            null == notificationCollection.Value)
            return false;

        foreach (var notificationValue in notificationCollection.Value)
        {
            if (!string.IsNullOrEmpty(notificationValue.TenantId) &&
                settings.AzureAdSettings.TenantId != notificationValue.TenantId)
                return false;

            if (settings.SubscriptionSettings.ClientState != notificationValue.ClientState)
                return false;
        }

        return true;
    }
}