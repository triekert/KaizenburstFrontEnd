using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using static Fasetto.Word.DI;



namespace Fasetto.Word
{
    /// <summary>
    /// Interaction logic for FinancePage.xaml
    /// </summary>
    public partial class SWBillingPage : BasePage<SWBillingPageViewModel>
    {
        #region Constructor

        /// <summary>s
        /// Default constructor
        /// </summary>
        public SWBillingPage() : base()
        {
            //InitializeComponent();
            ViewModelApplication.CurrentPageViewModel = ViewModelApplication.CurrentPageViewModel;
        }

        /// <summary>
        /// Constructor with specific view model
        /// </summary>
        /// <param name="specificViewModel">The specific view model to use for this page</param>
        public SWBillingPage(SWBillingPageViewModel specificViewModel) : base(specificViewModel)
        {
            ViewModelApplication.CurrentPageViewModel = ViewModelApplication.CurrentPageViewModel;
            ViewModelApplication.CurrentControlViewModel = ((SWBillingPageViewModel)ViewModelApplication.CurrentPageViewModel).Client;
            InitializeComponent();
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// Fired when the view model changes
        /// </summary>
        //protected override void OnViewModelChanged()
        //{
        //    // Make sure UI exists first
        //    if (MeterSelection == null)
        //        return;

        //    // Fade in chat message list
        //    var storyboard = new Storyboard();
        //    storyboard.AddFadeIn(1, from: true);
        //    storyboard.Begin(MeterSelection);

        //    // Make the message box focused
        //    //MessageText.Focus();
        //}

        #endregion

        /// <summary>
        /// Preview the input into the message box and respond as required
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MessageText_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Get the text box
            var textbox = sender as TextBox;

            // Check if we have pressed enter
            if (e.Key == Key.Enter)
            {
                // If we have control pressed...
                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                {
                    // Add a new line at the point where the cursor is
                    var index = textbox.CaretIndex;

                    // Insert the new line
                    textbox.Text = textbox.Text.Insert(index, Environment.NewLine);

                    // Shift the caret forward to the newline
                    textbox.CaretIndex = index + Environment.NewLine.Length;

                    // Mark this key as handled by us
                    e.Handled = true;
                }
                else
                    // Send the message
                    //ViewModel.Send();

                // Mark the key as handled
                e.Handled = true;
            }
        }

        private void MeterSelection_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }


        private void SetHierarchySelection(object sender, System.Windows.RoutedEventArgs e)
        {
            //((SWBillingPageViewModel)ViewModelApplication.CurrentPageViewModel).BillingPeriod = new BillingPeriodListViewModel(((SWBillingPageViewModel)ViewModelApplication.CurrentPageViewModel).Client.EditedKid);
            //((SWBillingPageViewModel)ViewModelApplication.CurrentPageViewModel).BillingPeriod.MSelectedBillingPeriod = ((SWBillingPageViewModel)ViewModelApplication.CurrentPageViewModel).SelectedBillingPeriod;
            if (((SWBillingPageViewModel)ViewModelApplication.CurrentPageViewModel).Client.EditedName != "Selected Client")
            { 
                ViewModelApplication.CurrentControlViewModel = ((SWBillingPageViewModel)ViewModelApplication.CurrentPageViewModel).BillingPeriod;
                ((SWBillingPageViewModel)ViewModelApplication.CurrentPageViewModel).BillingPeriod.mRequest = ((SWBillingPageViewModel)ViewModelApplication.CurrentPageViewModel).Client.EditedKid;
                ((SWBillingPageViewModel)ViewModelApplication.CurrentPageViewModel).PopulateAsync();
            }
        }

        //private void SetHierarchySelection(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    ViewModelApplication.CurrentControlViewModel =  ((SWBillingPageViewModel)ViewModelApplication.CurrentPageViewModel).Client;

        //}

    }
}
