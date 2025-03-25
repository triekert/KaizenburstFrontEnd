
using Dna;
using Fasetto.Word.Core;
using Fasetto.Word.Core.ApiModels.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static Fasetto.Word.Core.CoreDI;
using static Fasetto.Word.DI;

namespace Fasetto.Word
{
    /// <summary>
    /// This is the view-model of the UI.  It provides a data source
    /// for the TreeView (the FirstGeneration property), a bindable
    /// SearchText property, and the SearchCommand to perform a search.
    /// </summary>
    public class HierarchyTreeViewModel : BaseViewModel

    {

        #region Public Properties

        /// <summary>
        /// A list of all registered hierarchy elements
        /// </summary>
        public ObservableCollection<HierarchyViewModel> FirstGeneration { get; set; }


        /// <summary>
        /// The HierarchyViewModel of the selected treeViewITem
        /// </summary>
        public HierarchyViewModel mSelectedTreeItem { get; set; }

        /// <summary>
        /// 
        /// Store View Model of current ControlViewModel to allow reverse navigation
        /// </summary>
        public object PriorPopupViewModel { get; set; }


        #endregion

        #region Data

        //private readonly ReadOnlyCollection<HierarchyViewModel> mFirstGeneration;
        protected HierarchyViewModel mRootHierarchyElement;
        protected HierarchyViewModel mRootHierarchyElement1;
        private readonly ICommand mSearchCommand;
        public HierarchyListDataModel mHDML;
        public HierarchyResultListApiModel mPersist, mPersistTmp,mOriginal;
        public HierarchyDataModel mHDM;
        public string mTableName;
        public HierarchyElementViewModel mElement;
        private Point mLastMouseDown;

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
        /// The command to process keyboard stroke in Menu Control
        /// </summary>
        public ICommand GestureHandlerCommand { get; set; }

        //public ActionCommand<DragEventArgs> DropCommand { get; private set; }

        #endregion//Public Commands



        #region Constructor
        /// <summary>
        /// The HierarchyTreeViewModel is a visual interface for interacting with hierarchical
        /// Structures persisted on the database linked to the application
        /// Generic hierarchy structures with parent-child relationships may be used to represent
        /// appropriate data sets
        /// </summary>
        /// <param name="hierarchyTable"></param>
        /// The hierarchyTable passed through as a paremeter identifies the specific hierarchy set to be retrieved
        /// from persistent s
        public HierarchyTreeViewModel(string hierarchyTable)
        {
            #region Dummy Root HierarchyListDataModel
            //ViewModelApplication.CurrentPageViewModel = ViewModelApplication.CurrentPageViewModel;
            mHDML = new HierarchyListDataModel();
            mHDM = new HierarchyDataModel
            {
                KCategoryID = new Guid().ToString(),
                ParentCategoryID = "00000000-0000-0000-0000-000000000000",
                Description = "...Loading hierarchy data...",
                ShortName = "Loading...Please be patient",
                Children = new HierarchyListDataModel()
            };
            mHDML.Add(mHDM);

            mTableName = hierarchyTable;
            #endregion
            //retrieve hierarchy from persistent storage on server
            //To Do: Add mTableName as parameter when calling HierarchyAsync to populate hierarchy
            //ViewModelApplication.CurrentPopupContent = PopupContent.AddElement;
            //ViewModelApplication.PopupVisible = false;
            if (ViewModelApplication.CurrentPopupViewModel != null && (ViewModelApplication.CurrentPopupViewModel).GetType().Name == "HierarchyTreeViewModel")
            {
                PriorPopupViewModel = ViewModelApplication.CurrentPopupViewModel;
            }
            TaskManager.RunAndForget(HierarchyAsync);


            // Get the OptFinHierarchies currently configured - first populate 'root hierarchy' variable with all configured root hierarchy elements currently available


            UpdateTreeViewElements();
            CloseCommand = new RelayCommand(Close);
            GestureHandlerCommand = new DelegateCommand<ContextualEventArgs>(GestureHandler);
            //GestureHandlerCommand= new RelayParameterizedCommand<ContextualEventArgs>(ExecuteItemModeSelectionChanged);
            mSearchCommand = new SearchCategoryTreeCommand(this);
        }

        private void UpdateTreeViewElements()

