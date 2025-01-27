using Core.Abstractions.Events.Stages;
using Core.Options;
using Microsoft.Extensions.Options;
using WindowsApi.Abstractions;

namespace Core;

public class SetupStep(
    IWindowManager windowManager, 
    IRunEventPublisher publisher,
    IOptions<StepsOptions> options) : ISetupStep
{
    private readonly StepsOptions _options = options.Value;

    public async Task RunToCompletion()
    {
        await publisher.PublishEvent(new SetupEvent.Started());
        
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
            
            Console.WriteLine($"пытаюсь найти окно \"{_options.WindowName}\" ");
            var window = windowManager.Find(_options.WindowName);
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

            var sizeResult = windowManager.SetWindowPos(window, 0, 0, _options.WindowSize.X, _options.WindowSize.Y);
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
            var placeResult = windowManager.SetWindowPos(current, _options.WindowSize.X, 0, _options.ConsoleWindowSize.X, _options.ConsoleWindowSize.Y);

            setupResult = true;

        } while (!setupResult);
    }
}