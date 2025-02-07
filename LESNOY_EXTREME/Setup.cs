using System.Diagnostics;

namespace LESNOY_EXTREME;

public static class Setup
{
    public static async Task SetupWindow(Settings settings)
    {
        Console.Clear();
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

            Console.Write("Перемещаю окно консоли...   ");
            var moveConsoleResult = await MoveConsole();
            if (!moveConsoleResult)
                continue;
            Console.WriteLine("Перемещено");

            Console.Write("Запускаю аиду...   ");
            var launchAppResult = await LaunchApp();
            if (!launchAppResult)
                continue;
            Console.WriteLine("Запущено");

            await Task.Delay(TimeSpan.FromMilliseconds(200));

            Console.Write("Ожидаю окно подтверждения...   ");
            var confirmResult = await Confirm();
            if (!confirmResult)
                continue;
            
            Console.Write("Настраиваю главное окно...   ");
            var mainWindowResult = await MainWindow();
            if (!mainWindowResult)
                continue;
            Console.WriteLine("Настроено");
            
            Console.Write("Открываю окно стресс-теста...   ");
            await Button.PressButton(195, 50, 2);
            Console.WriteLine("Открыто");
            
            Console.Write("Настраиваю окно стресс-теста...   ");
            var stressWindowResult = await StressWindow(settings);
            if (!stressWindowResult)
                continue;
            Console.WriteLine("Настроено");
            
            Console.Write("Нажимаю галочки...   ");
            await Button.PressButton(55, 110, 1);
            await Button.PressButton(55, 125, 1);
            Console.WriteLine("Нажаты");
            
            Console.Write("Ожидаю окно Warning...  ");
            var warningResult = await Warning(settings);
            if (!warningResult)
                continue;
            
            Console.Write("Блокирую окно консоли...   ");
            var lockConsoleResult = await LockConsole(settings);
            if (!lockConsoleResult)
                continue;
            Console.WriteLine("Заблокировано");
            
            Console.WriteLine("Этап подготовки успешно завершен!");
            await Task.Delay(TimeSpan.FromSeconds(1));
            
            setupResult = true;

        } while (!setupResult);
    }

    private static async Task<bool> Warning(Settings settings)
    {
        var warningWindow = await FindWindow("Warning", TimeSpan.FromSeconds(5));

        if (warningWindow != IntPtr.Zero)
        {
            var warningSizeResult = 
                Window.SetWindowPos(warningWindow, 0, 0);
                
            if (warningSizeResult is false)
            {
                Console.WriteLine("Не удалось переместить окно подтверждения галочки Stress GPU");
                return false;
            }
            
            await Task.Delay(TimeSpan.FromSeconds(0.5));
            await Button.PressButton(300, 225, 1);
            
            await Task.Delay(TimeSpan.FromSeconds(0.5));
            await Button.PressButton(300, 215, 1);
            
            await Task.Delay(TimeSpan.FromSeconds(0.5));
            await Button.PressButton(300, 205, 1);

            Console.WriteLine("Закрыто");
            return true;
        }

        Console.WriteLine("Не найдено окно с подтверждением галочки Stress GPU. Продолжаем так");
        return true;
    }

    private static async Task<bool> MainWindow()
    {
        var mainWindow = await FindWindow("AIDA64 Extreme v7.20.6802", TimeSpan.FromMinutes(1));

        await Task.Delay(TimeSpan.FromSeconds(5));
            
        if (mainWindow == IntPtr.Zero)
        {
            Console.WriteLine("Не могу найти начальное окно аиды");
            return false;
        }
        
        var mainWindowPosResult = 
            Window.SetWindowPos(mainWindow, 0, 0);
                
        if (mainWindowPosResult is false)
        {
            Console.WriteLine("Не могу переместить начальное окно аиды");
            return false;
        }
            
        var mainFocusResult = Window.Focus(mainWindow);
        if (mainFocusResult is false)
        {
            Console.WriteLine("Не могу дать фокус на начальное окно аиды");
            return false;
        }

        return true;
    }

    private static async Task<bool> StressWindow(Settings settings)
    {
        var window = await FindWindow(settings.WindowName, TimeSpan.FromMinutes(1));

        if (window == IntPtr.Zero)
        {
            Console.WriteLine("Не могу найти окно стресс теста аиды");
            return false;
        }
            
        Window.RemoveBorders(window);
            
        var minimizeResult = Window.Minimize(window);
        if (!minimizeResult)
        {
            Console.WriteLine("Не могу свернуть окно аиды!");
            return false;
        }
            
        var maximizeResult = Window.Maximize(window);
        if (!maximizeResult)
        {
            Console.WriteLine("Не могу раскрыть окно аиды!");
            return false;
        }

        var sizeResult = Window.SetWindowPos(window, 0, 0, settings.WindowSize.X, settings.WindowSize.Y);
        if (!sizeResult)
        {
            Console.WriteLine("Не могу установить размер окна аиды!");
            return false;
        }

        var topmostResult = Window.MakeTopmost(window);
        if (!topmostResult)
        {
            Console.WriteLine("Не могу выставить окно аиды поверх остальных окон!");
            return false;
        }

        return true;
    }

    private static async Task<bool> MoveConsole()
    {
        var current = Window.GetCurrentWindow();
        if (current == IntPtr.Zero)
        {
            Console.WriteLine("Не могу найти окно консоли!");
            return false;
        }
        
        Window.SetWindowPos(current,800, 0, 800, 500);

        return true;
    }
    
    private static async Task<bool> LockConsole(Settings setting)
    {
        var current = Window.GetCurrentWindow();
        if (current == IntPtr.Zero)
        {
            Console.WriteLine("Не могу найти окно консоли!");
            return false;
        }
            
        var currentTopmostResult = Window.MakeTopmost(current);
        if (!currentTopmostResult)
        {
            Console.WriteLine("Не могу установить окно консоли поверх остальных окон!");
            return false;
        }
            
        var quickResult = Window.DisableQuickEdit();
        if (!quickResult)
        {
            Console.WriteLine("Не могу выключить редактирование в окне консоли!");
            return false;
        }
            
        var focusResult = Window.Focus(current);
        var topResult = Window.BringWindowToTop(current);
        var placeResult = Window.SetWindowPos(current,800, 0, 800, 500);
        
        return true;
    }

    private static async Task<bool> LaunchApp()
    {
        var file = new DirectoryInfo(Environment.CurrentDirectory).EnumerateFiles()
            .FirstOrDefault(file => file.Name.Contains("aida") && file.Extension is ".exe" or ".lnk");
        
        if (file is null)
        {
            Console.WriteLine("Не могу найти файл аиды в директории со скриптом. " +
                              "файл должен иметь \"aida\" в названии иметь расширение .exe, либо быть ярлыком на .exe ");
            return false;
        }
        
        var startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe", 
            Arguments = $"/c \"{file.Name}\"",
            RedirectStandardError = true,
        };

        try
        {
            var process = Process.Start(startInfo);
            if (process is null)
            {
                Console.WriteLine("Процесс аиды не смог запуститься");
                return false;
            }

            // ахахахахахахаххахахахахаххахахахахах
            await Task.Delay(TimeSpan.FromMilliseconds(200));
                
            if (process.HasExited)
            {
                var stdErr = process.StandardError.ReadToEnd();
                Console.WriteLine("Процесс аиды завершился: " + stdErr);
                return false;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Process.Start выкинул исключение: {e.Message}");
            return false;
        }
        
        return true;
    }

    private static async Task<bool> Confirm()
    {
        var confirmWindow = await FindWindow("Open File - Security Warning", TimeSpan.FromSeconds(10));
            
        if (confirmWindow != IntPtr.Zero)
        {
            var confirmSizeResult = 
                Window.SetWindowPos(confirmWindow, 0, 0);
                
            if (confirmSizeResult is false)
            {
                Console.WriteLine("Не удалось переместить окно подтверждения открытия аиды");
                return false;
            }
                
            await Task.Delay(TimeSpan.FromMilliseconds(500));
            await Button.PressButton(280, 220, 4);
            
            Console.WriteLine("Подтверждено");
        }
        else
        {
            Console.WriteLine("Не найдено окно с подтверждением открытия аиды. Продолжаем так");
        }
        
        return true;
    }
    
    private static async Task<IntPtr> FindWindow(string windowName, TimeSpan maxWait)
    {
        IntPtr window;
        var attemptDelay = TimeSpan.FromMilliseconds(200);
        var totalTime = TimeSpan.Zero;
        do
        {
            window = Window.Find(windowName);
            if (window != IntPtr.Zero)
                continue;
                
            totalTime += attemptDelay;
            await Task.Delay(attemptDelay);
        } while (window == IntPtr.Zero && totalTime < maxWait);

        return window;
    }
}