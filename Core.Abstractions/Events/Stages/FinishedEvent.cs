using OneOf;

namespace Core.Abstractions.Events.Stages;

public interface IFinishedEvent : IRunEvent
{
    public record Started : IFinishedEvent;
}