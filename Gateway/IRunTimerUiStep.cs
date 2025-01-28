namespace Gateway.UiSteps;

public interface IRunTimerUiStep
{
    void Start();
    void UpdatePassedTime(TimeSpan newPassed);
    void Finish();
}