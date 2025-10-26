using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using KcWinUI.Helpers;
using KcWinUI.Models;
using KcWinUI.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Newtonsoft.Json;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Notifications;
using static KcWinUI.Models.Curriculum;
namespace KcWinUI.Views;
public partial class TablePage : Page
{
    public static Curriculum curriculum = new();
    public List<List<List<Curriculum.Course>>> CourseList = new();
    public static string FilePath = "";
    public string Day
    {
        get; set;
    } = "1";
    public static int week = 1;
    public TablePage()
    {
        var iniHelper = new IniHelper("setting.ini");
        FilePath = System.IO.Path.Combine(AppContext.BaseDirectory, "TabList/"+iniHelper.Read("user", "username","") + ".json");
        InitializeComponent();
        Init();
        //TableGridHeader.ItemsSource = CourseList;
    }
    public async void Init()
    {
        TablePage.curriculum = await Api.GetCurriculumFile(TablePage.FilePath);
        curriculum.startDate = Curriculum.GetSemesterStartDate();
        curriculum.nian = Curriculum.GetCurrentSemester();
        // 获取当前周
        week = Curriculum.GetWeekIndex(curriculum.startDate, DateTime.Now.ToString("yyyy-MM-dd"));
        curriculum.ZhouInt = 20;
        TableGridHeader.ItemsSource = Curriculum.GetWeekDays(curriculum.startDate, week);
        //
        TableGridLeft.ItemsSource = TableTimeData.TimeDatas;

        zhoubox.ItemsSource = Enumerable.Range(1, curriculum.ZhouInt)
            .Select(i => i.ToString())
            .ToList();
        zhoubox.SelectedIndex = TablePage.week - 1;


        // 获取今天几号 
        Day = DateTime.Now.Day.ToString();
        // 
    }

    public static List<List<List<Curriculum.Course>>> GetKcLei(Curriculum curriculum, int zhou = 1)
    {
        var list = new List<List<List<Curriculum.Course>>>();
        //// 初始化 6*8 个空列表
        for (var col = 0; col < 7; col++)
        {
            var day = new List<List<Curriculum.Course>>();
            for (var row = 0; row < TableTimeData.TimeDatas.Length; row++)
            {
                var a = new List<Curriculum.Course>
                {
                    Curriculum.NewCourse(col, row)
                };
                day.Add(a);
            }
            list.Add(day);
        }
        if (curriculum.course != null)
        {
            foreach (var course in curriculum.course)
            {
                if (course.ClassWeekDetails != null)
                {
                    foreach (var section in course.ClassWeekDetails)
                    {
                        // 判断周是否在范围内
                        if (IsWeekInRange(section.Weeks, zhou))
                        {
                            if (section.Weekdays != null)
                            {
                                foreach (var weekday in section.Weekdays)
                                {
                                    var jies = GetJieData(weekday.Jie);
                                    if (jies != null && jies.Count > 0)
                                    {
                                        var first = jies[0];
                                        var lastTwo = jies[1];
                                        var c = JsonConvert.DeserializeObject<Course>(
                                            JsonConvert.SerializeObject(course)
                                        ) ?? new Course();

                                        c.WeekNoteDetail = weekday.Jie;
                                        c.ClassroomName = weekday.ClassroomName;
                                        c.Start = lastTwo - 1;
                                        c.End = jies[^1]; // 最后一个节次
                                        var d = list[first - 1][lastTwo - 1];
                                        if (string.IsNullOrEmpty(d[0].CourseName))
                                        {
                                            d[0] = c;
                                        }
                                        else
                                        {
                                            d.Add(course);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // 第二部分：去重合并节次
            for (int i = 0; i < list.Count; i++)
            {
                for (int index = 0; index < list[i].Count; index++)
                {
                    try
                    {
                        var firstCourse = list[i][index].FirstOrDefault();
                        if (firstCourse?.WeekNoteDetail != null)
                        {
                            var item = Curriculum.GetJieData(firstCourse.WeekNoteDetail);
                            if (item != null && item.Count > 2)
                            {
                                // 删除重复的节次
                                list[i].RemoveRange(index + 1, item.Count - 2);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.Error.WriteLine($"Error: {e.Message}");
                    }
                }
            }

        }
        return list;
    }
    public static bool IsWeekInRange(string? weeks, int zhou)
    {
        if (string.IsNullOrEmpty(weeks)) return false;
        // 支持如 "1-16"、"3,5,7"、"8-12周" 这种格式
        weeks = weeks.Replace("周", "");
        foreach (var part in weeks.Split(','))
        {
            if (part.Contains('-'))
            {
                var range = part.Split('-');
                if (int.TryParse(range[0], out int start) && int.TryParse(range[1], out int end))
                {
                    if (zhou >= start && zhou <= end) return true;
                }
            }
            else if (int.TryParse(part, out int single))
            {
                if (zhou == single) return true;
            }
        }
        return false;
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
                    //content += $"{item.CourseName}\n{item.ClassroomName}\n{item.ClassTime}\n{item.TeacherName}\n{TableGrid.Items.IndexOf(course)}\n\n";
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

    private void zhoubox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (zhoubox.SelectedItem is string zhouStr)
        {
            Debug.WriteLine($"选择了: {zhouStr}");
            TablePage.week = int.Parse(zhouStr);
            CourseList = GetKcLei(curriculum, week);
            var index = 0;
            foreach (var child in GridTable.Children)
            {
                if (index > 0)
                {
                    if (child is GridView element)
                    {
                        element.ItemsSource = CourseList[index - 1];
                    }
                }
                index++;
            }
        }
    }
}