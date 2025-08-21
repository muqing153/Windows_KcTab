using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KcTabWindow.UI;
/// <summary>
/// SettingPage.xaml 的交互逻辑
/// </summary>
public partial class SettingPage : Page
{
    IniHelper iniHelper = new("setting.ini");
    public SettingPage()
    {
        InitializeComponent();
        Init();

    }
    private void Init()
    {
        var yearList = new List<string>();
        string wjj = System.IO.Path.Combine(AppContext.BaseDirectory, "TabList");
        if (Directory.Exists(wjj))
        { 
            string[] strings = Directory.GetDirectories(wjj);
            foreach (var item in strings)
            {
                string[] split = item.Split("\\");
                yearList.Add(split[split.Length - 1]);
            }
            yearList.Sort();
            yearComboBox.ItemsSource = yearList;
            Debug.WriteLine(yearList.IndexOf(iniHelper.Read("Table","year","")));
            yearComboBox.SelectedIndex = yearList.IndexOf(iniHelper.Read("Table","year",""));
        }
    }
}
