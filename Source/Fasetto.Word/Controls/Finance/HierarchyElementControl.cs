using System.ComponentModel;
using System.Windows.Controls;
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
            InitializeComponent();

            // Set data context to settings view model
            ViewModelApplication.CurrentPopupViewModel= new HierarchyElementViewModel();
            //if (ViewModelApplication.AddElementViewModel == null)
            //    DataContext = new HierarchyElementViewModel();
            //else
            DataContext = (HierarchyElementViewModel)ViewModelApplication.CurrentPopupViewModel;



        }

        private void PasswordEntryControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void TextEntryControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
