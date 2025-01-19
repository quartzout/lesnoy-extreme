namespace LESNOY_EXTREME;

public record Settings
{
    public TimeSpan TestDuration { get; init; } = TimeSpan.FromMinutes(40);
    public TimeSpan ShutdownDelay { get; init; } = TimeSpan.FromMinutes(3);
    public TimeSpan TimerInterval { get; init; } = TimeSpan.FromSeconds(0.1);
    public string WindowName { get; init; } = "System Stability Test - AIDA64";
    public (int X, int Y) WindowSize { get; init; } = (800, 600);
    public (int X, int Y) ConsoleWindowSize { get; init; } = (600, 400);
    public int ClickCount { get; init; } = 5;
    public (int X, int Y) FinishClickOffset { get; init; } = (120, -30);
    public (int X, int Y) StartClickOffset { get; init; } = (70, -30);

}