using System;
using System.Globalization;
using System.Windows.Data;

namespace Fasetto.Word
{
    [ValueConversion(typeof(DateTime), typeof(string))]
    public class TimeToClosestComboBoxItemConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not DateTime targetDateTime)
                return null;

            var totalMinutes = targetDateTime.Hour * 60 + targetDateTime.Minute;
            var remainder = totalMinutes % 30;

            if (remainder >= 15) totalMinutes += (30 - remainder);
            else totalMinutes -= remainder;

            if (totalMinutes >= 1440) totalMinutes = 0;

            return $"{totalMinutes / 60:D2}:{totalMinutes % 60:D2}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var timeString = value?.ToString();

            if (!string.IsNullOrEmpty(timeString) && TimeSpan.TryParse(timeString, out var timeSpan))
            {
                return timeSpan;
            }

            return Binding.DoNothing;
        }
    }
}