using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Fasetto.Word
{
    /// <summary>
    /// Interaction logic for BudgetPeriodSelectionControl.xaml
    /// </summary>
    public partial class BudgetPeriodSelectionControl : UserControl
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

        public static readonly DependencyProperty LabelWidthProperty =
            DependencyProperty.Register(
                nameof(LabelWidth),
                typeof(GridLength),
                typeof(BudgetPeriodSelectionControl),
                new PropertyMetadata(GridLength.Auto, LabelWidthChangedCallback));

        #endregion

        #region Constructor

        public BudgetPeriodSelectionControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Dependency Callbacks

        public static void LabelWidthChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                if (d is BudgetPeriodSelectionControl control && e.NewValue is GridLength newLength)
                {
                    control.LabelColumnDefinition.Width = newLength;
                }
            }
            catch (Exception)
            {
                Debugger.Break();
                if (d is BudgetPeriodSelectionControl control)
                {
                    control.LabelColumnDefinition.Width = GridLength.Auto;
                }
            }
        }

        #endregion
    }
}