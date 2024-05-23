using System.Windows.Controls;
using System.Windows.Input;
using static Fasetto.Word.DI;

namespace Fasetto.Word
{

    /// <summary>
    /// Interaction logic for SettingsControl.xaml
    /// </summary>
    public partial class HierarchyElementControl : UserControl
    {/// <summary>
     /// This User Control is always linked to the popup component of the ApplicationViewModel
     /// </summary>
        public HierarchyElementControl()
        {
            //InitializeComponent();


            // Set data context to settings view model
            if (ViewModelApplication.CurrentPopupViewModel == null || ViewModelApplication.CurrentPopupViewModel.GetType().Name != "HierarchyElementViewModel")
            {  ViewModelApplication.CurrentPopupViewModel = new HierarchyElementViewModel();}

           
            //if (ViewModelApplication.AddElementViewModel == null)
            //    DataContext = new HierarchyElementViewModel();
            //else
            DataContext = (HierarchyElementViewModel)ViewModelApplication.CurrentPopupViewModel;

            InitializeComponent();
        }
        

        private void TextEntryControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        //private void HandleEsc(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Escape)
        //        ((HierarchyElementViewModel)DataContext).Close();
        //    e.Handled = true;   
        //}


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
