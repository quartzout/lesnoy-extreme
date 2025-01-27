using Core.Abstractions.Events;

namespace Core;

public interface IRunEventPublisher
{
    public Task PublishEvent(IRunEvent runEvent);
}