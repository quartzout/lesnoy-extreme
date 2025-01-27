using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Infrastructure.Abstractions;
using Infrastructure.Windows.Imports;

namespace Infrastructure.Windows.Services;

internal class WindowManager : IWindowManager
{
    private const int SwMaximize = 3;
    private const int SwMinimize = 6;

    
    public bool Maximize(IntPtr hWnd)
    {
        return User32.ShowWindow(hWnd, SwMaximize);
    }
    
    public bool Minimize(IntPtr hWnd)
    {
        return User32.ShowWindow(hWnd, SwMinimize);
    }

    public IntPtr Find(string caption)
    {
        return User32.FindWindow(null, caption);
    }

    public bool Focus(IntPtr hWnd)
    {
        return User32.SetForegroundWindow(hWnd);
    }
    
    public static uint MF_BYPOSITION = 0x400;
    public static uint MF_REMOVE = 0x1000;

    //assorted constants needed
    public static int GWL_STYLE = -16;
    public static int WS_CHILD = 0x40000000; //child window
    public static int WS_BORDER = 0x00800000; //window with border
    public static int WS_DLGFRAME = 0x00400000; //window with double border but no title
    public static int WS_CAPTION = WS_BORDER | WS_DLGFRAME; //window with a title bar
    
    public void RemoveBorders(IntPtr window) {
        IntPtr pFoundWindow = window;
        int style = User32.GetWindowLong(pFoundWindow, GWL_STYLE);
        User32.SetWindowLong(pFoundWindow, GWL_STYLE, (style & ~WS_CAPTION));
    }

    public bool BringWindowToTop(IntPtr hWnd)
    {
        return User32.BringWindowToTop(hWnd);
    }

    private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
    
    public static class SWP
    {
        public static readonly uint
            NOSIZE = 0x0001,
            NOMOVE = 0x0002,
            NOZORDER = 0x0004,
            NOREDRAW = 0x0008,
            NOACTIVATE = 0x0010,
            DRAWFRAME = 0x0020,
            FRAMECHANGED = 0x0020,
            SHOWWINDOW = 0x0040,
            HIDEWINDOW = 0x0080,
            NOCOPYBITS = 0x0100,
            NOOWNERZORDER = 0x0200,
            NOREPOSITION = 0x0200,
            NOSENDCHANGING = 0x0400,
            DEFERERASE = 0x2000,
            ASYNCWINDOWPOS = 0x4000;
    }

    public bool MakeTopmost(IntPtr hWnd)
    {
        return User32.SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, SWP.NOSIZE | SWP.NOMOVE);
    }

    public IntPtr GetCurrentWindow()
    {
        var process = Process.GetCurrentProcess();
        return process.MainWindowHandle;
    }
    
    private const int MOUSEEVENTF_LEFTDOWN = 0x02;
    private const int MOUSEEVENTF_LEFTUP = 0x04;
    private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
    private const int MOUSEEVENTF_RIGHTUP = 0x10;

    public void Click(int x, int y)
    {
        Kernel32.SetCursorPos(x, y);
        User32.mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
    }

    public (int X, int Y) GetCursorPos()
    {
        User32.GetCursorPos(out var point);
        return (point.X, point.Y);
    }

    public bool SetWindowPos(IntPtr hWnd, int x, int y, int cx, int cy)
    {
        return User32.SetWindowPos(hWnd, HWND_TOPMOST, x, y, cx, cy, SWP.NOOWNERZORDER);
    }

    /// <summary>
    /// This flag enables the user to use the mouse to select and edit text. To enable
    /// this option, you must also set the ExtendedFlags flag.
    /// </summary>
    const int QuickEditMode = 64;
    
    public const int STD_INPUT_HANDLE = -10;

    
    /// <summary>
    /// ExtendedFlags must be enabled in order to enable InsertMode or QuickEditMode.
    /// </summary>
    const int ExtendedFlags = 128;
    
    public bool DisableQuickEdit()
    {
        IntPtr conHandle = Kernel32.GetStdHandle(STD_INPUT_HANDLE);
        Console.WriteLine(conHandle);
        int mode;

        if (!Kernel32.GetConsoleMode(conHandle, out mode))
        {
            var error = Marshal.GetLastWin32Error();
            var errorMessage = new Win32Exception(error).Message;
            Console.WriteLine(errorMessage);
            return false;
        }

        mode &= ~(QuickEditMode | ExtendedFlags);

        if (!Kernel32.SetConsoleMode(conHandle, mode))
        {
            Console.WriteLine("12");
            return false;
        }

        return true;
    }
}