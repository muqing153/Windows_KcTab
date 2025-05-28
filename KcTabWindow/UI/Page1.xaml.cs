using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Controls;
using KcTabWindow.ViewData;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using System.IO;
using System.Windows.Media.Imaging;
using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using TextBlock = Wpf.Ui.Controls.TextBlock;
using static System.Net.Mime.MediaTypeNames;
using Wpf.Ui.Controls;
using MessageBox = Wpf.Ui.Controls.MessageBox;
using Wpf.Ui;
using Wpf.Ui.Extensions;
using Application = System.Windows.Application;
namespace KcTabWindow.UI;

public partial class Page1 : Page
{
    //public static string bengzhou = string.Empty;//本周
    public Page1()
    {
        InitializeComponent();
        //zhoubox.ItemsSource = MainWindow.Zhoulist;
        System.Timers.Timer timer = new System.Timers.Timer
        {
            AutoReset = true,
            Interval = 1000
        };
        timer.Elapsed += (s, e) =>
        {
            var now = DateTime.Now;
            string formattedTime = now.ToString("yyyy-MM-dd HH:mm:ss");
            Dispatcher.Invoke(() =>
            {
                itemtext.Text = $"{formattedTime}";
            });
        };
        timer.Start();
        Init();
        zhoubox.SelectionChanged += zhoubox_SelectionChanged;
    }


    public async void Init()
    {
        var response = JsonConvert.DeserializeObject<TeachingWeek>(new TeachingWeek().Get());
        zhoubox.ItemsSource = response.Data;
        if (response.Data != null && response.Data.Count > 0)
        {
            if (MainWindow.Curriculum != null)
            {
                var mrms = MainWindow.Curriculum.Data[0].Week;
                //string转int
                zhoubox.SelectedIndex = mrms - 1;
                AddDynamicColumns();
            }
        }
    }
    private void LoadDataGrid()
    {
    }

