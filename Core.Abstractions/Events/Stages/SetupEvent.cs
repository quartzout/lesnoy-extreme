namespace Core.Abstractions.Events.Stages;

public interface ISetupEvent : IRunEvent
{
    public class Started : ISetupEvent;
}
