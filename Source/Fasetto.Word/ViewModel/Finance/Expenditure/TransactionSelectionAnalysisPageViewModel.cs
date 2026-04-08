using EnvDTE;
using Fasetto.Word.Core;
using Fasetto.Word.Core.ApiModels.Controls;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using static Fasetto.Word.DI;


namespace Fasetto.Word
{
    /// <summary>
    /// A view model for managing hierarchies 
    /// </summary>
    public class TransactionSelectionAnalysisPageViewModel : BaseViewModel
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
        /// The CostHierarchy for Transaction processing for the selected client
        /// </summary>
        public HierarchyItemSelectionViewModel CostHierarchy { get; set; }

        /// <summary>
        /// The Cost category to be used for the allocation
        /// </summary>
        public HierarchyItemSelectionViewModel Category { get; set; }

        /// <summary>
        /// The PartyHierarchy for Transaction processing for the selected client
        /// </summary>
        public HierarchyItemSelectionViewModel Party { get; set; }

                /// <summary>
        /// The AccountHierarchy for Transaction processing for the selected client
        /// </summary>
        public HierarchyItemSelectionViewModel Account { get; set; }

        /// <summary>
        /// The AssetHierarchy for Transaction processing for the selected client
        /// </summary>
        public HierarchyItemSelectionViewModel Asset { get; set; }

        /// <summary>
        /// The ProjcetHierarchy for Transaction processing for the selected client
        /// </summary>
        public HierarchyItemSelectionViewModel Project { get; set; }

                /// <summary>
        /// The PersonHierarchy for Transaction processing for the selected client
        /// </summary>
        public HierarchyItemSelectionViewModel Person { get; set; }

        /// <summary>
        /// The selected CostHierarchy for the Transaction Classification processing
        /// </summary>
        public BudgetPeriodViewModel SelectedCostHierarchy { get; set; }

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
        /// API model for retrieving transaction data
 
        /// </summary>
        public ParameterTransactionApiModel mRequest { get; set; }


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
        public bool SetHierarchyCompleted { get; set; }


        /// <summary>
        /// True to show the attachment menu, false to hide it
        /// </summary>
        public bool UpdateHierarchyCompleted { get; set; }


        /// <summary>
        /// Populate parameters for retrieval of required hierarchy tree
        /// </summary>
        public ParameterHierarchyItemSelectApiModel HierarchyParam { get; set; }

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
        /// Indicates if the Cost Hierarchy Search is currently being loaded
        /// </summary>
        public bool CostHierarchySrchIsSaving { get; set; }




        /// <summary>
        /// Indicates if the Reconcile process currently underway
        /// </summary>
        public bool ReconcileInProgress { get; set; }



        /// <summary>
        /// A flag indicating if the account selection is complete
        /// </summary>
        public bool SelectAccountCompleted { get; set; }

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
        public ICommand ReconcileCommandNew { get; set; }



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

        /// <summary>
        /// The command to initialise the relevant cost hierarchy search
        /// </summary>
        public ICommand InitialiseCostHCommand { get; set; }

        /// <summary>
        /// The command to iniitalise the search for clients
        /// </summary>
        public ICommand InitialiseClientSrchCommand { get; set; }
        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public TransactionSelectionAnalysisPageViewModel()

        {
            //Manually turn side menu on
            //ViewModelApplication.SideMenuVisible = true;
            //Populate screen title
            //mViewModel = (HierarchyTreeViewModel)ViewModelApplication.CurrentControlViewModel;
            //var results = mViewModel.mHDML.FirstOrDefault(x => x.ParentCategoryID == "00000000-0000-0000-0000-000000000000");
            DisplayTitle = "Financial Transaction Management";
            BulkMeter = "5249FFEB-6907-46AA-9204-D4527E11F9CE";
            ViewModelApplication.CurrentControlViewModel=ViewModelApplication.CurrentControlViewModel;

            mRequest = new ParameterTransactionApiModel()
            {
                Client = (string)ViewModelApplication.FClientID ?? "4766E825-1B58-410D-B06B-5A2639CA22C8",
                Category = (string)ViewModelApplication.FCostHierarchyID
            };

            Client = new HierarchyItemSelectionViewModel
            {
                Label = "Select Client",
                //EditedName = mLoadingText,
                EditedName = (string)ViewModelApplication.ClientShortName ?? "Client",
                OriginalName = (string)ViewModelApplication.ClientShortName ?? "Client Lookup",
                OriginalKid = (string)ViewModelApplication.FClientID ?? "4766E825-1B58-410D-B06B-5A2639CA22C8",
                EditedKid = (string)ViewModelApplication.FClientID ,
                HierarchyTypeID = "1A8CCEE0-52D1-454B-8165-23EDB2241058",
                PrepareAction = SetClientHierarchySelectionAsync,
                PriorPopupViewModel = ViewModelApplication.CurrentPopupViewModel,
                CommitAction = UpdateClientSelectionAsync,
                //OriginalKid = (await ClientDataStore.GetLoginCredentialsAsync() ?).ClientID,

            };

            //ViewModelApplication.CurrentControlViewModel = ViewModelApplication.CurrentControlViewModel;
            ViewModelApplication.CurrentControlViewModel = Client;


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
                PriorPopupViewModel =ViewModelApplication.CurrentPopupViewModel,
                CommitAction = UpdateCostHierarchySelectionAsync,
            };

