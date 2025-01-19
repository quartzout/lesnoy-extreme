using System.Text.RegularExpressions;
using Gateway.InternalInterfaces;

namespace Gateway.Services;

public partial class SettingParser : ISettingParser
{
    public Dictionary<string, string> ParseSettingsFromFilename(string filename)
    {
        var regex = FilenameSettingsRegex();
        var match = regex.Match(filename);
        if (!match.Success)
            throw new ArgumentException($"Invalid filename: {filename}");
            
        var firstGroup = match.Groups.Values.ElementAtOrDefault(1)?.Value;
        var secondGroup = match.Groups.Values.ElementAtOrDefault(2)?.Value;
        if (firstGroup == null || secondGroup == null)
            return new Dictionary<string, string>();
            
        if (!float.TryParse(firstGroup, out var firstFloat)
            || !float.TryParse(secondGroup, out var secondFloat))
            throw new ArgumentException($"Invalid filename: {filename}");
            
        return new Dictionary<string, string>
        {
            { ISettingParser.TestDurationSettingName, TimeSpan.FromMinutes(firstFloat).ToString(@"hh\:mm\:ss") },
            { ISettingParser.ShutdownDelaySettingName, TimeSpan.FromMinutes(secondFloat).ToString(@"hh\:mm\:ss") }
        };
    }
    
    [GeneratedRegex(@"#([\d.]+)\s*#([\d.]+)")]
    private static partial Regex FilenameSettingsRegex();
}