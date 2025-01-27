namespace Core.Abstractions.Events.Stages;

public interface IStartAppEvent : IRunEvent
{
    public record Started : IStartAppEvent;

    public record ClickMade : IStartAppEvent;
}