namespace Core.Abstractions.Events.Stages;

public static class RunTimerEvent
{
    public record Started(TimeSpan Total);

    public record TimerUpdated(TimeSpan NewValue);

    public record Finished;
}