using Fasetto.Word.Core;
using System;
using System.Globalization;
using System.Windows.Media;

namespace Fasetto.Word
{
    /// <summary>
    /// A converter that takes in an RGB string such as FF00FF and converts it to a WPF brush
    /// </summary>
    public class TimeToItemConverter : BaseValueConverter<TimeToItemConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null||((string)value).Length == 0)
                return 0;
            else 
                return  (string)value;
        }
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var sname = Enum.GetName(typeof(ApplicationPage),value);
            return sname;
        }


 
    }
}
