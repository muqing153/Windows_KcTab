using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace KcTabWindow;

public class VPN
{
    public VPN()
    {
        Task.Run(async () =>
        {
            await ConnectToVPN();

        });

    }

    private static readonly HttpClientHandler handler = new HttpClientHandler
    {
        // 忽略 SSL 证书错误
        ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
    };

    private static readonly HttpClient client = new HttpClient(handler);

    public static async Task ConnectToVPN()
    {
        try
        {
            var vpnUri = new Uri("https://vpn.qdpec.edu.cn:10443");

            // 发送请求
            var response = await client.GetAsync(vpnUri);

            // 检查响应状态码
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("VPN Connected: " + responseBody);
            }
            else
            {
                Debug.WriteLine($"Error: {response.StatusCode}");
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Exception occurred: {e.Message}");
        }
    }
}
