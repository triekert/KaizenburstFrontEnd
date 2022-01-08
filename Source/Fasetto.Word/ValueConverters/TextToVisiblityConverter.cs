using System;
using System.Globalization;
using System.Windows;

namespace Fasetto.Word
{
    /// <summary>
    /// A converter that takes in a boolean and returns a <see cref="Visibility"/>
    /// </summary>
    public class TextToVisiblityConverter : BaseValueConverter<TextToVisiblityConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null)
                return (value==null) ? Visibility.Hidden : Visibility.Visible;
            else
                return (value == null) ? Visibility.Hidden : Visibility.Visible;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