            //ViewModelApplication.CurrentControlViewModel = ViewModelApplication.CurrentControlViewModel;
            ViewModelApplication.CurrentControlViewModel = Client;
            Category = new HierarchyItemSelectionViewModel
            {
                Label = "Cost Category",
                //EditedName = mLoadingText,
                EditedName = "Selected Category",
                ClientID = ViewModelApplication.FClientID,

                //HierarchyTypeID = "64413ae7-822f-4866-9ebe-433083d699ac",
                RootID = (string)ViewModelApplication.FCostHierarchyID,
                PrepareAction = SetCostCategorySelectionAsync,
                PriorPopupViewModel = ViewModelApplication.CurrentPopupViewModel,

                //HierarchyID = ((CostHierarchyViewModel)((CostHierarchyListViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy).MSelectedCostHierarchy).KCategoryID,

                CommitAction = SelectCategoryAsync,
                ProcessSelectionAction = ProcessSelectionActionAsync,
            };


            Party = new HierarchyItemSelectionViewModel
            {
                Label = "Transacting Party",
                //EditedName = mLoadingText,
                EditedName = "Selected Party",
                ClientID = ViewModelApplication.FClientID,
                HierarchyTypeID = "ADEEBB16-F553-48F8-955F-663227A4886C",
                PrepareAction = SetPartyHierarchySelectionAsync,
                //HierarchyTypeID = ((CostHierarchyListViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy).f,
                //HierarchyID = ((CostHierarchyViewModel)((CostHierarchyListViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy).MSelectedCostHierarchy).KCategoryID,
                PriorPopupViewModel = ViewModelApplication.CurrentPopupViewModel,
                CommitAction = SelectPartyAsync,
                ProcessSelectionAction = ProcessSelectionActionAsync,
            };


            Account = new HierarchyItemSelectionViewModel
            {
                Label = "Transacting Account",
                //EditedName = mLoadingText,
                EditedName = "Selected Account",
                ClientID = ViewModelApplication.FClientID,
                HierarchyTypeID = "A806FD4A-8F02-4CA0-BFCE-51A8587D9CC8",
                PrepareAction = SetAccountHierarchySelectionAsync,
                //HierarchyTypeID = ((CostHierarchyListViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy).f,
                //HierarchyID = ((CostHierarchyViewModel)((CostHierarchyListViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy).MSelectedCostHierarchy).KCategoryID,
                PriorPopupViewModel = ViewModelApplication.CurrentPopupViewModel,
                CommitAction = SelectAccountAsync,
                ProcessSelectionAction = ProcessSelectionActionAsync,
            };


            Project = new HierarchyItemSelectionViewModel
            {
                Label = "Project",
                //EditedName = mLoadingText,
                EditedName = "Selected Project",
                ClientID = ViewModelApplication.FClientID,
                HierarchyTypeID = "C77539FC-A801-46B5-9681-5902496BF83E",
                PrepareAction = SetProjectHierarchySelectionAsync,
                //HierarchyTypeID = ((CostHierarchyListViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy).f,
                //HierarchyID = ((CostHierarchyViewModel)((CostHierarchyListViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy).MSelectedCostHierarchy).KCategoryID,
                PriorPopupViewModel = ViewModelApplication.CurrentPopupViewModel,
                CommitAction = SelectProjectAsync,
                ProcessSelectionAction = ProcessSelectionActionAsync,
            };



            Asset = new HierarchyItemSelectionViewModel
            {
                Label = "Asset",
                //EditedName = mLoadingText,
                EditedName = "Selected Asset",
                ClientID = ViewModelApplication.FClientID,
                HierarchyTypeID = "56DA3516-FF85-4A9F-A8F6-56874B4CC8E7",
                PrepareAction = SetAssetHierarchySelectionAsync,
                //HierarchyTypeID = ((CostHierarchyListViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy).f,
                //HierarchyID = ((CostHierarchyViewModel)((CostHierarchyListViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy).MSelectedCostHierarchy).KCategoryID,
                PriorPopupViewModel = ViewModelApplication.CurrentPopupViewModel,
                CommitAction = SelectAssetAsync,
                ProcessSelectionAction = ProcessSelectionActionAsync,
            };


            Person = new HierarchyItemSelectionViewModel
            {
                Label = "Linked Person",
                //EditedName = mLoadingText,
                EditedName = "Selected Person",
                ClientID = ViewModelApplication.FClientID,
                HierarchyTypeID = "ADEEBB16-F553-48F8-955F-663227A4886C",
                PrepareAction = SetPersonHierarchySelectionAsync,
                //HierarchyTypeID = ((CostHierarchyListViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy).f,
                //HierarchyID = ((CostHierarchyViewModel)((CostHierarchyListViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy).MSelectedCostHierarchy).KCategoryID,
                PriorPopupViewModel = ViewModelApplication.CurrentPopupViewModel,
                CommitAction = SelectPersonAsync,
                ProcessSelectionAction = ProcessSelectionActionAsync,
            };


