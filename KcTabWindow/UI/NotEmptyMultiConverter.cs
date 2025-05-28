using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace KcTabWindow.UI
{

    public class NotEmptyMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Debug.WriteLine("Converter Called");
            if (values.Length < 2) return false;
            string account = values[0] as string ?? "";
            bool isCheckBoxChecked = values[1] is bool && (bool)values[1];  
            //bool a = box.ToLower().Equals("true");
            Debug.WriteLine(values[1] + " " + isCheckBoxChecked);
            return !string.IsNullOrWhiteSpace(account) && isCheckBoxChecked;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
