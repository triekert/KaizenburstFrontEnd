using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using static Fasetto.Word.DI;

namespace Fasetto.Word
{

    /// <summary>
    /// Interaction logic for SettingsControl.xaml
    /// </summary>
    public partial class ManageClassificationControl : UserControl
    {/// <summary>
     /// This User Control is always linked to the popup component of the ApplicationViewModel
     /// </summary>
        public ManageClassificationControl()
        {



            // Set data context to settings view model
            //ViewModelApplication.CurrentPopupViewModel = new ManageClassificationViewModel();
            //if (ViewModelApplication.AddElementViewModel == null)
            //    DataContext = new HierarchyElementViewModel();
            //else
            DataContext = (ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel;   
            
            ViewModelApplication.ControlParameter1= DataContext;
            //MyImage.Source = MyImage.Source;
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



        private void ScrollViewer_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Check if user pressed Ctrl + Enter (Return)
            if ((e.Key == Key.Return || e.Key == Key.Enter) &&
                (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (DataContext is ManageClassificationViewModel vm && vm.FocusAddClassificationCommand != null)
                {
                    if (vm.FocusAddClassificationCommand.CanExecute(null))
                    {
                        vm.FocusAddClassificationCommand.Execute(null);
                    }
                }

                // CRITICAL: Mark the event as fully handled so it stops bubbling 
                // and doesn't trigger parent window shortcuts or default commands (like closing)
                e.Handled = true;
            }
        }
            
            
            
            
            //private void HandleEsc(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Escape)
        //        ((HierarchyElementViewModel)DataContext).Close();
        //    e.Handled = true;   
        //}



        private void TreeView_KeyBoard(object sender, KeyEventArgs e)
        {
            // Check if user pressed Ctrl + Enter (Return)
            //if ((e.Key == Key.Return || e.Key == Key.Enter) &&
            //    (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            //{
            //    if (DataContext is ManageClassificationViewModel vm && vm.FocusAddClassificationCommand != null)
            //    {
            //        if (vm.FocusAddClassificationCommand.CanExecute(null))
            //        {
            //            vm.FocusAddClassificationCommand.Execute(null);
            //        }
            //    }

            //    // CRITICAL: Mark the event as fully handled so it stops bubbling 
            //    // and doesn't trigger parent window shortcuts or default commands (like closing)
            //    e.Handled = true;
            //}
        }

        private void DataGridRow_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.F2) && ((DocDataViewModel)Transaction.CurrentItem).DocURL != "\\somepath\\filename.jpg")

            {
                ((ManageClassificationViewModel)DataContext).OpenDocument((DocDataViewModel)Transaction.CurrentItem);
                e.Handled = true;
            }

            else
                if (Keyboard.IsKeyDown(Key.Insert))
                //link a new document if the placeholder is still not used, otherwise, create a new placeholder and fill
                {
                    ((ManageClassificationViewModel)DataContext).BrowseImage();
                    e.Handled = true;
                }

                else
                    if (Keyboard.IsKeyDown(Key.Delete))
                    //link a new document if the placeholder is still not used, otherwise, create a new placeholder and fill
                    //((ManageClassificationViewModel)DataContext).RemoveImage((DocDataViewModel)Transaction.SelectedItem);
                    {
                        ((ManageClassificationViewModel)DataContext).RemoveDocument((DocDataViewModel)Transaction.SelectedItem);
                        e.Handled = true;
                    }

            //if (e.Key == Key.Enter)
            //{
            //    NavigateOn();
            //}
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //if no document has been linked to the dummy placeholder yet, adda document into the placeholder...
            if (((DocDataViewModel)Transaction.CurrentItem).DocURL != "\\somepath\\filename.jpg")
                ((ManageClassificationViewModel)DataContext).OpenDocument((DocDataViewModel)Transaction.CurrentItem);
            else
                ((ManageClassificationViewModel)DataContext).BrowseImage();

            e.Handled = true;
            //MessageBox.Show($"The timeslot selected is {TransactionRec.TimeSlotStart}", $"The timeslot selected is {TransactionRec.TimeSlotStart}");
        }

        private void DataGridRow_MouseRightClick(object sender, MouseButtonEventArgs e)

        {
            // Find the cell or row clicked
            var dep = (DependencyObject)e.OriginalSource;
            while ((dep != null) && !(dep is DataGridCell) && !(dep is DataGridColumnHeadersPresenter))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }

            if (dep is DataGridCell cell && !cell.IsEditing && !cell.IsReadOnly)
            {
                if (!cell.IsFocused)
                {
                    cell.Focus();
                }

                var dataGrid = FindVisualParent<DataGrid>(cell);
                // Begin editing on the single right click
                dataGrid?.BeginEdit(e);
            }
        }

        private void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.Focus();
                textBox.SelectAll(); // Optional: highlights existing text for quick replacement
            }
        }

        // Helper method to traverse the visual tree
        private static T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            var parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;
            if (parentObject is T parent) return parent;
            return FindVisualParent<T>(parentObject);
        }
        private void NavigateOn()
        { }
        //TO Do: add spinners to commit buttons

        }
}