            TimeStart = new DateTimeViewModel
            {
                Label = "Month Start",
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
                Label = "Month End",
                OriginalDateTime = DateTime.Now,
                EditedDateTime = DateTime.Now,
                OriginalTime = new System.Windows.Controls.ComboBoxItem(),
                EditedTime = new System.Windows.Controls.ComboBoxItem(),
                //(DateTime.Now.AddHours(-1)).ToShortTimeString(),
                //EditedTime. = "System.Windows.Controls.ComboBoxItem: 00:30",//(DateTime.Now.AddHours(-1)).ToShortTimeString(),
            };
            TimeEnd.OriginalTime.Content = "00:00";
            TimeEnd.EditedTime.Content = "00:00";



            //DateReference = new DateTimeViewModel
            //{
            //    Label = "Calculation reference date",
            //    OriginalDateTime = DateTime.Now,
            //    EditedDateTime = DateTime.Now,
            //    OriginalTime = new System.Windows.Controls.ComboBoxItem(),
            //    EditedTime = new System.Windows.Controls.ComboBoxItem(),
            //    //(DateTime.Now.AddHours(-1)).ToShortTimeString(),
            //    //EditedTime. = "System.Windows.Controls.ComboBoxItem: 00:30",//(DateTime.Now.AddHours(-1)).ToShortTimeString(),
            //};
            //TimeEnd.OriginalTime.Content = "00:00";
            //TimeEnd.EditedTime.Content = "00:00";

            //CostHierarchy = new CostHierarchyListViewModel(Root.OriginalKid);
            //SelectedCostHierarchy = new CostHierarchyViewModel();
            //CostHierarchy.MSelectedCostHierarchy = SelectedCostHierarchy;


            // Create commands
            AttachmentButtonCommand = new RelayCommand(AttachmentButton);
            PopupClickawayCommand = new RelayCommand(PopupClickaway);
            ReconcileCommandNew = new RelayCommand(Reconcile);
            PopulateCommand = new RelayCommand(Populate);
            SearchCommand = new RelayCommand(Search);
            OpenSearchCommand = new RelayCommand(OpenSearch);
            CloseCommand = new RelayCommand(Close);
            ClearSearchCommand = new RelayCommand(ClearSearch);
            InitialiseCostHCommand = new RelayCommand(async () => await InitialiseCostHAsync());
            InitialiseClientSrchCommand = new RelayCommand(async () => await ClientSrchAsync());
            ReconcileCommand = new RelayCommand(async () => await ReconcileAsync());
            //ViewModelApplication.CurrentControlViewModel = null;

