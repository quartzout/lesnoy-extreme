using Core.Abstractions.Events;

namespace Core.Abstractions;

public interface IRunEventConsumer
{
    public IAsyncEnumerable<RunEvent> ConsumeAsync(CancellationToken cancellation);
}