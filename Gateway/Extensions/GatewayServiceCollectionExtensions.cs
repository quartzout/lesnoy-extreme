using Gateway.InternalInterfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Gateway.Extensions;

public static class GatewayServiceCollectionExtensions
{
    public static IServiceCollection AddGateway(this IServiceCollection services)
    {
        services.AddTransient<ISettingParser, Services.SettingParser>();
        return services;
    }
}