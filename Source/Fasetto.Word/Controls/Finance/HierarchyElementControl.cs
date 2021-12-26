using System.ComponentModel;
using System.Windows.Controls;
using static Fasetto.Word.DI;

namespace Fasetto.Word
{
    /// <summary>
    /// Interaction logic for SettingsControl.xaml
    /// </summary>
    public partial class HierarchyElementControl : UserControl
    {
        public HierarchyElementControl()
        {
            InitializeComponent();

            // Set data context to settings view model

            // If we are in design mode...
            if (DesignerProperties.GetIsInDesignMode(this))
                // Create new instance of settings view model
                DataContext = new HierarchyElementViewModel();
            else
                DataContext = new HierarchyElementViewModel();
        }
        public HierarchyElementControl(HierarchyElementViewModel NewElement)
        {
            InitializeComponent();

            // Set data context to settings view model

            // If we are in design mode...
            if (DesignerProperties.GetIsInDesignMode(this))
                // Create new instance of settings view model
                DataContext = new HierarchyElementViewModel();
            else
                DataContext = NewElement;
        }
        private void PasswordEntryControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
