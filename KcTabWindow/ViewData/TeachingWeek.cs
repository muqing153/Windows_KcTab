using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KcTabWindow.ViewData
{
    public class TeachingWeek
    {
        public List<WeekData> Data { get; set; }
        public class WeekData
        {
            public string Week { get; set; }
        }
    }
}
