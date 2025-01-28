namespace Gateway;

public class ConsoleTextWindowFactory : ITextWindowFactory
{
    public ITextWindow Create()
    {
        return new ConsoleTextWindow();
    }
}