namespace KcWinUI.Models;
public class TableHData
{
    public string Title
    {
        get; set;
    }
    public string Data
    {
        get; set;
    }

    public TableHData(string title, string data)
    {
        Title = title;
        Data = data;
    }
    
}
