using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KcTabWindow.ViewData;

public class Zhou
{
    public string Name { get; set; } = "第一周";
    public string rq { get; set; } = "2024-08-26";
    public Zhou()
    {

    }

    // 重写 ToString 方法，以便在 ComboBox 中显示 WeekName
    public override string ToString()
    {
        return Name;
    }
}
