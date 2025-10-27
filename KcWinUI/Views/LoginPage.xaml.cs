
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using KcWinUI.Helpers;
using KcWinUI.Models;
using KcWinUI.Services;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Newtonsoft.Json;
using Windows.Storage.Pickers;
namespace KcWinUI.Views;
/// <summary>
/// LoginPage.xaml 的交互逻辑
/// </summary>
public partial class LoginPage : Page
{
    public XueXiaoData xueXiao = new();
    IniHelper iniHelper = new("setting.ini");
    public LoginPage()
    {
        InitializeComponent();
        xueXiao = JsonConvert.DeserializeObject<XueXiaoData>("{\r\n    \"WebIP\": \"http://jw.qdpec.edu.cn:8088\",\r\n    \"json\": {\r\n        \"Data\": \"data[0].courses\",\r\n        \"Parsing\": {\r\n            \"courseName\": \"courseName\",\r\n            \"classWeek\": \"classWeek\",\r\n            \"teacherName\": \"teacherName\",\r\n            \"classroomName\": \"classroomName\",\r\n            \"weekDay\": \"weekDay\",\r\n            \"weekNoteDetail\": \"weekNoteDetail\",\r\n            \"ktmc\": \"ktmc\"\r\n        }\r\n    },\r\n    \"html\": {}\r\n}");
        //Debug.WriteLine(JsonConvert.SerializeObject(xuexiao));
        var x = iniHelper.Read("user", "xieyi", "False").Equals("True");
        checkbox.IsChecked = x;
        checkbox.Click += (s, e) =>
        {
            xieyi((bool)checkbox.IsChecked);
            iniHelper.Write("user", "xieyi", checkbox.IsChecked.ToString());
        };
        account.Text = iniHelper.Read("user", "username", "");
        password.Password = iniHelper.Read("user", "password", "");
        xieyi(x);
    }
    private void xieyi(bool isChecked)
    {
        StartButton.IsEnabled = isChecked;
        KcWebButton.IsEnabled = isChecked;
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
        iniHelper.Write("user", "username", account.Text);
        iniHelper.Write("user", "password", password.Password);
        StartButton.IsEnabled = false;
        try
        {
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
            var wjj = System.IO.Path.Combine(AppContext.BaseDirectory, "TabList\\");
            if (!Directory.Exists(wjj))
            {
                Directory.CreateDirectory(wjj);
            }
            wjj = System.IO.Path.Combine(wjj, account.Text + ".json");
            StartButton.Content = "导入中";
            //var jie = await Api.Get_sjkbms();
            //Debug.WriteLine(jie);
            var curriculum = new Curriculum();
            for (var i = 1; i <= 20; i++)
            {
                var GetCurriculum = await Api.GetCurriculum(i.ToString(), "");
                curriculum = Curriculum.ParsJSON(GetCurriculum, xueXiao, curriculum);
            }
            Curriculum.SaveCurriculum(wjj, curriculum);
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
        catch (Exception ex)
        {
            Toast.Subtitle = "登录失败";
            Toast.IsOpen = true;
            return;
        }

    }
    private async void KcZipButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {    //disable the button to avoid double-clicking
        //var senderButton = sender as Button;
        //senderButton.IsEnabled = false;
        //// Create a file picker
        //var openPicker = new Windows.Storage.Pickers.FileOpenPicker();
        //var window = App.MainWindow;
        //var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        //WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);
        //// Set options for your file picker
        //openPicker.ViewMode = PickerViewMode.Thumbnail;
        //openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
        //openPicker.FileTypeFilter.Add(".kczip");
        //// Open the picker for the user to pick a file
        //var file = await openPicker.PickSingleFileAsync();
        //if (file != null)
        //{

        //    Debug.WriteLine(file.Path + "-->" + AppContext.BaseDirectory);
        //    var v = Path.Combine(AppContext.BaseDirectory, "cache");
        //    //删除目录
        //    if (Directory.Exists(v))
        //    {
        //        Directory.Delete(v, true);
        //    }
        //    ZipFile.ExtractToDirectory(file.Path, v, overwriteFiles: true);
        //    //遍历所有文件
        //    var list = new List<string>();
        //    GetFile(list, v);
        //    foreach (var item in list)
        //    {
        //        try
        //        {
        //            var json = File.ReadAllText(item);
        //            var curriculum = JsonConvert.DeserializeObject<Curriculum>(json);
        //            var wjj = System.IO.Path.Combine(AppContext.BaseDirectory, "TabList", curriculum.Data[0].TopInfo[0].SemesterId);
        //            if (!Directory.Exists(wjj))
        //            {
        //                Directory.CreateDirectory(wjj);
        //            }
        //            var name=curriculum.Data[0].Date.Count;
        //            var zc = curriculum.Data[0].Date[name - 1].Mxrq;
        //            File.WriteAllText(Path.Combine(wjj,zc + ".json"), json);
        //        }
        //        catch (Exception ex)
        //        {
        //            Debug.WriteLine("排除掉错误的文件:" + ex.Message);
        //        }
        //        Debug.WriteLine(item);
        //    }
        //}
        //else
        //{
        //    //PickAPhotoOutputTextBlock.Text = "Operation cancelled.";
        //}
        //senderButton.IsEnabled = true;
    }
    public async void ParsJson_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(account.Text))
        {
            Toast.Subtitle = "请输入账号";
            Toast.IsOpen = true;
            return;
        }
        var openPicker = new Windows.Storage.Pickers.FileOpenPicker();
        var window = App.MainWindow;
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);
        // Set options for your file picker
        openPicker.ViewMode = PickerViewMode.Thumbnail;
        openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
        openPicker.FileTypeFilter.Add(".json");
        // Open the picker for the user to pick a file
        var file = await openPicker.PickSingleFileAsync();
        if (file != null)
        {
            var json = File.ReadAllText(file.Path);
            //读取JSON转成 XueXiaoData

            var cu = Curriculum.ParsJSON(json, xueXiao);
            //输出cu JSON
            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull // 忽略 null
            };
            var jsondata = JsonConvert.SerializeObject(cu, Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore // 忽略 null
            });
            TablePage.FilePath = System.IO.Path.Combine(AppContext.BaseDirectory, "TabList\\" + account.Text + ".json");
            File.WriteAllText(TablePage.FilePath, jsondata);
            Toast.Subtitle = "导入成功";
            Toast.IsOpen = true;
        }
        iniHelper.Write("user", "username", account.Text);
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
