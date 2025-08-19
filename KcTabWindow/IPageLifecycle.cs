using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KcTabWindow
{
    public interface IPageLifecycle
    {
        void OnPageClosed();
    }
}
