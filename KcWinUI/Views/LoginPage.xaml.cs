

using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using KcWinUI.Helpers;
using KcWinUI.Models;
using KcWinUI.Services;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Newtonsoft.Json;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace KcWinUI.Views;
/// <summary>
/// LoginPage.xaml 的交互逻辑
/// </summary>
public partial class LoginPage : Page
{
    public LoginPage()
    {
        InitializeComponent();
        var iniHelper = new IniHelper("setting.ini");
        var x = iniHelper.Read("user", "xieyi", "False").Equals("True");
        checkbox.IsChecked = x;
        checkbox.Click += (s, e) =>
        {
            xieyi((bool)checkbox.IsChecked);
            iniHelper.Write("user", "xieyi", checkbox.IsChecked.ToString());
        };
        xieyi(x);
    }
    private void xieyi(bool isChecked)
    {
        StartButton.IsEnabled = isChecked;
        KcWebButton.IsEnabled = isChecked;
        KcTokenButton.IsEnabled = isChecked;
    }
    private NavigationView navigationView;
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is NavigationView nav)
        {
            navigationView = nav;
            Debug.WriteLine("LoginPage 收到 NavigationView 参数");
        }
    }
    private async void StartButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (Toast.IsOpen)
        {
            Toast.IsOpen = false;
            await Task.Delay(300);
        }
        if (string.IsNullOrWhiteSpace(account.Text))
        {
            Toast.Subtitle = "请输入账号";
            Toast.IsOpen = true;
            return;
        }
        if (string.IsNullOrWhiteSpace(password.Password))
        {
            Toast.Subtitle = "请输入密码";
            Toast.IsOpen = true;
            return;
        }
        StartButton.IsEnabled = false;
        var token = await Api.LoginToken(account.Text, password.Password);
        if (token == string.Empty)
        {
            Toast.Subtitle = "登录失败";
            Toast.IsOpen = true;
            return;
        }
        Api.Token = token;
        Toast.Subtitle = "登录成功";
        Toast.IsOpen = true;
        var wjj = System.IO.Path.Combine(AppContext.BaseDirectory, "TabList");
        if (!Directory.Exists(wjj))
        {
            Directory.CreateDirectory(wjj);
        }
        var year = MainWindow.GetSchoolYearTerm(DateTime.Now);
        wjj = Path.Combine(wjj, year);
        if (!Directory.Exists(wjj))
        {
            Directory.CreateDirectory(wjj);
        }
        StartButton.Content = "导入中";
        for (var i = 1; i <= 20; i++)
        {
            var v = await Api.GetCurriculum(i.ToString(), "");
            var curriculum = JsonConvert.DeserializeObject<Curriculum>(v);
            //Debug.WriteLine(curriculum);
            int length = curriculum.Data[0].Date.Count;
            var zc = curriculum.Data[0].Date[length - 1].Mxrq;
            var path = System.IO.Path.Combine(wjj, $"{zc}.txt");
            File.WriteAllText(path, v);
            await Task.Delay(500);
        }
        StartButton.Content = "导入完成";
        await Task.Delay(1000);
        StartButton.IsEnabled = true;
        if (navigationView != null)
        {
            var a = navigationView.MenuItems[0] as NavigationViewItem;
            a.IsEnabled = true;
            var frame = navigationView.Content as Frame;
            navigationView.SelectedItem = a;
            frame.Navigate(typeof(TablePage));
        }

    }
    private async void KcZipButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {    //disable the button to avoid double-clicking
        var senderButton = sender as Button;
        senderButton.IsEnabled = false;
        // Create a file picker
        var openPicker = new Windows.Storage.Pickers.FileOpenPicker();
        var window = App.MainWindow;
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);
        // Set options for your file picker
        openPicker.ViewMode = PickerViewMode.Thumbnail;
        openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
        openPicker.FileTypeFilter.Add(".kczip");
        // Open the picker for the user to pick a file
        var file = await openPicker.PickSingleFileAsync();
        if (file != null)
        {

            Debug.WriteLine(file.Path + "-->" + AppContext.BaseDirectory);
            var v = Path.Combine(AppContext.BaseDirectory, "cache");
            //删除目录
            Directory.Delete(v, true);
            ZipFile.ExtractToDirectory(file.Path, v, overwriteFiles: true);
            //遍历所有文件
            var list = new List<string>();
            GetFile(list, v);
            foreach (var item in list)
            {
                try
                {
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("排除掉错误的文件:" + ex.Message);
                }
                Debug.WriteLine(item);
            }
        }
        else
        {
            //PickAPhotoOutputTextBlock.Text = "Operation cancelled.";
        }
        senderButton.IsEnabled = true;
    }
    /// <summary>
    /// 遍历所有文件
    /// </summary>
    /// <param name="path"></param>
    private void GetFile(List<string> list, string path)
    {
        //获取 path 目录下的所有文件 以及文件夹
        var files = Directory.GetFileSystemEntries(path);
        foreach (var file in files)
        {
            //判断是否是文件

            if (File.Exists(file) && file.EndsWith(".txt"))
            {
                list.Add(file);
            }
            else
            {
                GetFile(list, file);
            }
        }
    }
}
