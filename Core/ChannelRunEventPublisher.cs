using System.Threading.Channels;
using Core.Abstractions.Events;

namespace Core;

public class ChannelRunEventPublisher(ChannelWriter<IRunEvent> writer) : IRunEventPublisher
{
    public async Task PublishEvent(IRunEvent runEvent)
    {
        await writer.WriteAsync(runEvent);
    }
}