using System.Runtime.InteropServices;
using System.Text;
namespace KcWinUI.Helpers;

public class IniHelper
{
    public string FilePath { get; }

    public IniHelper(string path)
    {
        FilePath = System.IO.Path.Combine(AppContext.BaseDirectory,path);
        if (!System.IO.File.Exists(FilePath))
        {
            System.IO.File.Create(FilePath).Dispose();
        }
    }

    [DllImport("kernel32", CharSet = CharSet.Unicode)]
    private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

    [DllImport("kernel32", CharSet = CharSet.Unicode)]
    private static extern int GetPrivateProfileString(string section, string key, string defaultVal, StringBuilder retVal, int size, string filePath);

    // 写入
    public void Write(string section, string key, string value)
    {
        WritePrivateProfileString(section, key, value, FilePath);
    }

    // 读取
    public string Read(string section, string key, string defaultVal = "")
    {
        var retVal = new StringBuilder(255);
        GetPrivateProfileString(section, key, defaultVal, retVal, 255, FilePath);
        return retVal.ToString();
    }
}
