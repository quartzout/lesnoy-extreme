using System.Diagnostics;
using System.Security.Principal;
using Microsoft.Extensions.Options;
using Steps.Options;
using WindowManager;

namespace Steps;

public class MarcoStep(
    IWindowManager windowManager,
    IOptions<StepsOptions> optionsAccessor)
{
    
    private readonly StepsOptions _stepsOptions = optionsAccessor.Value;
    
    public async Task Run(CancellationToken token)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("привет!!!");

        try
        {
            if (_stepsOptions.DoOsSpecificChecks && !CheckAdmin()) return;

            SetupWindow();
            await StartTest();
            await RunTimer();
            await StopTest();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine("\n\nвы ошибка, нажмите чтобы выйти");
            Console.ReadLine();
        }
    }
    
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

    async Task RunShutdownTimer()
    {
        Console.Clear();
        Console.WriteLine("Запускаю экстренный таймер на выключение компьютера...");
        
        var stopwatch = Stopwatch.StartNew();
        var periodicTimer = new PeriodicTimer(_stepsOptions.TimerInterval);

        while (await periodicTimer.WaitForNextTickAsync() && stopwatch.Elapsed < _stepsOptions.ShutdownDelay)
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("ОКНО АИДЫ НЕ НАЙДЕНО!!!");
            Console.WriteLine("тест прерван, запущен таймер на выключение компьютера");
            Console.WriteLine($@"до выключения осталось: {_stepsOptions.ShutdownDelay - stopwatch.Elapsed:mm\m\ ss\s\ f}");
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

    void SetupWindow()
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
            
            Console.WriteLine($"пытаюсь найти окно \"{_stepsOptions.WindowName}\" ");
            var window = windowManager.Find(_stepsOptions.WindowName);
            if (window == IntPtr.Zero)
            {
                Console.WriteLine("не могу найти нужное окно!!!");
                continue;
            }
            
            Console.WriteLine("аида найдена спасибо!");
            
            windowManager.RemoveBorders(window);
            
            var minimizeResult = windowManager.Minimize(window);
            if (!minimizeResult)
            {
                Console.WriteLine("не могу скрыть аиду окно по неизвестной ошибке!");
                continue;
            }
            
            var maximizeResult = windowManager.Maximize(window);
            if (!maximizeResult)
            {
                Console.WriteLine("не могу раскрыть аиду окно на полный по неизвестной ошибке!");
                continue;
            }

            var sizeResult = windowManager.SetWindowPos(window, 0, 0, _stepsOptions.WindowSize.X, _stepsOptions.WindowSize.Y);
            if (!sizeResult)
            {
                Console.WriteLine("не могу установить размер аиды окна по неизвестной ошибке!");
                continue;
            }

            var topmostResult = windowManager.MakeTopmost(window);
            if (!topmostResult)
            {
                Console.WriteLine("не могу сделать аиду окно на поверх остальных окон по неизвестной ошибке!");
                continue;
            }
            
            var current = windowManager.GetCurrentWindow();
            if (current == IntPtr.Zero)
            {
                Console.WriteLine("не могу найти окно консоли скрипта!!!");
                continue;
            }
            
            var currentTopmostResult = windowManager.MakeTopmost(current);
            if (!currentTopmostResult)
            {
                Console.WriteLine("не могу сделать скрипта окно на поверх остальных окон по неизвестной ошибке!");
                continue;
            }
            
            var quickResult = windowManager.DisableQuickEdit();
            if (!quickResult)
            {
                Console.WriteLine("не могу выключить редактирование в окно скрипта по неизвестной ошибке!");
                continue;
            }
            
            var focusResult = windowManager.Focus(current);
            var topResult = windowManager.BringWindowToTop(current);
            var placeResult = windowManager.SetWindowPos(current, _stepsOptions.WindowSize.X, 0, _stepsOptions.ConsoleWindowSize.X,
                _stepsOptions.ConsoleWindowSize.Y);

            setupResult = true;

        } while (!setupResult);
    }

    async Task RunTimer()
    {
        Console.Clear();
        
        var stopwatch = Stopwatch.StartNew();
        using PeriodicTimer timer = new(TimeSpan.FromMilliseconds(50));
        
        while (await timer.WaitForNextTickAsync() && stopwatch.Elapsed < _stepsOptions.TestDuration)
        {
            try
            {
                var window = windowManager.Find(_stepsOptions.WindowName);
                if (window == IntPtr.Zero)
                {
                    await RunShutdownTimer();
                }

                Console.SetCursorPosition(0, 0);
                Console.WriteLine("проверки успешны, окно аиды известно, таймер запущен!");
                Console.WriteLine(
                    $@"прошло времени: {stopwatch.Elapsed:mm\m\ ss\s\ f} / {_stepsOptions.TestDuration:mm\m\ ss\s\ f}");

                var current = windowManager.GetCurrentWindow();
                if (current == IntPtr.Zero)
                {
                    Console.WriteLine("\n !!! не могу найти окно консоли скрипта!!! впринципе пофиг");
                }
                var focusResult = windowManager.Focus(current);
                if (!focusResult)
                {
                    //Console.WriteLine("\n !!! не могу дать фокус на окно консоли скрипта!!! впринципе пофиг");
                }

                var topResult = windowManager.BringWindowToTop(current);
                if (!topResult)
                {
                    Console.WriteLine("\n !!! не могу вывести на передний план окно консоли скрипта!!! впринципе пофиг");
                }
                
                var placeResult = windowManager.SetWindowPos(current, _stepsOptions.WindowSize.X, 0, _stepsOptions.ConsoleWindowSize.X,
                    _stepsOptions.ConsoleWindowSize.Y);
                if (!placeResult)
                {
                    Console.WriteLine(
                        "\n !!! не могу зафиксировать положения окно консоли скрипта!!! следите чтобы окно не перекрывало кнопку Stop аиды!!");
                }

                Console.WriteLine("\n\nнастройки:");
                Console.WriteLine($"\tпродолжительность теста: {_stepsOptions.TestDuration:mm\\m\\ ss\\s}");
                Console.WriteLine($"\tпри ошибке выключение компа через {_stepsOptions.ShutdownDelay:mm\\m\\ ss\\s}");
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

    async Task StartTest()
    {
        Console.Clear();
        Console.WriteLine("нажимаю на старт...");

        await PressButton(_stepsOptions.StartClickOffset.X, _stepsOptions.WindowSize.Y + _stepsOptions.StartClickOffset.Y, _stepsOptions.ClickCount);
    }

    async Task StopTest()
    {
        Console.Clear();
        Console.WriteLine("таймер завершился, приступаю к нажатиям на стоп...");

        await PressButton(_stepsOptions.FinishClickOffset.X, _stepsOptions.WindowSize.Y + _stepsOptions.FinishClickOffset.Y, _stepsOptions.ClickCount);
        
        Console.WriteLine("\n\nнажатия завершился!");
        Console.WriteLine("работа завершена, нажмите Enter чтобы выйти :)");
        Console.ReadLine();
    }

    async Task PressButton(int x, int y, int clickCount)
    {
        for (var clicksDone = 0; clicksDone < clickCount; clicksDone++)
        {
            Console.WriteLine($"Клик {clicksDone + 1}...");
            windowManager.Click(x, y);
            await Task.Delay(TimeSpan.FromSeconds(0.5));
        }
    }
}