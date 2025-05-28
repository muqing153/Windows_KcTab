using IWshRuntimeLibrary;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Threading;
using KcTabWindow.UI;
using KcTabWindow.ViewData;
using Windows.ApplicationModel.Activation;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
using Wpf.Ui;
using Path = System.IO.Path;
namespace KcTabWindow;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    // The.NET Generic Host provides dependency injection, configuration, logging, and other services.
    // https://docs.microsoft.com/dotnet/core/extensions/generic-host
    // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
    // https://docs.microsoft.com/dotnet/core/extensions/configuration
    // https://docs.microsoft.com/dotnet/core/extensions/logging
    private static readonly IHost _host = Host
        .CreateDefaultBuilder()
        .ConfigureAppConfiguration(c =>
        {
            c.SetBasePath(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location));
        })
        .ConfigureServices((context, services) =>
        {
            // 注册应用程序的服务，例如视图和视图模型
            services.AddSingleton<MainWindow>();  // 注册主窗口
            // 注册其他服务
            // services.AddSingleton<YourService>();
        }).Build();

    /// <summary>
    /// Gets registered service.
    /// </summary>
    /// <typeparam name="T">Type of the service to get.</typeparam>
    /// <returns>Instance of the service or <see langword="null"/>.</returns>
    public static T GetService<T>()
        where T : class
    {
        return _host.Services.GetService(typeof(T)) as T;
    }

    private static Mutex mutex;

    /// <summary>
    /// Occurs when the application is loading.
    /// </summary>
    private async void OnStartup(object sender, StartupEventArgs e)
    {

        mutex = new Mutex(true, "KcTabWindow", out bool ret);
        if (!ret)
        {
            Environment.Exit(0);
            return;
        }
        // 创建快捷方式

        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string shortcutPath = Path.Combine(desktopPath, "课程表.lnk"); // 快捷方式路径
        string targetPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "KcTabWindow.exe");
        if (!System.IO.File.Exists(shortcutPath))
        {
            WshShell shell = new();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
            shortcut.TargetPath = targetPath;
            shortcut.Description = "课程表";
            shortcut.Save();
        }

        // 显示主窗口

        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
        _host.Start();

        // Listen to notification activation
        ToastNotificationManagerCompat.OnActivated += toastArgs =>
        {
            // 获取传递的参数
            string argument = toastArgs.Argument;
            // 解析参数
            var args = ToastArguments.Parse(argument);

            // 检查特定的操作
            if (args.Contains("action") && args["action"] == "viewConversation")
            {
                // 获取 conversationId
                if (args.Contains("conversationId"))
                {
                    int conversationId = int.Parse(args["conversationId"]);
                    // 处理你的逻辑，例如打开对应的对话
                    // 获取 imagePath
                    string imagePath = args.Contains("imagePath") ? args["imagePath"] : string.Empty;

                    Application.Current.Dispatcher.Invoke(() =>
                    {

                        // 使用系统默认程序打开图片
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = imagePath,
                            UseShellExecute = true // 需要设置为 true 来使用系统默认程序
                        });
                        // 使用之前存储的图片路径
                        //MessageBox.Show($"查看对话: {conversationId}\n图片路径: {imagePath}");
                    });
                }
            }
        };
    }

    /// <summary>
    /// Occurs when the application is closing.
    /// </summary>
    private async void OnExit(object sender, ExitEventArgs e)
    {
        await _host.StopAsync();

        _host.Dispose();
    }

    /// <summary>
    /// Occurs when an exception is thrown by an application but not handled.
    /// </summary>
    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        // For more info see https://docs.microsoft.com/en-us/dotnet/api/system.windows.application.dispatcherunhandledexception?view=windowsdesktop-6.0
    }


}
