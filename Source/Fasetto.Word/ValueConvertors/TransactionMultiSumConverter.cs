using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Fasetto.Word
{
    public class TransactionMultiSumConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // Guard against null, empty, or uninitialized bindings
            if (values == null || values.Length < 2)
                return false;

            if (values[0] == DependencyProperty.UnsetValue || values[0] == null ||
                values[1] == DependencyProperty.UnsetValue || values[1] == null)
            {
                return false;
            }

            if (values[0] is IEnumerable items)
            {
                var targetKey = values[1]?.ToString()?.Trim();

                if (string.IsNullOrEmpty(targetKey))
                    return false;

                var matchingTransactions = items.OfType<TransactionViewModel>()
                    .Where(t => string.Equals(t.KFinTranID?.ToString()?.Trim(), targetKey, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (!matchingTransactions.Any())
                    return false;

                var totalAssigned = matchingTransactions
                    .Where(t => !string.IsNullOrWhiteSpace(t.KCategoryID))
                    .Sum(t => t.ActualAmount);



                return totalAssigned;
            }

            return 0.00M;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}