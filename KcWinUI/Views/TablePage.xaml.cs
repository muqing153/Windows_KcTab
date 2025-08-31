using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using KcWinUI.Models;
using KcWinUI.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Notifications;
namespace KcWinUI.Views;
public partial class TablePage : Page
{
    public static Curriculum curriculum = new();
    //public static string? FilePath;
    public TablePage()
    {
        InitializeComponent();
        Init();
        //TableGrid.Header = curriculum.Data[0].Date;
        TableGrid.ItemsSource = GetKcLei(curriculum);
        curriculum.Data[0].Date.Insert(0, new()
        {
            Xqmc = "节/期"
        });
        TableGridHeader.ItemsSource = curriculum.Data[0].Date;
    }
    public async void Init()
    {
        TablePage.curriculum = await Api.GetCurriculumFile();

        var listLeft = new List<string>();
        // 创建节次数据
        for (var i = 0; i < schedule.Count; i++)
        {
            listLeft.Add(schedule[i].Time1 + "\n" + schedule[i].Time2);
        }
        TableGridLeft.ItemsSource = listLeft;
        var listInt = new List<Zhou>();
        for (var i = 0; i < MainWindow.listPath.Count; i++)
        {
            listInt.Add(new()
            {
                position = $"{i + 1}",
                path = MainWindow.listPath[i]
            });
        }
        zhoubox.ItemsSource = listInt;
        //string 转int
        zhoubox.SelectedIndex = curriculum.Data[0].TopInfo[0].Week - 1;
        zhoubox.SelectionChanged += (s, e) =>
        {
            Zhou zhou = (Zhou)zhoubox.SelectedItem;
            var get = Api.GetCurriculumPath(zhou.path);
            TableGrid.ItemsSource = GetKcLei(get);
        };

    }
    private static List<(string Session, string Time1, string Time2)> schedule = new()
    {
    ("第 1 节", "08:20 - 09:05", "09:15 - 10:00"),
    ("第 2 节", "10:10 - 11:40", "10:30 - 12:00"),
    ("第 3 节", "13:30 - 14:15", "14:25 - 15:10"),
    ("第 4 节", "15:20 - 16:05", "16:15 - 17:00"),
    ("第 5 节", "18:30 - 19:15", "19:25 - 20:10")
};


    private static readonly string[] classTime = { "0102", "0304", "0506", "0708", "0910" };

    static int GetInt(string str)
    {
        for (var i = 0; i < classTime.Length; i++)
        {
            if (classTime[i] == str)
            {
                return i;
            }
        }
        return 0;
    }

    public static List<List<Curriculum.Course>> GetKcLei(Curriculum curriculum)
    {
        var list = new List<List<Curriculum.Course>>();

        // 初始化 6*8 个空列表
        for (var row = 0; row < 5; row++)
        {
            for (var col = 0; col <7; col++)
            {
                list.Add(new List<Curriculum.Course>());
            }
        }
        foreach (var course in curriculum.Data[0].Courses)
        {
            string classTimeStr = course.ClassTime;
            string part1 = classTimeStr.Substring(0, 1); // 星期
            string part2 = classTimeStr.Substring(1, 4); // 节次

            int dayIndex = int.Parse(part1) - 1; // 星期几 -> 列索引 0~6
            int periodIndex = GetInt(part2);      // 节次 -> 行索引 0~4

            if (periodIndex < 0 || dayIndex < 0 || dayIndex > 6)
                continue; // 跳过无效数据

            if (string.IsNullOrEmpty(course.ClassroomName))
                course.ClassroomName = "网课";

            list[periodIndex * 7 + dayIndex].Add(course);
        }
        return list;
    }

    private async void Button_Click_jietu(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (scrollViewer.Content is not FrameworkElement content)
            return;

        // 测量和排列，确保内容完整渲染
        content.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        content.Arrange(new Rect(0, 0, content.DesiredSize.Width, content.DesiredSize.Height));

        int width = (int)Math.Ceiling(content.ActualWidth);
        int height = (int)Math.Ceiling(content.ActualHeight);

        // 使用 RenderTargetBitmap 渲染 XAML 内容
        var rtb = new RenderTargetBitmap();
        await rtb.RenderAsync(content);
        var pixels = (await rtb.GetPixelsAsync()).ToArray();

        // 将透明区域填充为白色
        for (int i = 0; i < pixels.Length; i += 4)
        {
            byte alpha = pixels[i + 3];
            if (alpha == 0)
            {
                pixels[i + 0] = 255; // B
                pixels[i + 1] = 255; // G
                pixels[i + 2] = 255; // R
                pixels[i + 3] = 255; // A
            }
        }

        // 保存为 PNG
        StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(
            "ScrollViewerWhiteBG.png", CreationCollisionOption.ReplaceExisting);

        using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
        {
            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);

            encoder.SetPixelData(
                BitmapPixelFormat.Bgra8,
                BitmapAlphaMode.Premultiplied,
                (uint)rtb.PixelWidth,
                (uint)rtb.PixelHeight,
                96, 96,
                pixels);

            await encoder.FlushAsync();
        }


        // Toast XML 模板
        var toastXmlString =
            $@"<toast>
            <visual>
                <binding template='ToastGeneric'>
                    <text>截图</text>
                    <image placement='appLogoOverride' src='{file.Path}' alt='示例图片'/>
                </binding>
            </visual>
          </toast>";

        // 加载 XML
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(toastXmlString);

        // 创建 Toast
        var toast = new ToastNotification(xmlDoc);
        // 注册点击事件
        toast.Activated += (s, e) =>
        {
            // 通过 Process 打开文件夹
            try
            {
                Process.Start("explorer.exe", file.Path);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"打开文件夹失败: {ex.Message}");
            }
        };
        // 显示
        ToastNotificationManager.CreateToastNotifier().Show(toast);
    }

    private async void TableGrid_ItemClick(object sender, ItemClickEventArgs e)
    {
        Debug.WriteLine(e.ClickedItem);
        if (e.ClickedItem is List<Curriculum.Course> course)
        {
            if (course.Count > 0)
            {
                var contentDialog = new ContentDialog
                {
                    // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
                    XamlRoot = this.XamlRoot,
                    Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                    Title = "课程详情",
                    CloseButtonText = "Cancel",
                    DefaultButton = ContentDialogButton.Primary
                };
                var content = "";
                foreach (var item in course)
                {
                    content += $"{item.CourseName}\n{item.ClassroomName}\n{item.ClassTime}\n{item.TeacherName}\n{TableGrid.Items.IndexOf(course)}\n\n";
                }
                contentDialog.Content = content;
                ContentDialog dialog = contentDialog;
                var result = await dialog.ShowAsync();

            }
        }

    }
    private void TableGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

        Debug.WriteLine(sender);
        //if (e.ClickedItem is List<Curriculum.Course> course)
        //{
        //    if (course.Count > 0)
        //    {
        //        var contentDialog = new ContentDialog
        //        {
        //            // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
        //            XamlRoot = this.XamlRoot,
        //            Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
        //            Title = "Save your work?",
        //            PrimaryButtonText = "Save",
        //            SecondaryButtonText = "Don't Save",
        //            CloseButtonText = "Cancel",
        //            DefaultButton = ContentDialogButton.Primary
        //        };
        //        ContentDialog dialog = contentDialog;
        //        var result = await dialog.ShowAsync();

        //    }}
    }
}