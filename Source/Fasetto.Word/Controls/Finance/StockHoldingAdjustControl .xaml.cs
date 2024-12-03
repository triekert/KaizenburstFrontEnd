using System.Windows.Controls;
using System.Windows.Input;
using static Fasetto.Word.DI;

namespace Fasetto.Word
{

    /// <summary>
    /// Interaction logic for SettingsControl.xaml
    /// </summary>
    public partial class StockHoldingAdjustControl : UserControl
    {/// <summary>
     /// This User Control is always linked to the popup component of the ApplicationViewModel
     /// </summary>
        public StockHoldingAdjustControl()
        {
            InitializeComponent();

            // Set data context to settings view model
            if (ViewModelApplication.CurrentPopupViewModel.GetType().Name != "StockHoldingAdjustViewModel")

            { ViewModelApplication.CurrentPopupViewModel = new StockHoldingAdjustViewModel(((BudgetTreeViewModel)ViewModelApplication.CurrentControlViewModel).mRootHierarchyElement1);}
            //if (ViewModelApplication.AddElementViewModel == null)
            //    DataContext = new HierarchyElementViewModel();
            //else
            DataContext = (StockHoldingAdjustViewModel)ViewModelApplication.CurrentPopupViewModel;

        }
        

        private void TextEntryControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {

        }




        private void TreeView_KeyBoard(object sender, KeyboardEventArgs e)
        {
            //check to determine whether user would like to add an item to the hierarchy

            if (Keyboard.IsKeyDown(Key.Escape))
            {
                ((HierarchyElementViewModel)DataContext).Close();
                e.Handled = true;
            }

        }
    }
}
