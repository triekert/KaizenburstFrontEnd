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
    public partial class CostHierarchyComboboxControl : UserControl
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
            DependencyProperty.Register("LabelWidth3", typeof(GridLength), typeof(TextEntryControl), new PropertyMetadata(GridLength.Auto, LabelWidthChangedCallback));

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public CostHierarchyComboboxControl()
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
                (d as CostHierarchyComboboxControl).LabelColumnDefinition.Width = (GridLength)e.NewValue;
            }

            // Making ex available for developer on break
#pragma warning disable CS0168
            catch (Exception ex)
            //#pragma warning restore CS0168
            {
                // Make developer aware of potential issue
                Debugger.Break();

                (d as CostHierarchyComboboxControl).LabelColumnDefinition.Width = GridLength.Auto;
            }
        }

        #endregion

        private void ComboBox1_Selected(object sender, RoutedEventArgs e)
        {
            ViewModelApplication.CurrentPageViewModel = ViewModelApplication.CurrentPageViewModel;
            //var Test2 = ((CostHierarchyViewModel)((ComboBox)sender).SelectedItem).KCategoryID;
            //if (((CostHierarchyViewModel)((ComboBox)sender).SelectedItem).KCategoryID == "Test1")
            //{ return; }
            var Test3 = ((ComboBox)sender).SelectedItem;
            if (Test3 == null) {
//                MessageBox.Show(
                 
//                    "to the selected Client",
//                    "No cost structures currently linked",

//                    MessageBoxButton.OK,   MessageBoxImage.Information
//);

                return; }
            //((CostHierarchyListViewModel)ViewModelApplication.CurrentControlViewModel).MSelectedCostHierarchy = (CostHierarchyViewModel)((ComboBox)sender).SelectedItem;
            ((CostHierarchyListViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy).MSelectedCostHierarchy = (CostHierarchyViewModel)((ComboBox)sender).SelectedItem;

            ((CostHierarchyListViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy).Edit();
            ViewModelApplication.CurrentControlViewModel = ((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).Root;

        }
    }
}
