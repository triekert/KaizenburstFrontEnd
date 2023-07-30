using Fasetto.Word.Core;
using System;
using System.Windows.Forms;
using System.Windows.Input;
using static Fasetto.Word.DI;


namespace Fasetto.Word
{
    /// <summary>
    /// A view model for managing hierarchies 
    /// </summary>
    public class MeterSelectionPageViewModel : BaseViewModel
    {
        #region Private Members

        /// <summary>
        /// The text to show while loading text
        /// </summary>
        private string mLoadingText = "...";

        #endregion
        #region Protected Members

        /// <summary>
        /// The last searched text in this list
        /// </summary>
        protected string mLastSearchText;

        /// <summary>
        /// The text to search for in the search command
        /// </summary>
        protected string mSearchText;


        /// <summary>
        /// The chat thread items for the list
        /// </summary>
        //protected ObservableCollection<ChatMessageListItemViewModel> mItems;

        /// <summary>
        /// A flag indicating if the search dialog is open
        /// </summary>
        protected bool mSearchIsOpen;

        public HierarchyTreeViewModel mViewModel;
        #endregion

        #region Public Properties

        /// <summary>
        /// The chat thread items for the list
        /// NOTE: Do not call Items.Add to add messages to this list
        ///       as it will make the FilteredItems out of sync
        /// </summary>
        //public ObservableCollection<ChatMessageListItemViewModel> Items
        //{
        //    get => mItems;
        //    set
        //    {
        //        // Make sure list has changed
        //        if (mItems == value)
        //            return;

        //        // Update value
        //        mItems = value;

        //        // Update filtered list to match
        //        FilteredItems = new ObservableCollection<ChatMessageListItemViewModel>(mItems);
        //    }
        //}

        /// <summary>
        /// The chat thread items for the list that include any search filtering
        /// </summary>
        //public ObservableCollection<ChatMessageListItemViewModel> FilteredItems { get; set; }

        /// <summary>
        /// The title of this chat list
        /// </summary>
        public string DisplayTitle { get; set; }

        /// <summary>
        /// The Client for which Bulk Meter reconciliation is to be processed
        /// </summary>
        public HierarchyItemSelectionViewModel Root { get; set; }

        /// <summary>
        /// The Client for which Bulk Meter reconciliation is to be processed
        /// </summary>
        public HierarchyItemSelectionViewModel Meter { get; set; }

        /// <summary>
        /// The GUID for the Bulk Meter for which reconciliation is to be processed
        /// </summary>
        public string BulkMeter { get; set; }


        /// <summary>
        /// The Name for the Bulk Meter for which reconciliation is to be processed
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// The start time for analysis of readings
        /// </summary>
        public DateTimeViewModel TimeStart { get; set; }

        /// <summary>
        /// The start time for analysis of readings
        /// </summary>
        public DateTimeViewModel TimeEnd { get; set; }


        /// <summary>
        /// The start time for analysis of readings
        /// </summary>
        public DateTimeViewModel DateReference { get; set; }


        /// <summary>
        /// True to show the attachment menu, false to hide it
        /// </summary>
        public bool AttachmentMenuVisible { get; set; }

        /// <summary>
        /// True if any pop-up menus are visible
        /// </summary>
        public bool AnyPopupVisible => AttachmentMenuVisible;

        /// <summary>
        /// The view model for the attachment menu
        /// </summary>
        //public ChatAttachmentPopupMenuViewModel AttachmentMenu { get; set; }

        /// <summary>
        /// The text for the current message being written
        /// </summary>
        public string PendingMessageText { get; set; }

        /// <summary>
        /// The text to search for when we do a search
        /// </summary>
        public string SearchText
        {
            get => mSearchText;
            set
            {
                // Check value is different
                if (mSearchText == value)
                    return;

                // Update value
                mSearchText = value;

                // If the search text is empty...
                if (string.IsNullOrEmpty(SearchText))
                    // Search to restore messages
                    Search();
            }
        }

        /// <summary>
        /// A flag indicating if the search dialog is open
        /// </summary>
        public bool SearchIsOpen
        {
            get => mSearchIsOpen;
            set
            {
                // Check value has changed
                if (mSearchIsOpen == value)
                    return;

                // Update value
                mSearchIsOpen = value;

                // If dialog closes...
                if (!mSearchIsOpen)
                    // Clear search text
                    SearchText = string.Empty;
            }
        }

