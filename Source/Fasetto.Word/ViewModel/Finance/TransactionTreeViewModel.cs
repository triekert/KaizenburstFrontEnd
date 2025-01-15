
using Dna;
using Fasetto.Word.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using static Fasetto.Word.Core.CoreDI;
using static Fasetto.Word.DI;

namespace Fasetto.Word
{
    /// <summary>
    /// This is the view-model of the UI.  It provides a data source
    /// for the TreeView (the FirstGeneration property), a bindable
    /// SearchText property, and the SearchCommand to perform a search.
    /// </summary>
    public class TransactionTreeViewModel : BaseViewModel

    {

        #region Public Properties

        /// <summary>
        /// A set of Bulk Meter Recon records for the selected period
        /// </summary>
        public ObservableCollection<TransactionViewModel>Trans_action{ get; set; }
        public ObservableCollection<TransactionViewModel> OrgTransaction { get; set; }
        public ObservableCollection<TransactionViewModel> MPersist { get; set; }

        public int Trans_actionRec { get; set; }
        //public ObservableCollection<HierarchyViewModel> FirstGeneration1 { get; set; }
        /// <summary>
        /// A flag indicating if the login command is running
        /// </summary>
        public bool TransClassBuildIsRunning { get; set; }

        private object mStocksLock = new object();
        #endregion

        #region Data

        //private readonly ReadOnlyCollection<HierarchyViewModel> mFirstGeneration;
        //protected TransactionViewModel mRootHierarchyElement;
        //protected TransactionViewModel mRootHierarchyElement1;
        //private readonly ICommand mSearchCommand;
        public TransactionListDataModel mTDML;

        public TransactionResultListApiModel mChange;
  
        public TransactionViewModel mTVM;
        public ParameterTransactionApiModel mRequest;
        public string mClient;
        public int mMonthStart;
        public int mMonthEnd;
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
        #endregion//Public Commands

        #region Constructor
        /// <summary>
        /// The HierarchyTreeViewModel is a visual interface for interacting with hierarchical
        /// Structures persisted on the database linked to the application
        /// Generic hierarchy structures with parent-child relationships may be used to represent
        /// appropriate data sets
        /// </summary>
        /// <param name="hierarchyTable"></param>
        /// The hierarchyTable passed through as a parameter identifies the specific hierarchy set to be retrieved
        /// from persistent s
        public TransactionTreeViewModel(string client, DateTime timeStart, DateTime timeEnd, string category,string budget)
        {
            #region Build HierarchyViewCollection

            Trans_action = new ObservableCollection<TransactionViewModel>();
            BindingOperations.EnableCollectionSynchronization(Trans_action, mStocksLock);


            mTVM = new TransactionViewModel
            {

                ShortName = "Loading...Please be patient",
                //TimeSlotStart = new DateTime(2023, 1, 22, 0, 0, 0),
                //Missing = 3,
                //ChildMeters = 87,
                //VolumeIn = 3145.342F,
                //VolumeOut = 3215.124F


            };
            Trans_action.Add(mTVM);

            mRequest = new ParameterTransactionApiModel
            { 
                Client = client,
                MonthStart = int.Parse(timeStart.ToString("yyyyMMdd")),
                MonthEnd = int.Parse(timeEnd.ToString("yyyyMMdd")),
                Category = category,
                Budget = budget,
            };

            var MMmonth = timeStart.ToString("MM");
            //mTableName = hierarchyTable;
            #endregion
            //retrieve hierarchy from persistent storage on server
            //To Do: Add mTableName as parameter when calling HierarchyAsync to populate hierarchy

            //mTODStart = TODStart;
            //mTODEnd = TODEnd;
            PriorPopupViewModel = ViewModelApplication.CurrentPopupViewModel;
            TaskManager.RunAndForget(TransactionAsync);


            // Get the OptFinHierarchies currently configured - first populate 'root hierarchy' variable with all configured root hierarchy elements currently available


            //UpdateTreeViewElements();
            
            CloseCommand = new RelayCommand(Close);
            //mSearchCommand = new SearchCategoryTreeCommand(this);
        }





        #endregion // Constructor

        #region Properties
        #region Public Properties



        /// <summary>
        /// A flag indicating if the login command is running
        /// </summary>
        public bool TransactionBuildIsRunning { get; set; }

        /// <summary>
        /// Title to be published on Control
        /// </summary>
        public string ControlTitle { get; set; }
        //{get => mTableName;
        //    set
        //    {
        //        if (value == mTableName)
        //            return;

        //        mTableName = value;

        //    } }
        #endregion//Public Properties



        #region SearchCommand

        /// <summary>
        /// Returns the command used to execute a search in the Category tree.
        /// </summary>
        //public ICommand SearchCommand => mSearchCommand;

        //private class SearchCategoryTreeCommand : ICommand
        //{
        //    private readonly TransactionTreeViewModel mCategoryTree;

        //    public SearchCategoryTreeCommand(TransactionTreeViewModel CategoryTree)
        //    {
        //        mCategoryTree = CategoryTree;
        //    }

