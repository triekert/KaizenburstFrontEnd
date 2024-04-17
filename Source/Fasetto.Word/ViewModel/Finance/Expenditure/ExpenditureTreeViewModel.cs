
using Dna;
using Fasetto.Word.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static Fasetto.Word.DI;
using static Fasetto.Word.Core.CoreDI;
using System.Linq;

namespace Fasetto.Word
{
    /// <summary>
    /// This is the view-model of the UI.  It provides a data source
    /// for the TreeView (the FirstGeneration property), a bindable
    /// SearchText property, and the SearchCommand to perform a search.
    /// </summary>
    public class ExpenditureTreeViewModel : BaseViewModel

    {

        #region Public Properties

        /// <summary>
        /// A list of all registered hierarchy elements
        /// </summary>
        public ObservableCollection<ExpenditureViewModel> FirstGeneration { get; set; }


        //public ObservableCollection<ExpenditureViewModel> FirstGeneration1 { get; set; }

        #endregion

        #region Data

        //private readonly ReadOnlyCollection<ExpenditureViewModel> mFirstGeneration;
        protected ExpenditureViewModel mRootExpenditureElement;
        protected ExpenditureViewModel mRootExpenditureElement1;
        private readonly ICommand mSearchCommand;
        public ExpenditureListDataModel mHDML;
        public ExpenditureResultListApiModel mPersist, mPersistTmp,mOriginal;
        public ExpenditureDataModel mHDM;
        public string mTableName;
        public ExpenditureElementViewModel mElement;

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
        public ExpenditureTreeViewModel(string ExpenditureTable)
        {
            #region Dummy Root ExpenditureListDataModel
            mHDML = new ExpenditureListDataModel();
            mHDM = new ExpenditureDataModel
            {
                KCategoryID = new Guid().ToString(),
                ParentCategoryID = "00000000-0000-0000-0000-000000000000",
                Description = "...Loading Expenditure data...",
                ShortName = "Loading...Please be patient",
                Children = new ExpenditureListDataModel()
            };
            mHDML.Add(mHDM);

            mTableName = ExpenditureTable;
            #endregion
            //retrieve Expenditure from persistent storage on server
            //To Do: Add mTableName as parameter when calling HiearchyAsync to populate Expenditure
            TaskManager.RunAndForget(ExpenditureAsync);


            // Get the OptFinHierarchies currently configured - first populate 'root Expenditure' variable with all configured root Expenditure elements currently available


            UpdateTreeViewElements();
            CloseCommand = new RelayCommand(Close);
            mSearchCommand = new SearchCategoryTreeCommand(this);
        }

        private void UpdateTreeViewElements()

        {

            var rootElement = mHDML.FirstOrDefault(x => x.ParentCategoryID == "00000000-0000-0000-0000-000000000000");
            mRootExpenditureElement = new ExpenditureViewModel(rootElement)
            {
                IsExpanded = true
            };

            FirstGeneration = new ObservableCollection<ExpenditureViewModel>(
                new ExpenditureViewModel[]
                {
                    mRootExpenditureElement
                });


        }



        #endregion // Constructor

        #region Properties
        #region Public Properties



        /// <summary>
        /// A flag indicating if the login command is running
        /// </summary>
        public bool ExpenditureBuildIsRunning { get; set; }

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
            private readonly ExpenditureTreeViewModel mCategoryTree;

            public SearchCategoryTreeCommand(ExpenditureTreeViewModel CategoryTree)
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
        /// Return Expenditure of interest from Object persistence infrastructure
        /// User credentials are used to determine access authorisation
        /// </summary>
        /// <returns></returns>
        public async Task ExpenditureAsync()
        {
            await RunCommandAsync(() => ExpenditureBuildIsRunning, async () =>
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
                var result = await WebRequests.PostAsync<ApiResponse<ExpenditureResultListApiModel>>(
                // Set URL
                    RouteHelpers.GetAbsoluteRoute(ApiRoutes.ReturnExpenditure),
                    mTableName,
                    bearerToken: token);

                // If the response has an error...
                if (await result.HandleErrorIfFailedAsync("Expenditure retrieval Failed"))
                    // We are done
                    return;

                // OK successfully registered (and logged in)... now get aprpropriate tree view data
                //for now; keep a snapshot of persisted data
                mOriginal = result.ServerResponse.Response;
                ;
               
                try
                {
                    //var ExpenditureResultApiModels = mOriginal.ToList();
                    //make a clone of the persisted data for manipulation on front end
                    mPersist = new ExpenditureResultListApiModel();
                    mPersist.Clone(mOriginal, mPersist);
                }
                 catch (Exception e)
                {
                    throw e;
                }


                RefreshExpenditure();


            });
        }

