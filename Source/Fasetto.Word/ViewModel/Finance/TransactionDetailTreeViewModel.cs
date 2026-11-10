using Fasetto.Word.Core;
using Fasetto.Word.Core.ApiModels.Controls;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using static Fasetto.Word.DI;

namespace Fasetto.Word
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    /// <summary>
    /// This is the view-model of the UI.  It provides a data source
    /// for the TreeView (the FirstGeneration property), a bindable
    /// SearchText property, and the SearchCommand to perform a search.
    /// </summary>
    public class TransactionDetailTreeViewModel : BaseViewModel

    {

        #region Public Properties

        /// <summary>
        /// Field containing description of the transaction
        /// </summary>
        public TextEntryViewModel TransactionDescription { get; set; }

        /// <summary>
        /// Field containing actual allocation  of the total transaction to the selected cost category
        /// </summary>
        public TextEntryViewModel Amount { get; set; }

        /// <summary>
        /// The Party to be linked for the allocation
        /// </summary>
        public HierarchyItemSelectionViewModel Party { get; set; }

        /// <summary>
        /// The Account linked to the transaction
        /// </summary>
        public HierarchyItemSelectionViewModel Account { get; set; }


        /// <summary>
        /// The start time for analysis of readings
        /// </summary>
        public DateTimeViewModel TransactionDate { get; set; }

        /// <summary>
        /// A flag indicating if the account selection is complete
        /// </summary>
        public bool SelectAccountCompleted { get; set; }

        /// <summary>
        /// A flag indicating if the account selection is complete
        /// </summary>
        public bool SelectPartyCompleted { get; set; }

        /// <summary>
        /// True to show the hierarchy retrieval command is running
        /// </summary>
        public bool SetHierarchyCompleted { get; set; }


        /// <summary>
        /// Populate parameters for retrieval of required hierarchy tree
        /// </summary>
        public ParameterHierarchyItemSelectApiModel HierarchyParam { get; set; }



        /// <summary>
        /// Transaction data for the selected transaction being processsed
        /// </summary>
        public TransactionViewModel MSelected  { get; set; }
            /// <summary>
        /// A set of TransactionDetail  records for the selected transaction
        /// </summary>
        public ObservableCollection<TransactionViewModel>TransactionDetail{ get; set; }


        private object mStocksLock = new object();
        public string ControlTitle { get; set; }



        /// <summary>
        /// Store enum of current popup view
        /// </summary>
        public PopupContent PriorPopupContent { get; set; }
        //public ObservableCollection<HierarchyViewModel> FirstGeneration1 { get; set; }


        /// <summary>
        /// Indicates if the current control is pending an update (in progress)
        /// </summary>
        public bool Working { get; set; }


        /// <summary>
        /// The text for the Edit transaction level properties
        /// </summary>
        public string TransactionEditButtonText { get; set; }



        #endregion

        #region Data

        //private readonly ReadOnlyCollection<HierarchyViewModel> mFirstGeneration;
        //protected BulkReconViewModel mRootHierarchyElement;
        //protected BulkReconViewModel mRootHierarchyElement1;
        //private readonly ICommand mSearchCommand;
        //public BulkReconListDataModel mBRDML;
        //public BulkReconResultListApiModel mPersist, mPersistTmp,mOriginal;
        //public BulkReconViewModel mBRVM;
        //public ParameterBulkReconApiModel mRequest;
        //public string mBulkMeter;
        //public DateTime mTimeStart;
        //public DateTime mTimeEnd;
        //public int mTODStart;
        //public int mTODEnd;
        /// <summary>
        /// 
        /// Store View Model of current popup to allow reverse navigation
        /// </summary>
        public object PriorPopupViewModel { get; set; }
        //public HierarchyElementViewModel mElement;

        //IEnumerator<HierarchyManagementViewModel> mMatchingCategoryEnumerator;

        //public HierarchyManagementTreeViewModel(IEnumerator<HierarchyManagementViewModel> matchingCategoryEnumerator)
        //{
        //    MatchingCategoryEnumerator = matchingCategoryEnumerator;
        //}

        private string mSearchText = "", mSearchKCategoryID = string.Empty, mParentCategoryID = string.Empty;

        #endregion // Data
        #region Public Commands


        /// <summary>
        /// The command to close the settings menu
        /// </summary>
        public ICommand CloseCommand { get; set; }

        /// <summary>
        /// Edit attributes set at transaction level
        /// </summary>
        public ICommand TransactionEditCommand { get; set; }
 

        /// <summary>
        /// The command to search a new image for loading into the image window
        /// </summary>
        public ICommand BrowseImageCommand { get; set; }





#endregion Public Commands

        #region Constructor
        /// <summary>
        /// The HierarchyTreeViewModel is a visual inteface for interacting with hiearchical
        /// Structures persisted on the database linked to the application
        /// Generic hierarchy structures with parent-child relationships may be used to represent
        /// appropriate data sets
        /// </summary>
        /// <param name="hierarchyTable"></param>
        /// The hierarchyTable passed through as a paremeter identifies the specific hierarchy set to be retrieved
        /// from persistent s
        public TransactionDetailTreeViewModel(string mKFinTranID)
        {
            #region Build TransactionDetailCollection


 


            TransactionDetail = new ObservableCollection<TransactionViewModel>();


            MSelected = new TransactionViewModel();
            MSelected = ((TransactionTreeViewModel)ViewModelApplication.CurrentPopupViewModel).Trans_action.FirstOrDefault(x => x.KFinTranID == mKFinTranID);


            RefreshTransactionList(mKFinTranID, ((TransactionTreeViewModel)ViewModelApplication.CurrentPopupViewModel).MPersist);
            ((TransactionTreeViewModel)ViewModelApplication.CurrentPopupViewModel).TransSelector = ((DataGrid)((TransactionTreeViewModel)ViewModelApplication.CurrentPopupViewModel).Transaction).SelectedIndex;
            PriorPopupViewModel = ViewModelApplication.CurrentPopupViewModel;
            PriorPopupContent = ViewModelApplication.CurrentPopupContent;


            TransactionDate = new DateTimeViewModel
            {
                Label = "Transaction Date",
                OriginalDateTime = MSelected.Posted_Date,
                EditedDateTime = MSelected.Posted_Date,
                IsDateOnly = true,
                IsReadOnly = false,
            };



            TransactionDescription = new TextEntryViewModel
            {
                Label = "Transaction Description",
                OriginalText = MSelected.Description,
                EditedText = MSelected.Description,
            };


            // Create Node Name
            Amount = new TextEntryViewModel
            {
                Label = "Transaction Amount",
                OriginalText = MSelected.TransAmount.ToString("C", CultureInfo.CurrentCulture),
                EditedText = MSelected.TransAmount.ToString("C", CultureInfo.CurrentCulture)
                //PrepareAction = PrepareAllocationAsync,
                //CommitAction = UpdateAllocationAsync,
            };
            Party = new HierarchyItemSelectionViewModel
            {
                Label = "Transacting Party",
                //EditedName = mLoadingText,
                OriginalKid = MSelected.KPartyID,
                OriginalName = MSelected.KPartyName,
                EditedKid = MSelected.KPartyID,
                EditedName = MSelected.KPartyName,
                HierarchyTypeID = "ADEEBB16-F553-48F8-955F-663227A4886C",
                PrepareAction = SetPartyHierarchySelectionAsync,
                //HierarchyTypeID = ((CostHierarchyListViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy).f,
                //HierarchyID = ((CostHierarchyViewModel)((CostHierarchyListViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy).MSelectedCostHierarchy).KCategoryID,
                CommitAction = SelectPartyAsync,
                ProcessSelectionAction = ProcessSelectionActionAsync,
            };
            Account = new HierarchyItemSelectionViewModel
            {
                Label = "Transacting Account",
                //EditedName = mLoadingText,
                OriginalKid = MSelected.KAccountID,
                OriginalName = MSelected.KAccountName,
                EditedKid = MSelected.KAccountID,
                EditedName = MSelected.KAccountName,


                //ClientID = Selected.KClientID,
                HierarchyTypeID = "A806FD4A-8F02-4CA0-BFCE-51A8587D9CC8",
                PrepareAction = SetAccountHierarchySelectionAsync,
                //HierarchyTypeID = ((CostHierarchyListViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy).f,
                //HierarchyID = ((CostHierarchyViewModel)((CostHierarchyListViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy).MSelectedCostHierarchy).KCategoryID,
                CommitAction = SelectAccountAsync,
                ProcessSelectionAction = ProcessSelectionActionAsync,
            };


            // Get the OptFinHierarchies currently configured - first populate 'root hierarchy' variable with all configured root hierarchy elements currently available
            TransactionEditButtonText = "Update Transaction level data";
            ((HierarchyItemSelectionViewModel)Account).PriorPopupViewModel = this;
            ((HierarchyItemSelectionViewModel)Account).PriorPopupContent = PopupContent.TransactionDetail;
            ((HierarchyItemSelectionViewModel)Party).PriorPopupViewModel = this;
            ((HierarchyItemSelectionViewModel)Account).PriorPopupContent = PopupContent.TransactionDetail;
            // Create commands
            CloseCommand = new RelayCommand(Close);
            TransactionEditCommand = new RelayCommand(async () => await TransactionEditAsync());

            //mSearchCommand = new SearchCategoryTreeCommand(this);
        }



        #endregion // TransactionDetailCollection

        #endregion // Constructor

        public void RefreshTransactionList(string mKFinTranID, ObservableCollection<TransactionViewModel> mPersist)
        {
            lock (mStocksLock)
            {
                TransactionDetail.Clear();
            }

            //mPersist = result.ServerResponse.Response;
            //if (matches.Count>0)
            var matches = mPersist.Where(x => x.KFinTranID == mKFinTranID).ToList();

            foreach (var item in matches)
            {

                var mTVM = new TransactionViewModel

                {
                    Posted_Date = item.Posted_Date,
                    Month = item.Month,
                    Description = item.Description,
                    TransAmount = item.TransAmount,
                    ActualAmount = item.ActualAmount,
                    ShortName = item.ShortName,
                    KCategoryID = item.KCategoryID,
                    KFinActualID = item.KFinActualID,
                    KFinTranID = item.KFinTranID,
                    KPartyID = item.KPartyID,
                    KPartyName = item.KPartyName,
                    IsChanged = false,
                    FCatSrchID = item.FCatSrchID,
                    KHierarchyID = item.KHierarchyID,
                    IsDocLinked = item.IsDocLinked,
                    Notes = item.Notes,
                    KAccountID = item.KAccountID,
                    KAccountName = item.KAccountName,
                    KProjectID = item.KProjectID,
                    KProjectName = item.KProjectName,
                    KAssetID = item.KAssetID,
                    KAssetName = item.KAssetName,
                    KPersonID = item.KPersonID,
                    KPersonName = item.KPersonName,
                    Units = item.Units,
                    KClientID = item.KClientID,

                };

                //Lock collection to prevent contention with UI


                AddItem(mTVM);

            }


        }

        /// <summary>
        /// This function allows the addition of an TransactionViewModel to the Trans_action collection
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(TransactionViewModel item)
        {
            lock (mStocksLock)
            {
                TransactionDetail.Add(item);

            }
        }


        public void Close()
        {
            var mType = ((TransactionDetailTreeViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel.GetType().Name;        
            var mPopup = ((TransactionDetailTreeViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupContent;
            ViewModelApplication.CurrentPopupViewModel = ((TransactionDetailTreeViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel;
            ViewModelApplication.CurrentPopupContent = mPopup;
            ViewModelApplication.PopupVisible = true;
            //var mTransactionDetailTreeViewModel = ViewModelApplication.CurrentPopupViewModel;
            ////((TransactionTreeViewModel)ViewModelApplication.CurrentPopupViewModel).Transaction.SelectedIndex
            //ViewModelApplication.PopupVisible = false;
            ////ViewModelApplication.ControlParameter1 = null;

            ////Give control back to parent 'Popup view model'


            //if (mType == "TransactionDetailTreeViewModel")
            //{
            //    ViewModelApplication.CurrentPopupContent = 0;
            //    ViewModelApplication.CurrentPopupViewModel = mTransactionDetailTreeViewModel;
            //    ViewModelApplication.CurrentPopupContent = PopupContent.TransactionDetail;
            //}
            //else
            //{

            //    ViewModelApplication.CurrentPopupContent = PopupContent.Transaction;
            //}

            //ViewModelApplication.PopupVisible = true;
            ////((TransactionTreeViewModel)ViewModelApplication.CurrentPopupViewModel).Transaction.SelectedIndex

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
                DateTarget = TransactionDate.EditedDateTime,
            };
            ((HierarchyItemSelectionViewModel)ViewModelApplication.CurrentControlViewModel).PriorPopupViewModel = ViewModelApplication.CurrentPopupViewModel;
            ((HierarchyItemSelectionViewModel)ViewModelApplication.CurrentControlViewModel).PriorPopupContent= ViewModelApplication.CurrentPopupContent;
            ViewModelApplication.CurrentPopupViewModel = new HierarchyTreeViewModel(HierarchyParam, true); 


                ((HierarchyTreeViewModel)ViewModelApplication.CurrentPopupViewModel).SearchText = Party.OriginalKid;
                ((HierarchyTreeViewModel)ViewModelApplication.CurrentPopupViewModel).PerformKIdSearch();


                return true;
            });

        }

        public async Task<bool>TransactionEditAsync()
        {
            return await RunCommandAsync(() => Working, async () =>
            {

                var tmp0 = ((TransactionDetailTreeViewModel)ViewModelApplication.CurrentPopupViewModel).
                PriorPopupViewModel;
                var Selected = ((TransactionDetailTreeViewModel)ViewModelApplication.CurrentPopupViewModel).MSelected;
                var Mtmp2 = ((TransactionTreeViewModel)((TransactionDetailTreeViewModel)(ViewModelApplication.CurrentPopupViewModel)).PriorPopupViewModel).mChange;
                var Mtmp = ((TransactionTreeViewModel)((TransactionDetailTreeViewModel)ViewModelApplication.CurrentPopupViewModel).
                PriorPopupViewModel).MPersist;
                var IntAmnt = Selected.ActualAmount;
                decimal.TryParse(Amount.EditedText, NumberStyles.Currency, CultureInfo.CurrentCulture, out IntAmnt);
                if (!(Selected.TransAmount != IntAmnt || Selected.Description != TransactionDescription.EditedText || TransactionDate.EditedDateTime != Selected.Posted_Date ||
                Party.EditedKid != Selected.KPartyID || Account.EditedKid != Selected.KAccountID))
                    //if no changes, do nothing
                    return true;
                //Else Modify the Selected Transaction with updated values 
                var Selected1 = new TransactionViewModel();
                Selected1?.CopyPropertiesFrom(Selected);
                Selected1.TransAmount = IntAmnt;
                Selected1.Posted_Date = TransactionDate.EditedDateTime;
                Selected1.Description = TransactionDescription.EditedText;
                Selected1.KAccountID = Account.EditedKid;
                Selected1.KAccountName = Account.EditedName;
                Selected1.KPartyID = Party.EditedKid;
                Selected1.KPartyName = Party.EditedName;
                Selected1.DateEffective = DateTime.Now;
                //Create a Change record to Persist Changes on the datapase
                Mtmp2.Add(Selected1.ToApiModel("c"));
                //Update changes on all transaction allocations for the same transaction
                // Update all matching transaction records in Mtmp using LINQ
                Mtmp.Where(x => x.KFinTranID == Selected1.KFinTranID).ToList().ForEach(item =>
                {
                    item.TransAmount = Selected1.TransAmount;
                    item.Posted_Date = Selected1.Posted_Date;
                    item.Description = Selected1.Description;
                    item.KAccountID = Selected1.KAccountID;
                    item.KAccountName = Selected1.KAccountName;
                    item.KPartyID = Selected1.KPartyID;
                    item.KPartyName = Selected1.KPartyName;
                    item.DateEffective = Selected1.DateEffective;
                });

                ((TransactionTreeViewModel)tmp0).RefreshTransactionList();
                //Refresh UI for Transaction Detail List
                //((TransactionDetailTreeViewModel)((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel).RefreshTransactionList(Selected.KFinTranID,
                //    Mtmp);


                await ((TransactionTreeViewModel)tmp0).PersistTransClassAsync();
                Close();
                return true;
            }).ConfigureAwait(false);

        }


        public async Task<bool> SetAccountHierarchySelectionAsync() =>
            // Lock this command to ignore any other requests while processing

            await RunCommandAsync(() => SetHierarchyCompleted, async () =>
            {
                // Update the Party value on the server...

                ViewModelApplication.CurrentControlViewModel = Account;
                HierarchyParam = new ParameterHierarchyItemSelectApiModel
                {
                    Level = 0,
                    HierarchyTypeID = Account.HierarchyTypeID,
                    ClientID = ViewModelApplication.FClientID,
                    DateTarget = TransactionDate.EditedDateTime,
                };
                ((HierarchyItemSelectionViewModel)ViewModelApplication.CurrentControlViewModel).PriorPopupViewModel = ViewModelApplication.CurrentPopupViewModel;
                ((HierarchyItemSelectionViewModel)ViewModelApplication.CurrentControlViewModel).PriorPopupContent = ViewModelApplication.CurrentPopupContent;

                ViewModelApplication.CurrentPopupViewModel = new HierarchyTreeViewModel(HierarchyParam, true);
                ((HierarchyTreeViewModel)ViewModelApplication.CurrentPopupViewModel).SearchText = Account.OriginalKid;
                ((HierarchyTreeViewModel)ViewModelApplication.CurrentPopupViewModel).PerformKIdSearch();
                return true;
            }).ConfigureAwait(false);


        /// <summary>
        /// Initialises the Cost Hierarchy Search
        /// </summary>
        /// <returns>Returns true if successful, false otherwise</returns>
        public async Task<bool> SelectPartyAsync()
        {
            // Lock this command to ignore any other requests while processing


            return await RunCommandAsync(() => SelectPartyCompleted, async () =>
            {

                //ViewModelApplication.ControlPopupParty = ViewModelApplication.CurrentPopupViewModel;



                    ViewModelApplication.PopupVisible = true;
               


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
                //ViewModelApplication.CurrentPopupViewModel = ((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy;
                return true;
            });
        }


        /// <summary>
        /// Used tp insert a new node with the currently selected node as parent
        /// </summary>

        public async Task<bool> ProcessSelectionActionAsync()
        {
            // Lock this command to ignore any other requests while processing

            return await RunCommandAsync(() => SelectAccountCompleted, async () =>
            {

                //if (ViewModelApplication.ControlParameter1 != null)
                //{
                ViewModelApplication.CurrentPopupViewModel = ((HierarchyTreeViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel;

                ViewModelApplication.CurrentPopupContent = PopupContent.TransactionDetail;

                ViewModelApplication.PopupVisible = true;
                //}
                //((ManageClassificationViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy = new CostHierarchyListViewModel(Root.EditedKid)
                //{
                //    MSelectedCostHierarchy = new CostHierarchyViewModel()
                //};
                //ViewModelApplication.CurrentPopupViewModel = ((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy;
                return true;
            }).ConfigureAwait(false);

        }

            ~TransactionDetailTreeViewModel()
        {
            Console.WriteLine("Destructor: Object Destroyed!");
        }

    }

}