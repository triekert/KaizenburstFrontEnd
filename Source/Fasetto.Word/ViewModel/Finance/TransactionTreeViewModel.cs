
using Dna;
using Fasetto.Word.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
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


        //public ObservableCollection<HierarchyViewModel> FirstGeneration1 { get; set; }

        #endregion

        #region Data

        //private readonly ReadOnlyCollection<HierarchyViewModel> mFirstGeneration;
        //protected TransactionViewModel mRootHierarchyElement;
        //protected TransactionViewModel mRootHierarchyElement1;
        //private readonly ICommand mSearchCommand;
        public TransactionListDataModel mTDML;
        //public TransactionResultListApiModel mPersist, mPersistTmp,mOriginal;
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
        /// The HierarchyTreeViewModel is a visual inteface for interacting with hiearchical
        /// Structures persisted on the database linked to the application
        /// Generic hierarchy structures with parent-child relationships may be used to represent
        /// appropriate data sets
        /// </summary>
        /// <param name="hierarchyTable"></param>
        /// The hierarchyTable passed through as a paremeter identifies the specific hierarchy set to be retrieved
        /// from persistent s
        public TransactionTreeViewModel(string client, DateTime timeStart, DateTime timeEnd)
        {
            #region Build HierarchyViewCollection
            Trans_action = new ObservableCollection<TransactionViewModel>();

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
                MonthStart = int.Parse(timeStart.ToString("yyyyMM")),
                MonthEnd = int.Parse(timeEnd.ToString("yyyyMM"))
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
                    Trans_action = new ObservableCollection<TransactionViewModel>();
                    //Transaction.Clear();
                    var matches = result.ServerResponse.Response.ToList();


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
                        KFinTranID = item.KFinTranID


                    };
                        Trans_action.Add(mTVM); 
                    }

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

        #region Search Logic -Short Name

        //public void PerformSearch()
        //{
        //    if (MatchingCategoryEnumerator == null || !MatchingCategoryEnumerator.MoveNext())
        //        VerifyMatchingCategoryEnumerator();

        //    var Category = MatchingCategoryEnumerator.Current;

        //    if (Category == null)
        //        return;

        //    // Ensure that this Category is in view.
        //    if (Category.mParent != null)
        //        Category.mParent.IsExpanded = true;

        //    Category.IsSelected = true;
        //    //Category.IsExpanded = false;
        //}

        //private void VerifyMatchingCategoryEnumerator()
        //{
        //    var matches = FindMatches(mSearchText, mRootHierarchyElement);
        //    MatchingCategoryEnumerator = matches.GetEnumerator();

        //    if (!MatchingCategoryEnumerator.MoveNext())
        //    {
        //        MessageBox.Show(
        //            "No matching names were found - please check your spelling.",
        //            "Search for Tree Item failed",
        //            MessageBoxButton.OK,
        //            MessageBoxImage.Information
        //            );
        //    }
        //}

        //private IEnumerable<TransactionViewModel> FindMatches(string searchText, TransactionViewModel Category)
        //{
        //    if (Category.NameContainsText(searchText))
        //        yield return Category;

        //    foreach (var child in Category.Children)
        //        foreach (var match in FindMatches(searchText, child))
        //            yield return match;
        //}

        #endregion // Search Logic

        #region Search Logic //KCategoryID
        public IEnumerator<TransactionViewModel> MatchingKCategoryEnumerator { get; private set; }

        #endregion // SearchKCategoryID
        #region Search Logic //KCategoryID
        //private void PerformKIdSearch()
        //{

        //    if (MatchingKCategoryEnumerator == null || !MatchingKCategoryEnumerator.MoveNext())
        //        VerifyMatchingKCategoryEnumerator();
        //    var KCategory = MatchingKCategoryEnumerator.Current;
        //    if (KCategory == null)
        //        return;

        //    // Ensure that this Category is in view.
        //    if (KCategory.mParent != null)
        //        KCategory.mParent.IsExpanded = true;

        //    KCategory.IsSelected = true;
        //}

        //private void VerifyMatchingKCategoryEnumerator()
        //{
        //    //var matchK = FindKMatches(mParentID, mTarget);
        //    var matchK = FindKMatches(mSearchText, mRootHierarchyElement);
        //    MatchingKCategoryEnumerator = matchK.GetEnumerator();
        //    _ = !MatchingKCategoryEnumerator.MoveNext();

        //}

        //private IEnumerable<TransactionViewModel> FindKMatches(string searchText, TransactionViewModel Category)
        //{
        //    //var mSearchText = searchText;
        //    if (Category.KCategoryIdContainsText(searchText))
        //        yield return Category;

        //    foreach (var child in Category.Children)
        //        foreach (var matchK in FindKMatches(searchText, child))
        //            yield return matchK;
        //}

        #endregion //Search Logic //KCategoryID
 

        public void Close()
        {
            // Close settings menu

            //var mType = ((TransactionTreeViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel.GetType().Name;
            //ViewModelApplication.CurrentPopupViewModel = ((TransactionTreeViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel;
            //var TransactionTreeViewModel = ViewModelApplication.CurrentPopupViewModel;
            ViewModelApplication.PopupVisible = false;


            //ViewModelApplication.CurrentPopupContent = PopupContent.HierarchyItemSelection;
            //ViewModelApplication.CurrentPageViewModel = ViewModelApplication.CurrentPageViewModel;
            ViewModelApplication.CurrentControlViewModel = ((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).Root;

            //ViewModelApplication.PopupVisible = true;


        }

 



    }

}