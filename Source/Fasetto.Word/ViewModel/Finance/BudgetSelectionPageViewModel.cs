using Fasetto.Word.Core;
using Fasetto.Word.Core.ApiModels.Controls;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using static Fasetto.Word.DI;
using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace Fasetto.Word
{
    /// <summary>
    /// A view model for managing hierarchies 
    /// </summary>
    public class BudgetSelectionPageViewModel : BaseViewModel
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
        /// The CostHierarchy for Transaction processing for the selected client
        /// </summary>
        public HierarchyItemSelectionViewModel CostHierarchy { get; set; }

        /// <summary>
        /// The Budget List for financial management
        /// </summary>
        public BudgetPeriodListViewModel Budget { get; set; }

        /// <summary>
        /// The selected Budget for processing of budget management process
        /// </summary>
        public BudgetPeriodViewModel SelectedBudget { get; set; }

        /// <summary>
        /// The Budget MonthList for financial management
        /// </summary>
        public BudgetMonthListViewModel BudgetMonthList { get; set; }

        /// <summary>
        /// The month selected for processing
        /// </summary>
        public BudgetMonthViewModel SelectedBudgetMonth { get; set; }

        /// <summary>
        /// Indicates if the email is current being saved
        /// </summary>
        public bool ClientIsSaving { get; set; }

        /// <summary>
        /// Saves the current email to the server
        /// </summary>
        public ICommand SaveClientCommand { get; set; }





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
        /// True to show the attachment menu, false to hide it
        /// </summary>
        public bool UpdateHierarchyCompleted { get; set; }


        /// <summary>
        /// True to show the attachment menu, false to hide it
        /// </summary>
        public bool SetHierarchyCompleted { get; set; }


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





        /// <summary>
        /// Populate parameters for retrieval of required hierarchy tree
        /// </summary>
        public ParameterHierarchyItemSelectApiModel HierarchyParam { get; set; }



        /// <summary>
        /// Create an array of months for the budget period
        /// </summary>
        public List<int>  BudgetMonth { get; set; }

        ///// <summary>
        ///// The selected Budget budget month for the budget detail selection
        ///// </summary>
        //public int SelectedBudgetMonth { get; set; }


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
        public ICommand BudgetLoadCommand { get; set; }

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
        public BudgetSelectionPageViewModel()
        {
            //Populate screen title
            //mViewModel = (HierarchyTreeViewModel)ViewModelApplication.CurrentControlViewModel;
            //var results = mViewModel.mHDML.FirstOrDefault(x => x.ParentCategoryID == "00000000-0000-0000-0000-000000000000");
            DisplayTitle = "Select Budget for Review";
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
                //CommitAction = SaveFirstNameAsync
            };


            CostHierarchy = new HierarchyItemSelectionViewModel
            {

                Label = "Select Cost Hierarchy",
                EditedName = (string)ViewModelApplication.CostHierarchyShortName ?? "Cost Hierarchy Name",
                OriginalName = (string)ViewModelApplication.CostHierarchyShortName ?? "Cost Hierarchy",
                OriginalKid = (string)ViewModelApplication.FCostHierarchyID,
                EditedKid = (string)ViewModelApplication.FCostHierarchyID,
                HierarchyTypeID = "64413ae7-822f-4866-9ebe-433083d699ac",
                PrepareAction = SetCostHierarchySelectionAsync,
                Level = 1,
                PriorPopupViewModel = ViewModelApplication.CurrentPopupViewModel,
                CommitAction = UpdateCostHierarchySelectionAsync,
            };

            //ViewModelApplication.CurrentControlViewModel = ViewModelApplication.CurrentControlViewModel;
            ViewModelApplication.CurrentControlViewModel = Client;
            //ViewModelApplication.CurrentControlViewModel = ((SWBillingPageViewModel)ViewModelApplication.CurrentPageViewModel).Client;
            Budget = new BudgetPeriodListViewModel(CostHierarchy.OriginalKid);
            SelectedBudget = new BudgetPeriodViewModel();
            BudgetMonthList = new BudgetMonthListViewModel();
            SelectedBudgetMonth = new BudgetMonthViewModel();

            Budget.MSelectedBudgetPeriod = SelectedBudget;
            BudgetMonth = new List<int> ();

            // Create commands
            AttachmentButtonCommand = new RelayCommand(AttachmentButton);
            PopupClickawayCommand = new RelayCommand(PopupClickaway);
            BudgetLoadCommand = new RelayCommand(BudgetDetail);
            PopulateCommand = new RelayCommand(Populate);
            SearchCommand = new RelayCommand(Search);
            OpenSearchCommand = new RelayCommand(OpenSearch);
            CloseCommand = new RelayCommand(Close);
            ClearSearchCommand = new RelayCommand(ClearSearch);
            //

            // Make a default menu
            //AttachmentMenu = new ChatAttachmentPopupMenuViewModel();
        }

        #endregion

        #region Command Methods


        public async Task<bool> SetCostHierarchySelectionAsync()
        {
            // Lock this command to ignore any other requests while processing

            return await RunCommandAsync(() => SetHierarchyCompleted, async () =>
            {
                // Update the First Name value on the server...

                //((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy.ClientID = ((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).Client.EditedKid;
                //((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy.RootID = ((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).Client.RootID;
                ViewModelApplication.CurrentControlViewModel = ((BudgetSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy;
                HierarchyParam = new ParameterHierarchyItemSelectApiModel
                {

                    ClientID = ViewModelApplication.FClientID,
                    Level = 1,
                    HierarchyTypeID = CostHierarchy.HierarchyTypeID
                };
                ViewModelApplication.CurrentPopupViewModel = new HierarchyTreeViewModel1(HierarchyParam);
                return true;
            });

        }



        ///<summary>
        /// Update Client selection for current session
        /// </summary>
        /// <returns></returns>
        public async Task<bool> UpdateCostHierarchySelectionAsync()
        {
            // Lock this command to ignore any other requests while processing

            return await RunCommandAsync(() => UpdateHierarchyCompleted, async () =>
            {
                // Update the First Name value on the server...

                //ViewModelApplication.FCostHierarchyID = CostHierarchy.EditedKid;
                //ViewModelApplication.CostHierarchyShortName = CostHierarchy.EditedName;
                //ViewModelApplication.PopupVisible = false;
                //ViewModelApplication.CurrentPopupViewModel = null;
                ViewModelApplication.CurrentPopupContent = 0;
                CostHierarchy.OriginalName = CostHierarchy.EditedName;
                CostHierarchy.OriginalKid = CostHierarchy.EditedKid;
                PopulateAsync();
                return true;
            });

        }



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
                PopulateAsync();

                return true;
            });

        }


        /// <summary>
        /// When the user clicks the send button, sends the message
        /// </summary>
        public void BudgetDetail()
        {
            //ViewModelApplication.CurrentPopupContent = PopupContent.AddElement;
            //To do: Lookup to be user rights and available options driven
            //BulkMeter = "5249ffeb-6907-46aa-9204-d4527e11f9ce";

            if (SelectedBudget == null)
            { var Nm = (ViewModelApplication.CurrentPopupViewModel).GetType().Name; };
            if (SelectedBudget == null || SelectedBudget.KBudgetID == null)

            //To DO - message user
            {
                MessageBox.Show($"First select a valid Budget and Month to proceed...");
                return;
            };

            //AddMonthRange();
            _ = SelectedBudget.KBudgetID;
            //BudgetMonth
            //if (SelectedBudget.KBudgetID != ((HierarchyBillingTreeViewModel)ViewModelApplication.CurrentPopupViewModel).mBudget)
            //{ ViewModelApplication.CurrentPopupContent = PopupContent.AddElement; };
            //
            //ShortName = Meter.EditedName;
            //if (ViewModelApplication.CurrentPopupViewModel!= null)
            //            {
            //    if (ViewModelApplication.CurrentPopupViewModel.GetType().Name == "HierarchyBudgetTreeViewModel")
            //    {
            //        //if (SelectedBudget.KBudgetID == (((HierarchyBudgetTreeViewModel)ViewModelApplication.CurrentPopupViewModel).mBudget).c
            //        //{
            //        //}
            //        //else
            //        //{
            //        //    ViewModelApplication.CurrentPopupContent = PopupContent.AddElement;
            //        //};
            //    }
            //}
            ////((SWBillingPageViewModel)ViewModelApplication.CurrentPageViewModel).SelectedBudget = SelectedBudget.KBudgetID;
            //var MType = ViewModelApplication.CurrentPopupViewModel.GetType().Name;
            //ViewModelApplication.CurrentPopupViewModel = new HierarchyBillingTreeViewModel(SelectedBudget.KBudgetID);
            //((HierarchyBillingTreeViewModel)ViewModelApplication.CurrentPopupViewModel).ControlTitle = "Water & Sewerage Billing : FROM " + SelectedBudget.TimeStart.ToString("d/MM/yyyy")
            //    + " TO " + SelectedBudget.TimeEnd.ToString("d/MM/yyyy");
            ViewModelApplication.CurrentPopupContent = PopupContent.BudgetReview;
            ViewModelApplication.PopupVisible = true;

        }




        public void Populate()
        {
            PopulateAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task PopulateAsync()
        {
            ((BudgetSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).Budget.mRequest
                = new BudgetPeriodResultApiModel
                {
                    CostHierarchy = ((BudgetSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy.OriginalKid,

                };


            //await Budget.CostHierarchyAsync();
            Budget = new BudgetPeriodListViewModel(Client.EditedKid);
            //{
            //    MSelectedBudgetPeriod = ((BudgetSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).SelectedBudget
            //};
        }



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
            ViewModelApplication.GoToPage(ApplicationPage.Chat);
            ViewModelApplication.CurrentControlViewModel = null;
            ViewModelApplication.CurrentPopupViewModel = null;
            ViewModelApplication.CurrentPopupContent = 0;
        }

        #region Helpers
        public void AddMonthRange()
        {
            // Close settings menu
            //var  BudgetMonth1 = new List<int>();
            //BudgetMonth1.Clear();
            var x = SelectedBudget.MonthStart;
            while (x <= SelectedBudget.MonthEnd)
            {
                BudgetMonth.Add(x);
                if ((x%100)!= 12)
                { x++; }
                else
                { x = (((x/100) + 1) * 100) + 1; }
            }


        }


        #endregion

        #endregion
    }
}
