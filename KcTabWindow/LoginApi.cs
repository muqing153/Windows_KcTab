using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.System;
using Console = System.Diagnostics.Debug;

namespace KcTabWindow
{
    public class USERDATA
    {
        public string account;
        public string password;
        public string rsa;
    }
    public class LoginApi
    {
        public LoginApi()
        {
            LoadUserData();
        }
        public static string CASTGC = string.Empty;
        private string JSESSIONID = string.Empty;
        public static USERDATA UserData = new USERDATA();
        public static void SaveUserData()
        {
            string jsonString = JsonConvert.SerializeObject(UserData, Formatting.Indented);
            File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"User.Data"), jsonString);
        }
        public static void LoadUserData()
        {
            string v = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "User.Data");
            if (File.Exists(v))
            {
                // 从文件中读取 JSON 字符串
                string loadedJsonString = File.ReadAllText(v);
                // 反序列化为 USERDATA 对象
                USERDATA? loadedUser = JsonConvert.DeserializeObject<USERDATA>(loadedJsonString);
                if (loadedUser != null)
                {
                    UserData = loadedUser;
                }
            }
        }

        public async Task One()
        {
            var client = new RestClient(new RestClientOptions("http://pass.qdpec.edu.cn:19580/tpass/login")
            {
                FollowRedirects = true,
                ThrowOnAnyError = false,
                Proxy = new WebProxy() { UseDefaultCredentials = false }, // 禁用代理
                UserAgent = "Apifox/1.0.0 (https://apifox.com)"
            });
            var request = new RestRequest("", Method.Post);
            request.AddHeader("Accept", "*/*");
            request.AddHeader("Host", "pass.qdpec.edu.cn:19580");
            request.AddHeader("Connection", "keep-alive");
            request.AddParameter("rsa", UserData.rsa);
            request.AddParameter("ul", "12");
            request.AddParameter("pl", "13");
            request.AddParameter("execution", "e1s1");
            request.AddParameter("_eventId", "submit");
            // 发送第一次请求
            var response = await client.ExecuteAsync(request);
            if (response.ErrorException != null)
            {
                Console.WriteLine($"请求错误: {response.ErrorException.Message}");
                // return;
            }
            Console.WriteLine("第一次请求返回：");
            foreach (Cookie cookie in response.Cookies)
            {
                Console.WriteLine($"{cookie.Name}={cookie.Value}");
                if (cookie.Name == "JSESSIONID")
                {
                    JSESSIONID = cookie.Value;
                    client = new RestClient(new RestClientOptions("http://pass.qdpec.edu.cn:19580/tpass/login")
                    {
                        FollowRedirects = false,
                        ThrowOnAnyError = false,
                        Proxy = new WebProxy() { UseDefaultCredentials = false }, // 禁用代理
                        UserAgent = "Apifox/1.0.0 (https://apifox.com)"
                    });
                    request = new RestRequest("", Method.Post);
                    request.AddHeader("Cookie", "JSESSIONID=" + cookie.Value);
                    request.AddHeader("Accept", "*/*");
                    request.AddHeader("Host", "pass.qdpec.edu.cn:19580");
                    request.AddHeader("Connection", "keep-alive");
                    request.AddParameter("rsa", UserData.rsa);
                    request.AddParameter("ul", "12");
                    request.AddParameter("pl", "13");
                    request.AddParameter("execution", "e1s1");
                    request.AddParameter("_eventId", "submit");
                    var c = await client.ExecuteAsync(request);
                    Console.WriteLine("第二次请求返回：");
                    foreach (Cookie cook in c.Cookies)
                    {
                        Console.WriteLine($"{cook.Name}={cook.Value}");
                        if (cook.Name.Equals("CASTGC"))
                        {
                            CASTGC = cook.Value;
                            break;
                        }
                    }
                    break;
                }
            }
        }


        public bool GetToken(string response)
        {
            string pattern = @"token=([^&]+)";
            Match match = Regex.Match(response, pattern);
            if (match.Success)
            {
                string jwt = match.Groups[1].Value;
                Token = jwt;
            }
            else
            {
                Token = string.Empty;
            }
            return match.Success;
        }
        //public string Token = string.Empty;
        //public string CASTGC = "TGT-202413220343-56074-ukzSz4Ml9LPOcHY1fZKMgPlmsbm5FLx2GZ7qMbl2aoHJaJBgEp-tpass";
        //public string JSESSIONID = string.Empty;

        public static string Token = string.Empty;
        public async Task<bool> loginSso()
        {

            var client = new RestClient(new RestClientOptions("http://jw.qdpec.edu.cn:8088/njwhd/loginSso")
            {
                FollowRedirects = false,
                ThrowOnAnyError = false,
                Proxy = new WebProxy() { UseDefaultCredentials = false }, // 禁用代理
                UserAgent = "Apifox/1.0.0 (https://apifox.com)",
                // CookieContainer = cookieContainer // 允许 RestClient 处理 Cookies
            });
            var request = new RestRequest("")
            {
                Method = Method.Get
            };
            string Cookie = $"CASTGC={CASTGC};" +
                "neusoft_cas_pd=B45C58DDE84507C733B17B3DB70C44FE003BEC6923E1C604B3EA9B47A79DAAC7;neusoft_cas_un=202413220343; ";
            request.AddHeader("Cookie", Cookie);
            if (JSESSIONID != string.Empty)
            {
                request.AddCookie("JSESSIONID", JSESSIONID, "/", "jw.qdpec.edu.cn");
            }
            //client.UserAgent =;
            request.AddHeader("Accept", "*/*");
            request.AddHeader("Host", "jw.qdpec.edu.cn:8088");
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Referer", "http://jw.qdpec.edu.cn:8088/njwhd/loginSso?ticket=ST-98218-IGlkN5McdK7esGZkxMEe-tpass");

            var response = await client.ExecuteAsync(request);
            if (GetToken(response.Content))
            {
                Console.WriteLine("Token:" + Token);
                return true;
            }
            var location = response.Headers.FirstOrDefault(x => x.Name == "Location")?.Value.ToString();
            Console.WriteLine(location);
            if (location != null)
            {
                client = new RestClient(new RestClientOptions(location)
                {
                    FollowRedirects = false,
                    ThrowOnAnyError = false,
                    Proxy = new WebProxy() { UseDefaultCredentials = false }, // 禁用代理
                    UserAgent = "Apifox/1.0.0 (https://apifox.com)",
                    // CookieContainer = cookieContainer // 允许 RestClient 处理 Cookies
                });
                request = new RestRequest("")
                {
                    Method = Method.Get
                };
                request.AddHeader("Cookie", Cookie);
                response = await client.ExecuteAsync(request);
                location = response.Headers.FirstOrDefault(x => x.Name == "Location")?.Value.ToString();
                Console.WriteLine("第二次：" + location);
                if (location != null)
                {
                    client = new RestClient(new RestClientOptions(location)
                    {
                        FollowRedirects = false,
                        ThrowOnAnyError = false,
                        Proxy = new WebProxy() { UseDefaultCredentials = false }, // 禁用代理
                        UserAgent = "Apifox/1.0.0 (https://apifox.com)",
                        // CookieContainer= cookieContainer // 允许 RestClient 处理 Cookies
                    });
                    request = new RestRequest("")
                    {
                        Method = Method.Get
                    };
                    response = await client.ExecuteAsync(request);
                    foreach (Cookie cookie in response.Cookies)
                    {
                        if (cookie.Name == "JSESSIONID")
                        {
                            JSESSIONID = cookie.Value;
                        }
                        return await loginSso();
                        Console.WriteLine(cookie.Name + ":" + cookie.Value);
                    }
                    Console.WriteLine("最终：" + response.Content);
                }
            }
            return false;
        }


        public static void SingToken()
        {
            var a = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(a, "Token.ini");
            if (File.Exists(filePath))
            {
                Token = File.ReadAllText(filePath);
            }
            // 读取文件

        }
        public static void SaveToken(string str)
        {
            var a = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(a, "Token.ini");

            // 写入文件
            File.WriteAllText(filePath, str ?? "");

        }
    }
}
