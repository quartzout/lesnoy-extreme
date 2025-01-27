using Core.Abstractions.Events;
using MediatR;

namespace Gateway;

public class RunEventUiHandler(ISender sender) : IRunEventUiHandler
{
    public async Task Handle(RunEvent runEvent)
    {
        var payload = runEvent.Value;
        var payloadType = payload.GetType();
        var uiMessageConcreteType = typeof(UiMessage<>).MakeGenericType(payloadType); 
        var uiMessage = Activator.CreateInstance(uiMessageConcreteType, payload);
        if (uiMessage is null)
            throw new InvalidOperationException($"Could not create instance of {uiMessageConcreteType}");
        
        await sender.Send(uiMessage);
    }
}