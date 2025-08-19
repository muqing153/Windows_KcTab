
using RestSharp;
using System.Diagnostics;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using KcTabWindow.UI;
using System.Security.Policy;
using System.Text.RegularExpressions;
using static KcTabWindow.Api;
using Windows.Networking.Sockets;
using System.Windows.Shapes;
using KcTabWindow.ViewData;
using System.Threading.Tasks;
namespace KcTabWindow;

public class Api
{
    public const string api = "http://10.1.2.1";
    public static string Token = string.Empty;
    /// <summary>
    /// 获取课程表
    /// </summary>
    /// <param name="week"></param>
    /// <param name="kbjcmsid"></param>
    /// <returns></returns>
    public static async Task<string> GetCurriculum(string week, string kbjcmsid)
    {
        var client = new RestClient(new RestClientOptions($"http://jw.qdpec.edu.cn:8088/njwhd/student/curriculum?week={week}&kbjcmsid={kbjcmsid}")
        {
            FollowRedirects = false,
            ThrowOnAnyError = false,
            Proxy = new WebProxy() { UseDefaultCredentials = false }, // 禁用代理
            UserAgent = "Apifox/1.0.0 (https://apifox.com)",
        });
        var request = new RestRequest("", Method.Post);
        request.AddHeader("Pragma", "no-cache");
        request.AddHeader("token", LoginApi.Token);
        //client.UserAgent = "Apifox/1.0.0 (https://apifox.com)";
        request.AddHeader("Accept", "*/*");
        request.AddHeader("Host", "jw.qdpec.edu.cn:8088");
        request.AddHeader("Connection", "keep-alive");
        RestResponse restResponse = await client.ExecuteAsync(request);
        //if(restResponse.Content)
        var str=restResponse.Content??string.Empty;
        return str;
        //return string.Empty;
    }
    public static async Task<Curriculum> GetCurriculumFile()
    {
        string wjj = System.IO.Path.Combine(AppContext.BaseDirectory, "TabList");
        //获取当前日期 yyyy-mm-dd
        string currentDateStr = DateTime.Now.ToString("yyyy-MM-dd");
        DateTime currentDate = DateTime.Parse(currentDateStr);
        int i = 1;
        foreach (var filePath in MainWindow.listPath)
        {
            string fileDateStr = System.IO.Path.GetFileNameWithoutExtension(filePath);
            DateTime fileDate = DateTime.Parse(fileDateStr);
            // 比较日期是否 >= 今天
            if (fileDate >= currentDate || i>= MainWindow.listPath.Count)
            {
                Debug.WriteLine($"符合要求的文件：{filePath} (日期: {fileDateStr})");
                Curriculum? curriculum = JsonConvert.DeserializeObject<Curriculum>(File.ReadAllText(filePath));
                if (curriculum != null)
                {
                    curriculum.Data[0].Week = i;
                    return curriculum;
                }
            }
            i++;
        }
        return new Curriculum();
    }

    public static async Task<string> GetCurriculumFile(string currentDateStr)
    {
        string wjj = System.IO.Path.Combine(AppContext.BaseDirectory, "TabList");
        //获取当前日期 yyyy-mm-dd

        //string currentDateStr = DateTime.Now.ToString("yyyy-MM-dd");
        DateTime currentDate = DateTime.Parse(currentDateStr);

        foreach (var filePath in MainWindow.listPath)
        {
            // 用正则表达式提取日期部分（格式 yyyy-MM-dd）
            Match match = Regex.Match(filePath, @"\d{4}-\d{2}-\d{2}");
            if (!match.Success)
                continue; // 跳过不含日期的文件

            string fileDateStr = match.Value;
            DateTime fileDate = DateTime.Parse(fileDateStr);

            // 比较日期是否 >= 今天
            if (fileDate >= currentDate)
            {
                Debug.WriteLine($"符合要求的文件：{filePath} (日期: {fileDateStr})");
                return File.ReadAllText(filePath);
                // 进一步处理...
            }
        }
        return string.Empty;
    }
    public static async Task<string> Get_sjkbms()
    {

        var client = new RestClient(new RestClientOptions("http://jw.qdpec.edu.cn:8088/njwhd/Get_sjkbms")
        {
            Proxy = new WebProxy() { UseDefaultCredentials = false }, // 禁用代理
            UserAgent = "Apifox/1.0.0 (https://apifox.com)",
        });
        var request = new RestRequest()
        {
            Method = Method.Post
        };
        request.AddHeader("Host", "jw.qdpec.edu.cn:8088");
        request.AddHeader("token", LoginApi.Token);
        var response = await client.ExecuteAsync(request);
        //restRequest.AddHeader("Token", LoginApi.Token);
        //var a = await restClient.ExecuteAsync(restRequest);
        return response.Content??string.Empty;
    }
}
