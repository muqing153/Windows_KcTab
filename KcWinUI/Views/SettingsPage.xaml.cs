using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using IWshRuntimeLibrary;
using KcWinUI.Helpers;
using KcWinUI.ViewModels;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace KcWinUI.Views;

// TODO: Set the URL for your privacy policy by updating SettingsPage_PrivacyTermsLink.NavigateUri in Resources.resw.
public sealed partial class SettingsPage : Page
{
    public SettingsViewModel ViewModel
    {
        get;
    }
    public readonly IniHelper iniHelper = new("setting.ini");
    public SettingsPage()
    {
        ViewModel = App.GetService<SettingsViewModel>();
        InitializeComponent();
        beijing_box.SelectedIndex = SetBackground(iniHelper.Read("theme", "background", "Mica"));
        beijing_box.SelectionChanged += (s, e) =>
        {
            var v = beijing_box.SelectedItem as string;
            SetBackground(v);
            iniHelper.Write("theme", "background", v);
        };
        var wjj = Path.Combine(AppContext.BaseDirectory, "TabList");
        //获取目录下的年份文件夹
        var pattern = @"^(?<start>\d{4})-(?<end>\d{4})-(?<index>\d+)$";
        var regex = new Regex(pattern);

        var sortedFolders = Directory.GetDirectories(wjj)
            .Select(Path.GetFileName)
            .Select(name =>
            {
                var match = regex.Match(name);
                if (!match.Success) return null;

                int start = int.Parse(match.Groups["start"].Value);
                int end = int.Parse(match.Groups["end"].Value);
                int index = int.Parse(match.Groups["index"].Value);

                // 校验 end == start + 1
                if (end != start + 1) return null;
                return new
                {
                    Name = name,
                    StartYear = start,
                    Index = index
                };
            })
            .Where(x => x != null)
            .OrderBy(x => x.StartYear) // 先按起始年份排序
            .ThenBy(x => x.Index)      // 再按序号排序
            .Select(x => x.Name);
        var year= iniHelper.Read("Table", "year", string.Empty);
        if (year != string.Empty && sortedFolders.Count<string>()>0)
        {
            xuenian_box.ItemsSource = sortedFolders;
            xuenian_box.SelectedItem = year;
        }
    }
    public static int SetBackground(string str)
    {
        switch (str)
        {
            case "Mica":
                App.MainWindow.SystemBackdrop = new MicaBackdrop
                {
                    Kind = MicaKind.Base
                };
                return 0;
            case "MicaAlt":
                App.MainWindow.SystemBackdrop = new MicaBackdrop
                {
                    Kind = MicaKind.BaseAlt
                };
                return 1;
            case "Acrylic":
                var DesktopAcrylicBackdrop = new DesktopAcrylicBackdrop();
                App.MainWindow.SystemBackdrop = DesktopAcrylicBackdrop;
                return 2;
            default:
                App.MainWindow.SystemBackdrop = null;
                return 3;
        }
    }


    private void HyperlinkButton_Click_newtubiao(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        string shortcutPath = Path.Combine(desktopPath, "一柚表.lnk");
        var appPath = Path.Combine(AppContext.BaseDirectory, "KcWinUI.exe");
        // 创建 WshShell 对象
        WshShell shell = new WshShell();
        IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);

        // 设置快捷方式属性
        shortcut.TargetPath = appPath;
        shortcut.WorkingDirectory = Path.GetDirectoryName(appPath);
        shortcut.WindowStyle = 1; // 1 - 普通窗口, 3 - 最大化, 7 - 最小化
        shortcut.Description = "这是我的应用程序";
        shortcut.IconLocation = appPath;
        shortcut.Save();
        Debug.WriteLine("桌面快捷方式创建完成！");
    }
}
