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
using System.Windows.Threading;
using static Fasetto.Word.Core.CoreDI;
using static Fasetto.Word.DI;
using static Fasetto.Word.HierarchyControl;

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
        /// Store View Model of PopuplViewModel accessing this view model to allow reverse navigation
        /// </summary>
        public object PriorPopupViewModel { get; set; }

        /// <summary>
        /// copy of the view model called when manipulation of a hierarchy element is reguired
        /// </summary>
        public HierarchyElementViewModel MHierarchyElementViewModel { get; set; }



        /// <summary>
        /// Link to ScrollViewer managing the hierarchy tree
        /// </summary>
        public ScrollViewer MScrollViewer { get; set; }

        private readonly DispatcherTimer _clickTimer;
        private const int DoubleClickTime = 300; // milliseconds
        private bool _doubleClickDetected;
        private object mtmp;

        private readonly DispatcherTimer _scrollTimer;
        private Point _currentMousePosition;
        private bool _isDraggingSelection = false;

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
        public ParameterHierarchyItemSelectApiModel mHierarchy;
        public HierarchyElementViewModel mElement;
        private Point mLastMouseDown;
        private HierarchyViewModel mDraggedItemTest, mDraggedItem, mTarget;
        private string mSourceCategoryName;

        private TreeViewItem mTargetT, mDraggedT;

        private string mDestinationCategoryID, mDestinationID, mSourceID, mParentID;

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
            mHDM = null;

            mTableName = hierarchyTable;
            mHierarchy = new ParameterHierarchyItemSelectApiModel
            {
                FHierarchyID = hierarchyTable,
                ClientID = ViewModelApplication.FClientID,
                DateTarget = DateTime.Now
            };
            #endregion
            //retrieve hierarchy from persistent storage on server
            //To Do: Add mTableName as parameter when calling HierarchyAsync to populate hierarchy
            //ViewModelApplication.CurrentPopupContent = PopupContent.AddElement;
            //ViewModelApplication.PopupVisible = false;
            if (ViewModelApplication.CurrentPopupViewModel != null && (ViewModelApplication.CurrentPopupViewModel).GetType().Name == "HierarchyTreeViewModel")
            {
                PriorPopupViewModel = ViewModelApplication.CurrentPopupViewModel;
            }

            // Find ScrollViewer after the TreeView template is generated

            TaskManager.RunAndForget(HierarchyAsync);


            // Get the OptFinHierarchies currently configured - first populate 'root hierarchy' variable with all configured root hierarchy elements currently available


            UpdateTreeViewElements();
            CloseCommand = new RelayCommand(async () => await CloseAsync());
            GestureHandlerCommand = new DelegateCommand<ContextualEventArgs>(GestureHandler);
            //GestureHandlerCommand= new RelayParameterizedCommand<ContextualEventArgs>(ExecuteItemModeSelectionChanged);
            mSearchCommand = new SearchCategoryTreeCommand(this);

            // Timer to delay single-click action until we know it's not a double-click
            _clickTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(DoubleClickTime)
            };
            _clickTimer.Tick += ClickTimer_Tick;

            ///Initialising _scrollTimer
            ///

            _scrollTimer = new DispatcherTimer
            {
                // Adjust interval to control scrolling speed
                Interval = TimeSpan.FromMilliseconds(40)
            };
            _scrollTimer.Tick += ScrollTimer_Tick;

            //// Attach mouse event
            //this.MouseLeftButtonUp += OnMouseLeftButtonUp; 

        }



        private void ClickTimer_Tick(object sender, EventArgs e)
        {
            _clickTimer.Stop();

            if (!_doubleClickDetected & mtmp.GetType().Name == "MouseButtonEventArgs")
            {
                // Perform single-click action
            ((MouseButtonEventArgs)mtmp).Handled = true;
                //if (mSelectedTreeItem.Page != "Folder")
                //{
                    RunSelectedMenu();
                //}
                //else
                //{ }
                return;
            }
            else
                if (!_doubleClickDetected & mtmp.GetType().Name == "MouseEventArgs")
                {
                    // Perform single-click action
                    ((MouseEventArgs)mtmp).Handled = true;
                    //if (mSelectedTreeItem.Page != "Folder")
                    //{
                    RunSelectedMenu();
                    //}
                    //else
                    //{ }
                    return;
                }
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
        /// A flag indicating if the login command is running
        /// </summary>
        public bool PersistHierarchyIsRunning { get; set; }

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
                //var token = (await scopedClientDataStore.GetLoginCredentialsAsync())?.Token;
                var token = ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).Token;
                // Call the server and attempt to register with the provided credentials
                // If we don't have a token (then not logged in...)
                if (string.IsNullOrEmpty(token))
                    // Then do nothing more
                    return;
                var result = await WebRequests.PostAsync<ApiResponse<HierarchyResultListApiModel>>(
                // Set URL
                    //RouteHelpers.GetAbsoluteRoute(ApiRoutes.ReturnHierarchy),
                    //mTableName,
                    //bearerToken: token);

                    RouteHelpers.GetAbsoluteRoute(ApiRoutes.GenericHierarchyLookup),
                    mHierarchy,
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
        /// -used when elements of the treeview are being manipulated on the front end
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
                ud1 = null;
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
        ///...
        //To Do:When a node is approved for deletion, any objects referencing that node must refererence the first ascendant still existing.
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

        public async Task CloseAsync()
        {
            // Close settings menu
            //await RunCommandAsync(() => HierarchyBuildIsRunning, async () =>
            //{
                //PersistHierarchyChangesAsync();
                TaskManager.RunAndForget(PersistHierarchyAsync);
            // Close settings menu
            //Log($"Done work on calling thread *****");

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
                    if (ViewModelApplication.CurrentPopupViewModel != null && ViewModelApplication.CurrentPopupViewModel.GetType().Name == "HierarchyTreeViewModel" && ((HierarchyTreeViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel != null)
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
            //});

        }




        /// <summary>
        /// Interpret Keyboard and Pointing device Gestures
        /// </summary>
        /// <param name="parameter"></param>
        public void GestureHandler(object parameter)
        {
            
            var tmp = ((ContextualEventArgs)parameter).OriginalEventArgs;
            mtmp = tmp;
            var eventTmp = tmp.GetType().Name;
            var isDrag = false;

            var tmp1 = ((ContextualEventArgs)parameter).Context ==null? "None" : ((ContextualEventArgs)parameter).Context.GetType().Name;
            //if (!(eventTmp == "MouseButtonEventArgs" || eventTmp == "MouseEventArgs" || eventTmp == "KeyEventArgs"))
            //    return;



                switch (eventTmp)
                {

                    case "MouseButtonEventArgs" or "MouseEventArgs" :
                    {
                        //if (eventTmp == "DragEventArgs")
                        //{
                        //    var dragEvent = (DragEventArgs)tmp;
                        //    if (dragEvent.RoutedEvent == DragDrop.DragEnterEvent)
                        //        isDrag = true;
                        //    else
                        //        isDrag = false;
                        //    break;
                        //    //}

                        if (eventTmp == "MouseButtonEventArgs")
                        {

                            var mouseArgs = (MouseButtonEventArgs)tmp;
                            var srcElement = mouseArgs.OriginalSource as UIElement;
                            var srcType = srcElement?.GetType().FullName;
                            if (srcType == "System.Windows.Controls.TextBlock")
                            mSelectedTreeItem = (HierarchyViewModel)((TreeView)mouseArgs.Source).SelectedItem;


                            //If button is clicked when the mouse is over the expander triagngle of the TreeView
                            if (srcType == "System.Windows.Shapes.Path")
                                return;

                            if (srcType != "System.Windows.Controls.TextBlock")
                            {
                                //mouseArgs.Handled = true;
                                return;
                            }
                            if (mouseArgs.RoutedEvent == UIElement.PreviewMouseLeftButtonDownEvent)
                            {
                                //mouseEvent.Handled = true;                             
                                //break;
                            }
                            if (mouseArgs.RoutedEvent == UIElement.PreviewMouseLeftButtonUpEvent)
                            {
                                //if (MScrollViewer == null)
                                //{
                                //    var item = GetNearestContainer(mouseArgs.OriginalSource as UIElement);

                                //    var MTreeView = (TreeView)mouseArgs.Source;
                                //    MScrollViewer = FindScrollViewer(MTreeView);
                                //    //MScrollViewer.IsHitTestVisible = true;
                                //    MScrollViewer?.ReleaseMouseCapture();
                                //    _scrollTimer.Stop();
                                //}
                            }

                            if (mouseArgs.ClickCount == 1)
                            {
                                if (!isDrag)
                                {
                                    _doubleClickDetected = false;
                                    _clickTimer.Stop();
                                    _clickTimer.Start();
                                }
                                mouseArgs.Handled = true;
                                break;
                            }

                            if (mouseArgs.ClickCount > 1)
                            {
                                _doubleClickDetected = true;
                                _clickTimer.Stop();
                                mouseArgs.Handled = true;
                                if (!(ViewModelApplication.CurrentSideMenuContent != SideMenuContent.Menu))
                                    EditHierarchyElement(mSelectedTreeItem);
                                return;
                            }

                            break;
                        }
                        else // MouseEventArgs
                            if (eventTmp == "MouseEventArgs")
                            {
                                var mouseEvent = (MouseEventArgs)tmp;
                                //mSource = (TreeViewItem)((ContextualEventArgs)parameter).Context;
                                // check left button pressed AND mouse move routed event

                                if (mouseEvent.RoutedEvent == Mouse.MouseEnterEvent)
                                {
                                    mouseEvent.Handled = true;
                                    break;
                                }

                                if (mouseEvent.RoutedEvent == Mouse.MouseLeaveEvent)
                                {
                                    mouseEvent.Handled = true;
                                    break;
                                }

                                if (mouseEvent.RoutedEvent == Mouse.MouseDownEvent)
                                {
                                    //mouseEvent.Handled = true;                             
                                    break;
                                }

                                if ((mouseEvent.RoutedEvent == Mouse.PreviewMouseMoveEvent) & mouseEvent.LeftButton == MouseButtonState.Pressed)
                                {
                                    //{
                                    //    try
                                    //    {
                                    var item = GetNearestContainer(mouseEvent.OriginalSource as UIElement);

                                    if (item != null)
                                    {
                                        if (mouseEvent.LeftButton == MouseButtonState.Pressed & item != null)
                                        {
                                            var isCtrl = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
                                            var isShift = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
                                            var currentPosition = mouseEvent.GetPosition(item);
                                            mDraggedItem = (HierarchyViewModel)((TreeViewItem)item).Header;
                                            mDraggedT = item;

                                            //            //return;

                                            //Check for dragging of treeview item
                                            if ((Math.Abs(currentPosition.X - mLastMouseDown.X) > 10.0) ||
                                                (Math.Abs(currentPosition.Y - mLastMouseDown.Y) > 10.0))
                                            {

                                                mSourceCategoryName = mDraggedItem.ShortName;
                                                mLastMouseDown = currentPosition;

                                                if (mDraggedItem != null)

                                                {
                                                    isDrag = true;
                                                    mTarget = null;//ensure target is reset
                                                    if (!isCtrl & !isShift)
                                                    {

                                                        var finalDropEffect = DragDrop.DoDragDrop(item, mDraggedItem,
                                                            DragDropEffects.Move);

                                                        //Checking target is not null and item is dragging(moving)
                                                        if ((finalDropEffect == DragDropEffects.Move) && (mTarget != null))
                                                        {
                                                            // A Move drop was accepted
                                                            if (mTarget != null & mDraggedItem != null)
                                                            {
                                                                if (CheckDropTarget(mTargetT,mDraggedT))
                                                                    ((HierarchyTreeViewModel)ViewModelApplication.CurrentPopupViewModel).MoveHierarchyElement(mDraggedItem, mTarget);// MoveItem();

                                                                mTargetT = null;
                                                                //mSource = null;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        var finalDropEffect = DragDrop.DoDragDrop(item, mDraggedItem, DragDropEffects.Copy);
                                                        if ((finalDropEffect == DragDropEffects.Copy) && (mTarget != null) && (isCtrl||isShift))
                                                        {
                                                            // A Copy drop was accepted
                                                            if (mTarget != null & mDraggedItem != null)
                                                            {
                                                                if (CheckDropTarget(mTargetT, mDraggedT))
                                                                    ((HierarchyTreeViewModel)ViewModelApplication.CurrentPopupViewModel).CopyHierarchyElement(mDraggedItem, mTarget);// MoveItem();
                                                                mTargetT = null;
                                                                //mSource = null;
                                                            }


                                                        }
                                                        }

                                                    }

                                                }

                                            }
                                        }





                                        break;
                                    }

                                }                       // if we reach here (eventTmp != "MouseButtonEventArgs") we must still break
                                break;
                            }
                    


                case  "DragEventArgs":
                    {
                        var dragEvent = (DragEventArgs)tmp;

                        var item = GetNearestContainer(dragEvent.OriginalSource as UIElement);
                        //mDraggedT = item;

                        //if (MScrollViewer == null)
                        //{
                        //    var MTreeView = (TreeView)dragEvent.Source;
                        //    MScrollViewer = FindScrollViewer(MTreeView);
                        //}
                        ////MScrollViewer = FindScrollViewer(dragEvent.OriginalSource as DependencyObject);

                        //if (dragEvent.RoutedEvent ==DragDrop.PreviewDragOverEvent & MScrollViewer !=null)

                        //{


                        //    //MTreeview.IsHitTestVisible = false;
                        //     var MTreeView = (TreeView)dragEvent.Source;
                        //   var mouseCaptured = MScrollViewer.IsMouseCaptured;
                        //    MScrollViewer?.CaptureMouse();
                        //    _scrollTimer.Start();

                        //Scroll treeview when dragging element past the upper and lower extert of displayed content

                        if (dragEvent.Source is not TreeView treeView) return;

                        // Retrieve the internal ScrollViewer using VisualTreeHelper
                        var scrollViewer = FindVisualChild<ScrollViewer>(treeView);
                        if (scrollViewer == null) return;

                        // Get position of mouse relative to TreeView
                        var currentPosition = dragEvent.GetPosition(treeView);

                        double tolerance = 20; // Distance in pixels from edge to start scrolling
                        double offset = 10;    // Scroll speed/step size



                        if (currentPosition.Y < tolerance)
                        {
                            // Near top edge - scroll up
                            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - offset);
                        }
                        else if (currentPosition.Y > treeView.ActualHeight - tolerance)
                        {
                            // Near bottom edge - scroll down
                            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + offset);
                        }


                        if (dragEvent.RoutedEvent == DragDrop.PreviewDragOverEvent)

                        {

                            currentPosition = dragEvent.GetPosition(item);

                            if ((Math.Abs(currentPosition.X - mLastMouseDown.X) > 10.0) ||
                               (Math.Abs(currentPosition.Y - mLastMouseDown.Y) > 10.0))
                            {
                                // Verify that this is a valid drop and then store the drop target
                                item = GetNearestContainer(dragEvent.OriginalSource as UIElement);

                                if (item == null)
                                { dragEvent.Effects = DragDropEffects.None; }
                                //else
                                //{
                                    mTargetT = item;
                                    mTarget = (HierarchyViewModel)mTargetT.GetType().GetProperties().Single(c => c.Name == "DataContext").GetValue(mTargetT);
                                    //if (mSourceID == mDestinationID)
                                    //{ }
                                    if (dragEvent.Effects == DragDropEffects.Move)
                                    { dragEvent.Effects = CheckDropTarget(mTargetT, mDraggedT) ? DragDropEffects.Move : DragDropEffects.None; }
                                    else
                                    { dragEvent.Effects = DragDropEffects.Copy; }
                                //}
                            }
                            dragEvent.Handled = true;
                        }

                        break;
                    }

                case "MouseWheelEventArgs":
                {
                        return;
                    }

                case "ScrollChangedEventArgs":
                {
                        return;
                    }

                case "KeyEventArgs":
                        {
                            if (tmp1 == "String")
                            {
                                SearchText = SearchText;
                                if (((KeyEventArgs)tmp).Key == Key.Enter)
                                //((KeyEventArgs)tmp).Handled = true;
                                { SearchCommand.Execute(null); }
                                break;
                            }
                            else
                            {
                                mSelectedTreeItem = (HierarchyViewModel)(((ContextualEventArgs)parameter).Context);
                                if (((KeyEventArgs)tmp).Key == Key.Enter)
                                {
                                    ((KeyEventArgs)tmp).Handled = true;
                                if ((ViewModelApplication.CurrentSideMenuContent != SideMenuContent.Menu) & (mSelectedTreeItem.Page != "Hierarchy") & ViewModelApplication.CurrentPopupContent != 0 & (mSelectedTreeItem.Page != "Folder")) 
                                    EditHierarchyElement(mSelectedTreeItem);
                                    else
                                    {
                                            RunSelectedMenu();
                                    }
                                    break;
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
                                break;
                            }
                        }

                    // Unknown
                    default:
                        return;
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


        private bool CheckDropTarget(FrameworkElement  TargetT, FrameworkElement DraggedT)
        {
            //Check whether the target item is valid for the intended operation



            //Check that move will not cause infinite loop(Ancestor-descendant - Ancestor)
            //Check that the item being moved is not an Ancestor of the item being moved to
            //the KCategoryID attribute of the item being moved may not be an ancestor of the
            //item being moved too.
            //If this constraint is met, the boolean is set to TRUE

            var targetID = ((HierarchyViewModel)TargetT.GetType().GetProperties().Single(c => c.Name == "DataContext").GetValue(TargetT)).KCategoryID;
            var draggedID = ((HierarchyViewModel)DraggedT.GetType().GetProperties().Single(c => c.Name == "DataContext").GetValue(DraggedT)).KCategoryID;


            if (targetID == draggedID  || TreeViewHelper.GetChildTreeViewItems(DraggedT,TargetT))
            { return false; }
            //var mDestinationID = (string)res.GetType().GetProperties().Single(c => c.Name == "KId").GetValue(res);

            MatchingKCategoryEnumerator = null;

            //return PerformKIdSearch();
            return true;

        }



        /// Use Popup View to add a Hierarchy Element
        /// </summary>
        private void RunSelectedMenu()
        {
            //Prepopulate
            //Only allow one execution of  the function per event
            //if (!ViewModelApplication.SideMenuVisible)
            //    return;
            //if (mSelectedTreeItem == null || ((string)mSelectedTreeItem.Page).Length == 0 || mSelectedTreeItem.Page == "Folder")//|| mSelectedTreeItem.Children.Count > 0
            //    return;
            //((KeyEventArgs)tmp).Handled = true;
            //If the item selected is part of a hierarchy structure, navigate to the next level
            if (mSelectedTreeItem.Page == "Hierarchy")
            {
                if (mSelectedTreeItem.Root != "")
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
                {
                    if (!(ViewModelApplication.CurrentSideMenuContent == SideMenuContent.Menu))
                        EditHierarchyElement(mSelectedTreeItem);
                }

            }
            else
                //Ensure that the OpenMenu option has a valid link..
                if (mSelectedTreeItem.Page != "" & mSelectedTreeItem.Children.Count ==0)
                { ViewModelApplication.OpenMenu(mSelectedTreeItem.Root, mSelectedTreeItem.Page); }
                else
                    if (mSelectedTreeItem.Children.Count > 0 )
                    {
                        if (!mSelectedTreeItem.IsExpanded == true)
                        {
                            mSelectedTreeItem = mSelectedTreeItem.Children[0];
                            mSelectedTreeItem.IsSelected = true;
                            mSelectedTreeItem.mParent.IsExpanded = true;
                        }
                        else
                            mSelectedTreeItem.IsExpanded = false;
                    }
                    else
                    {
                        //mSelectedTreeItem.IsExpanded = false;
                        EditHierarchyElement(mSelectedTreeItem);
                    }


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
                                CommonHierarchyElement(mDraggedItem, mTarget);

                    var MHierarchyElementViewModel  = (HierarchyElementViewModel)ViewModelApplication.CurrentPopupViewModel;
                    MHierarchyElementViewModel .ShortName.OriginalText = mDraggedItem.ShortName;
                    MHierarchyElementViewModel .ShortName.EditedText = mDraggedItem.ShortName;
                    MHierarchyElementViewModel .Description.OriginalText = mDraggedItem.Description;
                    MHierarchyElementViewModel .Description.EditedText = mDraggedItem.Description;
                    MHierarchyElementViewModel .Page = mDraggedItem.Page;
                    MHierarchyElementViewModel .Root.OriginalText = mDraggedItem.Root;
                    MHierarchyElementViewModel .Root.EditedText = mDraggedItem.Root;
                    MHierarchyElementViewModel .IsMenuItem = mDraggedItem.IsMenuItem;
                    MHierarchyElementViewModel .ParentShortName = mDraggedItem.ParentShortName;
                    MHierarchyElementViewModel .ParentCategoryID = mDraggedItem.ParentCategoryID;
                    MHierarchyElementViewModel .KCategoryID = mDraggedItem.KCategoryID;
                    MHierarchyElementViewModel .DateEffective = mDraggedItem.DateEffective;
                    MHierarchyElementViewModel .DateDiscontinued = mDraggedItem.DateDiscontinued;
                    MHierarchyElementViewModel .AddNodeButtonText = null;
                    MHierarchyElementViewModel .EditNodeButtonText = "Update Selected Element";
                    MHierarchyElementViewModel .DeleteNodeButtonText = null;
                    MHierarchyElementViewModel .CopyNodeButtonText = null;
                    MHierarchyElementViewModel .MoveNodeButtonText = null;
                    MHierarchyElementViewModel .HeadingText = "Update Selected Element";
                    MHierarchyElementViewModel .HierarchyType = mDraggedItem.HierarchyType;
                    MHierarchyElementViewModel .HierarchyTypeID = mDraggedItem.HierarchyTypeID;
                    MHierarchyElementViewModel .FHierarchyID = mDraggedItem.HierarchyTypeID;
                    MHierarchyElementViewModel .Type.OriginalKid = mDraggedItem.HierarchyTypeID;
                    MHierarchyElementViewModel .Type.OriginalName = mDraggedItem.HierarchyType;
                    MHierarchyElementViewModel .Type.EditedKid = mDraggedItem.HierarchyTypeID;
                    MHierarchyElementViewModel .Type.EditedName = mDraggedItem.HierarchyType;
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
            //populate common attributes on ViewModel using  a dummy for the target
            CommonHierarchyElement(mDraggedItem, mDraggedItem);
            //var ParentNodeClient = results.FirstOrDefault().FClientID;
            //var MHierarchyElementViewModel  = (HierarchyElementViewModel)ViewModelApplication.CurrentPopupViewModel;
            MHierarchyElementViewModel .ShortName.OriginalText = "New Element Name";
            MHierarchyElementViewModel .Description.OriginalText = "Description of New Element";
            MHierarchyElementViewModel .ShortName.EditedText = "New Element Name";
            MHierarchyElementViewModel .Description.EditedText = "Description of New Element";
            //if (mPage == "Hierarchy")
            //    MHierarchyElementViewModel .Page = mPage;
            //else
            MHierarchyElementViewModel .Page = "";
            MHierarchyElementViewModel .Root.OriginalText = "Element Root";
            MHierarchyElementViewModel .Root.EditedText = "Element Root";
            //MHierarchyElementViewModel .IsMenuItem = mDraggedItem.IsMenuItem;
            //MHierarchyElementViewModel .ParentShortName = mDraggedItem.ShortName;
            //MHierarchyElementViewModel .ParentCategoryID = mDraggedItem.KCategoryID;
            //MHierarchyElementViewModel .KCategoryID = Guid.NewGuid().ToString().ToUpper();
            //MHierarchyElementViewModel .DateEffective = DateTime.Today;
            //MHierarchyElementViewModel .DateDiscontinued = new DateTime(9999, 12, 31);
            MHierarchyElementViewModel .AddNodeButtonText = "Add new Hierarchy Element";
            //MHierarchyElementViewModel .EditNodeButtonText = null;
            //MHierarchyElementViewModel .DeleteNodeButtonText = null;
            //MHierarchyElementViewModel .CopyNodeButtonText = null;
            //MHierarchyElementViewModel .MoveNodeButtonText = null;
            //MHierarchyElementViewModel .HierarchyType = mDraggedItem.HierarchyType;
            //MHierarchyElementViewModel .HierarchyTypeID = mDraggedItem.HierarchyTypeID;
            //MHierarchyElementViewModel .Type.OriginalKid = mDraggedItem.HierarchyTypeID;
            //MHierarchyElementViewModel .Type.OriginalName = mDraggedItem.HierarchyType;
            ////MHierarchyElementViewModel .FClientID = ViewModelApplication.FClientID;
            MHierarchyElementViewModel .HeadingText = "Add new Hierarchy Element";
            //MHierarchyElementViewModel .FHierarchyID = mDraggedItem.FHierarchyID;
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
            //populate common attributes on ViewModel using  a dummy for the target
              CommonHierarchyElement(mDraggedItem, mDraggedItem);
            //var MHierarchyElementViewModel  = (HierarchyElementViewModel)ViewModelApplication.CurrentPopupViewModel;
            //MHierarchyElementViewModel .ShortName.OriginalText = mDraggedItem.ShortName;
            //MHierarchyElementViewModel .ShortName.EditedText = mDraggedItem.ShortName;
            //MHierarchyElementViewModel .Description.OriginalText = mDraggedItem.Description;
            //MHierarchyElementViewModel .Description.EditedText = mDraggedItem.Description;
            //MHierarchyElementViewModel .ParentShortName = mDraggedItem.ParentShortName;
            //MHierarchyElementViewModel .ParentCategoryID = mDraggedItem.ParentCategoryID;
            //MHierarchyElementViewModel .Page = mDraggedItem.Page;
            //MHierarchyElementViewModel .Root.OriginalText = mDraggedItem.Root;
            //MHierarchyElementViewModel .Root.EditedText = mDraggedItem.Root;
            //MHierarchyElementViewModel .IsMenuItem = mDraggedItem.IsMenuItem;
            //MHierarchyElementViewModel .KCategoryID = mDraggedItem.KCategoryID;
            //MHierarchyElementViewModel .DateEffective = mDraggedItem.DateEffective;
            //MHierarchyElementViewModel .DateDiscontinued = mDraggedItem.DateDiscontinued;
            //MHierarchyElementViewModel .AddNodeButtonText = null;
            //MHierarchyElementViewModel .EditNodeButtonText = null;
            //MHierarchyElementViewModel .CopyNodeButtonText = null;
            //MHierarchyElementViewModel .MoveNodeButtonText = null;
            MHierarchyElementViewModel .DeleteNodeButtonText = "Delete Selected Element";
            MHierarchyElementViewModel .HeadingText = "Delete Selected Element";
            //MHierarchyElementViewModel .HierarchyType = mDraggedItem.HierarchyType;
            //MHierarchyElementViewModel .HierarchyTypeID = mDraggedItem.HierarchyTypeID;
            //MHierarchyElementViewModel .FHierarchyID = mDraggedItem.HierarchyTypeID;
            //MHierarchyElementViewModel .Type.OriginalKid = mDraggedItem.HierarchyTypeID;
            //MHierarchyElementViewModel .Type.OriginalName = mDraggedItem.HierarchyType;


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
                        CommonHierarchyElement(mDraggedItem, mTarget);

            //var MHierarchyElementViewModel  = (HierarchyElementViewModel)ViewModelApplication.CurrentPopupViewModel;
            //MHierarchyElementViewModel .ShortName.OriginalText = mDraggedItem.ShortName;
            //MHierarchyElementViewModel .Description.OriginalText = mDraggedItem.Description;
            //MHierarchyElementViewModel .ShortName.EditedText = mDraggedItem.ShortName;
            //MHierarchyElementViewModel .Description.EditedText = mDraggedItem.Description;
            //MHierarchyElementViewModel .Page = mDraggedItem.Page;
            //MHierarchyElementViewModel .Root.OriginalText = mDraggedItem.Root;
            //MHierarchyElementViewModel .Root.EditedText = mDraggedItem.Root;
            //MHierarchyElementViewModel .IsMenuItem = mDraggedItem.IsMenuItem;
            //MHierarchyElementViewModel .ParentShortName = mTarget.ShortName;
            //MHierarchyElementViewModel .ParentCategoryID = mTarget.KCategoryID;
            //MHierarchyElementViewModel .KCategoryID = mDraggedItem.KCategoryID;
            //MHierarchyElementViewModel .DateEffective = mDraggedItem.DateEffective;
            //MHierarchyElementViewModel .DateDiscontinued = new DateTime(9999, 12, 31);
            //MHierarchyElementViewModel .AddNodeButtonText = null;
            MHierarchyElementViewModel .MoveNodeButtonText = "Move Selected Element";
            //MHierarchyElementViewModel .DeleteNodeButtonText = null;
            //MHierarchyElementViewModel .CopyNodeButtonText = null;
            //MHierarchyElementViewModel .EditNodeButtonText = null;
            //MHierarchyElementViewModel .HierarchyType = mDraggedItem.HierarchyType;
            //MHierarchyElementViewModel .HierarchyTypeID = mDraggedItem.HierarchyTypeID;
            MHierarchyElementViewModel .HeadingText = "Move Selected Element (with descendants)";
            //MHierarchyElementViewModel .FHierarchyID = mDraggedItem.HierarchyTypeID;
            //MHierarchyElementViewModel .Type.OriginalKid = mDraggedItem.HierarchyTypeID;
            //MHierarchyElementViewModel .Type.OriginalName = mDraggedItem.HierarchyType;

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
            //if (mDraggedItem == null)
            //    return;
            //ViewModelApplication.CurrentPopupContent = PopupContent.AddElement;
            CommonHierarchyElement(mDraggedItem, mTarget);
            //Prepopulate
            var MHierarchyElementViewModel  = (HierarchyElementViewModel)ViewModelApplication.CurrentPopupViewModel;
            //MHierarchyElementViewModel .ShortName.OriginalText = mDraggedItem.ShortName;
            //MHierarchyElementViewModel .Description.OriginalText = mDraggedItem.Description;
            //MHierarchyElementViewModel .ShortName.EditedText = mDraggedItem.ShortName;
            //MHierarchyElementViewModel .Description.EditedText = mDraggedItem.Description;
            //MHierarchyElementViewModel .Page = mDraggedItem.Page;
            //MHierarchyElementViewModel .Root.OriginalText = mDraggedItem.Root;
            //MHierarchyElementViewModel .Root.EditedText = mDraggedItem.Root;
            //MHierarchyElementViewModel .IsMenuItem = mDraggedItem.IsMenuItem;
            //MHierarchyElementViewModel .ParentShortName = mTarget.ShortName;
            //MHierarchyElementViewModel .ParentCategoryID = mTarget.KCategoryID;
            //MHierarchyElementViewModel .KCategoryID = mDraggedItem.KCategoryID;
            //MHierarchyElementViewModel .DateEffective = DateTime.Today;
            //MHierarchyElementViewModel .DateDiscontinued = new DateTime(9999, 12, 31);
            //MHierarchyElementViewModel .AddNodeButtonText = null;
            //MHierarchyElementViewModel .EditNodeButtonText = null;
            //MHierarchyElementViewModel .MoveNodeButtonText = null;
            MHierarchyElementViewModel .CopyNodeButtonText = "Copy Selected Element";
            //MHierarchyElementViewModel .DeleteNodeButtonText = null;
            //MHierarchyElementViewModel .HierarchyType = mDraggedItem.HierarchyType;
            //MHierarchyElementViewModel .HierarchyTypeID = mDraggedItem.HierarchyTypeID;
            MHierarchyElementViewModel .HeadingText = "Copy Selected Element (with descendants)";
            //MHierarchyElementViewModel .FHierarchyID = mDraggedItem.HierarchyTypeID;
            //MHierarchyElementViewModel .Type.OriginalKid = mDraggedItem.HierarchyTypeID;
            //MHierarchyElementViewModel .Type.OriginalName = mDraggedItem.HierarchyType;

            //ViewModelApplication.CurrentPopupContent = PopupContent.AddElement;
            ViewModelApplication.PopupVisible = true;
            //ViewModelApplication.SettingsMenuVisible = true;
        }


        /// <summary>
        /// Copy the selected hierarchy (with all descendants) to the element selected as the destination
        /// "Copy Of " is used as a prefix for all elements in the element family being copied
        /// </summary>
        public void CommonHierarchyElement(HierarchyViewModel mDraggedItem, HierarchyViewModel mTarget)
        {
            ViewModelApplication.CurrentPopupContent = PopupContent.AddElement;
            //Prepopulate
            MHierarchyElementViewModel  = (HierarchyElementViewModel)ViewModelApplication.CurrentPopupViewModel;
            MHierarchyElementViewModel .ShortName.OriginalText = mDraggedItem.ShortName;
            MHierarchyElementViewModel .Description.OriginalText = mDraggedItem.Description;
            MHierarchyElementViewModel .ShortName.EditedText = mDraggedItem.ShortName;
            MHierarchyElementViewModel .Description.EditedText = mDraggedItem.Description;
            MHierarchyElementViewModel .Page = mDraggedItem.Page;
            MHierarchyElementViewModel .Root.OriginalText = mDraggedItem.Root;
            MHierarchyElementViewModel .Root.EditedText = mDraggedItem.Root;
            MHierarchyElementViewModel .IsMenuItem = mDraggedItem.IsMenuItem;
            MHierarchyElementViewModel .ParentShortName = mTarget.ShortName;
            MHierarchyElementViewModel .ParentCategoryID = mTarget.KCategoryID;
            MHierarchyElementViewModel .KCategoryID = mDraggedItem.KCategoryID;
            MHierarchyElementViewModel .DateEffective = DateTime.Today;
            MHierarchyElementViewModel .DateDiscontinued = new DateTime(9999, 12, 31);
            MHierarchyElementViewModel .HierarchyType = mDraggedItem.HierarchyType;
            MHierarchyElementViewModel .HierarchyTypeID = mDraggedItem.HierarchyTypeID;
            MHierarchyElementViewModel .FHierarchyID = mDraggedItem.HierarchyTypeID;
            MHierarchyElementViewModel .Type.OriginalKid = mDraggedItem.HierarchyTypeID;
            MHierarchyElementViewModel .Type.OriginalName = mDraggedItem.HierarchyType;


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
            //await RunCommandAsync(() => PersistHierarchyIsRunning, async () =>
            //{
            await Task.Run(async () =>
            {
                // Log it
                //Log($"Doing work on inner thread for ");

                // Wait 
                //await Task.Delay(500);

                // Log it
                //Log($"Done work on inner thread for ");


                // Store single transient instance of client data store
                //await Task.Delay(1);
                var matches = mPersist.Where(x => x.IsUnderReview  == true).OrderByDescending(x => x.DateEffective).ToList();
                var category = matches.FirstOrDefault();

                //

                if (category != null)
                {

                    var scopedClientDataStore = ClientDataStore;

                    // Update values from local cache
                    // Get the user token
                    //var token = (await scopedClientDataStore.GetLoginCredentialsAsync())?.Token;
                    var token = ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).Token;
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

                    // Log it
                    //Log($"Done work on inner thread for ");
                }
                // return to menu
            });

        //    }
        //);
        }

        #region Helper Methods

        /// <summary>
        /// Output a message with the current thread ID appended
        /// </summary>
        /// <param name="message"></param>
        //private static void Log(string message)
        //{
        //    // Write line
        //    Debug.WriteLine($"{message} [{Thread.CurrentThread.ManagedThreadId}]");
        //}

        private T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(child);

            while (parent != null && !(parent is T))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent as T;
        }


        /// <summary>
        /// Finds the first ScrollViewer in the visual tree of the given element.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static ScrollViewer FindScrollViewer(DependencyObject element)
        {
            if (element == null) return null;
            if (element is ScrollViewer viewer) return viewer;

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var child = VisualTreeHelper.GetChild(element, i);
                var result = FindScrollViewer(child);
                if (result != null) return result;
            }
            return null;
        }

        /// <summary>
        /// Finds the first ScrollViewer in the visual tree of the given element.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static TreeView FindTreeView(DependencyObject element)
        {
            if (element == null) return null;
            if (element is TreeView viewer) return viewer;

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var child = VisualTreeHelper.GetChild(element, i);
                var result = FindTreeView(child);
                if (result != null) return result;
            }
            return null;
        }
        private static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for ( var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T typedChild) return typedChild;

                var childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null) return childOfChild;
            }
            return null;
        }



        private void ScrollTimer_Tick(object sender, EventArgs e)
        {
            // Check if mouse is above the ScrollViewer
            if (MScrollViewer == null)
                return;
            if (_currentMousePosition.Y < 0)
            {
                 MScrollViewer.LineUp();
            }
            // Check if mouse is below the ScrollViewer
            else if (_currentMousePosition.Y >  MScrollViewer.ActualHeight)
            {
                 MScrollViewer.LineDown();
            }
        }

        public static class TreeViewHelper
        {
            public static bool GetChildTreeViewItems(FrameworkElement parent, FrameworkElement target)
            {
                var childItems = new List<FrameworkElement>();
                var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
                //if (childrenCount ==0)
                //    return false;

                for (var i = 0; i < childrenCount;)
                {
                    var child = VisualTreeHelper.GetChild(parent, i) as FrameworkElement;
                    i++;
                    var typeName = child.GetType().Name;
                    var parentType = parent.GetType().Name;

                    if (child != null && child.GetType().Name == "TreeViewItem" && child == target)
                    {
                        return true;
                    }
                    else
                        if (i == childrenCount)
                        {
                            if (GetChildTreeViewItems(child, target))
                                return true;
                        }



                }
                return false;

            }

        }



        #endregion

    }

}