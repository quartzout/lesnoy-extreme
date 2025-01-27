using Infrastructure.Abstractions;
using Infrastructure.Windows.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Windows;

public static class InfrastructureWindowsServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureWindows(this IServiceCollection services)
    {
        services.AddSingleton<IWindowManager, WindowManager>();
        return services;
    }
}