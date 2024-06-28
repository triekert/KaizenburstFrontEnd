using System.Windows.Controls;
using System.Windows.Input;
using static Fasetto.Word.DI;


namespace Fasetto.Word
{

    /// <summary>
    /// Interaction logic for SettingsControl.xaml
    /// </summary>
    public partial class ManageClassificationTemplateControl : UserControl
    {/// <summary>
     /// This User Control is always linked to the popup component of the ApplicationViewModel
     /// </summary>
        public ManageClassificationTemplateControl()
        {
            InitializeComponent();


            // Set data context to settings view model
            //ViewModelApplication.CurrentPopupViewModel = new ManageClassificationViewModel();
            //if (ViewModelApplication.AddElementViewModel == null)
            //    DataContext = new HierarchyElementViewModel();
            //else
            DataContext = (ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel;   
            
            ViewModelApplication.ControlParameter1= DataContext;
            //MyImage.Source = MyImage.Source;

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

 
                ((ManageClassificationViewModel)DataContext).Close();
                //e.Handled = true;
                //ImagePath
                 
           else
                if (Keyboard.IsKeyDown(Key.F2))
                    ((ManageClassificationViewModel)DataContext).OpenDocument();
 
                 else
                    if (Keyboard.IsKeyDown(Key.Insert))
                        ((ManageClassificationViewModel)DataContext).BrowseImage();
                    e.Handled = true;
        }

        private void DataGridRow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                NavigateOn();
            }
        }

        private void NavigateOn()
        { }


        }
}
