
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
    public class BillingPeriodListViewModel : BaseViewModel

    {

        #region Public Properties

        /// <summary>
        /// A set of Bulk Meter Recon records for the selected period
        /// </summary>
        public ObservableCollection<BillingPeriodViewModel> BillingPeriodList{ get; set; }

        /// <summary>
        /// The selected Billing Period view model
        /// </summary>
        public BillingPeriodViewModel MSelectedBillingPeriod { get; set; }

        //public ObservableCollection<HierarchyViewModel> FirstGeneration1 { get; set; }

        #endregion

        #region Data

        public BillingPeriodViewModel mBPVM;
        public string mRequest;
        public string mBulkMeter;
        /// <summary>
        /// Indicates if the current text is in edit mode
        /// </summary>
        public bool Editing { get; set; }


        private string mSearchText = "", mSearchKCategoryID = string.Empty, mParentCategoryID = string.Empty;

        #endregion // Data
        #region Public Commands
        /// <summary>
        /// The command to close the settings menu
        /// </summary>
        public ICommand CloseCommand { get; set; }

        /// <summary>
        /// Puts the control into edit mode
        /// </summary>
        public ICommand EditCommand { get; set; }
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
        public BillingPeriodListViewModel(string client)
        {
            #region Build HierarchyViewCollection
            BillingPeriodList = new ObservableCollection<BillingPeriodViewModel> {

             new BillingPeriodViewModel
            {

                FClientID = "Loading...Please be patient",
                KBillingPeriodID = "Test1",
                //TimeSlotStart = new DateTime(2023, 1, 22, 0, 0, 0),
                //Missing = 3,
                //ChildMeters = 87,
                //VolumeIn = 3145.342F,
                //VolumeOut = 3215.124F


            } };



            //mTableName = hierarchyTable;
            #endregion
            //retrieve hierarchy from persistent storage on server
            //To Do: Add mTableName as parameter when calling HierarchyAsync to populate hierarchy
            mRequest = client;
            TaskManager.RunAndForget(BillingPeriodAsync);


            // Get the OptFinHierarchies currently configured - first populate 'root hierarchy' variable with all configured root hierarchy elements currently available


            //UpdateTreeViewElements();
            CloseCommand = new RelayCommand(Close);
            EditCommand = new RelayCommand(Edit);
            //mSearchCommand = new SearchCategoryTreeCommand(this);
        }


        /// <summary>
        /// Puts the control into edit mode
        /// </summary>
        public void Edit()
        {
            // Set the edited text to the current value


            //Go into edit mode
            MSelectedBillingPeriod = MSelectedBillingPeriod;
            //((SWBillingPageViewModel)ViewModelApplication.CurrentPageViewModel).BillingPeriod.MSelectedBillingPeriod = MSelectedBillingPeriod;
            ((SWBillingPageViewModel)ViewModelApplication.CurrentPageViewModel).SelectedBillingPeriod = MSelectedBillingPeriod;
            Editing = true;
            //ViewModelApplication.CurrentControlViewModel
        }


        #endregion // Constructor

        #region Properties
        #region Public Properties



        /// <summary>
        /// A flag indicating if the login command is running
        /// </summary>
        public bool BulkReconBuildIsRunning { get; set; }

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
        //    private readonly BulkReconTreeViewModel mCategoryTree;

        //    public SearchCategoryTreeCommand(BulkReconTreeViewModel CategoryTree)
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
        public async Task BillingPeriodAsync()
        {
            await RunCommandAsync(() => BulkReconBuildIsRunning, async () =>
            {

                // Store single transient instance of client data store
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
                var result = await WebRequests.PostAsync<ApiResponse<BillingPeriodResultListApiModel>>(
                // Set URL
                    RouteHelpers.GetAbsoluteRoute(ApiRoutes.ReturnBillingPeriods),
                    mRequest,
                    bearerToken: token);

                // If the response has an error...
                if (await result.HandleErrorIfFailedAsync("Billing Period List retrieval Failed"))
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
                    //mPersist = new BulkReconResultListApiModel();
                    //mPersist.Clone(mOriginal, mPersist);
                    //BulkRecon.Clear();
                    BillingPeriodList = new ObservableCollection<BillingPeriodViewModel>();
                    //BulkRecon.Clear();
                    var matches = result.ServerResponse.Response.ToList();


                    foreach (var item in matches)
                    {

                    var mBPVM = new BillingPeriodViewModel

                        {
                            KBillingPeriodID = item.KBillingPeriodID,
                            TimeStart = item.TimeStart,
                            FClientID = item.FClientID,
                            TimeEnd = item.TimeEnd,
                        };
                        BillingPeriodList.Add(mBPVM); 
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

        public IEnumerator<BillingPeriodListViewModel> MatchingCategoryEnumerator { get; private set; }

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

        //private IEnumerable<BulkReconViewModel> FindMatches(string searchText, BulkReconViewModel Category)
        //{
        //    if (Category.NameContainsText(searchText))
        //        yield return Category;

        //    foreach (var child in Category.Children)
        //        foreach (var match in FindMatches(searchText, child))
        //            yield return match;
        //}

        #endregion // Search Logic

        #region Search Logic //KCategoryID
        public IEnumerator<BillingPeriodViewModel> MatchingKCategoryEnumerator { get; private set; }

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

        //private IEnumerable<BulkReconViewModel> FindKMatches(string searchText, BulkReconViewModel Category)
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


            ViewModelApplication.PopupVisible = false;



        }





    }

}