        //    public bool CanExecute(object parameter)
        //    {
        //        return true;
        //    }

        //    event EventHandler ICommand.CanExecuteChanged
        //    {
        //        // I intentionally left these empty because
        //        // this command never raises the event, and
        //        // not using the WeakEvent pattern here can
        //        // cause memory leaks.  WeakEvent pattern is
        //        // not simple to implement, so why bother.
        //        add { }
        //        remove { }
        //    }

        //    public void Execute(object parameter)
        //    {
        //        mCategoryTree.PerformSearch();
        //    }
        //}

        #endregion // SearchCommand

        #endregion //Properties

        /// <summary>
        /// Return Hierarchy of interest from Object persistence infrastructure
        /// User credentials are used to determine access authorisation
        /// </summary>
        /// <returns></returns>
        public async Task TransactionAsync()
        {
            await RunCommandAsync(() => TransactionBuildIsRunning, async () =>
            {

                // Store single transcient instance of client data store
                var scopedClientDataStore = ClientDataStore;
                //
                //return;
                //

                // Update values from local cache
                // Get the user token
                var token = (await scopedClientDataStore.GetLoginCredentialsAsync())?.Token;
                // Call the server and attempt to register with the provided credentials
                // If we don't have a token (then not logged in...)
                if (string.IsNullOrEmpty(token))
                    // Then do nothing more
                    return;
                var result = await WebRequests.PostAsync<ApiResponse<TransactionResultListApiModel>>(
                // Set URL
                    RouteHelpers.GetAbsoluteRoute(ApiRoutes.ReturnTransaction),
                    mRequest,
                    bearerToken: token);

                // If the response has an error...
                if (await result.HandleErrorIfFailedAsync("Transaction retrieval Failed"))
                    // We are done
                    return;

                // OK successfully registered (and logged in)... now get appropriate tree view data
                //for now; keep a snapshot of persisted data
                //mOriginal = result.ServerResponse.Response;

                ;
               
                try
                {
                    //var hierarchyResultApiModels = mOriginal.ToList();
                    //make a clone of the persisted data for manipulation on front end
                    //mPersist = new TransactionResultListApiModel();
                    //mPersist.Clone(mOriginal, mPersist);
                    //Transaction.Clear();
                    //lock (mStocksLock) {

                    //    Trans_action = new ObservableCollection<TransactionViewModel>();
                    //}

                    //BindingOperations.EnableCollectionSynchronization(Trans_action, mStocksLock);
                    OrgTransaction = new ObservableCollection<TransactionViewModel>();



                    mChange = new TransactionResultListApiModel();




                    MPersist = new ObservableCollection<TransactionViewModel>();
                    var matches = result.ServerResponse.Response.OrderByDescending(x => x.Posted_Date).ThenBy(x => x.KFinTranID).ThenBy(x => x.ShortName).ToList();
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
                            KClientID = item.KClientID,
                            KAccountID = item.KAccountID,
                            KAccountName = item.KAccountName,
                            Units = item.Units,
                        };

                        //Lock collection to prevent contention with UI

                            MPersist.Add(mTVM);
                    }


                    RefreshTransactionList();
                    //Clone(Trans_action, OrgTransaction);

                    Trans_actionRec = 0;
                    
                    //if (Trans_action.Count != mPersist.Count)
                    //{ 
                    //};


                }
                 catch (Exception e)
                {
                    throw e;
                }


            });
        }


        #region SearchText

        /// <summary>
        /// Gets/sets a fragment of the name to search for.
        /// </summary>
        public string SearchText
        {
            get => mSearchText;
            set
            {
                if (value == mSearchText)
                    return;

                mSearchText = value;

                MatchingCategoryEnumerator = null;
            }
        }

        public IEnumerator<TransactionViewModel> MatchingCategoryEnumerator { get; private set; }

        #endregion // SearchText

        //#endregion // Properties

        

        #region Search Logic //KCategoryID
        public IEnumerator<TransactionViewModel> MatchingKCategoryEnumerator { get; private set; }

        #endregion // SearchKCategoryID

 

        public void Close()
        {
            // Close settings menu

            //Call API to persist changes if any...

            var UpPersist = ((TransactionTreeViewModel)ViewModelApplication.CurrentPopupViewModel).Trans_action;
            var OPersist = ((TransactionTreeViewModel)ViewModelApplication.CurrentPopupViewModel).OrgTransaction;
            //create record set of CRUD members of mPersist based on changes appearing in Trans_act -> Delete, Add and change flags



            if (ViewModelApplication.CurrentPageViewModel.GetType().Name == "BudgetSelectionPageViewModel" ||ViewModelApplication.CurrentPageViewModel.GetType().Name == "ExpenditureVSBudgetPageViewModel")
            {
                if (ViewModelApplication.CurrentPageViewModel.GetType().Name == "BudgetSelectionPageViewModel")
                { 
                ViewModelApplication.CurrentPopupViewModel = PriorPopupViewModel;
                ViewModelApplication.CurrentPopupContent = PopupContent.BudgetReview;
                ViewModelApplication.CurrentPopupViewModel = PriorPopupViewModel;// ((BudgetAdjustViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel;
                ViewModelApplication.PopupVisible = true;
                }
                else
                { 
                ViewModelApplication.CurrentPopupViewModel = PriorPopupViewModel;
                    if (ViewModelApplication.CurrentPopupViewModel.GetType().Name == "ExpenditureAdjustViewModel")

                    {

                        ViewModelApplication.CurrentPopupContent = PopupContent.ExpenditureAdjust;
                        ViewModelApplication.PopupVisible = true;
                    }
                    else { 

                            ViewModelApplication.CurrentPopupContent = PopupContent.ExpenditureReview;
                            ViewModelApplication.CurrentPopupViewModel = PriorPopupViewModel;// ((BudgetAdjustViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel;
                            ViewModelApplication.PopupVisible = true;
                          }
                }
            }
            else
            {
                ViewModelApplication.PopupVisible = false;
                ViewModelApplication.CurrentControlViewModel = ((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).Client;

                if (UpPersist == null || OPersist == null) { return; }
                var except = UpPersist.Except(OPersist);
                //TO DO: Map PopupViewModel to PopupContent with converter
                //ViewModelApplication.CurrentPopupContent = PopupContent.HierarchyItemSelection;
                var tmpTbl = (TransactionResultListApiModel)((TransactionTreeViewModel)ViewModelApplication.CurrentPopupViewModel).mChange;
                var res = from c in tmpTbl
                          group c by new { c.KFinActualID, c.ChangeType } into transApi
                          select transApi.OrderByDescending(x => x.DateEffective)
                                          .FirstOrDefault();



                ViewModelApplication.CurrentPopupViewModel = ((TransactionTreeViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel;

            }


        }

        public async Task PersistTransClassAsync()
        {
            await RunCommandAsync(() => TransClassBuildIsRunning, async () =>
            {

                // Store single transcient instance of client data store
                var scopedClientDataStore = ClientDataStore;

                // Update values from local cache
                // Get the user token
                var token = (await scopedClientDataStore.GetLoginCredentialsAsync())?.Token;
                // Call the server and attempt to register with the provided credentials
                // If we don't have a token (then not logged in...)
                if (string.IsNullOrEmpty(token))
                    // Then do nothing more
                    return;
                var result = await WebRequests.PostAsync<ApiResponse<TransactionResultListApiModel>>(
                // Set URL
                    RouteHelpers.GetAbsoluteRoute(ApiRoutes.PersistClassification),
                    mChange,
                    bearerToken: token);

                // If the response has an error...
                if (await result.HandleErrorIfFailedAsync("Classification Update Failed"))
                    // We are done
                    return;
                mChange.Clear();

                // return to menu


            });
        }





        /// <summary>
        /// This function allows the addition of an TransactionViewModel to the Trans_action collection
        /// </summary>
        /// <param name="item"></param>
        public void AddItem( TransactionViewModel item)
        {
            lock (mStocksLock)
            {
               Trans_action.Add(item);

            }
        }


        /// <summary>
        /// This function allows the removal of a TransactionViewModel from the Trans_action collection
        /// </summary>
        /// <param name="item"></param>
        public void RemoveItem(TransactionViewModel item)
        {
            lock (mStocksLock)
            {
                Trans_action.Remove(item);

            }
        }



        /// <summary>
        /// This function removes items from the target list included in the source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public void Remove(ObservableCollection<TransactionViewModel> source, ObservableCollection<TransactionViewModel> target)
        {
            foreach (var item in source)
                target.Remove(item);
        }

        /// <summary>
        /// This method will make a clone of the source List of objects
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public void Clone(ObservableCollection<TransactionViewModel> source, ObservableCollection<TransactionViewModel> target)
        {
            foreach (var item in source)
            {
                var mTR = new TransactionViewModel
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
                    KHierarchyID = item.KHierarchyID,
                    DateEffective = item.DateEffective,
                    KPartyID = item.KPartyID,
                    KPartyName = item.KPartyName,
                    FCatSrchID = item.FCatSrchID,
                    IsTemplate = item.IsTemplate,
                    Notes = item.Notes,
                    KClientID = item.KClientID,
                };
                target.Add(mTR);
            }

        }

        public void RefreshTransactionList()
        {
            lock (mStocksLock)
            {
                Trans_action.Clear();
            }
            var mTest = MPersist.GroupBy(x => x.KFinTranID)
            .Select(g => g.First()).ToList();
            var matches = mTest.OrderByDescending(x => x.Posted_Date).ThenBy(x => x.KFinTranID).ThenBy(x => x.ShortName).ToList();
            //mPersist = result.ServerResponse.Response;
            //if (matches.Count>0)

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
                    Units = item.Units,
                    KClientID = item.KClientID,
                };

                //Lock collection to prevent contention with UI


                AddItem(mTVM);

            }


        }

    }


}