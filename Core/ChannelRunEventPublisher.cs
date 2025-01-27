using System.Threading.Channels;
using Core.Abstractions.Events;

namespace Core;

public class ChannelRunEventPublisher(ChannelWriter<RunEvent> writer) : IRunEventPublisher
{
    public async Task PublishEvent(RunEvent runEvent)
    {
        await writer.WriteAsync(runEvent);
    }
}