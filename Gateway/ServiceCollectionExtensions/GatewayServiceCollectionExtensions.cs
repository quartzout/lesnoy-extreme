using Microsoft.Extensions.DependencyInjection;

namespace Gateway.ServiceCollectionExtensions;

public static class GatewayServiceCollectionExtensions
{
    public static IServiceCollection AddGateway(this IServiceCollection services)
    {
        services.AddUi();
        
        return services;
    }
}