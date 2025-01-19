using Gateway.InternalInterfaces;
using Microsoft.Extensions.Configuration;

namespace Gateway.ConfigurationSources.Filename;

public class FilenameConfigurationSource(ISettingParser settingParser) : IConfigurationSource
{
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new FilenameConfigurationProvider(settingParser);
    }
}