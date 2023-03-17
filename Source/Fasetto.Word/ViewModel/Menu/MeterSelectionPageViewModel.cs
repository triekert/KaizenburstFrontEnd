using static Fasetto.Word.DI;
using static Fasetto.Word.Core.CoreDI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Fasetto.Word.Core;
using System.Security.Cryptography.X509Certificates;
using System;

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
        public HierarchyItemSelectionViewModel Client { get; set; }

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
        public ICommand SendCommand { get; set; }

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

            Client = new HierarchyItemSelectionViewModel
            {
                Label = "Client",
                EditedName = mLoadingText,

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
            TimeStart.EditedTime.Content = "00:30";


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
            TimeEnd.EditedTime.Content = "00:30";

            // Create commands
            AttachmentButtonCommand = new RelayCommand(AttachmentButton);
            PopupClickawayCommand = new RelayCommand(PopupClickaway);
            SendCommand = new RelayCommand(Send);
            SearchCommand = new RelayCommand(Search);
            OpenSearchCommand = new RelayCommand(OpenSearch);
            CloseCommand = new RelayCommand(Close);
            ClearSearchCommand = new RelayCommand(ClearSearch);

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
        public void Send()
        {
            ViewModelApplication.CurrentPopupContent = PopupContent.AddElement;
            //To do: Lookup to be user rights and available options driven
            BulkMeter = "5249ffeb-6907-46aa-9204-d4527e11f9ce";
            ShortName = "Tre Donne";

            ViewModelApplication.CurrentPopupViewModel = new BulkReconTreeViewModel(BulkMeter, TimeStart.EditedDateTime, TimeEnd.EditedDateTime);
            ((BulkReconTreeViewModel)ViewModelApplication.CurrentPopupViewModel).ControlTitle = "Bulk Meter Recon Detail: " + ShortName;
            ViewModelApplication.CurrentPopupContent = PopupContent.BulkRecon;
            ViewModelApplication.PopupVisible = true;
            //ViewModelApplication.CurrentPopupViewModel = new BulkReconlTreeViewModel(BulkMeter, TimeStart.EditedDateTime, TimeEnd.EditedDateTime);
            //((BulkReconDetailTreeViewModel)ViewModelApplication.CurrentPopupViewModel).ControlTitle = "Bulk Meter Recon Detail: " + ShortName;
            //ViewModelApplication.CurrentPopupContent = PopupContent.BulkReconDetail;
            //ViewModelApplication.PopupVisible = true;
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
            ViewModelApplication.CurrentSideMenuViewModel = null;
            ViewModelApplication.GoToPage(ApplicationPage.Chat);}
        #endregion
    }
}
