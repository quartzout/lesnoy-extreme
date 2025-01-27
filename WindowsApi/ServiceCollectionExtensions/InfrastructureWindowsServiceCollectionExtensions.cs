using Microsoft.Extensions.DependencyInjection;
using WindowsApi.Abstractions;
using WindowsApi.Services;

namespace WindowsApi.ServiceCollectionExtensions;

public static class InfrastructureWindowsServiceCollectionExtensions
{
    public static IServiceCollection AddWindowsApi(this IServiceCollection services)
    {
        services.AddSingleton<IWindowManager, WindowManager>();
        services.AddSingleton<IWinRightsService, WinRightsService>();
        return services;
    }
}