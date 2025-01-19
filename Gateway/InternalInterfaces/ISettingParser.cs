namespace Gateway.InternalInterfaces;

public interface ISettingParser
{
    Dictionary<string, string> ParseSettingsFromFilename(string filename);

    public static sealed string TestDurationSettingName => "TestDuration";
    public static sealed string ShutdownDelaySettingName => "ShutdownDelay";
}