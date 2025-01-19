using Microsoft.Extensions.DependencyInjection;

namespace WindowManager;

public static class WindowManagerServiceCollectionExtensions
{
    public static IServiceCollection AddWindowManager(this IServiceCollection services)
    {
        services.AddSingleton<IWindowManager, WindowManager.Services.WindowManager>();
        return services;
    }
}