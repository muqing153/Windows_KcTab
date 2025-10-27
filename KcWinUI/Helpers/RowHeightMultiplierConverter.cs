using KcWinUI.Models;
using Microsoft.UI.Xaml.Data;
using System;
using System.Diagnostics;

namespace KcWinUI.Helpers;
public class RowHeightMultiplierConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        // 默认基准高度
        var baseHeight = 60;
        if (value != null && value is string a)
        {
            var jie = Curriculum.GetJieData(a);
            if (jie != null)
            {
                //Debug.WriteLine(jie.Count-1);
                return baseHeight * (jie.Count - 1);
            }
        }
        return baseHeight;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }

}
