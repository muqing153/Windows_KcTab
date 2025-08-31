using System.ComponentModel;
using System.Diagnostics;
using KcWinUI.Helpers;
using KcWinUI.Models;
using KcWinUI.Services;
using KcWinUI.Views;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.UI.ViewManagement;

namespace KcWinUI;
public sealed partial class MainWindow : WindowEx
{
    private readonly Microsoft.UI.Dispatching.DispatcherQueue dispatcherQueue;

    private readonly UISettings settings;
    public string? Selected;

    public event PropertyChangedEventHandler? PropertyChanged;

    public static List<string> listPath = new();
    public MainWindow()
    {
        InitializeComponent();
        //Init();
        AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/WindowIcon.ico"));
        //Content = null;
        Title = "AppDisplayName".GetLocalized();
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
        AppTitleBarText.Text = "AppDisplayName".GetLocalized() + " PC";//+ "app_version".GetLocalized();
        // Theme change code picked from https://github.com/microsoft/WinUI-Gallery/pull/1239
        dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();
        settings = new UISettings();
        settings.ColorValuesChanged += Settings_ColorValuesChanged; // cannot use FrameworkElement.ActualThemeChanged event
    }

    private void nvSample_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
    {
        var item = args.InvokedItemContainer as NavigationViewItem;
        if (item == null)
        {
            return;
        }
        var pageTag = item.Tag?.ToString();
        if (!string.IsNullOrEmpty(pageTag))
        {
            // 拼命名空间 + 类名
            var pageNamespace = "KcWinUI.Views"; // 比如 MyApp.Views 或 MyApp.Pages
            var pageType = Type.GetType($"{pageNamespace}.{pageTag}");

            if (pageType != null)
            {
                if (pageTag == "LoginPage")
                {
                    ContentFrame.Navigate(pageType, nvSample);
                }
                else
                {
                    ContentFrame.Navigate(pageType);
                }
            }
        }

        if (args.IsSettingsInvoked)
        {
            ContentFrame.Navigate(typeof(SettingsPage));
        }
        //清理缓存
        //nvSample.Header = item.Content;
    }

    // this handles updating the caption button colors correctly when indows system theme is changed
    // while the app is open
    private void Settings_ColorValuesChanged(UISettings sender, object args)
    {
        // This calls comes off-thread, hence we will need to dispatch it to current app's thread
        dispatcherQueue.TryEnqueue(() =>
        {
            TitleBarHelper.ApplySystemThemeToCaptionButtons();
            SettingsPage.SetBackground(new IniHelper("setting.ini").Read("theme", "background", "Mica"));
        });
    }

    private void NavigationViewControl_DisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs args)
    {
        AppTitleBar.Margin = new Thickness()
        {
            Left = sender.CompactPaneLength * (sender.DisplayMode == NavigationViewDisplayMode.Minimal ? 2 : 1),
            //Left = sender.CompactPaneLength,
            Top = AppTitleBar.Margin.Top,
            Right = AppTitleBar.Margin.Right,
            Bottom = AppTitleBar.Margin.Bottom
        };
    }
    public static string GetSchoolYearTerm(DateTime date)
    {
        int year = date.Year;
        int month = date.Month;

        string schoolYear;
        string term;

        if (month >= 8)
        {
            // 8月 ~ 12月：新学年上学期
            schoolYear = year + "-" + (year + 1);
            term = "1";
        }
        else if (month == 1)
        {
            // 1月：依然是上学期，但属于上一年的学年
            schoolYear = (year - 1) + "-" + year;
            term = "1";
        }
        else
        {
            // 2月 ~ 7月：是下学期，属于上一个学年
            schoolYear = (year - 1) + "-" + year;
            term = "2";
        }

        return schoolYear + "-" + term;
    }

    private async void nvSample_Loaded(object sender, RoutedEventArgs e)
    {
        var wjj = Path.Combine(AppContext.BaseDirectory, "TabList");
        //Debug.WriteLine(wjj);
        if (!Directory.Exists(wjj))
        {
            Directory.CreateDirectory(wjj);
        }
        //new DateTime(2025, 2, 1)
        var inihelper = new IniHelper("setting.ini");
        var year = inihelper.Read("Table", "year", string.Empty);
        if (year == string.Empty)
        {
            year = GetSchoolYearTerm(DateTime.Now);
            inihelper.Write("Table", "year", year);
        }
        var xuenian = Path.Combine(wjj, year);
        if (!Directory.Exists(xuenian))
        {
            Directory.CreateDirectory(xuenian);
        }
        var strings = Directory.GetFiles(xuenian);
        foreach (var item in strings)
        {
            if (item.EndsWith(".txt"))
            {
                listPath.Add(item);
                Debug.WriteLine(item);
            }
        }
        if (listPath.Count == 0)
        {
            nvSample.SelectedItem = nvSample.MenuItems[1];
            ContentFrame.Navigate(typeof(LoginPage),nvSample);
            return;
        }
        TableView.IsEnabled = true;
        //StartPage();
        nvSample.SelectedItem = nvSample.MenuItems[0];

        ContentFrame.Navigate(typeof(TablePage));
        return;
    }
}