        /// <summary>
        /// Method to refresh element Expenditure
        /// -used when elements of the treefiew are being manipulated on the front end
        /// </summary>
        public void RefreshExpenditure()
        {

            mHDML.Clear();
            //build a tree view, always starting with the root element, which is also the classification for the Expenditure
            mHDML.AddRange(ExpandExpenditureData(mPersist, "00000000-0000-0000-0000-000000000000", "Root"));
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
        /// This funtion builds a Expenditure of elements based on a
        /// a Expenditure result returned when querying a database structure
        /// on which the Expenditure structures are persisted
        /// </summary>
        /// <param name="results"></param>
        /// This is a class of <ExpenditureResultListApiModel></ExpenditureResultListApiModel>
        /// <param name="KCategoryID"></param>
        /// the ID of the parent for finding descendants is passsed through as a string
        /// <returns></returns>
        private ExpenditureListDataModel ExpandExpenditureData(ExpenditureResultListApiModel results, string KCategoryID, string mParentShortName)
        {
            mPersist = results;
            // Find all children
            var TempDate = new DateTime(9999, 12, 31, 0, 0, 0);
            //var children = results.Where(x => x.ParentCategoryID == KCategoryID && x.DateEffective <= DateTime.Today && x.DateDiscontinued == new DateTime(9999,12,31,0,0,0) && !x.IsDeleteElement ).OrderBy(x=>x.ShortName).ToList();//
            //To do:  accept a date parameter to retroactively modify hierarchy data
            //var children = results.Where(x => x.ParentCategoryID == KCategoryID && x.DateEffective <= DateTime.Today && x.DateDiscontinued >  DateTime.Today && !x.IsDeleteElement).OrderBy(x => x.ShortName).ToList();//
            var children = results.Where(x => x.ParentCategoryID == KCategoryID && x.DateEffective <= DateTime.Today && x.DateDiscontinued > DateTime.Today && !x.IsDeleteElement).OrderBy(x => (x.ShortName.ParseInt())).ThenBy(x => x.ShortName).ToList();//
            // Hierarchy cannot be expanded
            if (children.Count() == 0)
                return new ExpenditureListDataModel();
            //...otherwise, return all descendants recursively
            var elements = new ExpenditureListDataModel();
            foreach (var item in children)
            {
                var ud1 = new ExpenditureDataModel
                {
                    //var u = ExpenditureDataModel;
                    ShortName = item.ShortName,
                    Description = item.Description,
                    //Card = item.Card,
                    //Frequency = item.Frequency,
                    KCategoryID = item.KCategoryID,
                    ParentCategoryID = item.ParentCategoryID,
                    DateEffective = item.DateEffective,
                    DateDiscontinued = item.DateDiscontinued,
                    KChangeID = item.KChangeID,
                    IsUnderReview = item.IsUnderReview,


                    //To Do: make provision to add Icons to make the UI more intuitive and attractive
                    //FIconID = item.FIconID,
                    Children = new ExpenditureListDataModel()
                };
                ud1.Children = ExpandExpenditureData(results, ud1.KCategoryID, ud1.ShortName);
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

        public IEnumerator<ExpenditureViewModel> MatchingCategoryEnumerator { get; private set; }

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
            var matches = FindMatches(mSearchText, mRootExpenditureElement);
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

        private IEnumerable<ExpenditureViewModel> FindMatches(string searchText, ExpenditureViewModel Category)
        {
            if (Category.NameContainsText(searchText))
                yield return Category;

            foreach (var child in Category.Children)
                foreach (var match in FindMatches(searchText, child))
                    yield return match;
        }

        #endregion // Search Logic

        #region Search Logic //KCategoryID
        public IEnumerator<ExpenditureViewModel> MatchingKCategoryEnumerator { get; private set; }

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
            var matchK = FindKMatches(mSearchText, mRootExpenditureElement);
            MatchingKCategoryEnumerator = matchK.GetEnumerator();
            _ = !MatchingKCategoryEnumerator.MoveNext();

        }

        private IEnumerable<ExpenditureViewModel> FindKMatches(string searchText, ExpenditureViewModel Category)
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
        public void MoveElement(ExpenditureElementViewModel element)
        {
            mSearchText = element.KCategoryID;
            mParentCategoryID = element.ParentCategoryID;
            mPersistTmp = new ExpenditureResultListApiModel();
            //var sourceElement = from ExpenditureDataModel in this
            //                    where KCategoryID
            var matches = mPersist.Where(x => x.KCategoryID == element.KCategoryID && x.DateEffective <= element.DateDiscontinued).OrderByDescending(x => x.DateEffective).ToList();
            var category = matches.FirstOrDefault();

            //

            if (category != null)
            //Move the selected element by discontinuing it at the current 'Parent'
            //and adding a new element at the new parent location (all descendants are autotomatically
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
                    var mPersistElement = new ExpenditureResultApiModel
                        {
                        ShortName = (element.ShortName.EditedText ?? element.ShortName.OriginalText),
                        Description = (element.Description.EditedText ?? element.Description.OriginalText),
                        //FIconID = category.FIconID,
                        //Frequency = category.Frequency,
                        //FinExpenditureID = category.FinHierarchyID,
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
 
                    var mPersistElement = new ExpenditureResultApiModel
                    {
                    ShortName = (element.ShortName.EditedText ?? element.ShortName.OriginalText),
                    Description = (element.Description.EditedText ?? element.Description.OriginalText),
                    FIconID = category.FIconID,
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
                RefreshExpenditure();
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
        public void CopyElement(ExpenditureElementViewModel element)
        {
            try
            {
                //mSearchText = mCategoryKId;
                mParentCategoryID = element.ParentCategoryID;
                mPersistTmp = new ExpenditureResultListApiModel();
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
                    var mPersistElement = new ExpenditureResultApiModel
                    {
                        ShortName = (element.ShortName.EditedText ?? element.ShortName.OriginalText),
                        Description = (element.Description.EditedText ?? element.Description.OriginalText),
                        FIconID = category.FIconID,
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
                RefreshExpenditure();
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

                //var sourceElement = from ExpenditureDataModel in this
                //                    where KCategoryID
                var matches = from category in mPersist
                              where category.ParentCategoryID == mCategoryKId && category.DateDiscontinued == new DateTime(9999,12,31) && category.DateEffective <= (DateTime.Today)
                              select category;
                foreach (var category in matches)


                {
                    var mPersistElement = new ExpenditureResultApiModel
                    {
                        FIconID = category.FIconID,
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
        public void DeleteElement(ExpenditureElementViewModel element)
        {
            try
            {
                //mSearchText = mCategoryKId;
                var mElement = element as ExpenditureElementViewModel;


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
                        mPersistTmp = new ExpenditureResultListApiModel();
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


                RefreshExpenditure();
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
        public void DeleteElement1(string mCategoryKId, ExpenditureElementViewModel element)
        {
            var mElement = element as ExpenditureElementViewModel;
            var catprev = (mPersist.Where(x => x.KCategoryID == element.KCategoryID && x.DateEffective == element.DateEffective).OrderByDescending(x => x.DateEffective).ToList()).FirstOrDefault();
            try
            {

                //var sourceElement = from ExpenditureDataModel in this
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
                            mPersistTmp = new ExpenditureResultListApiModel();
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
        /// Add a new element to the Expenditure Tree
        /// The calling programme is to generate a GUID for the new element
        /// </summary>
        /// <param name="mNewElement"></param>
        public void AddElement(ExpenditureElementViewModel element)
        {
            mSearchText = element.KCategoryID;
            //Gemerate GUID for root of new Expenditure element
            var mRoot = element.Root.EditedText == element.Root.OriginalText ? Guid.NewGuid().ToString().ToUpper() : element.Root.EditedText;
            var mPersistElement = new ExpenditureResultApiModel
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
                //FExpenditureID = mTableName,
                //Create new root element if not already existing
            };
            mPersist.Add(mPersistElement);


            RefreshExpenditure();
            PerformKIdSearch();
            //TO DO: Add code to create root element of Expenditure when creating a new Expenditure type menu item
            //if page == 'Expenditure', create new element guid(), use Expenditure name +description, parent = 00000000

        }

        /// <summary>
        /// Edit element in hiearchy tree

        /// </summary>
        /// <param name="element"></param>
        public void EditElement(ExpenditureElementViewModel element)
        {
            mSearchText = element.KCategoryID;

            var matches = from category in mPersist
                          where category.KCategoryID == element.KCategoryID && category.DateDiscontinued == new DateTime(9999,12,31)// && (category.DateEffective <= element.DateDiscontinued)
                          select category;
            foreach (var category in matches)
                //if this is a newly added element, just update the instance
                if (category.ShortName != (element.ShortName.EditedText ?? element.ShortName.OriginalText) || category.Description != (element.Description.EditedText ?? element.Description.OriginalText)
                        || category.DateEffective != element.DateEffective)
                { 
                    if (category.DateEffective == element.DateEffective)
                    category.KChangeID = element.KChangeID;
                    category.IsUnderReview = true;
                    category.ShortName = element.ShortName.EditedText ?? element.ShortName.OriginalText;
                    category.Description = element.Description.EditedText ?? element.Description.OriginalText;
                    mSearchText = category.KCategoryID;

                }
                else
                {
                    category.DateDiscontinued =  element.DateEffective.AddSeconds(-10);
                    category.KChangeID = element.KChangeID;
                    category.IsUnderReview = true;

                    var mPersistElement = new ExpenditureResultApiModel
                    {
                        ShortName = (element.ShortName.EditedText ?? element.ShortName.OriginalText),
                        Description = (element.Description.EditedText ?? element.Description.OriginalText),
                        ParentCategoryID = element.ParentCategoryID,
                        DateDiscontinued = element.DateDiscontinued,
                        DateEffective = element.DateEffective,
                    //Unique ID for change element
                        KCategoryID = element.KCategoryID,
                        IsUnderReview = true,
                        IsNewElement = true,
                        FHierarchyID = category.FHierarchyID,
                        KChangeID = element.KChangeID
                    };
                    mPersist.Add(mPersistElement);

                   mSearchText = mPersistElement.KCategoryID;
                }
 
                    RefreshExpenditure();
                    PerformKIdSearch();
                    return;


        }

        #endregion //Tree Manipulation

        public void Close()
        {
            // Close settings menu
            ViewModelApplication.SideMenuVisible = true;
            //ViewModelApplication.CurrentSideMenuViewModel = null;
            ViewModelApplication.GoToPage(ApplicationPage.Chat);


        }

        /// <summary>
        /// Persist all items changed or added on Expenditure to back end database. Depending on stage
        /// of change control, changes may be forwarded for recommendation or finally approved and implemented
        /// on back end
        /// </summary>
        public async Task PersistExpenditureChangesAsync()
        {
            await PersistExpenditureAsync();
            Close();
        }
        public async Task PersistExpenditureAsync()
        {
            await RunCommandAsync(() => ExpenditureBuildIsRunning, async () =>
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
                var result = await WebRequests.PostAsync<ApiResponse<ExpenditureResultListApiModel>>(
                // Set URL
                    RouteHelpers.GetAbsoluteRoute(ApiRoutes.PersistHierarchy),
                    mPersist,
                    bearerToken: token);

                // If the response has an error...
                if (await result.HandleErrorIfFailedAsync("Expenditure retrieval Failed"))
                    // We are done
                    return;

                // return to menu


            });
        }



    }

}