using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using KcTabWindow.ViewData;
using Wpf.Ui.Controls;

namespace KcTabWindow.UI
{
    /// <summary>
    /// LoginPage.xaml 的交互逻辑
    /// </summary>
    public partial class LoginPage : Grid
    {
        ContentDialog contentDialog;
        public LoginPage(ContentDialog contentDialog)
        {
            this.contentDialog = contentDialog;
            InitializeComponent();
            StartButton.Click += StartButton_Click;
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            string wjj = System.IO.Path.Combine(AppContext.BaseDirectory, "TabList");
            if (!Directory.Exists(wjj))
            {
                Directory.CreateDirectory(wjj);
            }
            StartButton.IsEnabled = false;
            StartButton.Content = "导入中";
            string accountText = account.Text;
            LoginApi.Token = accountText;
            for(int i = 1; i <= 20; i++)
            {

                string v = await Api.GetCurriculum(i.ToString(), "");
                var curriculum = JsonConvert.DeserializeObject<Curriculum>(v);
                Debug.WriteLine(curriculum);
                int length = curriculum.Data[0].Date.Count;
                string zc = curriculum.Data[0].Date[length-1].Mxrq;
                string path = System.IO.Path.Combine(wjj, $"{zc}.txt");
                File.WriteAllText(path, v);
                await Task.Delay(500);
            }
            StartButton.Content = "导入完成";
            await Task.Delay(1000);
            contentDialog.Hide();
        }

        // TextChanged 事件处理程序
        private void Edit_TextChanged(object sender, TextChangedEventArgs e)
        {
            // 你可以在这里添加你需要的逻辑
            // 例如：打印文本框的当前内容
            string accountText = account.Text;
            if (String.IsNullOrEmpty(accountText))
            {
                StartButton.IsEnabled = false;
            }
            else
            {
                StartButton.IsEnabled = true;
            }
            // 这里也可以在每次文本改变时，执行其他逻辑，比如更新按钮的状态
        }
    }
}
