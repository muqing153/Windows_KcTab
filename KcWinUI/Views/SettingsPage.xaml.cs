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
        //获取目录下的学号.json
        if (!Directory.Exists(wjj))
        {
            Directory.CreateDirectory(wjj);
        }
        // 获取所有匹配 “学号.json” 格式的文件
        var jsonFiles = Directory.GetFiles(wjj, "*.json", SearchOption.TopDirectoryOnly)
                                 .ToList();
        foreach (var file in jsonFiles)
        {
            Debug.WriteLine(file);
            var name = Path.GetFileNameWithoutExtension(file);
            var item = new ComboBoxItem()
            {
                Content = name,
                Tag = file
            };
            xuenian_box.Items.Add(item);
        }
        if (jsonFiles.Count==0)
        { 
            xuenian_box.IsEnabled = false;
        }
        else
        {
          var user =  iniHelper.Read("user", "username", "");
            if (user != "")
            {
                var item = xuenian_box.Items.Cast<ComboBoxItem>().FirstOrDefault(i => i.Content.ToString() == user);
                if (item != null)
                {
                    xuenian_box.SelectedItem = item;
                }
            }
        }
        xuenian_box.SelectionChanged += (s, e) =>
        {
            var item = xuenian_box.SelectedItem as ComboBoxItem;
            iniHelper.Write("user", "username", item?.Content.ToString() ?? "");
        };
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
        var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        var shortcutPath = Path.Combine(desktopPath, "一柚表.lnk");

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
