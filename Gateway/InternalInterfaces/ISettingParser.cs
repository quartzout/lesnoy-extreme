using Core.Options;

namespace Gateway.InternalInterfaces;

public interface ISettingParser
{
    Dictionary<string, string> ParseSettingsFromFilename(string filename);

    public static sealed string TestDurationSettingName => $"{nameof(StepsOptions)}:{nameof(StepsOptions.TestDuration)}";
    public static sealed string ShutdownDelaySettingName => $"{nameof(StepsOptions)}:{nameof(StepsOptions.ShutdownDelay)}";
}