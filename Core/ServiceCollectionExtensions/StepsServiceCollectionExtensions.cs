using Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Core.ServiceCollectionExtensions;

internal static class StepsServiceCollectionExtensions
{
    public static IServiceCollection AddSteps(this IServiceCollection services)
    {
        services.AddSingleton<IRunner, Runner>();
        services.AddTransient<ISetupStep, SetupStep>();
        
        return services;
    }
}