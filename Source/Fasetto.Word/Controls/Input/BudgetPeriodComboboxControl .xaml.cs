using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using static Fasetto.Word.DI;

namespace Fasetto.Word
{
    /// <summary>
    /// Interaction logic for TextEntryControl.xaml
    /// </summary>
    public partial class BudgetPeriodComboboxControl : UserControl
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

        //public CostHierarchyListViewModel mBPVM;
        // Using a DependencyProperty as the backing store for LabelWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelWidthProperty =
            DependencyProperty.Register("LabelWidth6", typeof(GridLength), typeof(TextEntryControl), new PropertyMetadata(GridLength.Auto, LabelWidthChangedCallback));

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public BudgetPeriodComboboxControl()
        {
            //var mBPLVM = new CostHierarchyListViewModel("8A8425E2-5766-4014-8C2F-01BD84DBC370");
            //ViewModelApplication.CurrentControlViewModel = mBPLVM;
            //DataContext = mBPLVM;
            InitializeComponent();

        }

        #endregion

        #region Dependency Callbacks

        /// <summary>
        /// Called when the label width has changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void LabelWidthChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                // Set the column definition width to the new value
                (d as BudgetPeriodComboboxControl).LabelColumnDefinition.Width = (GridLength)e.NewValue;
            }

            // Making ex available for developer on break
#pragma warning disable CS0168
            catch (Exception ex)
            //#pragma warning restore CS0168
            {
                // Make developer aware of potential issue
                Debugger.Break();

                (d as BudgetPeriodComboboxControl).LabelColumnDefinition.Width = GridLength.Auto;
            }
        }

        #endregion

        private void ComboBox1_Selected(object sender, RoutedEventArgs e)
        {
            if ((BudgetPeriodViewModel)((ComboBox)sender).SelectedItem != null)
            {
                if (ViewModelApplication.CurrentPageViewModel.GetType().Name == "BudgetSelectionPageViewModel")
                {
                    ((BudgetSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).SelectedBudget = (BudgetPeriodViewModel)((ComboBox)sender).SelectedItem;
                    ((BudgetSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).AddMonthRange();
                    ((BudgetMonthListViewModel)((BudgetSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).BudgetMonthList).BuildMonthList();
                }
                else
                { ((ExpenditureVSBudgetPageViewModel)ViewModelApplication.CurrentPageViewModel).SelectedBudget = (BudgetPeriodViewModel)((ComboBox)sender).SelectedItem; 
                    ((ExpenditureVSBudgetPageViewModel)ViewModelApplication.CurrentPageViewModel).AddMonthRange();
                    ((BudgetMonthListViewModel)((ExpenditureVSBudgetPageViewModel)ViewModelApplication.CurrentPageViewModel).BudgetMonthList).BuildMonthList();                
                }
            }
            //else
            //{ ((BudgetPeriodListViewModel)((BudgetSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).Budget).MSelectedBudgetPeriod = ((BudgetSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).SelectedBudget; }

        }
    }
}
