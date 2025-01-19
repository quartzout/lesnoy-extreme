namespace Steps.Options;

public record StepsOptions
{
    public static string SectionName = nameof(StepsOptions);
    
    public required TimeSpan TestDuration { get; init; }
    public required TimeSpan ShutdownDelay { get; init; }
    public required TimeSpan TimerInterval { get; init; } 
    public required string WindowName { get; init; } 
    public required Vector WindowSize { get; init; } 
    public required Vector ConsoleWindowSize { get; init; }
    public required int ClickCount { get; init; }
    public required Vector FinishClickOffset { get; init; }
    public required Vector StartClickOffset { get; init; }

}