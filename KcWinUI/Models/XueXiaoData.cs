namespace KcWinUI.Models;


public class XueXiaoData
{
    public JsonConfig? Json
    {
        get; set;
    }

    // HTML 可以是任意对象，所以这里使用 object
    public object? Html
    {
        get; set;
    }

    public string? WebIP
    {
        get; set;
    }

    // 静态属性，用于表示模式（1: 覆盖模式, 2: 追加模式, 3: ...）
    public static int MODS { get; set; } = 0;
}

public class JsonConfig
{
    public string? Data
    {
        get; set;
    }
    public Parsing? Parsing
    {
        get; set;
    }
}

public class Parsing
{
    public string? CourseName
    {
        get; set;
    }
    public string? ClassWeek
    {
        get; set;
    }
    public string? TeacherName
    {
        get; set;
    }
    public string? ClassroomName
    {
        get; set;
    }
    public string? WeekDay
    {
        get; set;
    }
    public string? WeekNoteDetail
    {
        get; set;
    }
    public string? Ktmc
    {
        get; set;
    }
}
