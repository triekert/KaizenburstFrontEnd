using System;
using System.Globalization;
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
}
