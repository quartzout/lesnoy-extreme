using Core.Abstractions.Events;

namespace Core.Abstractions;

public interface IRunEventConsumer
{
    public IAsyncEnumerable<IRunEvent> ConsumeAsync();

    public bool HasCompleted();
}