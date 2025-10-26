
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using DevLab.JmesPath;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static KcWinUI.Models.Curriculum;
namespace KcWinUI.Models;
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 "required" 修饰符或声明为可为 null。
public class Curriculum
{
    public Curriculum()
    {
        course = new List<Course>();
    }
    /// <summary>
    /// 获取节数据解析，例如 “101102” → [1, 1, 2]；“1,101102” 等
    /// </summary>
    public static List<int>? GetJieData(string? str)
    {
        if (string.IsNullOrEmpty(str))
            return null;

        List<int> jies = new List<int>();
        var parts = str.Split(',');

        if (parts.Length > 1)
        {
            jies.Add(int.Parse(parts[0].Substring(0, 1)));
            foreach (var part in parts)
            {
                jies.Add(int.Parse(part.Substring(1)));
            }
        }
        else
        {
            jies.Add(int.Parse(str[0].ToString()));
            int i = 1;
            while (i < str.Length)
            {
                jies.Add(int.Parse(str.Substring(i, Math.Min(2, str.Length - i))));
                i += 2;
            }
        }
        return jies;
    }

    /// <summary>
    /// 获取当前学期
    /// </summary>
    public static string GetCurrentSemester()
    {
        var now = DateTime.Now;
        var year = now.Year;
        var month = now.Month;

        // 上半年（1~6月） → 去年-今年-2
        if (month >= 1 && month <= 6)
        {
            return $"{year - 1}-{year}-2";
        }
        // 下半年（7~12月） → 今年-明年-1
        else
        {
            return $"{year}-{year + 1}-1";
        }
    }

    /// <summary>
    /// 获取当前学期的开学日期
    /// </summary>
    public static string GetSemesterStartDate()
    {
        var now = DateTime.Now;
        var year = now.Year;
        var month = now.Month;
        DateTime startDate;

        if (month >= 1 && month <= 6)
        {
            // 上半年 第二学期：次年2月20日
            startDate = new DateTime(year, 2, 20);
        }
        else
        {
            // 下半年 第一学期：当年9月1日
            startDate = new DateTime(year, 9, 1);
        }

        return startDate.ToString("yyyy-MM-dd");
    }

    /// <summary>
    /// 根据开学日期和目标日期计算是第几周
    /// </summary>
    public static int GetWeekIndex(string startDate, string targetDate)
    {
        DateTime start = DateTime.Parse(startDate, CultureInfo.InvariantCulture);
        DateTime target = DateTime.Parse(targetDate, CultureInfo.InvariantCulture);

        var diff = target - start;
        if (diff.TotalMilliseconds < 0)
            return 0;

        int days = (int)Math.Floor(diff.TotalDays);
        return (days / 7) + 1;
    }

    /// <summary>
    /// 获取某一周的周一到周日日期
    /// </summary>
    public static List<TableHData> GetWeekDays(string startDate, int weekIndex)
    {
        DateTime start = DateTime.Parse(startDate, CultureInfo.InvariantCulture);
        DateTime weekStart = start.AddDays((weekIndex - 1) * 7);
        string[] weekNames = { "一", "二", "三", "四", "五", "六", "日" };

        var result = new List<TableHData>();
        for (int i = 0; i < 7; i++)
        {
            DateTime day = weekStart.AddDays(i);
            string formatted = day.ToString("yyyy-MM-dd");
            result.Add(new TableHData(weekNames[i], formatted));
        }
        return result;
    }

    /// <summary>
    /// 创建新课程对象
    /// </summary>
    public static Course NewCourse(int day, int j)
    {
        return new Course
        {
            CourseName = "",
            ClassroomName = "",
            WeekDay = day,
            WeekNoteDetail = $"{day}{j.ToString("D2")}" // "01"
        };
    }

