
using System.Text.Json.Serialization;
namespace KcWinUI.Models;
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 "required" 修饰符或声明为可为 null。
public class Curriculum
{
    [JsonPropertyName("Msg")]
    public string Msg { get; set; }

    [JsonPropertyName("code")]
    public string Code { get; set; }

    [JsonPropertyName("data")]
    public List<DataItem> Data { get; set; }

    [JsonPropertyName("needClassName")]
    public string NeedClassName { get; set; }

    [JsonPropertyName("needClassRoomNub")]
    public string NeedClassRoomNub { get; set; }
    public class DateInfo
    {
        [JsonPropertyName("xqmc")]
        public string Xqmc { get; set; }

        [JsonPropertyName("mxrq")]
        public string Mxrq { get; set; }

        [JsonPropertyName("zc")]
        public string Zc { get; set; }

        [JsonPropertyName("xqid")]
        public string Xqid { get; set; }

        [JsonPropertyName("rq")]
        public string Rq { get; set; }
    }

    public class Course
    {
        [JsonPropertyName("classWeek")]
        public string ClassWeek { get; set; }

        [JsonPropertyName("teacherName")]
        public string TeacherName { get; set; }

        [JsonPropertyName("weekNoteDetail")]
        public string WeekNoteDetail { get; set; }

        [JsonPropertyName("buttonCode")]
        public string ButtonCode { get; set; }

        [JsonPropertyName("xkrs")]
        public int Xkrs { get; set; }

        [JsonPropertyName("ktmc")]
        public string Ktmc { get; set; }

        [JsonPropertyName("classTime")]
        public string ClassTime { get; set; }

        [JsonPropertyName("classroomNub")]
        public string ClassroomNub { get; set; }

        [JsonPropertyName("jx0408id")]
        public string Jx0408Id { get; set; }

        [JsonPropertyName("buildingName")]
        public string BuildingName { get; set; }

        [JsonPropertyName("courseName")]
        public string CourseName { get; set; }

        [JsonPropertyName("isRepeatCode")]
        public string IsRepeatCode { get; set; }

        [JsonPropertyName("jx0404id")]
        public string Jx0404Id { get; set; }

        [JsonPropertyName("weekDay")]
        public string WeekDay { get; set; }

        [JsonPropertyName("classroomName")]
        public string ClassroomName { get; set; }
        public string GetClassroomName()
        {
            return ClassroomName == null ? "网课" : ClassroomName;
        }


        [JsonPropertyName("khfs")]
        public string Khfs { get; set; }

        [JsonPropertyName("startTime")]
        public string StartTime { get; set; }

        [JsonPropertyName("endTIme")]
        public string EndTime { get; set; }

        [JsonPropertyName("location")]
        public string Location { get; set; }

        [JsonPropertyName("fzmc")]
        public string Fzmc { get; set; }

        [JsonPropertyName("classWeekDetails")]
        public string ClassWeekDetails { get; set; }

        [JsonPropertyName("coursesNote")]
        public int CoursesNote { get; set; }
    }

    public class Node
    {
        [JsonPropertyName("nodeName")]
        public string NodeName { get; set; }

        [JsonPropertyName("nodeNumber")]
        public string NodeNumber { get; set; }
    }

    public class TopInfo
    {
        [JsonPropertyName("semesterId")]
        public string SemesterId { get; set; }

        [JsonPropertyName("week")]
        public int Week { get; set; }

        [JsonPropertyName("today")]
        public string Today { get; set; }

        [JsonPropertyName("weekday")]
        public string Weekday { get; set; }

        [JsonPropertyName("maxWeek")]
        public string MaxWeek { get; set; }
    }


    public class DataItem
    {
        [JsonPropertyName("date")]
        public List<DateInfo> Date { get; set; }

        [JsonPropertyName("courses")]
        public List<Course> Courses { get; set; }

        [JsonPropertyName("nodesLst")]
        public List<Node> NodesLst { get; set; }

        [JsonPropertyName("item")]
        public List<List<List<Course>>> Item { get; set; }

        [JsonPropertyName("week")]
        public int Week { get; set; }

        [JsonPropertyName("nodes")]
        public Nodes Nodes { get; set; }

        [JsonPropertyName("weekday")]
        public string Weekday { get; set; }

        [JsonPropertyName("bz")]
        public string Bz { get; set; }

        [JsonPropertyName("topInfo")]
        public List<TopInfo> TopInfo { get; set; }
    }

    public class Nodes
    {
        [JsonPropertyName("sw")]
        public List<string> Sw { get; set; }

        [JsonPropertyName("ws")]
        public List<string> Ws { get; set; }

        [JsonPropertyName("zw")]
        public List<string> Zw { get; set; }

        [JsonPropertyName("xw")]
        public List<string> Xw { get; set; }
    }
}
