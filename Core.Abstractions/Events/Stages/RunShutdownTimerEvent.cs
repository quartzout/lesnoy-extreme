using Core.Abstractions.Models;

namespace Core.Abstractions.Events.Stages;

public interface IRunShutdownTimerEvent : IRunEvent
{
    public record Started(TimeSpan Total, ShutdownError Reason) : IRunShutdownTimerEvent;

    public record TimerUpdated(TimeSpan NewValue) : IRunShutdownTimerEvent;
}