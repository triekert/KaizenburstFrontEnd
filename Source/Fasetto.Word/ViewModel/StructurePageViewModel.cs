using static Fasetto.Word.DI;
using static Fasetto.Word.Core.CoreDI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Fasetto.Word.Core;
using Fasetto.Word.Core.ApiModels.Controls;
using System.Threading.Tasks;

namespace Fasetto.Word
{
    /// <summary>
    /// A view model for a chat message thread list
    /// </summary>
    public class StructurePageViewModel : BaseViewModel
    {
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
        protected ObservableCollection<ChatMessageListItemViewModel> mItems;

        /// <summary>
        /// A flag indicating if the search dialog is open
        /// </summary>
        protected bool mSearchIsOpen;

        public FinanceTreeViewModel mViewModel;
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
        /// The Client for which Bulk Meter reconciliation is to be processed
        /// </summary>
        public HierarchyItemSelectionViewModel Client { get; set; }

        /// <summary>
        /// True to show the attachment menu, false to hide it
        /// </summary>
        public bool UpdateHierarchyCompleted { get; set; }


        /// <summary>
        /// True to show the attachment menu, false to hide it
        /// </summary>
        public bool SetHierarchyCompleted { get; set; }

        /// <summary>
        /// The title of this application page
        /// </summary>
        public string DisplayTitle { get; set; }

        /// <summary>
        /// Populate parameters for retrieval of required hierarchy tree
        /// </summary>
        public ParameterHierarchyItemSelectApiModel HierarchyParam { get; set; }

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
        public ICommand CloseSearchCommand { get; set; }

        /// <summary>
        /// The command for when the user wants to clear the search text
        /// </summary>
        public ICommand ClearSearchCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public StructurePageViewModel()
        {
            //Populate screen title
            //mViewModel = (HierarchyTreeViewModel)ViewModelApplication.CurrentControlViewModel;
            //var results = mViewModel.mHDML.FirstOrDefault(x => x.ParentCategoryID == "00000000-0000-0000-0000-000000000000");
            DisplayTitle = "Select relevant Client";
            //BulkMeter = "5249FFEB-6907-46AA-9204-D4527E11F9CE";

            Client = new HierarchyItemSelectionViewModel
            {
                Label = "Select Client",
                //EditedName = mLoadingText,
                EditedName = "Selected Client",
                OriginalName = "Root Client Organisation",
                OriginalKid = "4766E825-1B58-410D-B06B-5A2639CA22C8",
                EditedKid = "4766E825-1B58-410D-B06B-5A2639CA22C8",
                HierarchyTypeID = "1A8CCEE0-52D1-454B-8165-23EDB2241058",
                CommitAction = UpdateClientSelectionAsync,
                PrepareAction = SetClientSelectionAsync,
                //CommitAction = SaveFirstNameAsync
            };

        }

        #endregion

        #region Command Methods


        public async Task<bool> SetClientSelectionAsync()
        {
            // Lock this command to ignore any other requests while processing

            return await RunCommandAsync(() => SetHierarchyCompleted, async () =>
            {
                // Update the First Name value on the server...

                //((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy.ClientID = ((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).Client.EditedKid;
                //((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy.RootID = ((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).Client.RootID;
                ViewModelApplication.CurrentControlViewModel = ((BudgetSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).Client;
                HierarchyParam = new ParameterHierarchyItemSelectApiModel
                {

                    //ClientID = ViewModelApplication.FClientID,
                    //Level = 1,
                    //HierarchyTypeID = CostHierarchy.HierarchyTypeID
                };
                ViewModelApplication.CurrentPopupViewModel = new HierarchyTreeViewModel1(HierarchyParam);
                return true;
            });

        }

        ///<summary>
        /// Update Client selection for current session
        /// </summary>
        /// <returns></returns>
        public async Task<bool> UpdateClientSelectionAsync()
        {
            // Lock this command to ignore any other requests while processing

            return await RunCommandAsync(() => UpdateHierarchyCompleted, async () =>
            {
                // Update the First Name value on the server...

                //ViewModelApplication.FClientID = Client.EditedKid;
                //ViewModelApplication.ClientShortName = Client.EditedName;
                Client.OriginalName = Client.EditedName;
                ViewModelApplication.PopupVisible = false;
                ViewModelApplication.CurrentPopupViewModel = null;
                ViewModelApplication.CurrentPopupContent = 0;
                //
                //PopulateAsync();

                return true;
            });

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
            mViewModel = (FinanceTreeViewModel)ViewModelApplication.CurrentControlViewModel;
            var results = mViewModel.mPersist.Where(x => x.IsUnderReview || x.IsDeleteElement).ToList();
            var mPersistElement = new HierarchyResultApiModel();
            //var results = mViewModel.mPersist.OrderBy(x => x.ShortName).ToList();
            if (results.Count > 0)
                //mViewModel.mPersistTmp = (HierarchyResultApiModel)results;
                //ToDo:Where a new hierarchy is referred to in the a new menu item, Create the root element for this new hierarchy

                results = mViewModel.mPersist.Where(x => (x.IsNewElement) & x.Page == "Finance").ToList();
            if (results.Count > 0)
            {
                //mViewModel.mPersist.AddRange(results);
                foreach (var row in results)

                {
                    mPersistElement.DateDiscontinued = row.DateDiscontinued;
                    mPersistElement.DateEffective = row.DateEffective;
                    mPersistElement.ShortName = row.ShortName;
                    mPersistElement.Description = row.Description;
                    mPersistElement.KCategoryID = row.Root;
                    mPersistElement.Page = row.Page;
                    mPersistElement.IsNewElement = row.IsNewElement;
                    mPersistElement.IsUnderReview = row.IsUnderReview;
                    mPersistElement.ParentCategoryID = "00000000-0000-0000-0000-000000000000";
                    mPersistElement.FHierarchyID = row.Root;
                }
                mViewModel.mPersist.Add(mPersistElement);

            }
            _ = mViewModel.PersistHierarchyChangesAsync();
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
            mViewModel = (FinanceTreeViewModel)ViewModelApplication.CurrentControlViewModel;
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
        public void CloseSearch() => SearchIsOpen = false;

        #endregion
    }
}
