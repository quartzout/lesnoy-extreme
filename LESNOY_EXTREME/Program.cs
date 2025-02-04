using System.Diagnostics;
using System.Security.Principal;
using System.Text.RegularExpressions;
using LESNOY_EXTREME;

bool CheckAdmin()
{
    var identity = WindowsIdentity.GetCurrent();
    var principal = new WindowsPrincipal(identity);
    var isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);

    if (!isAdmin)
    {
        Console.WriteLine("\nПерезапустите скрипт от имени администратора");
        Console.WriteLine("\nНажмите Enter чтобы закрыть");
        Console.ReadLine();
        return false;
    }

    return true;
}

Settings ReadSettings()
{
    Settings GetSettings()
    {
        var filename = Process.GetCurrentProcess().ProcessName;
    
        var regex = FilenameSettingsRegex();
        var match = regex.Match(filename);
        if (!match.Success)
            return new Settings();
    
        var firstGroup = match.Groups.Values.ElementAtOrDefault(1)?.Value;
        var secondGroup = match.Groups.Values.ElementAtOrDefault(2)?.Value;
        if (firstGroup == null || secondGroup == null)
            return new Settings();
    
        if (!float.TryParse(firstGroup, out var firstFloat)
            || !float.TryParse(secondGroup, out var secondFloat))
            return new Settings();
    
        return new Settings
        {
            TestDuration = TimeSpan.FromMinutes(firstFloat),
            ShutdownDelay = TimeSpan.FromMinutes(secondFloat)
        };
    }
    
    var settings = GetSettings();
    
    return settings;
}

async Task RunShutdownTimer(Settings settings)
{
    Console.Clear();
    Console.WriteLine("Запускаю экстренный таймер на выключение компьютера...");
    
    var stopwatch = Stopwatch.StartNew();
    var periodicTimer = new PeriodicTimer(settings.TimerInterval);

    while (await periodicTimer.WaitForNextTickAsync() && stopwatch.Elapsed < settings.ShutdownDelay)
    {
        Console.SetCursorPosition(0, 0);
        Console.WriteLine("ОКНО АИДЫ НЕ НАЙДЕНО!!!");
        Console.WriteLine("Тест прерван, запущен таймер на выключение компьютера");
        Console.WriteLine($@"До выключения осталось: {settings.ShutdownDelay - stopwatch.Elapsed:mm\m\ ss\s\ f}");
    }
    
    Shutdown();
    return;

    void Shutdown()
    {
        Console.WriteLine("\n\n ВЫКЛЮЧЕНИЕ КОМПЬЮТЕРА...");
        Process.Start("shutdown","/s /t 0");
        while (true) ;
    }
}

async Task RunTimer(Settings settings)
{
    Console.Clear();
    
    var stopwatch = Stopwatch.StartNew();
    using PeriodicTimer timer = new(TimeSpan.FromMilliseconds(50));
    
    while (await timer.WaitForNextTickAsync() && stopwatch.Elapsed < settings.TestDuration)
    {
        try
        {
            var window = Window.Find(settings.WindowName);
            if (window == IntPtr.Zero)
            {
                await RunShutdownTimer(settings);
            }

            Console.SetCursorPosition(0, 0);
            Console.WriteLine("Таймер запущен!");
            Console.WriteLine(
                $@"прошло времени: {stopwatch.Elapsed:mm\m\ ss\s\ f} / {settings.TestDuration:mm\m\ ss\s\ f}");

            var current = Window.GetCurrentWindow();
            if (current == IntPtr.Zero)
            {
                Console.WriteLine("\n ?Внимание? не могу найти окно консоли скрипта");
            }
            var focusResult = Window.Focus(current);
            if (!focusResult)
            {
                //Console.WriteLine("\n !!! не могу дать фокус на окно консоли скрипта!!! впринципе пофиг");
            }

            var topResult = Window.BringWindowToTop(current);
            if (!topResult)
            {
                Console.WriteLine("\n ?Внимание? Не могу вывести на передний план окно консоли скрипта");
            }
            
            var placeResult = Window.SetWindowPos(current, settings.WindowSize.X, 0, settings.ConsoleWindowSize.X,
                settings.ConsoleWindowSize.Y);
            if (!placeResult)
            {
                Console.WriteLine(
                    "\n ?Внимание? Не могу зафиксировать положение окна консоли! Следите чтобы окно не перекрывало кнопку Stop аиды!");
            }

            Console.WriteLine("\n\nнастройки:");
            Console.WriteLine($"\tпродолжительность теста: {settings.TestDuration:mm\\m\\ ss\\s}");
            Console.WriteLine($"\tпри ошибке выключение компьютера через {settings.ShutdownDelay:mm\\m\\ ss\\s}");
            Console.WriteLine($"чтобы изменить поменяйте цифры после # в названии файла например TEST_NAME #30 #2.exe");
            Console.WriteLine("(или #0.1 для значений меньше минут)");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    
    stopwatch.Stop();
}

async Task StartTest(Settings settings)
{
    Console.Clear();
    
    Console.WriteLine("Нажимаю на старт...");

    await Button.PressButton(settings.StartClickOffset.X, settings.WindowSize.Y + settings.StartClickOffset.Y, settings.ClickCount);
}

async Task StopTest(Settings settings)
{
    Console.Clear();
    Console.WriteLine("Таймер завершился, приступаю к нажатиям на стоп...");

    await Button.PressButton(settings.FinishClickOffset.X, settings.WindowSize.Y + settings.FinishClickOffset.Y, settings.ClickCount);
    
    Console.WriteLine("\n\nНажатия выполнены!");
    Console.WriteLine("Работа завершена, нажмите Enter чтобы выйти :)");
    Console.ReadLine();
}

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("привет!!!");

try
{
    if (!CheckAdmin()) return;
    var settings = ReadSettings();
    await Setup.SetupWindow(settings);
    await StartTest(settings);
    await RunTimer(settings);
    await StopTest(settings);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
    Console.WriteLine("\n\nКритическая ошибка, нажмите чтобы выйти");
    Console.ReadLine();
}

partial class Program
{
    [GeneratedRegex(@"#([\d.]+)\s*#([\d.]+)")]
    private static partial Regex FilenameSettingsRegex();
}
