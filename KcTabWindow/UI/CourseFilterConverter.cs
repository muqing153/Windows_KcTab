using System.Windows.Data;
using System.Globalization;
using KcTabWindow.ViewData;
namespace KcTabWindow.UI
{

    public class CourseFilterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is List<Curriculum.Course> courses && parameter is string xqid)
            {
                var filteredCourses = courses
                    .Where(c => c.WeekDay == xqid)
                    .Select(c => $"{c.CourseName}\n({c.ClassroomName})")
                    .ToList();
                return string.Join("\n", filteredCourses);
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class CourseToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Curriculum.Course course)
            {
                return $"{course.CourseName}\n{course.ClassroomName}";
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}