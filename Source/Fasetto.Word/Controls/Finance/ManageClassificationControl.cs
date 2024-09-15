using System.Windows.Controls;
using System.Windows.Input;
using static Fasetto.Word.DI;
using System.Collections.ObjectModel;

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


        private void TreeView_KeyBoard(object sender, KeyboardEventArgs e)
        {
            //check to determine whether user would like to add an item to the hierarchy

           if (Keyboard.IsKeyDown(Key.Escape))


            { ((ManageClassificationViewModel)DataContext).Close();
            e.Handled = true;}
                //ImagePath

           //else
           //     if (Keyboard.IsKeyDown(Key.F2))
           //         ((ManageClassificationViewModel)DataContext).OpenDocument();
 
           //      else
           //         if (Keyboard.IsKeyDown(Key.Insert))
           //             ((ManageClassificationViewModel)DataContext).BrowseImage();
           //         e.Handled = true;
        }

        private void DataGridRow_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.F2) && ((DocDataViewModel)Transaction.SelectedItem).DocURL != "\\somepath\\filename.jpg")

            {
                ((ManageClassificationViewModel)DataContext).OpenDocument((DocDataViewModel)Transaction.SelectedItem);
            }

            else
                if (Keyboard.IsKeyDown(Key.Insert))
            //link a new document if the placeholder is still not used, otherwise, create a new placeholder and fill
                {
                ((ManageClassificationViewModel)DataContext).BrowseImage();
                }

                else
                    if (Keyboard.IsKeyDown(Key.Delete))
                //link a new document if the placeholder is still not used, otherwise, create a new placeholder and fill
                //((ManageClassificationViewModel)DataContext).RemoveImage((DocDataViewModel)Transaction.SelectedItem);
                ((ManageClassificationViewModel)DataContext).RemoveDocument((DocDataViewModel)Transaction.SelectedItem);
            e.Handled = true;
            //if (e.Key == Key.Enter)
            //{
            //    NavigateOn();
            //}
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //if no document has been linked to the dummy placeholder yet, adda document into the placeholder...
            if (((DocDataViewModel)Transaction.SelectedItem).DocURL != "\\somepath\\filename.jpg")
                ((ManageClassificationViewModel)DataContext).OpenDocument((DocDataViewModel)Transaction.SelectedItem);
            else
                ((ManageClassificationViewModel)DataContext).BrowseImage();

            e.Handled = true;
            //MessageBox.Show($"The timeslot selected is {TransactionRec.TimeSlotStart}", $"The timeslot selected is {TransactionRec.TimeSlotStart}");
        }

        private void DataGridRow_MouseRightClick(object sender, MouseButtonEventArgs e)
        {
            var tempT = new ObservableCollection<TransactionViewModel>();
            foreach (var tT in Transaction.ItemsSource)
                tempT.Add((TransactionViewModel)tT);

            //var mTimeStart = tempBR.OrderBy(x => x.TimeSlotStart).ToList().FirstOrDefault().TimeSlotStart;
            //var mTimeEnd = tempBR.OrderByDescending(x => x.TimeSlotStart).ToList().FirstOrDefault().TimeSlotStart.AddMinutes(30);
            //MessageBox.Show($" timeslot ends at {mTimeEnd}", $" The timeslot selected starts at {mTimeStart}");
        }
        private void NavigateOn()
        { }
        //TO Do: add spinners to commit buttons

        }
}
