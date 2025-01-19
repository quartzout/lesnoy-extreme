using Gateway.ConfigurationSources.Filename;
using Gateway.InternalInterfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Gateway.Extensions;

public static class OptionsServiceCollectionExtensions
{
    public static IHostApplicationBuilder AddProcessNameConfigurationSource(this IHostApplicationBuilder builder)
    {
        var provider = builder.Services.BuildServiceProvider();
        var settingsParser = provider.GetRequiredService<ISettingParser>();

        builder.Configuration.Add(new ProcessNameConfigurationSource(settingsParser));
        return builder;
    }
}