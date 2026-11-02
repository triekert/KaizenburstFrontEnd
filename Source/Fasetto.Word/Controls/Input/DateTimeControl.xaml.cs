using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Fasetto.Word
{
    /// <summary>
    /// Interaction logic for DateTimeControl.xaml
    /// </summary>
    public partial class DateTimeControl : UserControl
    {
        #region Dependency Properties

        /// <summary>
        /// The label width of the control
        /// </summary>
        public GridLength LabelWidth
        {
            get => (GridLength)GetValue(LabelWidthProperty);
            set => SetValue(LabelWidthProperty, value);
        }

        // Using a DependencyProperty as the backing store for LabelWidth.
        public static readonly DependencyProperty LabelWidthProperty =
            DependencyProperty.Register(
                nameof(LabelWidth),
                typeof(GridLength),
                typeof(DateTimeControl),
                new PropertyMetadata(GridLength.Auto, LabelWidthChangedCallback));

        #endregion

        #region Constructor

        public DateTimeControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Dependency Callbacks

        public static void LabelWidthChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                if (d is DateTimeControl control && e.NewValue is GridLength newLength)
                {
                    control.LabelColumnDefinition.Width = newLength;
                }
            }
            catch (Exception ex)
            {
                Debugger.Break();
                if (d is DateTimeControl control)
                {
                    control.LabelColumnDefinition.Width = GridLength.Auto;
                }
            }
        }

        #endregion

        private void TP_Selected(object sender, RoutedEventArgs e)
        {
            if (TP.SelectedValue is string timeString)
            {
                // Access or process the selected time string directly
                var selectedTime = timeString;
            }
        }

        private void TP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TP.SelectedValue is string timeString)
            {
                // Handle selection change logic using the time string
                var selectedTime = timeString;
            }
        }
    }
}