    /// <summary>
    /// 保存课程表
    /// </summary>
    public static bool SaveCurriculum(string path, Curriculum curr)
    {
        try
        {
            int mod = XueXiaoData.MODS;
            var jsondata = JsonConvert.SerializeObject(curr, Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore // 忽略 null
            });
            File.WriteAllText(path, jsondata);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 创建新课程表
    /// </summary>
    public static Curriculum NewCurriculum()
    {
         return new Curriculum();
    }

    /// <summary>
    /// 解析 JSON 数据生成课程表
    /// </summary>
    public static Curriculum ParsJSON(string json, XueXiaoData xuexiao, Curriculum? currdebug = null)
    {
        currdebug ??= new Curriculum();
        try
        {
            if (xuexiao.Json == null)
                throw new Exception("请先设置 json");

            // 使用 JmesPath 获取主数据节点
            var jmes = new JmesPath();
            var dataExpr = xuexiao.Json.Data ?? "";
            var resultDataJson = jmes.Transform(json, dataExpr);
            JToken dataArray = JToken.Parse(resultDataJson);

            foreach (var element in dataArray.Children())
            {
                // ✅ 用 SelectToken 直接提取
                var courseName = element.SelectToken(xuexiao.Json.Parsing.CourseName)?.ToString()
                                 ?? throw new Exception("courseName 未能提取");
                var teacherName = element.SelectToken(xuexiao.Json.Parsing.TeacherName)?.ToString() ?? "";
                var ktmc = element.SelectToken(xuexiao.Json.Parsing.Ktmc)?.ToString() ?? "";
                var weekNoteDetail = element.SelectToken(xuexiao.Json.Parsing.WeekNoteDetail)?.ToString() ?? "";
                var classroomName = element.SelectToken(xuexiao.Json.Parsing.ClassroomName)?.ToString() ?? "";
                var classWeek = element.SelectToken(xuexiao.Json.Parsing.ClassWeek)?.ToString() ?? "";

                Debug.WriteLine($"解析课程: {courseName}");

                // 查找或创建课程对象
                var co = currdebug.course.FirstOrDefault(a => a.CourseName == courseName) ?? new Course();
                co.CourseName = courseName;
                co.TeacherName = teacherName;
                co.Ktmc = ktmc;

                co.ClassWeekDetails ??= new List<ClassWeekDetails>();
                var existing = co.ClassWeekDetails.FirstOrDefault(a => a.Weeks == classWeek);

                if (existing != null)
                {
                    if (!existing.Weekdays!.Any(w => w.Jie == weekNoteDetail))
                    {
                        existing.Weekdays.Add(new Weekday { Jie = weekNoteDetail, ClassroomName = classroomName });
                    }
                }
                else
                {
                    co.ClassWeekDetails.Add(new ClassWeekDetails
                    {
                        Weeks = classWeek,
                        Weekdays = new List<Weekday>
                    {
                        new Weekday { Jie = weekNoteDetail, ClassroomName = classroomName }
                    }
                    });
                }

                if (!currdebug.course.Any(a => a.CourseName == courseName))
                {
                    currdebug.course.Add(co);
                }
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"ParsJSON Error: {ex.Message}");
        }
        return currdebug;
    }

    public List<Course> course;
    public class Course
    {
        [JsonPropertyName("classWeek")]
        public string ClassWeek
        {
            get; set;
        }

        [JsonPropertyName("teacherName")]
        public string TeacherName
        {
            get; set;
        }

        [JsonPropertyName("weekNoteDetail")]
        public string WeekNoteDetail
        {
            get; set;
        }
        //public int Height
        //{
        //    get; set;
        //} = 100;
        [JsonPropertyName("buttonCode")]
        public string ButtonCode
        {
            get; set;
        }

        [JsonPropertyName("xkrs")]
        public int Xkrs
        {
            get; set;
        }

        [JsonPropertyName("ktmc")]
        public string Ktmc
        {
            get; set;
        }

        [JsonPropertyName("classTime")]
        public string ClassTime
        {
            get; set;
        }

        [JsonPropertyName("classroomNub")]
        public string ClassroomNub
        {
            get; set;
        }

        [JsonPropertyName("jx0408id")]
        public string Jx0408Id
        {
            get; set;
        }

        [JsonPropertyName("buildingName")]
        public string BuildingName
        {
            get; set;
        }

        [JsonPropertyName("courseName")]
        public string CourseName
        {
            get; set;
        }

        [JsonPropertyName("isRepeatCode")]
        public string IsRepeatCode
        {
            get; set;
        }

        [JsonPropertyName("jx0404id")]
        public string Jx0404Id
        {
            get; set;
        }

        [JsonPropertyName("weekDay")]
        public int WeekDay
        {
            get; set;
        }

        [JsonPropertyName("classroomName")]
        public string ClassroomName
        {
            get; set;
        }

        public int Start
        {
            get; set;
        }
        public int End
        {
            get; set;
        }
        public string GetClassroomName()
        {
            return ClassroomName == null ? "网课" : ClassroomName;
        }


        [JsonPropertyName("khfs")]
        public string Khfs
        {
            get; set;
        }

        [JsonPropertyName("startTime")]
        public string StartTime
        {
            get; set;
        }

        [JsonPropertyName("endTIme")]
        public string EndTime
        {
            get; set;
        }

        [JsonPropertyName("location")]
        public string Location
        {
            get; set;
        }

        [JsonPropertyName("fzmc")]
        public string Fzmc
        {
            get; set;
        }

        [JsonPropertyName("classWeekDetails")]
        public List<ClassWeekDetails> ClassWeekDetails
        {
            get; set;
        }

        [JsonPropertyName("coursesNote")]
        public int CoursesNote
        {
            get; set;
        }
    }
    public class ClassWeekDetails
    {
        // 示例: "1", "1-7", "8,9,10,11"
        public string? Weeks
        {
            get; set;
        }

        // 默认 1-7
        public List<Weekday>? Weekdays
        {
            get; set;
        }
    }

    public class Weekday
    {
        public string? Jie
        {
            get; set;
        }

        public string? ClassroomName
        {
            get; set;
        }
    }

    public string nian; //这是学年的标记
    public string startDate; // 本学期是从几月几日开启的
    public int ZhouInt; //本学期一共有多少周 NUll时候默认20
}
