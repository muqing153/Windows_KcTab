
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
    public static ContentPresenter? GlobalContentPresenter;

    public static Curriculum? Curriculum;
    public MainWindow()
    {
        InitializeComponent();
        GlobalContentPresenter = this.ContentPresenter;
        //Debug.WriteLine(LoginApi.Token);
        //if ()
        Init();
    }
   public static List<string> listPath = new List<string>();
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
            //listPath = Array;
            Curriculum = await Api.GetCurriculumFile();
            startPage();
            return;
        }
        var contentDialog = new Wpf.Ui.Controls.ContentDialog(KcTabWindow.UI.MainWindow.GlobalContentPresenter)
        {
            CloseButtonText = "关闭"
        };
        contentDialog.Content = new LoginPage(contentDialog);
        // 以模态方式显示
        await contentDialog.ShowAsync();
        Init();
    }
    private void startPage()
    {
        card.Content = new Page1();
    }
    public static event Action OnLoginChanged;

    public static bool IsExist = true;
    private void FluentWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        IsExist = false;
    }
}