            // Make a default menu
            //AttachmentMenu = new ChatAttachmentPopupMenuViewModel();
        }

        #endregion

        //#region Command Methods

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
            ViewModelApplication.CurrentPopupContent = 0;
            ViewModelApplication.FClientID = ((HierarchyItemSelectionViewModel)((TransactionSelectionAnalysisPageViewModel)ViewModelApplication.CurrentPageViewModel).Client).EditedKid;
            ViewModelApplication.ClientShortName = ((HierarchyItemSelectionViewModel)((TransactionSelectionAnalysisPageViewModel)ViewModelApplication.CurrentPageViewModel).Client).EditedName;
            ViewModelApplication.FCostHierarchyID = ((HierarchyItemSelectionViewModel)((TransactionSelectionAnalysisPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy).EditedKid;
            ViewModelApplication.CostHierarchyShortName = ((HierarchyItemSelectionViewModel)((TransactionSelectionAnalysisPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy).EditedName;
            //ViewModelApplication.CurrentPopupContent = 0;
            //To do: Lookup to be user rights and available options driven
            //BulkMeter = "5249ffeb-6907-46aa-9204-d4527e11f9ce";
            if (ViewModelApplication.CurrentControlViewModel ==null)
            { return; }

            //if ((ViewModelApplication.CurrentControlViewModel).GetType().Name != "CostHierarchyListViewModel")
            //{ return; }

            var Test3 = ((HierarchyItemSelectionViewModel)((TransactionSelectionAnalysisPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy).EditedKid;
            if (Test3 == null)
            {
                System.Windows.MessageBox.Show(
                    "No cost structure has been selected",
                    "for Managing the Transactions",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }
            //if (((CostHierarchyListViewModel)ViewModelApplication.CurrentControlViewModel).MSelectedCostHierarchy.KCategoryID == null)

            ////To DO - message user
            //{
            //    System.Windows.MessageBox.Show($"First select a valid Transaction Client to proceed...");
            //    return;
            //};
            ShortName = ((HierarchyItemSelectionViewModel)((TransactionSelectionAnalysisPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy).EditedName;
            TimeEnd.OriginalDateTime = TimeEnd.EditedDateTime;
            TimeStart.OriginalDateTime = TimeStart.EditedDateTime;

            //Make start time and end time equal to overload sql call
            //var t1 = TimeEnd.EditedDateTime.ToString("yyyy/MM/dd");
            //var t2 = TimeStart.EditedDateTime.Hour.ToString("00");
            //var t3 = $"{TimeEnd.EditedDateTime.ToString("yyyy/MM/dd")}{" "}{TimeStart.EditedDateTime.Hour.ToString()}{":00:00"}";
            //TimeEnd.EditedDateTime = DateTime.Parse(t3);
            //TimeEnd.EditedDateTime = DateTime.Parse($"{TimeEnd.EditedDateTime.ToString("yyyy/MM/dd")}{" "}{TimeStart.EditedDateTime.Hour.ToString("00")}{":00:00"}");

            ViewModelApplication.CurrentPopupViewModel = new TransactionTreeViewModel(mRequest);
            ((TransactionTreeViewModel)ViewModelApplication.CurrentPopupViewModel).ControlTitle = "Financial Transaction Detail: " + ShortName;
            //force a reload of the BulkRecon Control
            ViewModelApplication.CurrentPopupContent = 0;
            ViewModelApplication.CurrentPopupContent = PopupContent.Transaction;

            ViewModelApplication.PopupVisible = true;

        }

        /// <summary>
        /// Initialises the Cost Hierarchy Search
        /// </summary>
        /// <returns>Returns true if successful, false otherwise</returns>
        public async Task<bool> ReconcileAsync()
        {
            // Lock this command to ignore any other requests while processing
            return await RunCommandAsync(() => ReconcileInProgress, async () =>
            {

                ViewModelApplication.CurrentPopupContent = 0;
                ViewModelApplication.FClientID = ((HierarchyItemSelectionViewModel)((TransactionSelectionAnalysisPageViewModel)ViewModelApplication.CurrentPageViewModel).Client).EditedKid;
                ViewModelApplication.ClientShortName = ((HierarchyItemSelectionViewModel)((TransactionSelectionAnalysisPageViewModel)ViewModelApplication.CurrentPageViewModel).Client).EditedName;
                ViewModelApplication.FCostHierarchyID = ((HierarchyItemSelectionViewModel)((TransactionSelectionAnalysisPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy).EditedKid;
                ViewModelApplication.CostHierarchyShortName = ((HierarchyItemSelectionViewModel)((TransactionSelectionAnalysisPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy).EditedName;
                mRequest.Client = ViewModelApplication.FClientID;
                mRequest.Category = Category.OriginalKid ?? CostHierarchy.OriginalKid;
                Category.OriginalName = Category.OriginalName ?? CostHierarchy.OriginalName;
                mRequest.MonthStart = int.Parse(TimeStart.EditedDateTime.ToString("yyyyMMdd"));
                mRequest.MonthEnd = int.Parse(TimeEnd.EditedDateTime.ToString("yyyyMMdd"));
                //ViewModelApplication.CurrentPopupContent = 0;
                //To do: Lookup to be user rights and available options driven
                //BulkMeter = "5249ffeb-6907-46aa-9204-d4527e11f9ce";
                if (ViewModelApplication.CurrentControlViewModel != null)
                {                 //if ((ViewModelApplication.CurrentControlViewModel).GetType().Name != "CostHierarchyListViewModel")
                                  //{ return; }

                    var Test3 = ((HierarchyItemSelectionViewModel)((TransactionSelectionAnalysisPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy).EditedKid;
                    if (Test3 == null)
                    {
                        System.Windows.MessageBox.Show(
                            "No cost structure has been selected",
                            "for Managing the Transactions",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

                        return false;
                    }
                    //if (((CostHierarchyListViewModel)ViewModelApplication.CurrentControlViewModel).MSelectedCostHierarchy.KCategoryID == null)

                    ////To DO - message user
                    //{
                    //    System.Windows.MessageBox.Show($"First select a valid Transaction Client to proceed...");
                    //    return;
                    //};
                    ShortName = Category.OriginalName;
                    TimeEnd.OriginalDateTime = TimeEnd.EditedDateTime;
                    TimeStart.OriginalDateTime = TimeStart.EditedDateTime;

                    //Make start time and end time equal to overload sql call
                    //var t1 = TimeEnd.EditedDateTime.ToString("yyyy/MM/dd");
                    //var t2 = TimeStart.EditedDateTime.Hour.ToString("00");
                    //var t3 = $"{TimeEnd.EditedDateTime.ToString("yyyy/MM/dd")}{" "}{TimeStart.EditedDateTime.Hour.ToString()}{":00:00"}";
                    //TimeEnd.EditedDateTime = DateTime.Parse(t3);
                    //TimeEnd.EditedDateTime = DateTime.Parse($"{TimeEnd.EditedDateTime.ToString("yyyy/MM/dd")}{" "}{TimeStart.EditedDateTime.Hour.ToString("00")}{":00:00"}");

                    ViewModelApplication.CurrentPopupViewModel = new TransactionTreeViewModel(mRequest);
                    ((TransactionTreeViewModel)ViewModelApplication.CurrentPopupViewModel).ControlTitle = "Financial Transaction Detail: " + ShortName;
                    //force a reload of the BulkRecon Control
                    ViewModelApplication.CurrentPopupContent = 0;
                    ViewModelApplication.CurrentPopupContent = PopupContent.Transaction;

                    ViewModelApplication.PopupVisible = true;

                }

                return true;
            });
        }

        /// <summary>
        /// When the user clicks the send button, sends the message
        /// </summary>
        public void Populate()
        {

        }

        public async Task<bool> SetCostHierarchySelectionAsync()
        {
            // Lock this command to ignore any other requests while processing

            return await RunCommandAsync(() => SetHierarchyCompleted, async () =>
            {
                // Update the First Name value on the server...

                //((TransactionSelectionAnalysisPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy.ClientID = ((TransactionSelectionAnalysisPageViewModel)ViewModelApplication.CurrentPageViewModel).Client.EditedKid;
                //((TransactionSelectionAnalysisPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy.RootID = ((TransactionSelectionAnalysisPageViewModel)ViewModelApplication.CurrentPageViewModel).Client.RootID;
                ViewModelApplication.CurrentControlViewModel = ((TransactionSelectionAnalysisPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy;
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

        public async Task<bool> SetClientHierarchySelectionAsync()
        {
            // Lock this command to ignore any other requests while processing

            return await RunCommandAsync(() => SetHierarchyCompleted, async () =>
            {
                // Update the First Name value on the server...

                ViewModelApplication.CurrentControlViewModel = ((TransactionSelectionAnalysisPageViewModel)ViewModelApplication.CurrentPageViewModel).Client;
                HierarchyParam = new ParameterHierarchyItemSelectApiModel
                {
                    //ClientID = Client.ClientID,
                    //FHierarchyID = Client.OriginalKid,
                    //RootID = Client.OriginalKid,
                    //Level = 1

                    RootID = ViewModelApplication.FClientID ?? "4766E825-1B58-410D-B06B-5A2639CA22C8",
                    Level = 1
                };
                ViewModelApplication.CurrentPopupViewModel = new HierarchyTreeViewModel1(HierarchyParam);
                return true;
            });

        }


        public async Task<bool> SetCostCategorySelectionAsync()
        {
            // Lock this command to ignore any other requests while processing

            return await RunCommandAsync(() => SetHierarchyCompleted, async () =>
            {
                // Update the Category Classification value on the server...

                ViewModelApplication.CurrentControlViewModel =Category;
                //ViewModelApplication.ControlParameter1 = ((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).Category;
                HierarchyParam = new ParameterHierarchyItemSelectApiModel
                {

                    Level = 100,
                    RootID = Category.RootID,
                };

                //var TypeName = (ViewModelApplication.ControlPopupCostCategory.GetType().Name) ?? "";
                if (ViewModelApplication.ControlPopupCostCategory == null || ViewModelApplication.ControlPopupCostCategory.GetType().Name != "HierarchyTreeViewModel1")
                {
                    ViewModelApplication.CurrentPopupViewModel = new HierarchyTreeViewModel1(HierarchyParam);
                    ViewModelApplication.ControlPopupCostCategory = ViewModelApplication.CurrentPopupViewModel;
                }
                else
                { ViewModelApplication.CurrentPopupViewModel = ViewModelApplication.ControlPopupCostCategory; }
                ((HierarchyTreeViewModel1)ViewModelApplication.CurrentPopupViewModel).SearchText = Category.OriginalKid;
                ((HierarchyTreeViewModel1)ViewModelApplication.CurrentPopupViewModel).PerformKIdSearch();
                ((HierarchyTreeViewModel1)ViewModelApplication.CurrentPopupViewModel).SearchText = "";
                return true;
            });

        }


        public async Task<bool> SetAccountHierarchySelectionAsync()
        {
            // Lock this command to ignore any other requests while processing

            return await RunCommandAsync(() => SetHierarchyCompleted, async () =>
            {
                // Update the Party value on the server...

                ViewModelApplication.CurrentControlViewModel = Account;
                HierarchyParam = new ParameterHierarchyItemSelectApiModel
                {
                    Level = 0,
                    HierarchyTypeID = Account.HierarchyTypeID,
                    ClientID = ViewModelApplication.FClientID,
                };
                ViewModelApplication.CurrentPopupViewModel = new HierarchyTreeViewModel1(HierarchyParam);
                ((HierarchyTreeViewModel1)ViewModelApplication.CurrentPopupViewModel).SearchText = Account.OriginalKid;

                //ViewModelApplication.ControlParameter1 = ((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).Party;
                return true;
            });

        }
        public async Task<bool> SetPartyHierarchySelectionAsync()
        {
            // Lock this command to ignore any other requests while processing

            return await RunCommandAsync(() => SetHierarchyCompleted, async () =>
            {
                // Update the Party value on the server...

                ViewModelApplication.CurrentControlViewModel = Party;
                HierarchyParam = new ParameterHierarchyItemSelectApiModel
                {

                    Level = 0,
                    ClientID = ViewModelApplication.FClientID,
                    HierarchyTypeID = Party.HierarchyTypeID,
                };
                if (ViewModelApplication.ControlPopupParty == null)
                { ViewModelApplication.CurrentPopupViewModel = new HierarchyTreeViewModel1(HierarchyParam); }
                else
                { ViewModelApplication.CurrentPopupViewModel = ViewModelApplication.ControlPopupParty; }
                ((HierarchyTreeViewModel1)ViewModelApplication.CurrentPopupViewModel).SearchText = Party.OriginalKid;
                ((HierarchyTreeViewModel1)ViewModelApplication.CurrentPopupViewModel).PerformKIdSearch();
                ((HierarchyTreeViewModel1)ViewModelApplication.CurrentPopupViewModel).SearchText = "";
                //ViewModelApplication.ControlParameter1 = ((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).Party;
                return true;
            });

        }

        public async Task<bool> SetPersonHierarchySelectionAsync()
        {
            // Lock this command to ignore any other requests while processing

            return await RunCommandAsync(() => SetHierarchyCompleted, async () =>
            {
                // Update the Party value on the server...

                ViewModelApplication.CurrentControlViewModel = Person;
                HierarchyParam = new ParameterHierarchyItemSelectApiModel
                {

                    Level = 0,
                    ClientID = ViewModelApplication.FClientID,
                    HierarchyTypeID = Party.HierarchyTypeID,
                };
                if (ViewModelApplication.ControlPopupParty == null)
                { ViewModelApplication.CurrentPopupViewModel = new HierarchyTreeViewModel1(HierarchyParam); }
                else
                { ViewModelApplication.CurrentPopupViewModel = ViewModelApplication.ControlPopupParty; }
                ((HierarchyTreeViewModel1)ViewModelApplication.CurrentPopupViewModel).SearchText = Party.OriginalKid;
                ((HierarchyTreeViewModel1)ViewModelApplication.CurrentPopupViewModel).PerformKIdSearch();
                ((HierarchyTreeViewModel1)ViewModelApplication.CurrentPopupViewModel).SearchText = "";
                //ViewModelApplication.ControlParameter1 = ((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).Party;
                return true;
            });

        }

        public async Task<bool> SetProjectHierarchySelectionAsync()
        {
            // Lock this command to ignore any other requests while processing

            return await RunCommandAsync(() => SetHierarchyCompleted, async () =>
            {
                // Update the Party value on the server...

                ViewModelApplication.CurrentControlViewModel = Project;
                HierarchyParam = new ParameterHierarchyItemSelectApiModel
                {
                    Level = 0,
                    HierarchyTypeID = Project.HierarchyTypeID,
                    ClientID = ViewModelApplication.FClientID,
                };
                ViewModelApplication.CurrentPopupViewModel = new HierarchyTreeViewModel1(HierarchyParam);
                ((HierarchyTreeViewModel1)ViewModelApplication.CurrentPopupViewModel).SearchText = Project.OriginalKid;

                //ViewModelApplication.ControlParameter1 = ((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).Party;
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
                if (ViewModelApplication.FClientID != Client.EditedKid)
                { 
                    CostHierarchy.OriginalKid = null;
                    CostHierarchy.OriginalName = null;
                    ViewModelApplication.FCostHierarchyID = null;
                    ViewModelApplication.CostHierarchyShortName = null;
                }
                ViewModelApplication.FClientID = Client.EditedKid;
                ViewModelApplication.ClientShortName = Client.EditedName;
                Client.OriginalName = Client.EditedName;
                ViewModelApplication.PopupVisible = false;
                ViewModelApplication.CurrentPopupViewModel = null;

                ViewModelApplication.CurrentPopupContent = 0;

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

                ViewModelApplication.FCostHierarchyID = CostHierarchy.EditedKid;
                ViewModelApplication.CostHierarchyShortName = CostHierarchy.EditedName;
                ViewModelApplication.PopupVisible = false;
                ViewModelApplication.CurrentPopupViewModel = null;
                ViewModelApplication.CurrentPopupContent = 0;
                CostHierarchy.OriginalName = CostHierarchy.EditedName;
                mRequest.Category = CostHierarchy.OriginalKid;
                return true;
            });

        }


        /// <summary>
        /// Initialises the Cost Hierarchy Search
        /// </summary>
        /// <returns>Returns true if successful, false otherwise</returns>
        public async Task<bool> SelectCategoryAsync()
        {
            // Lock this command to ignore any other requests while processing


            return await RunCommandAsync(() => UpdateHierarchyCompleted, async () =>
            {


                mRequest.Category = Category.EditedKid;
                Category.OriginalName  = Category.EditedName;
                Category.OriginalKid = Category.EditedKid;
                return true;
            });
        }

        /// <summary>
        /// Initialises the Cost Hierarchy Search
        /// </summary>
        /// <returns>Returns true if successful, false otherwise</returns>
        public async Task<bool> SelectPartyAsync()
        {
            // Lock this command to ignore any other requests while processing


            return await RunCommandAsync(() => UpdateHierarchyCompleted, async () =>
            {

                //ViewModelApplication.ControlPopupParty = ViewModelApplication.CurrentPopupViewModel;
                mRequest.Party = Party.EditedKid;
                Party.OriginalName = Party.EditedName;
                Party.OriginalKid = Party.EditedKid;
                return true;

            });
        }


        /// <summary>
        /// Initialises the Cost Hierarchy Search
        /// </summary>
        /// <returns>Returns true if successful, false otherwise</returns>
        public async Task<bool> SelectPersonAsync()
        {
            // Lock this command to ignore any other requests while processing


            return await RunCommandAsync(() => UpdateHierarchyCompleted, async () =>
            {

                //ViewModelApplication.ControlPopupParty = ViewModelApplication.CurrentPopupViewModel;
                if (Person.EditedName != "Selected Person")
                    Person.OriginalName = Person.EditedName;
                    Person.OriginalKid = Person.EditedKid;
                    mRequest.Person = Person.EditedKid;


                ViewModelApplication.ControlPopupParty = ViewModelApplication.CurrentPopupViewModel;
                if (ViewModelApplication.ControlParameter1 != null)
                {
                    if (ViewModelApplication.CurrentPopupContent != PopupContent.Classify)
                    {
                        ViewModelApplication.ControlParameter1 = null;
                        ViewModelApplication.CurrentPopupContent = PopupContent.Classify;
                    }
                    ViewModelApplication.PopupVisible = true;
                }


                return true;
            });
        }



        public async Task<bool> SelectAccountAsync()
        {
            // Lock this command to ignore any other requests while processing

            return await RunCommandAsync(() => SelectAccountCompleted, async () =>
            {
                if (Account.EditedName != "Selected Account")
                    //ViewModelApplication.CurrentPopupViewModel = ViewModelApplication.CurrentPopupViewModel;
                    Account.OriginalName = Account.EditedName;
                     mRequest.Account = Account.EditedKid;
                    Account.OriginalKid = Account.EditedKid;

                if (ViewModelApplication.ControlParameter1 != null)
                {
                    if (ViewModelApplication.CurrentPopupContent != PopupContent.Classify)
                    {
                        ViewModelApplication.ControlParameter1 = null;
                        ViewModelApplication.CurrentPopupContent = PopupContent.Classify;
                    }
                    ViewModelApplication.PopupVisible = true;
                }
                //((ManageClassificationViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy = new CostHierarchyListViewModel(Root.EditedKid)
                //{
                //    MSelectedCostHierarchy = new CostHierarchyViewModel()
                //};
                //ViewModelApplication.CurrentControlViewModel = ((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy;
                return true;
            });
        }



        public async Task<bool> SelectProjectAsync()
        {
            // Lock this command to ignore any other requests while processing

            return await RunCommandAsync(() => SelectAccountCompleted, async () =>
            {
                if (Project.EditedName != "Selected Project")
                    //ViewModelApplication.CurrentPopupViewModel = ViewModelApplication.CurrentPopupViewModel;
                    Project.OriginalName = Project.EditedName;
                    Project.OriginalKid = Project.EditedKid;
                    mRequest.Project = Project.EditedKid;
                if (ViewModelApplication.ControlParameter1 != null)
                {
                    if (ViewModelApplication.CurrentPopupContent != PopupContent.Classify)
                    {
                        ViewModelApplication.ControlParameter1 = null;
                        ViewModelApplication.CurrentPopupContent = PopupContent.Classify;
                    }
                    ViewModelApplication.PopupVisible = true;
                }
                //((ManageClassificationViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy = new CostHierarchyListViewModel(Root.EditedKid)
                //{
                //    MSelectedCostHierarchy = new CostHierarchyViewModel()
                //};
                //ViewModelApplication.CurrentControlViewModel = ((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy;
                return true;
            });
        }



        public async Task<bool> SetAssetHierarchySelectionAsync()
        {
            // Lock this command to ignore any other requests while processing

            return await RunCommandAsync(() => SetHierarchyCompleted, async () =>
            {
                // Update the Party value on the server...

                ViewModelApplication.CurrentControlViewModel = Asset;
                HierarchyParam = new ParameterHierarchyItemSelectApiModel
                {
                    Level = 0,
                    HierarchyTypeID = Asset.HierarchyTypeID,
                    ClientID = ViewModelApplication.FClientID,
                };
                ViewModelApplication.CurrentPopupViewModel = new HierarchyTreeViewModel1(HierarchyParam);
                ((HierarchyTreeViewModel1)ViewModelApplication.CurrentPopupViewModel).SearchText = Asset.OriginalKid;
                ((HierarchyTreeViewModel1)ViewModelApplication.CurrentPopupViewModel).PerformKIdSearch();
                ((HierarchyTreeViewModel1)ViewModelApplication.CurrentPopupViewModel).SearchText = "";
                //ViewModelApplication.ControlParameter1 = ((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).Party;
                return true;
            });

        }



        public async Task<bool> SelectAssetAsync()
        {
            // Lock this command to ignore any other requests while processing

            return await RunCommandAsync(() => SelectAccountCompleted, async () =>
            {
                if (Asset.EditedName != "Selected Asset")
                    //ViewModelApplication.CurrentPopupViewModel = ViewModelApplication.CurrentPopupViewModel;
                    Asset.OriginalName = Asset.EditedName;
                    Asset.OriginalKid = Asset.EditedKid;
                    mRequest.Asset = Asset.EditedKid;
                if (ViewModelApplication.ControlParameter1 != null)
                {
                    if (ViewModelApplication.CurrentPopupContent != PopupContent.Classify)
                    {
                        ViewModelApplication.ControlParameter1 = null;
                        ViewModelApplication.CurrentPopupContent = PopupContent.Classify;
                    }
                    ViewModelApplication.PopupVisible = true;
                }
                //((ManageClassificationViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy = new CostHierarchyListViewModel(Root.EditedKid)
                //{
                //    MSelectedCostHierarchy = new CostHierarchyViewModel()
                //};
                //ViewModelApplication.CurrentControlViewModel = ((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy;
                return true;
            });
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
        /// Initialises the Cost Hierarchy Search
        /// </summary>
        /// <returns>Returns true if successful, false otherwise</returns>
        public async Task<bool> InitialiseCostHAsync()
        {
            //// Lock this command to ignore any other requests while processing
            //return await RunCommandAsync(() => CostHierarchySrchIsSaving, async () =>
            //{

            //    ((TransactionSelectionAnalysisPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy = new CostHierarchyListViewModel(Root.EditedKid)
            //    {
            //        MSelectedCostHierarchy = new CostHierarchyViewModel()
            //    };
            //    ViewModelApplication.CurrentControlViewModel = ((TransactionSelectionAnalysisPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy;
            return true;
            //});
        }


        /// <summary>
        /// Initialises the Client Search
        /// </summary>
        /// <returns>Returns true if successful, false otherwise</returns>
        public async Task<bool> ClientSrchAsync()
        {
            // Lock this command to ignore any other requests while processing
            return await RunCommandAsync(() => CostHierarchySrchIsSaving, async () =>
            {

                ViewModelApplication.CurrentControlViewModel = ((TransactionSelectionAnalysisPageViewModel)ViewModelApplication.CurrentPageViewModel).Client;
                return true;
            });
        }


        public async Task<bool> ProcessSelectionActionAsync()
        {
            // Lock this command to ignore any other requests while processing

            return await RunCommandAsync(() => SelectAccountCompleted, async () =>
            {

                //if (ViewModelApplication.ControlParameter1 != null)
                //{
                //ViewModelApplication.CurrentPopupViewModel = ViewModelApplication.ControlParameter1;
                //if (ViewModelApplication.CurrentPopupContent != PopupContent.Classify)
                //{
                //    ViewModelApplication.ControlParameter1 = null;
                //    ViewModelApplication.CurrentPopupContent = PopupContent.Classify;
                //}
                ViewModelApplication.PopupVisible =false;
                //}
                //((ManageClassificationViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy = new CostHierarchyListViewModel(Root.EditedKid)
                //{
                //    MSelectedCostHierarchy = new CostHierarchyViewModel()
                //};
                //ViewModelApplication.CurrentControlViewModel = ((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy;
                return true;
            }
            );
        }



        /// <summary>
        /// Closes the search dialog
        /// </summary>
        public void Close()
        { 
        // Close settings menu
        ViewModelApplication.SideMenuVisible = true;
            //ViewModelApplication.CurrentSideMenuViewModel = null;
            ViewModelApplication.CurrentPopupContent = 0;
            ViewModelApplication.FClientID = ((HierarchyItemSelectionViewModel)((TransactionSelectionAnalysisPageViewModel)ViewModelApplication.CurrentPageViewModel).Client).EditedKid;
            ViewModelApplication.ClientShortName = ((HierarchyItemSelectionViewModel)((TransactionSelectionAnalysisPageViewModel)ViewModelApplication.CurrentPageViewModel).Client).EditedName;
            ViewModelApplication.FCostHierarchyID = ((HierarchyItemSelectionViewModel)((TransactionSelectionAnalysisPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy).EditedKid;
            ViewModelApplication.CostHierarchyShortName = ((HierarchyItemSelectionViewModel)((TransactionSelectionAnalysisPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy).EditedName;
            ViewModelApplication.CurrentControlViewModel = null;
            ViewModelApplication.CurrentPopupViewModel = null;
            ViewModelApplication.CurrentPopupContent = 0; 
            //ViewModelApplication.CurrentPopupContent = PopupContent.AddElement; ;
            ViewModelApplication.GoToPage(ApplicationPage.Chat);}

        //#endregion
    }
}
