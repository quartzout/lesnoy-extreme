using Microsoft.Extensions.DependencyInjection;

namespace Steps;

public static class StepsServiceCollectionExtensions
{
    public static IServiceCollection AddSteps(this IServiceCollection services)
    {
        services.AddScoped<MarcoStep>();
        return services;
    }
}