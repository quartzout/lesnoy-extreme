using Microsoft.Extensions.DependencyInjection;

namespace Core.ServiceCollectionExtensions;

public static class CoreServiceCollectionExtensions
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddSteps();
        services.AddRunEventPublishing();
        
        return services;
    }
}