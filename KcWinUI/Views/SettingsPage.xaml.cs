using System;
using System.Diagnostics;
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

    private void beijing_box_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {

    }
}
