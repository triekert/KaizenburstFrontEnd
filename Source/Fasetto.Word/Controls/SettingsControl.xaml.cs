using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using static Fasetto.Word.DI;

namespace Fasetto.Word
{
    /// <summary>
    /// Interaction logic for SettingsControl.xaml
    /// </summary>
    public partial class SettingsControl : UserControl
    {
        public SettingsControl()
        {
            InitializeComponent();

            // Set data context to settings view model

            // If we are in design mode...
            if (DesignerProperties.GetIsInDesignMode(this))
                // Create new instance of settings view model
                DataContext = new SettingsViewModel();
            else
                DataContext = ViewModelSettings;
        }

        private void TextEntryControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