        #endregion

        #region Public Commands

        /// <summary>
        /// The command for when the attachment button is clicked
        /// </summary>
        public ICommand AttachmentButtonCommand { get; set; }

        /// <summary>
        /// The command for when the area outside of any popup is clicked
        /// </summary>
        public ICommand PopupClickawayCommand { get; set; }

        /// <summary>
        /// The command for when the user clicks the send button
        /// </summary>
        public ICommand ReconcileCommand { get; set; }


        /// <summary>
        /// The command for when the user clicks the send button
        /// </summary>
        public ICommand ReconcileTODCommand { get; set; }
        /// <summary>
        /// The command for populating client information for search
        /// </summary>
        public ICommand PopulateCommand { get; set; }

        /// <summary>
        /// The command for when the user wants to search
        /// </summary>
        public ICommand SearchCommand { get; set; }

        /// <summary>
        /// The command for when the user wants to open the search dialog
        /// </summary>
        public ICommand OpenSearchCommand { get; set; }

        /// <summary>
        /// The command for when the user wants to close to search dialog
        /// </summary>
        public ICommand CloseCommand { get; set; }

        /// <summary>
        /// The command for when the user wants to clear the search text
        /// </summary>
        public ICommand ClearSearchCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public MeterSelectionPageViewModel()
        {
            //Populate screen title
            //mViewModel = (HierarchyTreeViewModel)ViewModelApplication.CurrentControlViewModel;
            //var results = mViewModel.mHDML.FirstOrDefault(x => x.ParentCategoryID == "00000000-0000-0000-0000-000000000000");
            DisplayTitle = "Bulk Meter Management";
            BulkMeter = "5249FFEB-6907-46AA-9204-D4527E11F9CE";

            Root = new HierarchyItemSelectionViewModel
            {
                Label = "Select Client",
                //EditedName = mLoadingText,
                EditedName = "Client",
                OriginalName = "Client Lookup",
                OriginalKid = "4766E825-1B58-410D-B06B-5A2639CA22C8",
                EditedKid = "4766E825-1B58-410D-B06B-5A2639CA22C8",
                HierarchyTypeID = "1A8CCEE0-52D1-454B-8165-23EDB2241058",

                //CommitAction = SaveFirstNameAsync
            };
            ViewModelApplication.CurrentControlViewModel = Root;

            Meter = new HierarchyItemSelectionViewModel
            {
                Label = "Meter Name",
                //EditedName = mLoadingText,
                EditedName = "TD Water Metering",
                OriginalName = "Original Meter Selection",
                EditedKid = "5249FFEB-6907-46AA-9204-D4527E11F9CE",
                //hard coded for hierarchy type linked to water meters
                HierarchyTypeID = "8A50E984-9E9F-44F6-9392-875E56A0B7CA",
                //CommitAction = SaveFirstNameAsync
            };

            TimeStart = new DateTimeViewModel
            {
                Label = "Period Start",
                OriginalDateTime = DateTime.Now.AddDays(-1),
                EditedDateTime = DateTime.Now.AddDays(-1),
                OriginalTime = new System.Windows.Controls.ComboBoxItem(),
                EditedTime = new System.Windows.Controls.ComboBoxItem(),
                //(DateTime.Now.AddHours(-1)).ToShortTimeString(),
                //EditedTime. = "System.Windows.Controls.ComboBoxItem: 00:30",//(DateTime.Now.AddHours(-1)).ToShortTimeString(),
            };
            TimeStart.OriginalTime.Content = "00:00";
            TimeStart.EditedTime.Content = "00:00";


            TimeEnd = new DateTimeViewModel
            {
                Label = "Period End",
                OriginalDateTime = DateTime.Now,
                EditedDateTime = DateTime.Now,
                OriginalTime = new System.Windows.Controls.ComboBoxItem(),
                EditedTime = new System.Windows.Controls.ComboBoxItem(),
                //(DateTime.Now.AddHours(-1)).ToShortTimeString(),
                //EditedTime. = "System.Windows.Controls.ComboBoxItem: 00:30",//(DateTime.Now.AddHours(-1)).ToShortTimeString(),
            };
            TimeEnd.OriginalTime.Content = "00:00";
            TimeEnd.EditedTime.Content = "00:00";

            DateReference = new DateTimeViewModel
            {
                Label = "Calculation reference date",
                OriginalDateTime = DateTime.Now,
                EditedDateTime = DateTime.Now,
                OriginalTime = new System.Windows.Controls.ComboBoxItem(),
                EditedTime = new System.Windows.Controls.ComboBoxItem(),
                //(DateTime.Now.AddHours(-1)).ToShortTimeString(),
                //EditedTime. = "System.Windows.Controls.ComboBoxItem: 00:30",//(DateTime.Now.AddHours(-1)).ToShortTimeString(),
            };
            TimeEnd.OriginalTime.Content = "00:00";
            TimeEnd.EditedTime.Content = "00:00";

            //ViewModelApplication.ControlParameter =  null;
            ViewModelApplication.ControlParameter1 = null;
            ViewModelApplication.ControlParameter2 = null;
            ViewModelApplication.ControlParameter3 = false;
            ViewModelApplication.ControlParameter4 = null;
            ViewModelApplication.ControlParameter5 = null;
            // Create commands
            AttachmentButtonCommand = new RelayCommand(AttachmentButton);
            PopupClickawayCommand = new RelayCommand(PopupClickaway);
            ReconcileCommand = new RelayCommand(Reconcile);
            ReconcileTODCommand = new RelayCommand(ReconcileTOD);
            PopulateCommand = new RelayCommand(Populate);
            SearchCommand = new RelayCommand(Search);
            OpenSearchCommand = new RelayCommand(OpenSearch);
            CloseCommand = new RelayCommand(Close);
            ClearSearchCommand = new RelayCommand(ClearSearch);
            //ViewModelApplication.CurrentControlViewModel = null;

            // Make a default menu
            //AttachmentMenu = new ChatAttachmentPopupMenuViewModel();
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// When the attachment button is clicked show/hide the attachment pop-up
        /// </summary>
        public void AttachmentButton()
        {
            // Toggle menu visibility
            AttachmentMenuVisible ^= true;
        }

        /// <summary>
        /// When the pop-up click away area is clicked hide any pop-ups
        /// </summary>
        public void PopupClickaway()
        {
            // Hide attachment menu
            AttachmentMenuVisible = false;
        }

        /// <summary>
        /// When the user clicks the send button, sends the message
        /// </summary>
        public void Reconcile()
        {
            ViewModelApplication.CurrentPopupContent = PopupContent.AddElement;
            //To do: Lookup to be user rights and available options driven
            //BulkMeter = "5249ffeb-6907-46aa-9204-d4527e11f9ce";
            if (Meter.EditedKid == null)

            //To DO - message user
            { MessageBox.Show($"First select a valid BulkMeter to proceed...");
                return;
            };
            ShortName = Meter.EditedName;
            TimeEnd.OriginalDateTime = TimeEnd.EditedDateTime;
            TimeStart.OriginalDateTime = TimeStart.EditedDateTime;

            //Make start time and end time equal to overload sql call
            //var t1 = TimeEnd.EditedDateTime.ToString("yyyy/MM/dd");
            //var t2 = TimeStart.EditedDateTime.Hour.ToString("00");
            //var t3 = $"{TimeEnd.EditedDateTime.ToString("yyyy/MM/dd")}{" "}{TimeStart.EditedDateTime.Hour.ToString()}{":00:00"}";
            //TimeEnd.EditedDateTime = DateTime.Parse(t3);
            //TimeEnd.EditedDateTime = DateTime.Parse($"{TimeEnd.EditedDateTime.ToString("yyyy/MM/dd")}{" "}{TimeStart.EditedDateTime.Hour.ToString("00")}{":00:00"}");

            ViewModelApplication.CurrentPopupViewModel = new BulkReconTreeViewModel(Meter.EditedKid, TimeStart.EditedDateTime, 
                DateTime.Parse($"{TimeEnd.EditedDateTime.ToString("yyyy/MM/dd")}{" "}{TimeStart.EditedDateTime.Hour.ToString("00")}{":00:00"}"), 0,0,DateReference.EditedDateTime);
            ((BulkReconTreeViewModel)ViewModelApplication.CurrentPopupViewModel).ControlTitle = "Bulk Meter Recon Detail: " + ShortName;
            ViewModelApplication.CurrentPopupContent = PopupContent.AddElement;
            ViewModelApplication.CurrentPopupContent = PopupContent.BulkRecon;

            ViewModelApplication.PopupVisible = true;

        }

        /// <summary>
        /// When the user clicks the send button, sends the message
        /// </summary>
        public void ReconcileTOD()
        {
            ViewModelApplication.CurrentPopupContent = PopupContent.AddElement;
            //To do: Lookup to be user rights and available options driven
            //BulkMeter = "5249ffeb-6907-46aa-9204-d4527e11f9ce";
            if (Meter.EditedKid == null)

            //To DO - message user
            {
                MessageBox.Show($"First select a valid BulkMeter to proceed...");
                return;
            };
            if (TimeEnd.EditedDateTime.Hour <= TimeStart.EditedDateTime.Hour)


            {
                MessageBox.Show($"Make Start Time of Day less that End Time of Day");
                return;
            };

            ShortName = Meter.EditedName;
            //TimeEnd.EditedDateTime = TimeEnd.EditedDateTime.AddHours(TimeEnd.EditedDateTime.Hour).AddMinutes(TimeEnd.EditedDateTime.Minute);
            TimeEnd.OriginalDateTime = TimeEnd.EditedDateTime;
            TimeStart.OriginalDateTime = TimeStart.EditedDateTime;
            ViewModelApplication.CurrentPopupViewModel = new BulkReconTreeViewModel(Meter.EditedKid, TimeStart.EditedDateTime, TimeEnd.EditedDateTime,TimeStart.EditedDateTime.Hour,TimeEnd.EditedDateTime.Hour, DateReference.EditedDateTime);
            ((BulkReconTreeViewModel)ViewModelApplication.CurrentPopupViewModel).ControlTitle = "Bulk Meter Recon Detail: " + ShortName;
            ViewModelApplication.CurrentPopupContent = PopupContent.BulkRecon;
            ViewModelApplication.PopupVisible = true;

        }


        /// <summary>
        /// When the user clicks the send button, sends the message
        /// </summary>
        public void Populate()
        {
            Meter.ClientID = Root.EditedKid;
            Meter.RootID = Root.RootID;

            ViewModelApplication.CurrentControlViewModel = Meter;
            ViewModelApplication.CurrentPageViewModel = ViewModelApplication.CurrentPageViewModel;
            //ViewModelApplication.CurrentPageViewModel = Meter;
        }

        /// <summary>
        /// Searches the current message list and filters the view
        /// </summary>
        public void Search()
        {
            // Make sure we don't re-search the same text
            //if ((string.IsNullOrEmpty(mLastSearchText) && string.IsNullOrEmpty(SearchText)) ||
            //    string.Equals(mLastSearchText, SearchText))
            //    return;

            // If we have no search text, or no items
            if (string.IsNullOrEmpty(SearchText))
            {
                // Make filtered list the same

                // Set last search text
                mLastSearchText = SearchText;

                return;
            }

            // Find all items that contain the given text
            //// TODO: Make more efficient search
            //FilteredItems = new ObservableCollection<ChatMessageListItemViewModel>(
            //    Items.Where(item => item.Message.ToLower().Contains(SearchText)));

            // Set last search text
            mViewModel = (HierarchyTreeViewModel)ViewModelApplication.CurrentControlViewModel;
            mViewModel.SearchText = SearchText;
            mViewModel.PerformSearch();
            //mViewModel.RefreshHierarchy();           
            mLastSearchText = SearchText;
        }

        /// <summary>
        /// Clears the search text
        /// </summary>
        public void ClearSearch()
        {
            // If there is some search text...
            if (!string.IsNullOrEmpty(SearchText))
                // Clear the text
                SearchText = string.Empty;
            // Otherwise...
            else
                // Close search dialog
                SearchIsOpen = false;
        }

        /// <summary>
        /// Opens the search dialog
        /// </summary>
        public void OpenSearch() => SearchIsOpen = true;

        /// <summary>
        /// Closes the search dialog
        /// </summary>
        public void Close()
        { 
        // Close settings menu
        ViewModelApplication.SideMenuVisible = true;
        //ViewModelApplication.CurrentSideMenuViewModel = null;
        ViewModelApplication.GoToPage(ApplicationPage.Chat);
        ViewModelApplication.CurrentControlViewModel = null;
        ViewModelApplication.CurrentPopupViewModel = null;
        ViewModelApplication.CurrentPopupContent = PopupContent.AddElement; ;
        }

        #endregion
    }
}