    public class RowData
    {
        public string StartTime { get; set; } // 开始时间
        public string EndTime { get; set; } // 结束时间
        public string Section { get; set; } // 课时节次
        //public Dictionary<string, Curriculum.Course?> Courses { get; set; } = [];
        public List<Curriculum.Course> Courses { get; set; } = [];
    }
    private void AddDynamicColumns()
    {
        if (MainWindow.Curriculum == null || MainWindow.Curriculum.Code.Equals("401"))
        {
            return;
        }
        datagrid.Columns.Clear();
        // 添加左侧“节次”列
        var cellTemplate = (DataTemplate)this.FindResource("SectionCellTemplate");
        var sectionColumn = new DataGridTemplateColumn
        {
            Header = "节/日期",
            Width = 100,
            CellTemplate = cellTemplate // 将 CellTemplate 赋值给列
        };
        datagrid.Columns.Add(sectionColumn);

        // 遍历 DateInfo，动态创建列
        foreach (var dateInfo in MainWindow.Curriculum.Data[0].Date)
        {
            //string columnKey = dateInfo.Xqid;
            string columnHeader = $"{dateInfo.Xqmc}({dateInfo.Rq})";

            var column = new DataGridTemplateColumn
            {
                Header = $"星期{columnHeader}",
                Width = 150,
            };
            //Debug.WriteLine($"星期{columnHeader}");
            cellTemplate = (DataTemplate)FindResource("InfoTabs");
            column.CellTemplate = cellTemplate; // 将 CellTemplate 赋值给列

            datagrid.Columns.Add(column);
        }
        AddSampleData();
    }
    List<(string Session, string Time1, string Time2)> schedule =
    [
            ("第 1 节", "08:20 - 09:05", "09:15 - 10:00"),
            ("第 2 节", "10:10 - 11:40", "10:30 - 12:00"),
            ("第 3 节", "13:30 - 14:15", "14:25 - 15:10"),
            ("第 4 节", "15:20 - 16:05", "16:15 - 17:00"),
            ("第 5 节", "18:30 - 19:15", "19:25 - 20:10")
    ];
    ObservableCollection<RowData> rows = [];
    private void AddSampleData()
    {
        rows.Clear();
        var dataItems = MainWindow.Curriculum!.Data[0]; // 获取 DataItem
        var nodes = dataItems.NodesLst; // 获取节次信息
        var courses = dataItems.Courses; // 获取所有课程
        var dateInfos = dataItems.Date; // 获取日期信息

        // 1. 遍历每个节次，创建行数据
        var ric = new[] { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" };
        int ric_i = 0;
        for (int a = 0; a < 5; a++)
        {
            var row = new RowData();
            // 2. 遍历 DateInfo，匹配课程
            //string columnKey = dateInfo.Xqid; // 列的标识
            for (int i = 0; i < 7; i++)
            {
                // 查找符合条件的课程（ClassTime 以 0102 结尾）
                var course = courses.FirstOrDefault(c => c.ClassTime.EndsWith($"{ric[ric_i]}{ric[ric_i + 1]}") && c.WeekDay == i.ToString());
                //row.Courses.Add(i.ToString(), course);
                row.Courses.Add(course??new Curriculum.Course());
                //Debug.WriteLine(course?.ClassroomName);
                // 如果找到课程，添加格式化字符串；否则添加空字符串
                //row.Courses.Add(i.ToString(), course != null ? $"{course.CourseName}\n({course.ClassroomName})" : "");
            }
            ric_i += 2;
            row.Section = schedule[a].Item1;
            row.StartTime = schedule[a].Item2;
            row.EndTime = schedule[a].Item3;
            rows.Add(row);
            //break;
        }
        datagrid.ItemsSource = rows;
    }
    private double _scale = 1.0;
    private void ScrollViewer_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        //Debug.WriteLine(e.Delta);
        // 检测是否按住 Ctrl 键
        if (Keyboard.Modifiers == ModifierKeys.Shift)
        {
            // 控制横向滚动
            scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - e.Delta);
            e.Handled = true; // 标记事件已处理
        }
        else if (Keyboard.Modifiers == ModifierKeys.Control)
        {
            // 根据鼠标滚轮的方向调整缩放比例
            if (e.Delta > 0)
            {
                _scale += 0.1; // 放大
            }
            else if (e.Delta < 0)
            {
                _scale = Math.Max(0.1, _scale - 0.1); // 缩小，确保不小于0.1
            }
            datagrid.LayoutTransform = new ScaleTransform(_scale, _scale);

            e.Handled = true;

        }
    }

    private void Page_KeyDown(object sender, KeyEventArgs e)
    {
        jianpan.Text = $"按下 {e.Key}";
    }

    private void Page_KeyUp(object sender, KeyEventArgs e)
    {
        jianpan.Text = string.Empty;
    }

    private void datagrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        // 尝试将发送方转换为 DataGrid
        if (sender is not Wpf.Ui.Controls.DataGrid dataGrid)
            return;

        // 获取点击的元素
        if (e.OriginalSource is not FrameworkElement hitElement)
            return;

        // 向上查找，直到找到 DataGridCell
        while (hitElement != null && hitElement is not DataGridCell)
        {
            hitElement = (FrameworkElement)VisualTreeHelper.GetParent(hitElement);
        }

        // 如果点击的元素是 DataGridCell
        if (hitElement is DataGridCell cell)
        {
            // 获取单元格所在行的数据上下文
            var rowData = cell.DataContext;
            //Debug.WriteLine(rowData);
            // 根据行数据的类型进行不同处理
            switch (rowData)
            {

                case RowData kcData:
                    // 获取当前列对应的索引
                    int columnIndex = dataGrid.Columns.IndexOf(cell.Column);
                    if (columnIndex == 0) // 第一列是节次列，跳过处理
                    {
                        return;
                    }
                    //columnIndex--; // 由于第一列是“节次”，所以需要减 1 才能对上 Courses 的索引

                    // 检查 `Courses` 字典中是否存在该索引的数据
                    //&& kcData.Courses.ContainsKey(columnIndex.ToString())
                    if (kcData.Courses != null )
                    {
                        // 获取该列对应的课程信息
                        var courseInfo = kcData.Courses[columnIndex];
                        if (courseInfo == null)
                        {
                            //System.Windows.MessageBox.Show($"课程信息：{courseInfo}");
                            return;
                        }
                        new ContentDialog(MainWindow.GlobalContentPresenter)
                        {
                            Title = "课程详情",
                            Content = $"{courseInfo.CourseName}\n{courseInfo.TeacherName}\n教室: {courseInfo.ClassroomName}\n时间: {courseInfo.StartTime}-{courseInfo.EndTime}\n{courseInfo.Ktmc}",
                            CloseButtonText = "关闭"
                        }.ShowAsync();
                    }
                    break;
            }
        }
    }

    private async void zhoubox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // 获取选中的项
        ComboBox? comboBox = sender as ComboBox;
        if (comboBox != null && comboBox.SelectedItem != null)
        {
            // 获取选中的第一个项
            int selectedIndex = comboBox.SelectedIndex;
            //selectedIndex;
            //Debug.WriteLine(selectedIndex);
            //MainWindow.Curriculum = JsonConvert.DeserializeObject<Curriculum>(await Api.GetCurriculum(selectedIndex.ToString(), ""));
            string v = File.ReadAllText(MainWindow.listPath[selectedIndex]);
            //string v = await Api.GetCurriculumFile(MainWindow.listPath[selectedIndex]);
            MainWindow.Curriculum= JsonConvert.DeserializeObject<Curriculum>
                (v);
            //Debug.WriteLine("selectedIndex:"+ selectedIndex);
            AddDynamicColumns(); // 1. 先动态创建列
        }
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        // 获取 ContentDialog 宿主（在 XAML 中定义）
        var contentDialog = new ContentDialog(KcTabWindow.UI.MainWindow.GlobalContentPresenter)
        {
            CloseButtonText = "关闭"
        };
        contentDialog.Content = new LoginPage(contentDialog);
        // 以模态方式显示
        await contentDialog.ShowAsync();
        if (LoginApi.Token == "" || LoginApi.Token == null)
        {
            //Debug.WriteLine("获取Token失败");
            return;
        }
        Init();
    }
    private async void Button_Exit(object sender, RoutedEventArgs e)
    {
        //退出登陆
        LoginApi.UserData.rsa = "";
        LoginApi.SaveUserData();
        LoginApi.SaveToken("");
        Application.Current.Shutdown();
        //SomeAsyncMethod();
    }

    private async void jietu_Click(object sender, RoutedEventArgs e)
    {

        // 确保 Grid 的尺寸大于零
        if (datagrid.ActualWidth <= 0 || datagrid.ActualHeight <= 0)
        {
            Wpf.Ui.Controls.MessageBox messageBox = new Wpf.Ui.Controls.MessageBox()
            {
                Title = "提示",
                Content = "请先设置好窗口大小再截图。",
                CloseButtonText = "关闭"
            };
            await messageBox.ShowDialogAsync();
            return;
        }


        // 进行测量和排列
        datagrid.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        datagrid.Arrange(new Rect(new Size(datagrid.DesiredSize.Width, datagrid.DesiredSize.Height)));

        // 创建 RenderTargetBitmap 对象
        var renderTargetBitmap = new RenderTargetBitmap((int)datagrid.ActualWidth, (int)datagrid.ActualHeight, 96d, 96d, System.Windows.Media.PixelFormats.Pbgra32);
        renderTargetBitmap.Render(datagrid); // 渲染 Grid
        //renderTargetBitmap转换为BitmapSource
        BitmapSource bitmapSource = renderTargetBitmap;
        //bitmapSource保存到本地路径

        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

        var outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output.png");
        Debug.WriteLine(outputPath);
        using (var stream = new FileStream(outputPath, FileMode.Create))
        {
            encoder.Save(stream);
        }

        // 将图像保存到剪贴板
        Clipboard.SetImage(bitmapSource);
        // 创建 Toast 内容
        new ToastContentBuilder()
            .AddArgument("action", "viewConversation")
            .AddArgument("conversationId", 114511)
            .AddArgument("imagePath", outputPath) // 将图片路径作为参数添加
            .AddText("截图成功") // 更改标题内容
            .AddText("已经复制到剪辑板") // 更改通知内容
            .AddInlineImage(new Uri(outputPath)) // 添加图片路径
            .Show();
        // Not seeing the Show() method? Make sure you have version 7.0, and if you're using .NET 6 (or later), then your TFM must be net6.0-windows10.0.17763.0 or greater
    }
}