namespace Core.Abstractions.Events.Stages;

public interface IRunTimerEvent : IRunEvent
{
    public record Started(TimeSpan Total) : IRunTimerEvent;

    public record TimerUpdated(TimeSpan NewValue) : IRunTimerEvent;
}