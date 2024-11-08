using CsvHelper;
using Dna;
using Fasetto.Word.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using static Fasetto.Word.Core.CoreDI;
using static Fasetto.Word.DI;

namespace Fasetto.Word
{
    /// <summary>
    /// This is the view-model of the UI.  It provides a data source
    /// for the TreeView (the FirstGeneration property), a bindable
    /// SearchText property, and the SearchCommand to perform a search.
    /// </summary>
    public class BudgetTreeViewModel : BaseViewModel

    {

        #region Public Properties

        /// <summary>
        /// A set of Bulk Meter Recon records for the selected period
        /// </summary>
        public ObservableCollection<BudgetViewModel> FirstGeneration{ get; set; }


        public ObservableCollection<BudgetViewModel> FirstGeneration1 { get; set; }

        #endregion

        #region Data

        protected BudgetViewModel mRootHierarchyElement;
        public BudgetViewModel mRootHierarchyElement1;
        private readonly ICommand mSearchCommand;
        public BudgetListDataModel mBDDML;
        public BudgetResultListApiModel mPersist, mPersistTmp, mOriginal;
        public BudgetDataModel mBDDM;
        public string mBillingPeriod;
        public ParameterBudgetApiModel mBudgetParameter;
        public HierarchyElementViewModel mElement;
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

