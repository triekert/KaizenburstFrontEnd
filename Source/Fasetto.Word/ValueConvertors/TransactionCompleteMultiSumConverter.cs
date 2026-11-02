using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Fasetto.Word
{
    public class TransactionCompleteMultiSumConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // 1. Guard against null or missing arguments
            if (values == null || values.Length < 3)
                return false;

            // 2. Guard against UnsetValue or null on any bound property
            if (values[0] == DependencyProperty.UnsetValue || values[0] == null ||
                values[1] == DependencyProperty.UnsetValue || values[1] == null ||
                values[2] == DependencyProperty.UnsetValue || values[2] == null)
            {
                return false;
            }

            if (values[0] is IEnumerable items)
            {
                var targetKey = values[1].ToString()?.Trim();

                // 3. Ignore dummy "Loading..." row key or empty keys
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

                var targetTranAmount = matchingTransactions.First().TransAmount;

                // Return true if amounts match or differ based on your coloring requirement
                return totalAssigned != targetTranAmount;
            }

            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}