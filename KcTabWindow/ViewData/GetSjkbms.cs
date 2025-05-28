using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KcTabWindow.ViewData
{
    public class GetSjkbms
    {
        public string Msg { get; set; }
        public int Code { get; set; }
        public List<DataItem> Data { get; set; }
        public class DataItem
        {
            public string Mrms { get; set; }
            public string Kbjcmsid { get; set; }
            public string Kbjcmsmc { get; set; }
        }

    }
}
