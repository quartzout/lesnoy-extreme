using System.Threading.Channels;
using Core.Abstractions;
using Core.Abstractions.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Core;

public static class CoreServiceCollectionExtensions
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddSingleton<Runner>();

        services.AddSingleton(Channel.CreateUnbounded<RunEvent>());
        services.AddSingleton(sp => sp.GetRequiredService<Channel<RunEvent>>().Reader);
        services.AddSingleton(sp => sp.GetRequiredService<Channel<RunEvent>>().Writer);

        services.AddTransient<IRunEventPublisher, ChannelRunEventPublisher>();
        services.AddTransient<IRunEventConsumer, ChannelRunEventConsumer>();
        
        return services;
    }
}