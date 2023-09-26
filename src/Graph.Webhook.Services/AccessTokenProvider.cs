using Microsoft.Identity.Client;
using Microsoft.Kiota.Abstractions.Authentication;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Graph.Webhook.Services;

public sealed class AccessTokenProvider : IAccessTokenProvider
{
    private readonly string[] _scopes =
        new string[]
        {
            "https://graph.microsoft.com/.default"
        };

    private readonly Configuration.Settings _settings;

    public AccessTokenProvider(
        Configuration.Settings settings
    )
        => this._settings = settings;


    public AllowedHostsValidator AllowedHostsValidator => new();

    public async Task<string> GetAuthorizationTokenAsync(
        Uri uri,
        Dictionary<string, object>? additionalAuthenticationContext = null,
        CancellationToken cancellationToken = default
    )
    {
        var confidentialClientApplication = this.BuildConfidentialClientApplication();

        var acquireTokenForClient = confidentialClientApplication
            .AcquireTokenForClient(
                this._scopes
            );
        var authenticationResult = await acquireTokenForClient
            .ExecuteAsync(
                cancellationToken
            );

        return authenticationResult.AccessToken;
    }

    private IConfidentialClientApplication BuildConfidentialClientApplication()
        =>
            ConfidentialClientApplicationBuilder
                .Create(this._settings.AzureAdSettings.ClientId)
                .WithTenantId(this._settings.AzureAdSettings.TenantId)
                .WithClientSecret(this._settings.AzureAdSettings.ClientSecret)
                .Build();
}