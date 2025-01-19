using Gateway.InternalInterfaces;
using Microsoft.Extensions.Configuration;

namespace Gateway.ConfigurationSources.Filename;

public class ProcessNameConfigurationSource(
    ISettingParser settingParser) : IConfigurationSource
{
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new ProcessNameConfigurationProvider(settingParser);
    }
}