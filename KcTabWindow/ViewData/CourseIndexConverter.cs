using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using static KcTabWindow.ViewData.Curriculum;

namespace KcTabWindow.ViewData
{
    public class CourseIndexConverter : IValueConverter
    {
        private static int index =0;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var list = value as IList<Curriculum.Course>;
            //foreach (var item in list)
            //{
            //    Debug.WriteLine(item);
            //}
            index++;
            if (index > 6)
            {
                index = 0;
            }
            if (list != null)
            {
                //$"{course.CourseName}\n({course.ClassroomName})
                return $"{list[index].CourseName}\n{list[index].ClassroomName}";
            }
            return ""; // 防止索引超出范围
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

}
