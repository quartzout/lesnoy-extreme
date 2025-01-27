using Core.Abstractions.Events;

namespace Gateway;

public interface IRunEventUiHandler
{
    Task Handle(RunEvent runEvent);
}