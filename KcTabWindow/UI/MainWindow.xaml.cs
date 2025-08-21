
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
    public static List<string> listPath = [];
    private async void Init()
    {
        string wjj = System.IO.Path.Combine(AppContext.BaseDirectory, "TabList");
        if (Directory.Exists(wjj))
        {
            //new DateTime(2025, 2, 1)
            var inihelper = new IniHelper("setting.ini");
            string year = inihelper.Read("Table", "year",string.Empty);
            Debug.WriteLine("IniHelper " + year);
            if (year == string.Empty) { 
                year = GetSchoolYearTerm(DateTime.Now);
                inihelper.Write("Table", "year",year);
            }
            var xuenian= Path.Combine(wjj,year);
            if (Directory.Exists(xuenian))
            {
                string[] strings = Directory.GetFiles(xuenian);
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
                    RootNavigation.Navigate(typeof(LoginPage));
                    return;
                }
                TableView.IsEnabled = true;
                Curriculum = await Api.GetCurriculumFile();
                StartPage();
                return;
            }
        }
        RootNavigation.Navigate(typeof(LoginPage));
    }
    private void StartPage()
    {
        RootNavigation.Navigate(typeof(TablePage));
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

}