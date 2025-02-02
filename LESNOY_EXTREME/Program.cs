using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
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

async Task SetupWindow(Settings settings)
{
    var setupResult = false;
    var firstAttempt = true;
    do
    {
        if (!firstAttempt)
        {
            Console.WriteLine();
            Console.WriteLine("перезапустите скрипт");
            Console.WriteLine("нажмите Enter чтобы повторить попытку");
            Console.ReadLine();
            Console.Clear();
        }

        firstAttempt = false;
        setupResult = false;
        
        var exeName = settings.PossibleFilenames.FirstOrDefault(filename => 
            File.Exists(Path.Combine(Environment.CurrentDirectory, filename)));
        if (exeName is null)
        {
            Console.WriteLine("Не могу найти exe аиды в директории со скриптом. " +
                              "Exe должен называться одиим из следующих имен: " +
                              $"{string.Join(',', settings.PossibleFilenames)}");
            continue;
        }

        Console.ReadLine();

        var startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe", 
            Arguments = $"/c \"{exeName}\"",
            RedirectStandardError = true,
        };

        Process? process;
        try
        {
            process = Process.Start(startInfo);
            if (process is null)
            {
                Console.WriteLine("Процесс аиды не смог запуститься");
                continue;
            }

            // ахахахахахахаххахахахахаххахахахахах
            await Task.Delay(TimeSpan.FromMilliseconds(200));
            
            if (process.HasExited)
            {
                var stdErr = process.StandardError.ReadToEnd();
                Console.WriteLine("Процесс аиды завершился: " + stdErr);
                continue;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Process.Start выкинул исключение: {e.Message}");
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

        IntPtr confirmWindow;
        var attempts = 0;
        do
        {
            confirmWindow = Window.Find("Open File - Security Warning");
            if (confirmWindow != IntPtr.Zero)
            {
                continue;
            }

            await Task.Delay(TimeSpan.FromMilliseconds(100));
            attempts++;
        } while (confirmWindow == IntPtr.Zero && attempts < 5);
        

        if (confirmWindow != IntPtr.Zero)
        {
            var confirmSizeResult = 
                Window.SetWindowPos(confirmWindow, 0, 0, settings.WindowSize.X, settings.WindowSize.Y);
            
            if (confirmSizeResult is false)
            {
                Console.WriteLine("Не удалось переместить окно подтверждения открытия аиды");
                continue;
            }
            
            await Task.Delay(TimeSpan.FromMilliseconds(500));
            await PressButton(280, 220, 4);
        }
        else
        {
            Console.WriteLine("Не найдено окно с подтверждением открытия аиды. Продолжаем так");
        }

        IntPtr mainWindow;
        var aidaSearchAttemptDelay = TimeSpan.FromMilliseconds(200);
        var aidaAttempts = TimeSpan.Zero;
        do
        {
            mainWindow = Window.Find("AIDA64 Extreme v7.50.7200");
            if (mainWindow != IntPtr.Zero)
                continue;
            
            aidaAttempts += aidaSearchAttemptDelay;
            await Task.Delay(aidaSearchAttemptDelay);
        } while (mainWindow == IntPtr.Zero && aidaSearchAttemptDelay < TimeSpan.FromMinutes(1));

        await Task.Delay(TimeSpan.FromSeconds(5));
        
        if (mainWindow == IntPtr.Zero)
        {
            Console.WriteLine("не могу найти окно аиды!!!");
            continue;
        }
        
        Console.WriteLine("аида найдена спасибо!");
        
        var mainWindowPosResult = 
            Window.SetWindowPos(mainWindow, 0, 0, settings.WindowSize.X, settings.WindowSize.Y);
            
        if (mainWindowPosResult is false)
        {
            Console.WriteLine("Не удалось переместить начальное окно аиды");
            continue;
        }
        
        var mainFocusResult = Window.Focus(mainWindow);
        if (mainFocusResult is false)
        {
            Console.WriteLine("Ошибка фокуса начального окна аиды");
            continue;
        }
        
        await Task.Delay(TimeSpan.FromMilliseconds(200));
        
        await PressButton(195, 50, 2);
        
        IntPtr window;
        var aidaWindowSearchAttemptDelay = TimeSpan.FromMilliseconds(200);
        var aidaWindowAttempts = TimeSpan.Zero;
        do
        {
            window = Window.Find(settings.WindowName);
            if (window != IntPtr.Zero)
                continue;
            
            aidaWindowAttempts += aidaWindowSearchAttemptDelay;
            await Task.Delay(aidaWindowSearchAttemptDelay);
        } while (window == IntPtr.Zero && aidaWindowAttempts < TimeSpan.FromMinutes(1));

        if (window == IntPtr.Zero)
        {
            Console.WriteLine("Не могу найти окно стресс теста аиды");
            continue;
        }
        
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
    Console.WriteLine("Нажимаю галочки...");
    
    await PressButton(55, 110, 1);
    await PressButton(55, 125, 1);

    await Task.Delay(TimeSpan.FromDays(1));
    
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
    await SetupWindow(settings);
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
