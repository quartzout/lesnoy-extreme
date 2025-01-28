namespace Gateway.UiSteps;

public class StartAppUiStep(ITextWindowFactory windowFactory) : IStartAppUiStep
{
    private ITextWindow? _window;
    private const int TotalClicks = 10;
    private int _clickCount = 0;

    private static string Template(int clicksMade) 
        => $"Запуск аиды...\nНажатий совершено: {clicksMade}/{TotalClicks}";

    public void Start()
    {
        _window = windowFactory.Create();
        _window.Render(Template(0));
    }

    public void LogClick()
    {
        if (_window is null)
            throw new InvalidOperationException("ClickMade has been called before Start");

        _clickCount++;
        _window.Render(Template(_clickCount));
    }

    public void Finish()
    {
        if (_window is null)
            throw new InvalidOperationException("Finish has been called before Start");
        
        _window.Dispose();
    }
}