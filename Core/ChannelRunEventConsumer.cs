using System.Threading.Channels;
using Core.Abstractions;
using Core.Abstractions.Events;

namespace Core;

public class ChannelRunEventConsumer(ChannelReader<IRunEvent> channelReader) : IRunEventConsumer
{
    public IAsyncEnumerable<IRunEvent> ConsumeAsync()
    {
        return channelReader.ReadAllAsync();
    }

    public bool HasCompleted()
    {
        return channelReader.Completion.IsCompleted;
    }
}