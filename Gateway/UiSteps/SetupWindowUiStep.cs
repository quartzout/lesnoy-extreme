namespace Gateway.UiSteps;

public class SetupWindowUiStep(ITextWindowFactory windowFactory) 
    : ISetupWindowUiStep
{
    private ITextWindow? _window;
    
    public void Start()
    {
        _window = windowFactory.Create(); 
        _window.Render("Подготовка...");
    }

    public void Finish()
    {
        if (_window is null)
            throw new InvalidOperationException("Finish has been called before Start");
        _window.Dispose();
    }
}