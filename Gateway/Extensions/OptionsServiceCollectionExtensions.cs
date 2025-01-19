using Gateway.ConfigurationSources.Filename;
using Gateway.InternalInterfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Steps.Options;

namespace Gateway.Extensions;

public static class OptionsServiceCollectionExtensions
{
    public static IServiceCollection AddSolutionOptions(this IServiceCollection services)
    {
        var provider = services.BuildServiceProvider();
        var settingsParser = provider.GetRequiredService<ISettingParser>();

        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Add(new FilenameConfigurationSource(settingsParser))
            .Build();
        
        services.Configure<StepsOptions>(configuration.GetSection(StepsOptions.SectionName));

        return services;
    }
}