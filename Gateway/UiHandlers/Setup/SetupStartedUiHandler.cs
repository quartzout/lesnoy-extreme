using Core.Abstractions.Events.Stages;
using MediatR;

namespace Gateway.UiHandlers.Setup;

public class SetupStartedUiHandler : IRequestHandler<UiMessage<SetupEvent.Started>>
{
    public Task Handle(UiMessage<SetupEvent.Started> request, CancellationToken cancellationToken)
    {
       Console.WriteLine("Setup Started");
       return Task.CompletedTask;
    }
}