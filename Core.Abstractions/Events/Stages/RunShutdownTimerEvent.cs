using Core.Abstractions.Models;

namespace Core.Abstractions.Events.Stages;

public static class RunShutdownTimerEvent
{
    public record Started(TimeSpan Total, ShutdownError Reason);

    public record TimerUpdated(TimeSpan NewValue);

    public record Finished;
}