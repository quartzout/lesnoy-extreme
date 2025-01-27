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

        services.AddSingleton(Channel.CreateUnbounded<IRunEvent>());
        services.AddSingleton(sp => sp.GetRequiredService<Channel<IRunEvent>>().Reader);
        services.AddSingleton(sp => sp.GetRequiredService<Channel<IRunEvent>>().Writer);

        services.AddTransient<IRunEventPublisher, ChannelRunEventPublisher>();
        services.AddTransient<IRunEventConsumer, ChannelRunEventConsumer>();
        
        return services;
    }
}