using System.Threading.Channels;
using Core.Abstractions;
using Core.Abstractions.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Core.ServiceCollectionExtensions;

public static class RunEventServiceCollectionExtensions
{
    public static IServiceCollection AddRunEventPublishing(this IServiceCollection services)
    {
        services.AddSingleton(Channel.CreateUnbounded<RunEvent>());
        services.AddSingleton(sp => sp.GetRequiredService<Channel<RunEvent>>().Reader);
        services.AddSingleton(sp => sp.GetRequiredService<Channel<RunEvent>>().Writer);

        services.AddTransient<IRunEventConsumer, ChannelRunEventConsumer>();
        services.AddTransient<IRunEventPublisher, ChannelRunEventPublisher>();

        return services;
    }
}