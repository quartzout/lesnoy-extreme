using Core.Options;
using Microsoft.Extensions.Options;

namespace Gateway.UiSteps;

public class RunTimerUiStep(
    ITextWindowFactory windowFactory,
    IOptions<StepsOptions> optionAccessor) : IRunTimerUiStep
{
    private readonly StepsOptions _stepsOptions = optionAccessor.Value;
    private ITextWindow? _window;

    private string Template(TimeSpan passed) => 
        $"Таймер запущен!\n{passed:hh\\ч\\ mm\\м\\ ss\\с} // {_stepsOptions.TestDuration:hh\\ч\\ mm\\м\\ ss\\с}";
    
    public void Start()
    {
        _window = windowFactory.Create();
        _window.Render(Template(TimeSpan.Zero));
    }

    public void UpdatePassedTime(TimeSpan newPassed)
    {
        if (_window is null)
            throw new InvalidOperationException("UpdatePassedTime has been called before Start");
        
        _window.Render(Template(newPassed));
    }

    public void Finish()
    {
        if (_window is null)
            throw new InvalidOperationException("UpdatePassedTime has been called before Start");
        
        _window.Dispose();
    }
    
}