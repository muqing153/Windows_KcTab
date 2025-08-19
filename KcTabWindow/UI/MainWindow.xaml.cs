
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using KcTabWindow.ViewData;
using Wpf.Ui.Controls;
namespace KcTabWindow.UI;
/// <summary>
/// MainWindow.xaml 的交互逻辑
/// </summary>
public partial class MainWindow : FluentWindow
{
    public static Curriculum? Curriculum;
    public MainWindow()
    {
        InitializeComponent();
        //RootNavigation.MenuItems[0] = new Page1();s
    }
    public static List<string> listPath = new();
    private async void Init()
    {
        string wjj = System.IO.Path.Combine(AppContext.BaseDirectory, "TabList");
        if (Directory.Exists(wjj))
        {
            string[] strings = Directory.GetFiles(wjj);
            foreach (var item in strings)
            {
                if (item.EndsWith(".txt"))
                {
                    listPath.Add(item);
                    Debug.WriteLine(item);
                }
            }
            Curriculum = await Api.GetCurriculumFile();
            StartPage();
            return;
        }
        RootNavigation.Navigate(typeof(LoginPage));
    }
    private void StartPage()
    {
        RootNavigation.Navigate(typeof(Page1));
    }
    public static bool IsExist = true;
    private void FluentWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        IsExist = false;
    }
    private void RootNavigation_Loaded(object sender, RoutedEventArgs e)
    {
        Init();
    }
}