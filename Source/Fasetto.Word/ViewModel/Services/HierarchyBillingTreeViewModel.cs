
using Dna;
using Fasetto.Word.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
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
    public class HierarchyBillingTreeViewModel : BaseViewModel

    {

        #region Public Properties

        /// <summary>
        /// A list of all registered hierarchy elements
        /// </summary>
        public ObservableCollection<HierarchyBillingViewModel> FirstGeneration { get; set; }


        //public ObservableCollection<HierarchyViewModel> FirstGeneration1 { get; set; }

        #endregion

        #region Data

        //private readonly ReadOnlyCollection<HierarchyViewModel> mFirstGeneration;
        protected HierarchyBillingViewModel mRootHierarchyElement;
        protected HierarchyBillingViewModel mRootHierarchyElement1;
        private readonly ICommand mSearchCommand;
        public HierarchyBillingListDataModel mHDML;
        public HierarchyBillingResultListApiModel mPersist, mPersistTmp,mOriginal;
        public HierarchyBillingDataModel mHDM;
        public string mBillingPeriod;
        public HierarchyElementViewModel mElement;

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
        public HierarchyBillingTreeViewModel(string hierarchyTable)
        {
            #region Dummy Root HierarchyBillingListDataModel
            //ViewModelApplication.CurrentPageViewModel = ViewModelApplication.CurrentPageViewModel;
            mHDML = new HierarchyBillingListDataModel();
            mHDM = new HierarchyBillingDataModel
            {
                KCategoryID = new Guid().ToString(),
                ParentCategoryID = "00000000-0000-0000-0000-000000000000",
                Description = "...Loading hierarchy data...",
                ShortName = "Loading...Please be patient",
                Children = new HierarchyBillingListDataModel()
            };
            mHDML.Add(mHDM);

            mBillingPeriod = hierarchyTable;
            #endregion
            //retrieve hierarchy from persistent storage on server
            //To Do: Add mTableName as parameter when calling HiearchyAsync to populate hierarchy
            //ViewModelApplication.CurrentPopupContent = PopupContent.AddElement;
            //ViewModelApplication.PopupVisible = false;
            TaskManager.RunAndForget(HierarchyAsync);


            // Get the OptFinHierarchies currently configured - first populate 'root hierarchy' variable with all configured root hierarchy elements currently available


            UpdateTreeViewElements();
            CloseCommand = new RelayCommand(Close);
            mSearchCommand = new SearchCategoryTreeCommand(this);
        }

        private void UpdateTreeViewElements()

        {

            var rootElement = mHDML.FirstOrDefault(x => x.ParentCategoryID == "00000000-0000-0000-0000-000000000000");
            mRootHierarchyElement = new HierarchyBillingViewModel(rootElement)
            {
                IsExpanded = true
            };

            FirstGeneration = new ObservableCollection<HierarchyBillingViewModel>(
                new HierarchyBillingViewModel[]
                {
                    mRootHierarchyElement
                });


        }



        #endregion // Constructor

        #region Properties
        #region Public Properties



        /// <summary>
        /// A flag indicating if the login command is running
        /// </summary>
        public bool HierarchyBuildIsRunning { get; set; }

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
        public ICommand SearchCommand => mSearchCommand;

        private class SearchCategoryTreeCommand : ICommand
        {
            private readonly HierarchyBillingTreeViewModel mCategoryTree;

            public SearchCategoryTreeCommand(HierarchyBillingTreeViewModel CategoryTree)
            {
                mCategoryTree = CategoryTree;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            event EventHandler ICommand.CanExecuteChanged
            {
                // I intentionally left these empty because
                // this command never raises the event, and
                // not using the WeakEvent pattern here can
                // cause memory leaks.  WeakEvent pattern is
                // not simple to implement, so why bother.
                add { }
                remove { }
            }

            public void Execute(object parameter)
            {
                mCategoryTree.PerformSearch();
            }
        }

        #endregion // SearchCommand

        #endregion //Properties

        /// <summary>
        /// Return Hierarchy of interest from Object persistance infrastructure
        /// User credentials are used to determine access authorisation
        /// </summary>
        /// <returns></returns>
        public async Task HierarchyAsync()
        {
            await RunCommandAsync(() => HierarchyBuildIsRunning, async () =>
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
                var result = await WebRequests.PostAsync<ApiResponse<HierarchyBillingResultListApiModel>>(
                // Set URL
                    RouteHelpers.GetAbsoluteRoute(ApiRoutes.ReturnSWBilling),
                    mBillingPeriod,
                    bearerToken: token);

                // If the response has an error...
                if (await result.HandleErrorIfFailedAsync("Hierarchy retrieval Failed"))
                    // We are done
                    return;

                // OK successfully registered (and logged in)... now get aprpropriate tree view data
                //for now; keep a snapshot of persisted data
                mOriginal = result.ServerResponse.Response;
                ;
               
                try
                {
                    //var hierarchyResultApiModels = mOriginal.ToList();
                    //make a clone of the persisted data for manipulation on front end
                    mPersist = new HierarchyBillingResultListApiModel();
                    mPersist.Clone(mOriginal, mPersist);
                }
                 catch (Exception e)
                {
                    throw e;
                }


                RefreshHierarchy();

                ViewModelApplication.CurrentControlViewModel = ViewModelApplication.CurrentControlViewModel;


            });
        }

        /// <summary>
        /// Method to refresh element Hierarchy
        /// -used when elements of the treefiew are being manipulated on the front end
        /// </summary>
        public void RefreshHierarchy()
        {

            mHDML.Clear();
            //build a tree view, always starting with the root element, which is also the classification for the hierarchy
            mHDML.AddRange(ExpandHierarchyData(mPersist, "00000000-0000-0000-0000-000000000000", "Root"));
            //Refresh the tree view title with the current name of the root element
            ControlTitle = (mPersist.Where(x => x.ParentCategoryID == "00000000-0000-0000-0000-000000000000").OrderByDescending(x => x.DateEffective).ToList().FirstOrDefault()).ShortName;

            //var matches = mPersist.OrderBy(x => x.DateEffective).ToList();
            //foreach (var category in matches)
            //{ category.ParentShortName = matches.First(x => x.ParentCategoryID == category.ParentCategoryID).ShortName; }
            //Update the viewModel with the returned values

            UpdateTreeViewElements();


            //}
        }



        /// <summary>
        /// This funtion builds a hierarchy of elements based on a
        /// a Hierarchy result returned when querying a database structure
        /// on which the hierarchy structures are persisted
        /// </summary>
        /// <param name="results"></param>
        /// This is a class of <HierarchyResultListApiModel></HierarchyResultListApiModel>
        /// <param name="KCategoryID"></param>
        /// the ID of the parent for finding descendants is passsed through as a string
        /// <returns></returns>
        private HierarchyBillingListDataModel ExpandHierarchyData(HierarchyBillingResultListApiModel results, string KCategoryID, string mParentShortName)
        {
            mPersist = results;
            // Find all children
            var TempDate = new DateTime(9999, 12, 31, 0, 0, 0);
            //var children = results.Where(x => x.ParentCategoryID == KCategoryID && x.DateEffective <= DateTime.Today && x.DateDiscontinued == new DateTime(9999,12,31,0,0,0) && !x.IsDeleteElement ).OrderBy(x=>x.ShortName).ToList();//
            //To do:  accept a date parameter to retroactively modify hierarchy data
            //var children = results.Where(x => x.ParentCategoryID == KCategoryID && x.DateEffective <= DateTime.Today && x.DateDiscontinued >  DateTime.Today && !x.IsDeleteElement).OrderBy(x => x.ShortName).ToList();//
            var children = results.Where(x => x.ParentCategoryID == KCategoryID && x.DateEffective <= DateTime.Today && x.DateDiscontinued > DateTime.Today ).OrderBy(x => (x.ShortName.ParseInt())).ThenBy(x => x.ShortName).ToList();//
            // Hierarchy cannot be expanded
            if (children.Count() == 0)
                return new HierarchyBillingListDataModel();
            //...otherwise, return all descendants recursively
            var elements = new HierarchyBillingListDataModel();
            foreach (var item in children)
            {
                var ud1 = new HierarchyBillingDataModel
                {

                    ShortName = item.ShortName,
                    Description = item.Description,
                    KCategoryID = item.KCategoryID,
                    ParentCategoryID = item.ParentCategoryID,
                    DateEffective = item.DateEffective,
                    DateDiscontinued = item.DateDiscontinued,
                    TotalConsumption = item.TotalConsumption,
                    WaterCost = item.WaterCost,
                    SewerCost = item.SewerCost,
                    TotalCost = item.TotalCost,
                    TimeStart = item.TimeStart,
                    Startreading = item.Startreading,
                    Endreading =item.Endreading,
                    TimeEnd = item.TimeEnd,
                    //TotalConsumption = item.TotalConsumption,
                    //KChangeID = item.KChangeID,
                    //IsUnderReview = item.IsUnderReview,
                    //Page = item.Page,
                    //Root = item.Root,
                    //IsMenuItem = item.IsMenuItem,


                    //To Do: make provision to add Icons to make the UI more intuitive and attractive
                    //FIconID = item.FIconID,
                    Children = new HierarchyBillingListDataModel()
                };
                ud1.Children = ExpandHierarchyData(results, ud1.KCategoryID, ud1.ShortName);
                elements.Add(ud1);
            }
            //var matches = elements.OrderBy(x => x.DateEffective).ToList();
            //foreach(var category in matches)
            //    { category.ParentShortName = matches.First(x=>x.ParentCategoryID == category.ParentCategoryID).ShortName; }

            return elements;
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

        public IEnumerator<HierarchyBillingViewModel> MatchingCategoryEnumerator { get; private set; }

        #endregion // SearchText

        //#endregion // Properties

        #region Search Logic -Short Name

        public void PerformSearch()
        {
            if (MatchingCategoryEnumerator == null || !MatchingCategoryEnumerator.MoveNext())
                VerifyMatchingCategoryEnumerator();

            var Category = MatchingCategoryEnumerator.Current;

            if (Category == null)
                return;

            // Ensure that this Category is in view.
            if (Category.mParent != null)
                Category.mParent.IsExpanded = true;

            Category.IsSelected = true;
            //Category.IsExpanded = false;
        }

        private void VerifyMatchingCategoryEnumerator()
        {
            var matches = FindMatches(mSearchText, mRootHierarchyElement);
            MatchingCategoryEnumerator = matches.GetEnumerator();

            if (!MatchingCategoryEnumerator.MoveNext())
            {
                MessageBox.Show(
                    "No matching names were found - please check your spelling.",
                    "Search for Tree Item failed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                    );
            }
        }

        private IEnumerable<HierarchyBillingViewModel> FindMatches(string searchText, HierarchyBillingViewModel Category)
        {
            if (Category.NameContainsText(searchText))
                yield return Category;

            foreach (var child in Category.Children)
                foreach (var match in FindMatches(searchText, child))
                    yield return match;
        }

        #endregion // Search Logic

        #region Search Logic //KCategoryID
        public IEnumerator<HierarchyBillingViewModel> MatchingKCategoryEnumerator { get; private set; }

        #endregion // SearchKCategoryID
        #region Search Logic //KCategoryID
        private void PerformKIdSearch()
        {

            if (MatchingKCategoryEnumerator == null || !MatchingKCategoryEnumerator.MoveNext())
                VerifyMatchingKCategoryEnumerator();
            var KCategory = MatchingKCategoryEnumerator.Current;
            if (KCategory == null)
                return;

            // Ensure that this Category is in view.
            if (KCategory.mParent != null)
                KCategory.mParent.IsExpanded = true;

            KCategory.IsSelected = true;
        }

        private void VerifyMatchingKCategoryEnumerator()
        {
            //var matchK = FindKMatches(mParentID, mTarget);
            var matchK = FindKMatches(mSearchText, mRootHierarchyElement);
            MatchingKCategoryEnumerator = matchK.GetEnumerator();
            _ = !MatchingKCategoryEnumerator.MoveNext();

        }

        private IEnumerable<HierarchyBillingViewModel> FindKMatches(string searchText, HierarchyBillingViewModel Category)
        {
            //var mSearchText = searchText;
            if (Category.KCategoryIdContainsText(searchText))
                yield return Category;

            foreach (var child in Category.Children)
                foreach (var matchK in FindKMatches(searchText, child))
                    yield return matchK;
        }

        #endregion //Search Logic //KCategoryID
    



        public void Close()
        {
            // Close settings menu
            ViewModelApplication.PopupVisible = false;
            TaskManager.RunAndForget(((HierarchyTreeViewModel)ViewModelApplication.CurrentSideMenuViewModel).HierarchyAsync);
            //ViewModelApplication.CurrentSideMenuViewModel = null;
            //TaskManager.RunAndForget(HierarchyAsync);
 
            //ViewModelApplication.GoToPage(ApplicationPage.Chat);


        }

        /// <summary>
        /// Persist all items changed or added on hierarchy to back end database. Depending on stage
        /// of change control, changes may be forwarded for recommendation or finally approved and implemented
        /// on back end
        /// </summary>
        public async Task PersistHierarchyChangesAsync()
        {
            await PersistHierarchyAsync();
            Close();
        }
        public async Task PersistHierarchyAsync()
        {
            await RunCommandAsync(() => HierarchyBuildIsRunning, async () =>
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
                var result = await WebRequests.PostAsync<ApiResponse<HierarchyResultListApiModel>>(
                // Set URL
                    RouteHelpers.GetAbsoluteRoute(ApiRoutes.PersistHierarchy),
                    mPersist,
                    bearerToken: token);

                // If the response has an error...
                if (await result.HandleErrorIfFailedAsync("Hierarchy retrieval Failed"))
                    // We are done
                    return;

                // return to menu


            });
        }



    }

}