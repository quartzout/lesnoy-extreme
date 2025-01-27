using System.Runtime.InteropServices;

namespace Infrastructure.Windows.Imports;

internal static class Kernel32
{
    [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetCursorPos(int x, int y);

    [DllImport("kernel32.dll", SetLastError=true)]
    public static extern bool GetConsoleMode(
        IntPtr hConsoleHandle,
        out int lpMode);

    [DllImport("kernel32.dll", SetLastError=true)]
    public static extern bool SetConsoleMode(
        IntPtr hConsoleHandle,
        int ioMode);

    [DllImport("Kernel32.dll", SetLastError = true)]
    public static extern IntPtr GetStdHandle(int nStdHandle);
}