namespace LESNOY_EXTREME;

public static class Button
{
    public static async Task PressButton(int x, int y, int clickCount)
    {
        for (var clicksDone = 0; clicksDone < clickCount; clicksDone++)
        {
            Console.Write("[Клик] ");
            Window.Click(x, y);
            await Task.Delay(TimeSpan.FromSeconds(0.5));
        }
    }
}