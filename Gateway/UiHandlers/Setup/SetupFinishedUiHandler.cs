using Core.Abstractions.Events.Stages;
using MediatR;

namespace Gateway.UiHandlers.Setup;

public class SetupFinishedUiHandler(ISetupWindowUiStep windowUiStep) 
    : IRequestHandler<UiMessage<SetupEvent.Finished>>
{
    public Task Handle(UiMessage<SetupEvent.Finished> request, CancellationToken cancellationToken)
    {
        windowUiStep.Finish();
        return Task.CompletedTask;
    }
}