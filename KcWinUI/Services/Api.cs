
using System.Diagnostics;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using KcWinUI.Core.Helpers;
using KcWinUI.Helpers;
using KcWinUI.Models;
using Newtonsoft.Json;
using RestSharp;
namespace KcWinUI.Services;
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
        request.AddHeader("token", Token);
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
        //var wjj = Path.Combine(AppContext.BaseDirectory, "TabList");
        //wjj = new IniHelper("setting.ini").Read("Table","year",string.Empty);
        //获取当前日期 yyyy-mm-dd
        var currentDateStr = DateTime.Now.ToString("yyyy-MM-dd");
        var currentDate = DateTime.Parse(currentDateStr);
        var i = 1;
        foreach (var filePath in MainWindow.listPath)
        {
            var fileDateStr = System.IO.Path.GetFileNameWithoutExtension(filePath);
            var fileDate = DateTime.Parse(fileDateStr);
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
        //var wjj = Path.Combine(AppContext.BaseDirectory, "TabList");
        //wjj = new IniHelper("setting.ini").Read("Table", "year", string.Empty);
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
    public static Curriculum GetCurriculumPath(string path)
    {
        return JsonConvert.DeserializeObject<Curriculum>(File.ReadAllText(path))??new Curriculum();
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
        request.AddHeader("token", Token);
        var response = await client.ExecuteAsync(request);
        //restRequest.AddHeader("Token", LoginApi.Token);
        //var a = await restClient.ExecuteAsync(restRequest);
        return response.Content??string.Empty;
    }


    public static async Task<string> LoginToken(string username,string password)
    {
        var client = new RestClient($"http://jw.qdpec.edu.cn:8088/njwhd/login?userNo={username}&pwd={Encrypt(password)}&encode=1&captchaData=&codeVal=");
        var request = new RestRequest("",Method.Post);
        request.AddHeader("Accept", "application/json, text/plain, */*");
        request.AddHeader("Accept-Language", "zh-CN,zh;q=0.9,en;q=0.8");
        request.AddHeader("Cache-Control", "no-cache");
        request.AddHeader("Connection", "keep-alive");
        request.AddHeader("Origin", "http://jw.qdpec.edu.cn:8088");
        request.AddHeader("Referer", "http://jw.qdpec.edu.cn:8088/");
        //client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/138.0.0.0 Safari/537.36 Edg/138.0.0.0";
        request.AddHeader("Host", "jw.qdpec.edu.cn:8088");
        var response = client.Execute(request);
        var json = response.Content;
        using var doc = JsonDocument.Parse(json);
        var code = doc.RootElement.GetProperty("code").GetString();
        if (code.Equals("0"))
        {
            return string.Empty;
        }
        var token = doc.RootElement.GetProperty("data").GetProperty("token").GetString();
        return token;
    }


    static string Encrypt(string input)
    {
        string key = "qzkj1kjghd=876&*";
        // AES-ECB 加密，PKCS7
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;

            byte[] plaintextBytes = Encoding.UTF8.GetBytes($"\"{input}\""); // JS 用 JSON.stringify
            using (ICryptoTransform encryptor = aes.CreateEncryptor())
            {
                byte[] encryptedBytes = encryptor.TransformFinalBlock(plaintextBytes, 0, plaintextBytes.Length);

                // 第一次 Base64
                string base64Once = Convert.ToBase64String(encryptedBytes);

                // 第二次 Base64
                string base64Twice = Convert.ToBase64String(Encoding.UTF8.GetBytes(base64Once));

                return base64Twice;
            }
        }
    }


}
