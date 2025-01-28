namespace Gateway;

public interface ITextWindow : IDisposable
{
    public void Render(string text);
}