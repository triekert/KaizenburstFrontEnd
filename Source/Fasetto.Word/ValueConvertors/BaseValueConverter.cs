using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Fasetto.Word
{
    /// <summary>
    /// A base value converter that allows direct XAML usage
    /// </summary>
    /// <typeparam name="T">The type of this value converter</typeparam>
    public abstract class BaseValueConverter<T> : MarkupExtension, IValueConverter
        where T : class, new()
    {
        #region Private Members

        /// <summary>
        /// A single static instance of this value converter
        /// </summary>
        private static T Converter = null;

        #endregion

        #region Markup Extension Methods

        /// <summary>
        /// Provides a static instance of the value converter 
        /// </summary>
        /// <param name="serviceProvider">The service provider</param>
        /// <returns></returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            // MarkupExtension.ProvideValue is abstract, so do NOT call base.ProvideValue.
            // Simply return the singleton converter instance for both runtime and design-time.
            return Converter ?? (Converter = new T());
        }

        #endregion

        #region Value Converter Methods

        /// <summary>
        /// The method to convert one type to another
        /// </summary>
        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

        /// <summary>
        /// The method to convert a value back to its source type
        /// </summary>
        public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);

        #endregion
    }
}