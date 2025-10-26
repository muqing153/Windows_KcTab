using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KcWinUI.Models;
public class TableTimeData
{
    public string Id
    {
        get; set;
    }
    public string StartTime
    {
        get; set;
    }
    public string EndTime
    {
        get; set;
    }

    public TableTimeData(string id, string startTime, string endTime)
    {
        Id = id;
        StartTime = startTime;
        EndTime = endTime;
    }
    public static TableTimeData[] TimeDatas = new TableTimeData[]
    {
        new TableTimeData("1", "08:20", "09:05"),
        new TableTimeData("2", "09:15", "10:00"),
        new TableTimeData("3", "10:20", "11:05"),
        new TableTimeData("4", "11:15", "12:00"),
        new TableTimeData("5", "13:30", "14:15"),
        new TableTimeData("6", "14:25", "15:10"),
        new TableTimeData("7", "15:30", "16:15"),
        new TableTimeData("8", "16:25", "17:10"),
        new TableTimeData("9", "18:30", "19:15"),
        new TableTimeData("10", "19:25", "20:10"),
        new TableTimeData("11", "20:15", "21:00"),
        new TableTimeData("12", "21:05", "22:40")
    };

}
