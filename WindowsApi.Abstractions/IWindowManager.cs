namespace WindowsApi.Abstractions;

public interface IWindowManager
{
    bool Maximize(IntPtr hWnd);
    bool Minimize(IntPtr hWnd);
    IntPtr Find(string caption);
    bool Focus(IntPtr hWnd);
    void RemoveBorders(IntPtr window);
    bool BringWindowToTop(IntPtr hWnd);
    bool MakeTopmost(IntPtr hWnd);
    IntPtr GetCurrentWindow();
    void Click(int x, int y);
    (int X, int Y) GetCursorPos();
    bool SetWindowPos(IntPtr hWnd, int x, int y, int cx, int cy);
    bool DisableQuickEdit();
}