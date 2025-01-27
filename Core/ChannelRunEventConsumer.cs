using System.Threading.Channels;
using Core.Abstractions;
using Core.Abstractions.Events;

namespace Core;

public class ChannelRunEventConsumer(ChannelReader<RunEvent> channelReader) : IRunEventConsumer
{
    public IAsyncEnumerable<RunEvent> ConsumeAsync()
    {
        return channelReader.ReadAllAsync();
    }

    public bool HasCompleted()
    {
        return channelReader.Completion.IsCompleted;
    }
}