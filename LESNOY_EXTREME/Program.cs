using System.Diagnostics;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Timers;
using LESNOY_EXTREME;

bool CheckAdmin()
{
    var identity = WindowsIdentity.GetCurrent();
    var principal = new WindowsPrincipal(identity);
    var isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);

    if (!isAdmin)
    {
        Console.WriteLine("\n!!ошибка! пожалуймта перезапустите скрипт от имени администратора!");
        Console.WriteLine("\nнажмите Enter чтобы закрыть");
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
        Console.WriteLine("тест прерван, запущен таймер на выключение компьютера");
        Console.WriteLine($@"до выключения осталось: {settings.ShutdownDelay - stopwatch.Elapsed:mm\m\ ss\s\ f}");
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

void SetupWindow(Settings settings)
{
    var setupResult = false;
    var firstAttempt = true;
    do
    {
        if (!firstAttempt)
        {
            Console.WriteLine("перезапустите аиду и\\или скрипт");
            Console.WriteLine("нажмите Enter чтобы повторить попытку");
            Console.ReadLine();
            Console.Clear();
        }

        firstAttempt = false;
        setupResult = false;
        
        Console.WriteLine($"пытаюсь найти окно \"{settings.WindowName}\" ");
        var window = Window.Find(settings.WindowName);
        if (window == IntPtr.Zero)
        {
            Console.WriteLine("не могу найти нужное окно!!!");
            continue;
        }
        
        Console.WriteLine("аида найдена спасибо!");
        
        Window.RemoveBorders(window);
        
        var minimizeResult = Window.Minimize(window);
        if (!minimizeResult)
        {
            Console.WriteLine("не могу скрыть аиду окно по неизвестной ошибке!");
            continue;
        }
        
        var maximizeResult = Window.Maximize(window);
        if (!maximizeResult)
        {
            Console.WriteLine("не могу раскрыть аиду окно на полный по неизвестной ошибке!");
            continue;
        }

        var sizeResult = Window.SetWindowPos(window, 0, 0, settings.WindowSize.X, settings.WindowSize.Y);
        if (!sizeResult)
        {
            Console.WriteLine("не могу установить размер аиды окна по неизвестной ошибке!");
            continue;
        }

        var topmostResult = Window.MakeTopmost(window);
        if (!topmostResult)
        {
            Console.WriteLine("не могу сделать аиду окно на поверх остальных окон по неизвестной ошибке!");
            continue;
        }
        
        var current = Window.GetCurrentWindow();
        if (current == IntPtr.Zero)
        {
            Console.WriteLine("не могу найти окно консоли скрипта!!!");
            continue;
        }
        
        var currentTopmostResult = Window.MakeTopmost(current);
        if (!currentTopmostResult)
        {
            Console.WriteLine("не могу сделать скрипта окно на поверх остальных окон по неизвестной ошибке!");
            continue;
        }
        
        var quickResult = Window.DisableQuickEdit();
        if (!quickResult)
        {
            Console.WriteLine("не могу выключить редактирование в окно скрипта по неизвестной ошибке!");
            continue;
        }
        
        var focusResult = Window.Focus(current);
        var topResult = Window.BringWindowToTop(current);
        var placeResult = Window.SetWindowPos(current, settings.WindowSize.X, 0, settings.ConsoleWindowSize.X,
            settings.ConsoleWindowSize.Y);

        setupResult = true;

    } while (!setupResult);
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
            Console.WriteLine("проверки успешны, окно аиды известно, таймер запущен!");
            Console.WriteLine(
                $@"прошло времени: {stopwatch.Elapsed:mm\m\ ss\s\ f} / {settings.TestDuration:mm\m\ ss\s\ f}");

            var current = Window.GetCurrentWindow();
            if (current == IntPtr.Zero)
            {
                Console.WriteLine("\n !!! не могу найти окно консоли скрипта!!! впринципе пофиг");
            }
            var focusResult = Window.Focus(current);
            if (!focusResult)
            {
                //Console.WriteLine("\n !!! не могу дать фокус на окно консоли скрипта!!! впринципе пофиг");
            }

            var topResult = Window.BringWindowToTop(current);
            if (!topResult)
            {
                Console.WriteLine("\n !!! не могу вывести на передний план окно консоли скрипта!!! впринципе пофиг");
            }
            
            var placeResult = Window.SetWindowPos(current, settings.WindowSize.X, 0, settings.ConsoleWindowSize.X,
                settings.ConsoleWindowSize.Y);
            if (!placeResult)
            {
                Console.WriteLine(
                    "\n !!! не могу зафиксировать положения окно консоли скрипта!!! следите чтобы окно не перекрывало кнопку Stop аиды!!");
            }

            Console.WriteLine("\n\nнастройки:");
            Console.WriteLine($"\tпродолжительность теста: {settings.TestDuration:mm\\m\\ ss\\s}");
            Console.WriteLine($"\tпри ошибке выключение компа через {settings.ShutdownDelay:mm\\m\\ ss\\s}");
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
    Console.WriteLine("нажимаю на старт...");

    await PressButton(settings.StartClickOffset.X, settings.WindowSize.Y + settings.StartClickOffset.Y, settings.ClickCount);
}

async Task StopTest(Settings settings)
{
    Console.Clear();
    Console.WriteLine("таймер завершился, приступаю к нажатиям на стоп...");

    await PressButton(settings.FinishClickOffset.X, settings.WindowSize.Y + settings.FinishClickOffset.Y, settings.ClickCount);
    
    Console.WriteLine("\n\nнажатия завершился!");
    Console.WriteLine("работа завершена, нажмите Enter чтобы выйти :)");
    Console.ReadLine();
}

async Task PressButton(int x, int y, int clickCount)
{
    for (var clicksDone = 0; clicksDone < clickCount; clicksDone++)
    {
        Console.WriteLine($"Клик {clicksDone + 1}...");
        Window.Click(x, y);
        await Task.Delay(TimeSpan.FromSeconds(0.5));
    }
}

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("привет!!!");

try
{
    if (!CheckAdmin()) return;
    var settings = ReadSettings();
    SetupWindow(settings);
    await StartTest(settings);
    await RunTimer(settings);
    await StopTest(settings);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
    Console.WriteLine("\n\nвы ошибка, нажмите чтобы выйти");
    Console.ReadLine();
}

partial class Program
{
    [GeneratedRegex(@"#([\d.]+)\s*#([\d.]+)")]
    private static partial Regex FilenameSettingsRegex();
}
