using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Graph.Webhook.Services;

public sealed class ScopedProcessHelper
{
    private readonly IServiceProvider _serviceProvider;

    public ScopedProcessHelper(
        IServiceProvider serviceProvider
    )
        => this._serviceProvider = serviceProvider;

    public void RunWithinScope(
        Models.NotificationCollection notificationCollection,
        Action<IServiceScope, Models.NotificationCollection> action
    )
    {
        Task
            .Run(
                () =>
                {
                    using var scope = this._serviceProvider.CreateScope();

                    action(scope, notificationCollection);
                }
            );
    }
}