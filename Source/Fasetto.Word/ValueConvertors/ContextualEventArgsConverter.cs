using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Fasetto.Word
{
    //Instead of inventing your own behavior, you can use the EventArgsConverter and the EventArgsConverterParameter properties to create a simple IValueConverter that combines the original event args and the object that you would usually pass as CommandParameter:

    public class ContextualEventArgs : EventArgs
    {
        public object? Context { get; }

        public EventArgs OriginalEventArgs { get; }

        public ContextualEventArgs(EventArgs original, object? context) : base()
        {
            Context = context;
            OriginalEventArgs = original;
        }
    }
    public class ContextualEventArgsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is EventArgs args)
            {
                return new ContextualEventArgs(args, parameter);
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IndentConverter : IValueConverter
    {
        private const int IndentSize = 16;  // hard-coded into the XAML template

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new GridLength(((GridLength)value).Value + IndentSize);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }


}
