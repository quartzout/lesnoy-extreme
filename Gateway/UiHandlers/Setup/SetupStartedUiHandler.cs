using Core.Abstractions.Events.Stages;
using MediatR;

namespace Gateway.UiHandlers.Setup;

public class SetupStartedUiHandler(ISetupWindowUiStep setupUiStep) 
    : IRequestHandler<UiMessage<SetupEvent.Started>>
{
    public Task Handle(UiMessage<SetupEvent.Started> request, CancellationToken cancellationToken)
    {
        setupUiStep.Start();
        return Task.CompletedTask;
    }
}