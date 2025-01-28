namespace Gateway;

public class ConsoleTextWindow : ITextWindow
{
    public void Render(string text)
    {
        Console.SetCursorPosition(0, 0);
        Console.Write(text);
    }

    public void Dispose()
    {
        Console.Clear();
    }
}