        {

            var rootElement = mHDML.FirstOrDefault(x => x.ParentCategoryID == "00000000-0000-0000-0000-000000000000");
            mRootHierarchyElement = new HierarchyViewModel(rootElement)
            {
                IsExpanded = true
            };

            FirstGeneration = new ObservableCollection<HierarchyViewModel>(
                new HierarchyViewModel[]
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

        //public HierarchyViewModel 
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
            private readonly HierarchyTreeViewModel mCategoryTree;

            public SearchCategoryTreeCommand(HierarchyTreeViewModel CategoryTree)
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
        /// Return Hierarchy of interest from Object persistence infrastructure
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
                var result = await WebRequests.PostAsync<ApiResponse<HierarchyResultListApiModel>>(
                // Set URL
                    RouteHelpers.GetAbsoluteRoute(ApiRoutes.ReturnHierarchy),
                    mTableName,
                    bearerToken: token);

                // If the response has an error...
                if (await result.HandleErrorIfFailedAsync("Hierarchy retrieval Failed"))
                    // We are done
                    return;

                // OK successfully registered (and logged in)... now get appropriate tree view data
                //for now; keep a snapshot of persisted data
                mOriginal = result.ServerResponse.Response;
                ;
               
                //if ()
                try
                {
                    //var hierarchyResultApiModels = mOriginal.ToList();
                    //make a clone of the persisted data for manipulation on front end
                    mPersist = new HierarchyResultListApiModel();
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

            var matches = mPersist.OrderBy(x => x.DateEffective).ToList();
            foreach (var category in matches)
            { category.ParentShortName = matches.First(x => x.ParentCategoryID == category.ParentCategoryID).ShortName; }
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
        private HierarchyListDataModel ExpandHierarchyData(HierarchyResultListApiModel results, string KCategoryID, string mParentShortName)
        {
            mPersist = results;
            // Find all children
            var TempDate = new DateTime(9999, 12, 31, 0, 0, 0);
            //var children = results.Where(x => x.ParentCategoryID == KCategoryID && x.DateEffective <= DateTime.Today && x.DateDiscontinued == new DateTime(9999,12,31,0,0,0) && !x.IsDeleteElement ).OrderBy(x=>x.ShortName).ToList();//
            //To do:  accept a date parameter to retroactively modify hierarchy data
            //var children = results.Where(x => x.ParentCategoryID == KCategoryID && x.DateEffective <= DateTime.Today && x.DateDiscontinued >  DateTime.Today && !x.IsDeleteElement).OrderBy(x => x.ShortName).ToList();//
            var children = results.Where(x => x.ParentCategoryID == KCategoryID && x.DateEffective <= DateTime.Now && x.DateDiscontinued > DateTime.Now && !x.IsDeleteElement).OrderBy(x => (x.ShortName.ParseInt())).ThenBy(x => x.ShortName).ToList();//
            // Hierarchy cannot be expanded
            if (children.Count() == 0)
                return new HierarchyListDataModel();
            //...otherwise, return all descendants recursively
            var elements = new HierarchyListDataModel();
            foreach (var item in children)
            {
                var ud1 = new HierarchyDataModel
                {
                    //var u = hierarchyDataModel;
                    ShortName = item.ShortName,
                    Description = item.Description,
                    //Card = item.Card,
                    //Frequency = item.Frequency,
                    KCategoryID = item.KCategoryID,
                    ParentCategoryID = item.ParentCategoryID,
                    ParentShortName = mParentShortName,
                    DateEffective = item.DateEffective,
                    DateDiscontinued = item.DateDiscontinued,
                    KChangeID = item.KChangeID,
                    IsUnderReview = item.IsUnderReview,
                    Page = item.Page,
                    Root = item.Root,
                    IsMenuItem = item.IsMenuItem,
                    HierarchyType = item.HierarchyType,
                    HierarchyTypeID = item.HierarchyTypeID,
                    FHierarchyID = item.FHierarchyID,
                    FClientID = item.FClientID,


                    //To Do: make provision to add Icons to make the UI more intuitive and attractive
                    //FIconID = item.FIconID,
                    Children = new HierarchyListDataModel()
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

        public IEnumerator<HierarchyViewModel> MatchingCategoryEnumerator { get; private set; }

        #endregion // SearchText

        //#endregion // Properties

        #region Search Logic -Short Name

        public void PerformSearch()
        {
            var isGuid = Guid.TryParse(mSearchText, out _);
            if (isGuid)
            { PerformKIdSearch(); }
            else
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
            }

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

        private IEnumerable<HierarchyViewModel> FindMatches(string searchText, HierarchyViewModel Category)
        {
            if (Category.NameContainsText(searchText))
                yield return Category;

            foreach (var child in Category.Children)
                foreach (var match in FindMatches(searchText, child))
                    yield return match;
        }

        #endregion // Search Logic

        #region Search Logic //KCategoryID
        public IEnumerator<HierarchyViewModel> MatchingKCategoryEnumerator { get; private set; }

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

        private IEnumerable<HierarchyViewModel> FindKMatches(string searchText, HierarchyViewModel Category)
        {
            //var mSearchText = searchText;
            if (Category.KCategoryIdContainsText(searchText))
                yield return Category;

            foreach (var child in Category.Children)
                foreach (var matchK in FindKMatches(searchText, child))
                    yield return matchK;
        }

        #endregion //Search Logic //KCategoryID
        #region Tree Manipulation
        /// <summary>
        /// Move element from one parent to another
        /// The calling programme is to ensure that no loops 
        /// are present where an element becomes its owndescendent
        /// </summary>
        /// <param name="mCategoryKId"></param>
        /// <param name="mParentKId"></param>
        public void MoveElement(HierarchyElementViewModel element)
        {
            mSearchText = element.KCategoryID;
            mParentCategoryID = element.ParentCategoryID;
            mPersistTmp = new HierarchyResultListApiModel();
            //var sourceElement = from HierarchyDataModel in this
            //                    where KCategoryID
            var matches = mPersist.Where(x => x.KCategoryID == element.KCategoryID && x.DateEffective <= element.DateDiscontinued).OrderByDescending(x => x.DateEffective).ToList();
            var category = matches.FirstOrDefault();

            //

            if (category != null)
            //Move the selected element by discontinuing it at the current 'Parent'
            //and adding a new element at the new parent location (all descendants are automatically
            //moved)
          
            {  
                if (category.DateEffective == element.DateEffective)
                {
                    //if the change is made to an element not yet committed
                    //remove changes made up to this point...
                    if (category.DateDiscontinued == element.DateDiscontinued)
                    {
                    category.IsUnderReview = true;
                        category.ShortName = (element.ShortName.EditedText ?? element.ShortName.OriginalText);
                        category.Description = (element.Description.EditedText ?? element.Description.OriginalText);
                        //category.FIconID = element.FIconID;
                        category.ParentCategoryID = mParentCategoryID;
                    }
                    else
                    { 
                    var mPersistElement = new HierarchyResultApiModel
                        {
                        ShortName = (element.ShortName.EditedText ?? element.ShortName.OriginalText),
                        Description = (element.Description.EditedText ?? element.Description.OriginalText),
                        //FIconID = category.FIconID,
                        //Frequency = category.Frequency,
                        //FinHierarchyID = category.FinHierarchyID,
                        ParentCategoryID = mParentCategoryID,
                        DateEffective = element.DateEffective,
                        DateDiscontinued=element.DateDiscontinued,
                        KCategoryID = element.KCategoryID,
                        KChangeID = element.KChangeID,
                        IsUnderReview = true,
                        IsNewElement = true,
                        };
                    mPersist.Add(mPersistElement);
                    }



                }
                else
                {
                //Doiscontinue the element in the current location
                category.KChangeID = element.KChangeID;
                category.DateDiscontinued = element.DateEffective.AddSeconds(-10);
                category.IsUnderReview = true;
 
                    var mPersistElement = new HierarchyResultApiModel
                    {
                    ShortName = (element.ShortName.EditedText ?? element.ShortName.OriginalText),
                    Description = (element.Description.EditedText ?? element.Description.OriginalText),
                    FIconID = category.FIconID,
                    Frequency = category.Frequency,
                    FHierarchyID = category.FHierarchyID,
                    ParentCategoryID = mParentCategoryID,
                    DateEffective = element.DateEffective,
                    DateDiscontinued = element.DateDiscontinued,
                    KCategoryID = element.KCategoryID,
                    KChangeID = element.KChangeID,
                    IsUnderReview = true,
                    IsNewElement = true,
                    };
                    mPersist.Add(mPersistElement);
                    //terminate the previous position of the element, and link to the change control

                }
                RefreshHierarchy();
                PerformKIdSearch();

            return;

            }
            return;

        }


        /// <summary>
        /// Copy Hierarchy Element from one location to another (allocate to a different parent Element)
        /// Simultaneously, copies must be made of all descendents and these 2 must be inserted as descendents
        /// of the newly copied apex element
        /// </summary>
        /// <param name="mCategoryKId"></param>
        /// <param name="mParentKId"></param>
        public void CopyElement(HierarchyElementViewModel element)
        {
            try
            {
                //mSearchText = mCategoryKId;
                mParentCategoryID = element.ParentCategoryID;
                mPersistTmp = new HierarchyResultListApiModel();
                mElement = element;
                //mSearchText = element.KCategoryID;

                //var sourceElement = from HierarchyDataModel in this
                //                    where KCategoryID
                var matches = from category in mPersist
                              where category.KCategoryID == element.KCategoryID && category.DateDiscontinued == new DateTime(9999,12,31) && category.DateEffective <= DateTime.Today

                              select category;
                foreach (var category in matches)


                {
                    //mSearchText = category.ShortName;
                    var mPersistElement = new HierarchyResultApiModel
                    {
                        ShortName = (element.ShortName.EditedText ?? element.ShortName.OriginalText),
                        Description = (element.Description.EditedText ?? element.Description.OriginalText),
                        FIconID = category.FIconID,
                        Frequency = category.Frequency,
                        FHierarchyID = category.FHierarchyID,
                        ParentCategoryID = mParentCategoryID,
                        DateEffective = mElement.DateEffective,
                        DateDiscontinued = new DateTime(9999, 12, 31),
                        KCategoryID = Guid.NewGuid().ToString().ToUpper(),
                        KChangeID = mElement.KChangeID,
                        IsUnderReview = true,
                        IsNewElement = true,
                    };
                    mPersistTmp.Add(mPersistElement);
                    //category.DateDiscontinued = mElement.DateDiscontinued;
                    //category.KChangeID = mElement.KChangeID;
                    mSearchText = mPersistElement.KCategoryID;
                    CopyElement1(element.KCategoryID, mPersistElement.KCategoryID);
                }

                mPersist.AddRange(mPersistTmp);
                RefreshHierarchy();
                PerformKIdSearch();
            }
            catch (Exception)
            {
            }


        }
        /// <summary>
        /// Iterate through all descendants of the primary element being copied
        /// and copy and move them to the new Parent structure
        /// </summary>
        /// <param name="mCategoryKId"></param>
        /// <param name="mParentKId"></param>
        public void CopyElement1(string mCategoryKId, string mParentKId)
        {
            try
            {

                //var sourceElement = from HierarchyDataModel in this
                //                    where KCategoryID
                var matches = from category in mPersist
                              where category.ParentCategoryID == mCategoryKId && category.DateDiscontinued == new DateTime(9999,12,31) && category.DateEffective <= (DateTime.Today)
                              select category;
                foreach (var category in matches)


                {
                    var mPersistElement = new HierarchyResultApiModel
                    {
                        FIconID = category.FIconID,
                        Frequency = category.Frequency,
                        FHierarchyID = category.FHierarchyID,
                        ParentCategoryID = mParentKId,
                        KCategoryID = Guid.NewGuid().ToString().ToUpper(),
                        ShortName = category.ShortName,
                        Description = category.Description,
                        DateEffective = mElement.DateEffective,
                        DateDiscontinued = new DateTime(9999, 12, 31),
                        KChangeID = mElement.KChangeID,
                        IsUnderReview = true,
                        IsNewElement = true,

                    };
                    mPersistTmp.Add(mPersistElement);
                    //category.DateDiscontinued = mElement.DateDiscontinued;
                    //category.KChangeID = mElement.KChangeID;
                    CopyElement1(category.KCategoryID, mPersistElement.KCategoryID);

                }
            }
            catch (Exception)
            {
            }


        }

        /// <summary>
        /// Discontinue the selected element with all its descendants

        /// </summary>
        /// <param name="mCategoryKId"></param>
        /// <param name="mParentKId"></param>
        public void DeleteElement(HierarchyElementViewModel element)
        {
            try
            {
                //mSearchText = mCategoryKId;
                var mElement = element as HierarchyElementViewModel;


                //              select category;

 
                var category = (mPersist.Where(x => x.KCategoryID == element.KCategoryID && x.DateEffective == element.DateEffective).OrderByDescending(x => x.DateEffective).ToList()).FirstOrDefault();
                var catprev = (mPersist.Where(x => x.KCategoryID == category.KCategoryID && x.DateDiscontinued < category.DateDiscontinued).OrderByDescending(x => x.DateEffective).ToList()).FirstOrDefault();
                //foreach (var category in matches)


                //var category = categoryset.FirstOrDefault();
                //if the category is currently under review and already reflected on back end then remove from backend
                if (category.IsUnderReview)
                //If element has not yet been persisted, just remove from the data set
                {
                    //if the node being discontinued was linked to another node during the same transaction
                    //restore the other node to previous condition
                    if (catprev != null)
                    {
                        catprev.DateDiscontinued = category.DateDiscontinued;
                        mSearchText = catprev.KCategoryID;
                    }
                    if (category.IsNewElement)
                    {
                        //If changes have not yet been persisted on database, remove the relevant nodes from the front end
                        //var catNode = mPersist.Where(x => x.KCategoryID == element.KCategoryID && x.DateEffective == element.DateEffective);
                        mPersistTmp = new HierarchyResultListApiModel();
                        foreach (var catno in mPersist.Where(x => x.KCategoryID == element.KCategoryID && x.DateEffective == element.DateEffective))
                            mPersistTmp.Add(catno);
                        mPersist.Remove(mPersistTmp, mPersist);
                        if (catprev == null)

                            DeleteElement1(element.KCategoryID,mElement);
                    }
                    else
                    {
                        category.IsDeleteElement = true;
                        if (catprev == null)
                            DeleteElement1(element.KCategoryID,mElement);
                    }
                }
                //    //mSearchText = category.ShortName;
                //if item is already under review, delete the changed stuff
                else 
                { 
                category.DateDiscontinued = mElement.DateDiscontinued;
                mSearchText = mElement.KCategoryID;
                category.IsUnderReview = true;
                DeleteElement1(mElement.KCategoryID,mElement);
                }

                //}


                RefreshHierarchy();
                PerformKIdSearch();
            }
            catch (Exception)
            {
            }


        }
        /// <summary>
        /// Iterate through all descendants of the primary element being copied
        /// and copy and move them to the new Parent structure
        /// </summary>
        /// <param name="mCategoryKId"></param>
        /// <param name="mParentKId"></param>
        public void DeleteElement1(string mCategoryKId, HierarchyElementViewModel element)
        {
            var mElement = element as HierarchyElementViewModel;
            var catprev = (mPersist.Where(x => x.KCategoryID == element.KCategoryID && x.DateEffective == element.DateEffective).OrderByDescending(x => x.DateEffective).ToList()).FirstOrDefault();
            try
            {

                //var sourceElement = from HierarchyDataModel in this
                //                    where KCategoryID

                var matches = from category in mPersist
                              where category.ParentCategoryID == mCategoryKId
                              select category;
                foreach (var category in matches)


                {
                    catprev = (mPersist.Where(x => x.KCategoryID == category.KCategoryID && x.DateDiscontinued < category.DateDiscontinued).OrderByDescending(x => x.DateEffective).ToList()).FirstOrDefault();
                    if (catprev != null)
                    {
                        catprev.DateDiscontinued = category.DateDiscontinued;
                        mSearchText = catprev.KCategoryID;
                    }
                    //foreach (var catprev)
                    if (category.IsUnderReview)
                //If element has not yet been persisted, just remove from the data set
                    { 

                        if (category.IsNewElement)
                        {
                        //If changes have not yet been persisted on database, remove the relevant nodes from the front end
                             //var catNode = mPersist.Where(x => x.KCategoryID == element.KCategoryID && x.DateEffective == element.DateEffective);
                            mPersistTmp = new HierarchyResultListApiModel();
                            foreach(var catno in mPersist.Where(x => x.KCategoryID == category.KCategoryID && x.DateEffective == category.DateEffective))
                            mPersistTmp.Add(catno);
                            mPersist.Remove(mPersistTmp, mPersist);
                            if (catprev == null)
                                DeleteElement1(category.KCategoryID,mElement);
                        }
                        else
                          category.IsDeleteElement = true;
                            if (catprev == null)
                            DeleteElement1(category.KCategoryID, mElement);

                    }
                    category.DateDiscontinued = mElement.DateDiscontinued;
                    category.IsUnderReview = true;
                    DeleteElement1(category.KCategoryID,mElement);

                    //}



                }
            }
            catch (Exception)
            {
            }


        }

        /// <summary>
        /// Add a new element to the Hierarchy Tree
        /// The calling programme is to generate a GUID for the new element
        /// </summary>
        /// <param name="mNewElement"></param>
        public void AddElement(HierarchyElementViewModel element)
        {
            mSearchText = element.KCategoryID;
            //Gemerate GUID for root of new hierarchy element
            var mRoot = element.Root.EditedText == element.Root.OriginalText ? Guid.NewGuid().ToString().ToUpper() : element.Root.EditedText;
            var mPersistElement = new HierarchyResultApiModel
            {
                ShortName = (element.ShortName.EditedText ?? element.ShortName.OriginalText),
                Description = (element.Description.EditedText ?? element.Description.OriginalText),
                ParentCategoryID = element.ParentCategoryID,
                DateEffective = element.DateEffective.AddSeconds(-10),
                DateDiscontinued = element.DateDiscontinued,
                KCategoryID = element.KCategoryID,
                KChangeID = element.KChangeID,
                IsUnderReview = true,
                IsNewElement= true,
                Page = element.Page,
                FHierarchyID = element.FHierarchyID,
                FClientID = element.FClientID,
                HierarchyTypeID = element.HierarchyTypeID,


                //Create new root element if not already existing
                Root = mRoot
            };
            mPersist.Add(mPersistElement);


            RefreshHierarchy();
            PerformKIdSearch();
            //TO DO: Add code to create root element of hierarchy when creating a new hierarchy type menu item
            //if page == 'Hierarchy', create new element guid(), use hierarchy name +description, parent = 00000000

        }

        /// <summary>
        /// Edit element in hiearchy tree

        /// </summary>
        /// <param name="element"></param>
        public void EditElement(HierarchyElementViewModel element)
        {
            mSearchText = element.KCategoryID;

            var matches = from category in mPersist
                          where category.KCategoryID == element.KCategoryID && category.DateDiscontinued == new DateTime(9999,12,31)// && (category.DateEffective <= element.DateDiscontinued)
                          select category;
             foreach(var category in matches)
                //if this is a newly added element, just update the instance
                if (category.ShortName != (element.ShortName.EditedText ?? element.ShortName.OriginalText) || category.Description != (element.Description.EditedText ?? element.Description.OriginalText)
                        || category.DateEffective != element.DateEffective||category.Page != element.Page|| category.Root != (element.Root.EditedText ?? element.Description.OriginalText) || category.HierarchyTypeID != element.HierarchyTypeID)
                { 
                    if (category.DateEffective == element.DateEffective)
                    category.KChangeID = element.KChangeID;
                    category.IsUnderReview = true;
                    category.ShortName = element.ShortName.EditedText ?? element.ShortName.OriginalText;
                    category.Description = element.Description.EditedText ?? element.Description.OriginalText;
                    category.Page = element.Page;
                    category.Root = element.Root.EditedText ?? element.Root.OriginalText;
                    category.HierarchyTypeID = element.HierarchyTypeID;
                    category.HierarchyType = element.HierarchyType;
                    category.DateEffective = element.DateEffective;
                    category.DateDiscontinued = element.DateDiscontinued;
                    mSearchText = category.KCategoryID;
                }
                else
                if (!(category.ShortName == (element.ShortName.EditedText ?? element.ShortName.OriginalText) && category.Description == (element.Description.EditedText ?? element.Description.OriginalText)
                       && category.DateEffective == element.DateEffective && category.Page == element.Page && category.Root == (element.Root.EditedText ?? element.Description.OriginalText)))
                {
                    category.DateDiscontinued =  element.DateEffective.AddSeconds(-10);
                    category.KChangeID = element.KChangeID;
                    category.IsUnderReview = true;

                    var mPersistElement = new HierarchyResultApiModel
                    {
                        ShortName = (element.ShortName.EditedText ?? element.ShortName.OriginalText),
                        Description = (element.Description.EditedText ?? element.Description.OriginalText),
                        ParentCategoryID = element.ParentCategoryID,
                        DateDiscontinued = element.DateDiscontinued,
                        DateEffective = element.DateEffective,
                        Page = element.Page,
                        Root = (element.Root.EditedText ?? element.Root.OriginalText),
                    //Unique ID for change element
                        KCategoryID = element.KCategoryID,
                        IsUnderReview = true,
                        IsNewElement = true,
                        FHierarchyID = category.FHierarchyID,
                        KChangeID = element.KChangeID,
                        HierarchyTypeID = element.HierarchyTypeID,
                        HierarchyType = element.HierarchyType,
                    };
                    mPersist.Add(mPersistElement);

                   mSearchText = mPersistElement.KCategoryID;
                }
 
                    RefreshHierarchy();
                    PerformKIdSearch();
                    return;


        }

        #endregion //Tree Manipulation

        public void Close()
        {
            PersistHierarchyChangesAsync();
            // Close settings menu
            if (mTableName == "2D7E4A7D-6F19-496E-8709-47E6A9ADDFA0")
            {
                ViewModelApplication.SideMenuVisible = true;
            }
            if (ViewModelApplication.CurrentPopupContent == null)
            {
                TaskManager.RunAndForget(((HierarchyTreeViewModel)ViewModelApplication.CurrentSideMenuViewModel).HierarchyAsync);
                //ViewModelApplication.CurrentSideMenuViewModel = null;
                //TaskManager.RunAndForget(HierarchyAsync);

                ViewModelApplication.GoToPage(ApplicationPage.Chat);
            }
            else
            { 
                if (ViewModelApplication.CurrentPopupViewModel != null && ViewModelApplication.CurrentPopupViewModel.GetType().Name =="HierarchyTreeViewModel" && ((HierarchyTreeViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel != null)
                {
                    ViewModelApplication.CurrentPopupViewModel = ((HierarchyTreeViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel; 
                    ViewModelApplication.CurrentPopupContent = 0;
                    ViewModelApplication.CurrentPopupContent = PopupContent.Hierarchy;

                }
                else 
                {
                    ViewModelApplication.CurrentPopupViewModel = null;
                    ViewModelApplication.PopupVisible = false; 
                }
            }


            //// Close settings menu
            //ViewModelApplication.SideMenuVisible = true;
            ////TaskManager.RunAndForget(((HierarchyTreeViewModel)ViewModelApplication.CurrentSideMenuViewModel).HierarchyAsync);
            ////ViewModelApplication.CurrentSideMenuViewModel = null;
            ////TaskManager.RunAndForget(HierarchyAsync);

            ViewModelApplication.GoToPage(ApplicationPage.Chat);

        }



        //Interpret Keyboard Gestures
        public void GestureHandler(object parameter)
        {
            var tmp = ((ContextualEventArgs)parameter).OriginalEventArgs;
            var eventTmp = tmp.GetType().Name;
            if (eventTmp == "MouseEventArgs" && ((MouseEventArgs)tmp).RoutedEvent.Name=="PreviewMouseMove")
            {
                var TmpTmp = ((MouseEventArgs)tmp).OriginalSource as UIElement;
                //try
                //{
                //    var item = GetNearestContainer(((MouseEventArgs)tmp).OriginalSource as UIElement);
                //    //mDraggedItemTest = (HierarchyViewModel)item.Header;
                //    if (((MouseEventArgs)tmp).LeftButton == MouseButtonState.Pressed)
                //    {
                //        var isCtrl = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
                //        var currentPosition = ((MouseEventArgs)tmp).GetPosition(item);

                //        //Check for dragging of treeview item
                //        if ((Math.Abs(currentPosition.X - mLastMouseDown.X) > 10.0) ||
                //            (Math.Abs(currentPosition.Y - mLastMouseDown.Y) > 10.0))
                //        {

                //            var mDraggedItem = (HierarchyViewModel)tvParameters.SelectedItem;
                //            mLastMouseDown = currentPosition;
                //            //mSourceCategoryName = mDraggedItem.ShortName;
                //            //draggedItem = (TreeViewItem)tvParameters.SelectedItem;
                //            //mSource = (TreeViewItem)tvParameters.SelectedItem;
                //            if (mDraggedItem != null)
                //            {
                //                //mTarget = null;//ensure target is reset
                //                if (!isCtrl)
                //                {

                //                    var finalDropEffect = DragDrop.DoDragDrop(tvParameters, tvParameters.SelectedValue,
                //                      DragDropEffects.Move);
                //                    //Checking target is not null and item is dragging(moving)
                //                    if ((finalDropEffect == DragDropEffects.Move) && (mTarget != null))
                //                    {
                //                        // A Move drop was accepted
                //                        //if (!mSource.Header.ToString().Equals(mTargetT.Header.ToString()))
                //                        //{
                //                        MoveHierarchyElement();// MoveItem();
                //                        mTargetT = null;
                //                        mSource = null;
                //                        //}

                //                    }
                //                }
                //                else
                //                {
                //                    var finalDropEffect = DragDrop.DoDragDrop(tvParameters, tvParameters.SelectedValue,
                //                      DragDropEffects.Copy);
                //                    if ((finalDropEffect == DragDropEffects.Copy) && (mTarget != null))
                //                    {
                //                        // A Copy drop was accepted
                //                        //if (!mSource.Header.ToString().Equals(mTargetT.Header.ToString()))
                //                        //{
                //                        CopyHierarchyElement();// CopyItem();
                //                        mTargetT = null;
                //                        mSource = null;
                //                        //}

                //                    }
                //                }



                //            }
                //        }
                //    }

                //}
                //catch (Exception)
                //{
                //}



                ((MouseEventArgs)tmp).Handled = true;
            }
            else
            {
                var tmp1 = ((ContextualEventArgs)parameter).Context.GetType().Name;


                if (ViewModelApplication.SideMenuVisible && ViewModelApplication.CurrentPopupViewModel == null)
                //if (mTableName == "2D7E4A7D-6F19-496E-8709-47E6A9ADDFA0")

                {
                    //if Gesture handler is triggered from Text Search Box...               
                    if (tmp1 == "String")
                    {
                        SearchText = SearchText;
                        if (((KeyEventArgs)tmp).Key == Key.Enter)
                        //((KeyEventArgs)tmp).Handled = true;
                        { SearchCommand.Execute(null); }
                    }
                    else
                    {

                        mSelectedTreeItem = (HierarchyViewModel)(((ContextualEventArgs)parameter).Context);
                        //ViewModelApplication.SideMenuVisible = true;

                        if (eventTmp == "MouseButtonEventArgs")
                        {
                            if ((((MouseButtonEventArgs)tmp).RightButton == MouseButtonState.Pressed) || (((MouseButtonEventArgs)tmp).LeftButton == MouseButtonState.Pressed))
                            {
                                ((MouseButtonEventArgs)tmp).Handled = true;
                                RunSelectedMenu();
                            }
                        }
                        else
                        if (eventTmp == "KeyEventArgs")
                        {
                            if ((((KeyEventArgs)tmp).Key == Key.Enter) || (((KeyEventArgs)tmp).Key == Key.Insert) || (((KeyEventArgs)tmp).Key == Key.Delete))
                            {
                                ((KeyEventArgs)tmp).Handled = true;
                                RunSelectedMenu();
                            }
                            ((KeyEventArgs)tmp).Handled = true;
                        }
                        //}
                    }
                }
                else
                //enable editing of hierarchy menu structure
                //if Gesture handler is triggered from Text Search Box...
                //

                {

                    if (tmp1 == "String")
                    {
                        SearchText = SearchText;
                        if (((KeyEventArgs)tmp).Key == Key.Enter)
                        //((KeyEventArgs)tmp).Handled = true;
                        { SearchCommand.Execute(null); }
                    }
                    else
                    {
                        mSelectedTreeItem = (HierarchyViewModel)(((ContextualEventArgs)parameter).Context);
                        //ViewModelApplication.SideMenuVisible = true;
                        if (eventTmp == "MouseButtonEventArgs")
                        {
                            if ((((MouseButtonEventArgs)tmp).RightButton == MouseButtonState.Pressed) || (((MouseButtonEventArgs)tmp).LeftButton == MouseButtonState.Pressed))
                            {
                                ((MouseButtonEventArgs)tmp).Handled = true;
                                EditHierarchyElement(mSelectedTreeItem);
                            }
                        }
                        else
                        if (eventTmp == "KeyEventArgs")

                        //Edit element
                        {
                            if (((KeyEventArgs)tmp).Key == Key.Enter)
                            {
                                ((KeyEventArgs)tmp).Handled = true;
                                EditHierarchyElement(mSelectedTreeItem);
                            }
                            else
                                if (((KeyEventArgs)tmp).Key == Key.Insert)
                            {
                                ((KeyEventArgs)tmp).Handled = true;
                                AddHierarchyElement(mSelectedTreeItem);
                            }
                            else
                                    if (((KeyEventArgs)tmp).Key == Key.Delete)
                            {
                                ((KeyEventArgs)tmp).Handled = true;
                                DeleteHierarchyElement(mSelectedTreeItem);
                            }
                            else
                                    if (((KeyEventArgs)tmp).Key == Key.F2)
                            {
                                ((KeyEventArgs)tmp).Handled = true;
                                NavigateElement(mSelectedTreeItem);
                            }
                        }
                        //((KeyEventArgs)tmp).Handled = true;
                    }

                }
            }
        }

        private TreeViewItem GetNearestContainer(UIElement element)
        {
            // Walk up the element tree to the nearest tree view item.
            var container = element as TreeViewItem;
            while ((container == null) && (element != null))
            {
                element = VisualTreeHelper.GetParent(element) as UIElement;
                container = element as TreeViewItem;
            }
            return container;
        }

        /// Use Popup View to add a Hierarchy Element
        /// </summary>
        private void RunSelectedMenu()
        {
            //Prepopulate
            //Only allow one execution of  the function per event
            //if (!ViewModelApplication.SideMenuVisible)
            //    return;
            if (mSelectedTreeItem == null || ((string)mSelectedTreeItem.Page).Length == 0) //|| mSelectedTreeItem.Children.Count > 0
                return;
            if (mSelectedTreeItem.Page == "Hierarchy")
            {
                var HierarchyParam = new ParameterHierarchyItemSelectApiModel
                {
                    FHierarchyID = mSelectedTreeItem.Root,
                };
                ViewModelApplication.CurrentPopupViewModel = new HierarchyTreeViewModel((string)mSelectedTreeItem.Root);//root);
                ViewModelApplication.CurrentPopupContent = 0;
                ViewModelApplication.CurrentPopupContent = PopupContent.Hierarchy;
                ViewModelApplication.PopupVisible = true;
            }
            else
            { ViewModelApplication.OpenMenu(mSelectedTreeItem.Root, mSelectedTreeItem.Page); }


        //// Close settings menu
        //ViewModelApplication.SideMenuVisible = true;
        ////TaskManager.RunAndForget(((HierarchyTreeViewModel)ViewModelApplication.CurrentSideMenuViewModel).HierarchyAsync);
        ////ViewModelApplication.CurrentSideMenuViewModel = null;
        ////TaskManager.RunAndForget(HierarchyAsync);

        //ViewModelApplication.GoToPage(ApplicationPage.Chat);

        }


            /// <summary>
            /// Navigate to next level in hierarchy if possible
            /// </summary>
            private void NavigateElement(HierarchyViewModel mDraggedItem)
            {

                //Prepopulate
                if (mDraggedItem == null)
                    return;
            if (mDraggedItem.Root == "")
                EditHierarchyElement(mDraggedItem);
            else
            //((HierarchyTreeViewModel)ViewModelApplication.CurrentControlViewModel).mSelectedTreeItem.ShortName = "TEstinG";
            {
                ViewModelApplication.CurrentPopupViewModel = new HierarchyTreeViewModel(mDraggedItem.Root);
                ViewModelApplication.CurrentPopupContent = 0;
                ViewModelApplication.CurrentPopupContent = PopupContent.Hierarchy;
            }
            //ViewModelApplication.CurrentPageViewModel = ViewModelApplication.CurrentPageViewModel;

            //RunSelectedMenu();

        }


                /// <summary>
                /// Use Popup view to edit existing Hierarchy Element
                /// </summary>
                private void EditHierarchyElement(HierarchyViewModel mDraggedItem)
                {

                    //Prepopulate
                    if (mDraggedItem == null)
                        return;
                    ViewModelApplication.CurrentPopupContent = PopupContent.AddElement;

                    var mAddElementViewModel = (HierarchyElementViewModel)ViewModelApplication.CurrentPopupViewModel;
                    mAddElementViewModel.ShortName.OriginalText = mDraggedItem.ShortName;
                    mAddElementViewModel.ShortName.EditedText = mDraggedItem.ShortName;
                    mAddElementViewModel.Description.OriginalText = mDraggedItem.Description;
                    mAddElementViewModel.Description.EditedText = mDraggedItem.Description;
                    mAddElementViewModel.Page = mDraggedItem.Page;
                    mAddElementViewModel.Root.OriginalText = mDraggedItem.Root;
                    mAddElementViewModel.Root.EditedText = mDraggedItem.Root;
                    mAddElementViewModel.IsMenuItem = mDraggedItem.IsMenuItem;
                    mAddElementViewModel.ParentShortName = mDraggedItem.ParentShortName;
                    mAddElementViewModel.ParentCategoryID = mDraggedItem.ParentCategoryID;
                    mAddElementViewModel.KCategoryID = mDraggedItem.KCategoryID;
                    mAddElementViewModel.DateEffective = mDraggedItem.DateEffective;
                    mAddElementViewModel.DateDiscontinued = mDraggedItem.DateDiscontinued;
                    mAddElementViewModel.AddNodeButtonText = null;
                    mAddElementViewModel.EditNodeButtonText = "Update Selected Element";
                    mAddElementViewModel.DeleteNodeButtonText = null;
                    mAddElementViewModel.CopyNodeButtonText = null;
                    mAddElementViewModel.MoveNodeButtonText = null;
                    mAddElementViewModel.HeadingText = "Update Selected Element";
                    mAddElementViewModel.HierarchyType = mDraggedItem.HierarchyType;
                    mAddElementViewModel.HierarchyTypeID = mDraggedItem.HierarchyTypeID;
                    mAddElementViewModel.FHierarchyID = mDraggedItem.HierarchyTypeID;
                    mAddElementViewModel.Type.OriginalKid = mDraggedItem.HierarchyTypeID;
                    mAddElementViewModel.Type.OriginalName = mDraggedItem.HierarchyType;
                    mAddElementViewModel.Type.EditedKid = mDraggedItem.HierarchyTypeID;
                    mAddElementViewModel.Type.EditedName = mDraggedItem.HierarchyType;
                    //ViewModelApplication.CurrentPopupContent = PopupContent.AddElement;
                    //ViewModelApplication.CurrentPopupViewModel = null;
                    //ViewModelApplication.CurrentPopupContent = PopupContent.SWBilling;
                   
                    ViewModelApplication.PopupVisible = true;
                    //ViewModelApplication.SettingsMenuVisible = true;
                }

        /// <summary>
        /// Use Popup View to add a Hierarchy Element
        /// </summary>
        private void AddHierarchyElement(HierarchyViewModel mDraggedItem)
        {

            if (mDraggedItem == null)
                return;
            ViewModelApplication.CurrentPopupContent = PopupContent.AddElement;
            //var ParentNodeClient = results.FirstOrDefault().FClientID;
            var mAddElementViewModel = (HierarchyElementViewModel)ViewModelApplication.CurrentPopupViewModel;
            mAddElementViewModel.ShortName.OriginalText = "New Element Name";
            mAddElementViewModel.Description.OriginalText = "Description of New Element";
            mAddElementViewModel.ShortName.EditedText = "New Element Name";
            mAddElementViewModel.Description.EditedText = "Description of New Element";
            //if (mPage == "Hierarchy")
            //    mAddElementViewModel.Page = mPage;
            //else
            mAddElementViewModel.Page = "";
            mAddElementViewModel.Root.OriginalText = "Element Root";
            mAddElementViewModel.Root.EditedText = "Element Root";
            mAddElementViewModel.IsMenuItem = mDraggedItem.IsMenuItem;
            mAddElementViewModel.ParentShortName = mDraggedItem.ShortName;
            mAddElementViewModel.ParentCategoryID = mDraggedItem.KCategoryID;
            mAddElementViewModel.KCategoryID = Guid.NewGuid().ToString().ToUpper();
            mAddElementViewModel.DateEffective = DateTime.Today;
            mAddElementViewModel.DateDiscontinued = new DateTime(9999, 12, 31);
            mAddElementViewModel.AddNodeButtonText = "Add new Hierarchy Element";
            mAddElementViewModel.EditNodeButtonText = null;
            mAddElementViewModel.DeleteNodeButtonText = null;
            mAddElementViewModel.CopyNodeButtonText = null;
            mAddElementViewModel.MoveNodeButtonText = null;
            mAddElementViewModel.HierarchyType = mDraggedItem.HierarchyType;
            mAddElementViewModel.HierarchyTypeID = mDraggedItem.HierarchyTypeID;
            mAddElementViewModel.Type.OriginalKid = mDraggedItem.HierarchyTypeID;
            mAddElementViewModel.Type.OriginalName = mDraggedItem.HierarchyType;
            mAddElementViewModel.FClientID = mDraggedItem.FClientID;
            mAddElementViewModel.HeadingText = "Add new Hierarchy Element";
            mAddElementViewModel.FHierarchyID = mDraggedItem.FHierarchyID;
            //ViewModelApplication.CurrentPopupContent = PopupContent.AddElement;
            ViewModelApplication.PopupVisible = true;
            //ViewModelApplication.SettingsMenuVisible = true;
        }


        /// <summary>
        /// Use Popup View to delete a Hierarchy Element
        /// </summary>
        private void DeleteHierarchyElement(HierarchyViewModel mDraggedItem)
        {
            //Prepopulate

            if (mDraggedItem == null)
                return;
            ViewModelApplication.CurrentPopupContent = PopupContent.AddElement;
            var mAddElementViewModel = (HierarchyElementViewModel)ViewModelApplication.CurrentPopupViewModel;
            mAddElementViewModel.ShortName.OriginalText = mDraggedItem.ShortName;
            mAddElementViewModel.ShortName.EditedText = mDraggedItem.ShortName;
            mAddElementViewModel.Description.OriginalText = mDraggedItem.Description;
            mAddElementViewModel.Description.EditedText = mDraggedItem.Description;
            mAddElementViewModel.ParentShortName = mDraggedItem.ParentShortName;
            mAddElementViewModel.ParentCategoryID = mDraggedItem.ParentCategoryID;
            mAddElementViewModel.Page = mDraggedItem.Page;
            mAddElementViewModel.Root.OriginalText = mDraggedItem.Root;
            mAddElementViewModel.Root.EditedText = mDraggedItem.Root;
            mAddElementViewModel.IsMenuItem = mDraggedItem.IsMenuItem;
            mAddElementViewModel.KCategoryID = mDraggedItem.KCategoryID;
            mAddElementViewModel.DateEffective = mDraggedItem.DateEffective;
            mAddElementViewModel.DateDiscontinued = mDraggedItem.DateDiscontinued;
            mAddElementViewModel.AddNodeButtonText = null;
            mAddElementViewModel.EditNodeButtonText = null;
            mAddElementViewModel.CopyNodeButtonText = null;
            mAddElementViewModel.MoveNodeButtonText = null;
            mAddElementViewModel.DeleteNodeButtonText = "Delete Selected Element";
            mAddElementViewModel.HeadingText = "Delete Selected Element";
            mAddElementViewModel.HierarchyType = mDraggedItem.HierarchyType;
            mAddElementViewModel.HierarchyTypeID = mDraggedItem.HierarchyTypeID;
            mAddElementViewModel.FHierarchyID = mDraggedItem.HierarchyTypeID;
            mAddElementViewModel.Type.OriginalKid = mDraggedItem.HierarchyTypeID;
            mAddElementViewModel.Type.OriginalName = mDraggedItem.HierarchyType;


            //ViewModelApplication.CurrentPopupContent = PopupContent.AddElement;
            ViewModelApplication.PopupVisible = true;
            //ViewModelApplication.SettingsMenuVisible = true;
        }

        /// <summary>
        /// Use Popup view to move existing Hierarchy Element
        /// </summary>
        public void MoveHierarchyElement(HierarchyViewModel mDraggedItem, HierarchyViewModel mTarget)
        {
            //Prepopulate

            if (mDraggedItem == null)
                return;
            ViewModelApplication.CurrentPopupContent = PopupContent.AddElement;

            var mAddElementViewModel = (HierarchyElementViewModel)ViewModelApplication.CurrentPopupViewModel;
            mAddElementViewModel.ShortName.OriginalText = mDraggedItem.ShortName;
            mAddElementViewModel.Description.OriginalText = mDraggedItem.Description;
            mAddElementViewModel.ShortName.EditedText = mDraggedItem.ShortName;
            mAddElementViewModel.Description.EditedText = mDraggedItem.Description;
            mAddElementViewModel.Page = mDraggedItem.Page;
            mAddElementViewModel.Root.OriginalText = mDraggedItem.Root;
            mAddElementViewModel.Root.EditedText = mDraggedItem.Root;
            mAddElementViewModel.IsMenuItem = mDraggedItem.IsMenuItem;
            mAddElementViewModel.ParentShortName = mTarget.ShortName;
            mAddElementViewModel.ParentCategoryID = mTarget.KCategoryID;
            mAddElementViewModel.KCategoryID = mDraggedItem.KCategoryID;
            mAddElementViewModel.DateEffective = mDraggedItem.DateEffective;
            mAddElementViewModel.DateDiscontinued = new DateTime(9999, 12, 31);
            mAddElementViewModel.AddNodeButtonText = null;
            mAddElementViewModel.MoveNodeButtonText = "Move Selected Element";
            mAddElementViewModel.DeleteNodeButtonText = null;
            mAddElementViewModel.CopyNodeButtonText = null;
            mAddElementViewModel.EditNodeButtonText = null;
            mAddElementViewModel.HierarchyType = mDraggedItem.HierarchyType;
            mAddElementViewModel.HierarchyTypeID = mDraggedItem.HierarchyTypeID;
            mAddElementViewModel.HeadingText = "Move Selected Element (with descendants)";
            mAddElementViewModel.FHierarchyID = mDraggedItem.HierarchyTypeID;
            mAddElementViewModel.Type.OriginalKid = mDraggedItem.HierarchyTypeID;
            mAddElementViewModel.Type.OriginalName = mDraggedItem.HierarchyType;

            //ViewModelApplication.CurrentPopupContent = PopupContent.AddElement;
            ViewModelApplication.PopupVisible = true;
            //ViewModelApplication.SettingsMenuVisible = true;
        }

        /// <summary>
        /// Copy the selected hierarchy (with all descendants) to the element selected as the destination
        /// "Copy Of " is used as a prefix for all elements in the element family being copied
        /// </summary>
        public void CopyHierarchyElement(HierarchyViewModel mDraggedItem, HierarchyViewModel mTarget)
        {
            if (mDraggedItem == null)
                return;
            ViewModelApplication.CurrentPopupContent = PopupContent.AddElement;
            //Prepopulate
            var mAddElementViewModel = (HierarchyElementViewModel)ViewModelApplication.CurrentPopupViewModel;
            mAddElementViewModel.ShortName.OriginalText = mDraggedItem.ShortName;
            mAddElementViewModel.Description.OriginalText = mDraggedItem.Description;
            mAddElementViewModel.ShortName.EditedText = mDraggedItem.ShortName;
            mAddElementViewModel.Description.EditedText = mDraggedItem.Description;
            mAddElementViewModel.Page = mDraggedItem.Page;
            mAddElementViewModel.Root.OriginalText = mDraggedItem.Root;
            mAddElementViewModel.Root.EditedText = mDraggedItem.Root;
            mAddElementViewModel.IsMenuItem = mDraggedItem.IsMenuItem;
            mAddElementViewModel.ParentShortName = mTarget.ShortName;
            mAddElementViewModel.ParentCategoryID = mTarget.KCategoryID;
            mAddElementViewModel.KCategoryID = mDraggedItem.KCategoryID;
            mAddElementViewModel.DateEffective = DateTime.Today;
            mAddElementViewModel.DateDiscontinued = new DateTime(9999, 12, 31);
            mAddElementViewModel.AddNodeButtonText = null;
            mAddElementViewModel.EditNodeButtonText = null;
            mAddElementViewModel.MoveNodeButtonText = null;
            mAddElementViewModel.CopyNodeButtonText = "Copy Selected Element";
            mAddElementViewModel.DeleteNodeButtonText = null;
            mAddElementViewModel.HierarchyType = mDraggedItem.HierarchyType;
            mAddElementViewModel.HierarchyTypeID = mDraggedItem.HierarchyTypeID;
            mAddElementViewModel.HeadingText = "Copy Selected Element (with descendants)";
            mAddElementViewModel.FHierarchyID = mDraggedItem.HierarchyTypeID;
            mAddElementViewModel.Type.OriginalKid = mDraggedItem.HierarchyTypeID;
            mAddElementViewModel.Type.OriginalName = mDraggedItem.HierarchyType;

            //ViewModelApplication.CurrentPopupContent = PopupContent.AddElement;
            ViewModelApplication.PopupVisible = true;
            //ViewModelApplication.SettingsMenuVisible = true;
        }


        /// <summary>
        /// Persist all items changed or added on hierarchy to back end database. Depending on stage
        /// of change control, changes may be forwarded for recommendation or finally approved and implemented
        /// on back end
        /// </summary>
        public async Task PersistHierarchyChangesAsync()
        {
            await PersistHierarchyAsync();
            //Close();
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