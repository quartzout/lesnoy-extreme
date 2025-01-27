using System.Diagnostics;
using Core.Abstractions;
using Core.Abstractions.Models;
using Core.Options;
using Microsoft.Extensions.Options;
using WindowsApi.Abstractions;

namespace Core;

internal class Runner(
    IWindowManager windowManager,
    IWinRightsService winRightsService,
    ISetupStep setupStep,
    IOptions<StepsOptions> optionsAccessor) : IRunner
{
    private readonly StepsOptions _stepsOptions = optionsAccessor.Value;
    
    public async Task<RunnerResult> RunToCompletion(CancellationToken token)
    {
        if (_stepsOptions.DoAdminCheck && !winRightsService.IsProcessAdmin()) 
            return RunnerResult.Error.AdminRightsRequired;

        await setupStep.RunToCompletion();
        await StartTest();
        await RunTimer();
        await StopTest();
        return new RunnerResult.Success();
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