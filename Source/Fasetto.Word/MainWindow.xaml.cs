using Fasetto.Word.Core;
using System;
using System.Windows;
using static Fasetto.Word.DI;


namespace Fasetto.Word
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new WindowViewModel(this);
            if (ViewModelApplication.CurrentCredential != null)
            {
                ViewModelApplication.CurrentSideMenuContent = SideMenuContent.Menu;
                ViewModelApplication.GoToPage(ApplicationPage.Chat);          
            }
        }

        private void AppWindow_Deactivated(object sender, EventArgs e)
        {
            // Show overlay if we lose focus
            (DataContext as WindowViewModel).DimmableOverlayVisible = true;
        }

        private void AppWindow_Activated(object sender, EventArgs e)
        {
            // Hide overlay if we are focused
            (DataContext as WindowViewModel).DimmableOverlayVisible = false;
        }
    }
}
