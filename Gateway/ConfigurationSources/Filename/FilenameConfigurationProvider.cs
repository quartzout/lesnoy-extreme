using System.Diagnostics;
using Gateway.InternalInterfaces;
using Microsoft.Extensions.Configuration;

namespace Gateway.ConfigurationSources.Filename;

public class FilenameConfigurationProvider(ISettingParser settingParser) : ConfigurationProvider
{
    public override void Load()
    {
        var filename = Process.GetCurrentProcess().ProcessName;
        var parsedSettings = settingParser.ParseSettingsFromFilename(filename);
        Data = parsedSettings;
    }
}