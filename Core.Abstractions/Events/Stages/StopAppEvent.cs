using OneOf;

namespace Core.Abstractions.Events.Stages;

public interface IStopAppEvent : IRunEvent
{
    public record Started : IStopAppEvent;

    public record ClickMade : IStopAppEvent;
}