        public string mSearchText = "", mSearchKCategoryID = string.Empty, mParentCategoryID = string.Empty;

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
        public BudgetTreeViewModel()
        {

            //retrieve hierarchy from persistent storage on server
            //To Do: Add mTableName as parameter when calling HierarchyAsync to populate hierarchy
            //ViewModelApplication.CurrentPopupContent = PopupContent.AddElement;
            //ViewModelApplication.PopupVisible = false;
            mBDDML = new BudgetListDataModel();
            mBDDM = new BudgetDataModel
            {
                KCategoryID = new Guid().ToString(),
                ParentCategoryID = "00000000-0000-0000-0000-000000000000",
                //Description = "...Loading budget data...",
                ShortName = "Loading...Please be patient",
                Children = new BudgetListDataModel()
            };
            mBDDML.Add(mBDDM);

            // for now, allow the same hierarchy model to be used by the ExpenditureVSBudget page
            if (ViewModelApplication.CurrentPageViewModel.GetType().Name == "BudgetSelectionPageViewModel")
            { 

            mBudgetParameter = new ParameterBudgetApiModel
            {
                BudgetID = ((BudgetPeriodViewModel)((BudgetSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).SelectedBudget).KBudgetID,

                BMonth =  ((BudgetMonthViewModel)((BudgetSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).SelectedBudgetMonth).BudgetMonth,

                BudgetName = ((BudgetPeriodViewModel)((BudgetSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).SelectedBudget).Name,
            }; }
                
            else
            {

            mBudgetParameter = new ParameterBudgetApiModel
            {
                BudgetID = ((BudgetPeriodViewModel)((ExpenditureVSBudgetPageViewModel)ViewModelApplication.CurrentPageViewModel).SelectedBudget).KBudgetID,

                BMonth = ((BudgetMonthViewModel)((ExpenditureVSBudgetPageViewModel)ViewModelApplication.CurrentPageViewModel).SelectedBudgetMonth).BudgetMonth,

                BudgetName = ((BudgetPeriodViewModel)((ExpenditureVSBudgetPageViewModel)ViewModelApplication.CurrentPageViewModel).SelectedBudget).Name,

                IsExpenditureReturn = true,
            };
            }

            //mBillingParameter.BillingPeriodID = mFBillingPeriodID;
            TaskManager.RunAndForget(BudgetDetailAsync);


            // Get the OptFinHierarchies currently configured - first populate 'root hierarchy' variable with all configured root hierarchy elements currently available

            PriorPopupViewModel = ViewModelApplication.CurrentPopupViewModel;
            UpdateTreeViewElements();
            CloseCommand = new RelayCommand(Close);
            mSearchCommand = new SearchCategoryTreeCommand(this);
        }





        #endregion // Constructor

        #region Properties
        #region Public Properties



        /// <summary>
        /// A flag indicating if the login command is running
        /// </summary>
        public bool BudgetDetailBuildIsRunning { get; set; }

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
        /// <summary>
        /// Method to refresh View Model 
        /// </summary>
        private void UpdateTreeViewElements()

        {
            //FirstGeneration = new ObservableCollection<BudgetViewModel>();

            //var matches = mBDDML.Where(x => x.ParentCategoryID == "00000000-0000-0000-0000-000000000000").ToList();

            //foreach (var item in matches)
            //{

            //    var mTDVM = new BudgetViewModel

            //    {

            //        ShortName = item.ShortName,
            //        KCategoryID = item.KCategoryID,
            //        BudgetAmountTotal = item.BudgetAmountTotal,
            //        BudgetAmountDescendants = item.BudgetAmountDescendants,
            //        BudgetAmount = item.BudgetAmount,
            //        ParentCategoryID = item.ParentCategoryID,

            //    };
            //    FirstGeneration.Add(mTDVM);

            //};

            //foreach (var item1 in BudgetDetail)
            //{ item1.Volume = item1.ReadingEnd - item1.ReadingStart; }
            //PriorPopupViewModel = ViewModelApplication.CurrentPopupViewModel;

            var rootElement = mBDDML.FirstOrDefault(x => x.ParentCategoryID == "00000000-0000-0000-0000-000000000000");
            mRootHierarchyElement = new BudgetViewModel(rootElement)
            {
                IsExpanded = true
            };

            FirstGeneration = new ObservableCollection<BudgetViewModel>(
                new BudgetViewModel[]
                {
                    mRootHierarchyElement
                });
            var IsExpanded = true;


        }

        /// Return Bulk Recon detail information for the selected bulk meter
        /// User credentials are used to determine access authorisation
        /// </summary>
        /// <returns></returns>
        public async Task BudgetDetailAsync()

        {
            await RunCommandAsync(() => BudgetDetailBuildIsRunning, async () =>
            {

                // Store single transient instance of client data store
                var scopedClientDataStore = ClientDataStore;

                // Update values from local cache
                // Get the user token
                var token = (await scopedClientDataStore.GetLoginCredentialsAsync())?.Token;
                // Call the server and attempt to register with the provided credentials
                // If we don't have a token (then not logged in...)
                if (string.IsNullOrEmpty(token))
                    // Then do nothing more
                    return;
                var result = await WebRequests.PostAsync<ApiResponse<BudgetResultListApiModel>>(
                    // Set URL
                    RouteHelpers.GetAbsoluteRoute(ApiRoutes.ReturnBudgetDetail),
                    mBudgetParameter,
                    bearerToken: token);

                // If the response has an error...
                if (await result.HandleErrorIfFailedAsync("Budget retrieval Failed"))
                    // We are done
                    return;

                // OK successfully registered (and logged in)... now get appropriate tree view data
                //for now; keep a snapshot of persisted data
                mOriginal = result.ServerResponse.Response;
                mBDDML = new BudgetListDataModel();

                var matches = mOriginal.Where(x => x.KCategoryID == x.KCategoryID).ToList();

                foreach (var item in matches)
                {

                    var mBDDM = new BudgetDataModel

                    {
                        //Description = item.Description,
                        ShortName = item.ShortName,
                        KCategoryID = item.KCategoryID,
                        ParentCategoryID = item.ParentCategoryID,
                        ParentShortName = item.ParentShortName,
                        BudgetAmountDescendants = item.BudgetAmountDescendants,
                        BudgetAmount = item.BudgetAmount,
                        BudgetAmountTotal = item.BudgetAmountTotal,
                        ActualAmountDescendants = item.ActualAmountDescendants,
                        ActualAmount =item.ActualAmount,
                        ActualAmountTotal = item.ActualAmountTotal,
                        Deviation = item.Deviation,
                        DeviationCum = item.DeviationCum,
                        BudgetTotCum = item.BudgetTotCum,
                        ActualTotCum = item.ActualTotCum,
                        IsStockTracked = item.IsStockTracked,



                     };
                    mBDDML.Add(mBDDM);
                }

                ;

                try
                {
                    mPersist = new BudgetResultListApiModel();
                    mPersist.Clone(mOriginal, mPersist);

                }
                catch (Exception e)
                {
                    throw e;
                }


                RefreshHierarchy();
                PerformKIdSearch();
                //UpdateTreeViewElements();

                ViewModelApplication.CurrentControlViewModel = ViewModelApplication.CurrentControlViewModel;


            });
        }


        #region SearchCommand

        /// <summary>
        /// Returns the command used to execute a search in the Category tree.
        /// </summary>
        public ICommand SearchCommand => mSearchCommand;

        private class SearchCategoryTreeCommand : ICommand
        {
            private readonly BudgetTreeViewModel mCategoryTree;

            public SearchCategoryTreeCommand(BudgetTreeViewModel CategoryTree)
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
        /// Method to refresh element Hierarchy
        /// -used when elements of the treeview are being manipulated on the front end
        /// </summary>
        public void RefreshHierarchy()
        {

        mBDDML.Clear();
            //build a tree view, always starting with the root element, which is also the classification for the hierarchy
            mBDDML.AddRange(ExpandHierarchyData(mPersist, "00000000-0000-0000-0000-000000000000", "Root"));
            //Refresh the tree view title with the current name of the root element
            ControlTitle = (mPersist.Where(x => x.ParentCategoryID == "00000000-0000-0000-0000-000000000000").ToList().FirstOrDefault()).ShortName + " / Budget: " + mBudgetParameter.BudgetName;

            var matches = mPersist.ToList();
            foreach (var category in matches)
            { category.ParentShortName = matches.First(x => x.ParentCategoryID == category.ParentCategoryID).ShortName; }
            //Update the viewModel with the returned values

            UpdateTreeViewElements();
            PerformKIdSearch();


            //}
        }



        /// <summary>
        /// This function builds a hierarchy of elements based on a
        /// a Hierarchy result returned when querying a database structure
        /// on which the hierarchy structures are persisted
        /// </summary>
        /// <param name="results"></param>
        /// This is a class of <HierarchyResultListApiModel></HierarchyResultListApiModel>
        /// <param name="KCategoryID"></param>
        /// the ID of the parent for finding descendants is passsed through as a string
        /// <returns></returns>
        private BudgetListDataModel ExpandHierarchyData(BudgetResultListApiModel results, string KCategoryID, string mParentShortName)
        {
            //mPersist = results;
            // Find all children
            var TempDate = new DateTime(9999, 12, 31, 0, 0, 0);
            //var children = results.Where(x => x.ParentCategoryID == KCategoryID && x.DateEffective <= DateTime.Today && x.DateDiscontinued == new DateTime(9999,12,31,0,0,0) && !x.IsDeleteElement ).OrderBy(x=>x.ShortName).ToList();//
            //To do:  accept a date parameter to retroactively modify hierarchy data
            //var children = results.Where(x => x.ParentCategoryID == KCategoryID && x.DateEffective <= DateTime.Today && x.DateDiscontinued >  DateTime.Today && !x.IsDeleteElement).OrderBy(x => x.ShortName).ToList();//
            var children = results.Where(x => x.ParentCategoryID == KCategoryID ).OrderBy(x => (x.ShortName.ParseInt())).ThenBy(x => x.ShortName).ToList();//
            // Hierarchy cannot be expanded
            if (children.Count() == 0)
                return new BudgetListDataModel();
            //...otherwise, return all descendants recursively
            var elements = new BudgetListDataModel();
            foreach (var item in children)
            {
                var ud1 = new BudgetDataModel
                {
                    //Description = item.Description,
                    ShortName = item.ShortName,
                    KCategoryID = item.KCategoryID,
                    ParentCategoryID = item.ParentCategoryID,
                    ParentShortName = item.ParentShortName,
                    BudgetAmountDescendants = item.BudgetAmountTotal - item.BudgetAmount,
                    BudgetAmount = item.BudgetAmount,
                    BudgetAmountTotal = item.BudgetAmountTotal,
                    ActualAmountDescendants = item.ActualAmountDescendants,
                    ActualAmount = item.ActualAmount,
                    ActualAmountTotal = item.ActualAmountTotal,
                    Deviation = item.Deviation,
                    DeviationCum = item.DeviationCum,
                    BudgetTotCum = item.BudgetTotCum,
                    ActualTotCum = item.ActualTotCum,
                    IsStockTracked = item.IsStockTracked,

                    //To Do: make provision to add Icons to make the UI more intuitive and attractive
                    //FIconID = item.FIconID,
                    Children = new BudgetListDataModel()
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

        public IEnumerator<BudgetViewModel> MatchingCategoryEnumerator { get; private set; }

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

        private IEnumerable<BudgetViewModel> FindMatches(string searchText, BudgetViewModel Category)
        {
            if (Category.NameContainsText(searchText))
                yield return Category;

            foreach (var child in Category.Children)
                foreach (var match in FindMatches(searchText, child))
                    yield return match;
        }

        #endregion // Search Logic

        #region Search Logic //KCategoryID
        public IEnumerator<BudgetViewModel> MatchingKCategoryEnumerator { get; private set; }

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

        private IEnumerable<BudgetViewModel> FindKMatches(string searchText, BudgetViewModel Category)
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
            //Give control back to parent 'Popup view model'
            //var mType = "";
            //if (((BudgetTreeViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel!= null)
            //{ mType = ((BudgetTreeViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel.GetType().Name; }
            ////   ViewModelApplication.CurrentPopupViewModel = ((BudgetTreeViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel;
            //var mBudgetTreeViewModel = ViewModelApplication.CurrentPopupViewModel;
            //ViewModelApplication.PopupVisible = false;
            ////_ = mBillingParameter.PropertyID;

            //if (mType == "BudgetTreeViewModel")
            //{
            //    ViewModelApplication.CurrentPopupContent = PopupContent.AddElement;
            //    ViewModelApplication.CurrentPopupViewModel = mBudgetTreeViewModel;
            //    ViewModelApplication.CurrentPopupContent = PopupContent.BudgetReview;
            //}
            //else
            //{
            //ViewModelApplication.CurrentPopupViewModel = ((BudgetTreeViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel;
            ViewModelApplication.CurrentPopupContent = 0;
            ViewModelApplication.CurrentPopupViewModel = null;


            ViewModelApplication.PopupVisible = false;


        }

        /// <summary>
        /// Persist all items changed or added on hierarchy to back end database. Depending on stage
        /// of change control, changes may be forwarded for recommendation or finally approved and implemented
        /// on back end
        /// </summary>
        //public async Task PersistHierarchyChangesAsync()
        //{
        //    await PersistHierarchyAsync();
        //    Close();
        //}
        //public async Task PersistHierarchyAsync()
        //{
        //    await RunCommandAsync(() => BudgetBuildIsRunning, async () =>
        //    {

        //        // Store single transient instance of client data store
        //        var scopedClientDataStore = ClientDataStore;

        //        // Update values from local cache
        //        // Get the user token
        //        var token = (await scopedClientDataStore.GetLoginCredentialsAsync())?.Token;
        //        // Call the server and attempt to register with the provided credentials
        //        // If we don't have a token (then not logged in...)
        //        if (string.IsNullOrEmpty(token))
        //            // Then do nothing more
        //            return;
        //        var result = await WebRequests.PostAsync<ApiResponse<BudgetResultListApiModel>>(
        //        // Set URL
        //            RouteHelpers.GetAbsoluteRoute(ApiRoutes.PersistHierarchy),
        //            mPersist,
        //            bearerToken: token);

        //        // If the response has an error...
        //        if (await result.HandleErrorIfFailedAsync("Hierarchy retrieval Failed"))
        //            // We are done
        //            return;

        //        // return to menu


        //    });
        //}



    }

}