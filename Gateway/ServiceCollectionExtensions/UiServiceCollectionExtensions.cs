using Microsoft.Extensions.DependencyInjection;

namespace Gateway.ServiceCollectionExtensions;

public static class UiServiceCollectionExtensions
{
    public static IServiceCollection AddUi(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
        });
        
        services.AddTransient<IRunEventUiHandler, RunEventUiHandler>();
        
        return services;
    }
}