
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
    public class HierarchyTreeViewModel : BaseViewModel

    {

        #region Public Properties

        /// <summary>
        /// A list of all registered hierarchy elements
        /// </summary>
        public ObservableCollection<HierarchyViewModel> FirstGeneration { get; set; }


        //public ObservableCollection<HierarchyViewModel> FirstGeneration1 { get; set; }

        #endregion

        #region Data

        //private readonly ReadOnlyCollection<HierarchyViewModel> mFirstGeneration;
        protected HierarchyViewModel mRootHierarchyElement;
        protected HierarchyViewModel mRootHierarchyElement1;
        private readonly ICommand mSearchCommand;
        public HierarchyListDataModel mHDML ;
        public HierarchyResultListApiModel mPersist,mPersistTmp;
        public HierarchyDataModel mHDM;
        public string mTableName;
        public HierarchyElementViewModel mElement;

        //IEnumerator<HierarchyManagementViewModel> mMatchingCategoryEnumerator;

        //public HierarchyManagementTreeViewModel(IEnumerator<HierarchyManagementViewModel> matchingCategoryEnumerator)
        //{
        //    MatchingCategoryEnumerator = matchingCategoryEnumerator;
        //}

        private string mSearchText = string.Empty, mSearchKCategoryID = string.Empty, mParentCategoryID = string.Empty;

        #endregion // Data

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
        public HierarchyTreeViewModel(string hierarchyTable)
        {
            #region Dummy Root HierarchyListDataModel
            mHDML = new HierarchyListDataModel();
            mHDM = new HierarchyDataModel
            {
                KCategoryID = new Guid().ToString(),
                ParentCategoryID = "",
                Description = "...Loading hierarchy data...",
                ShortName = "Loading...Please be patient",
                Children = new HierarchyListDataModel()
            };
            mHDML.Add(mHDM);

            mTableName = hierarchyTable;
            #endregion
            //retrieve hierarchy from persistent storage on server
            //To Do: Add mTableName as parameter when calling HiearchyAsync to populate hierarchy
            TaskManager.RunAndForget(HierarchyAsync);


            // Get the OptFinHierarchies currently configured - first populate 'root hierarchy' variable with all configured root hierarchy elements currently available


            UpdateTreeViewElements();


            mSearchCommand = new SearchCategoryTreeCommand(this);
        }

        private void UpdateTreeViewElements()

        {

            var rootElement = mHDML.FirstOrDefault(x => x.ParentCategoryID == "");
            mRootHierarchyElement = new HierarchyViewModel(rootElement);

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
                var result = await WebRequests.PostAsync<ApiResponse<HierarchyResultListApiModel>>(
                // Set URL
                    RouteHelpers.GetAbsoluteRoute(ApiRoutes.ReturnHierarchy),
                    mTableName,
                    bearerToken: token);

                // If the response has an error...
                if (await result.HandleErrorIfFailedAsync("Hierarchy retrieval Failed"))
                    // We are done
                    return;

                // OK successfully registered (and logged in)... now get users data
                var expenseHierarchy = result.ServerResponse.Response;
                mPersist = expenseHierarchy;
                //convert response into HierarchyListDataModel
                //mHDML.Clear();
                //mHDML.AddRange(ExpandHierarchyData(expenseHierarchy, "","Root"));
                ////Update the viewModel with the returned values

                //UpdateTreeViewElements();
                RefreshHierarchy();


            });
        }

        /// <summary>
        /// Method to refresh element Hierarchy 
        /// </summary>
        public void RefreshHierarchy()
        {

            mHDML.Clear();
            mHDML.AddRange(ExpandHierarchyData(mPersist, "", "Root"));

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
            var children = results.Where(x => x.ParentCategoryID == KCategoryID && x.DateDiscontinued == new DateTime() && x.DateEffective <= DateTime.Today).ToList();

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
                    ParentShortName  = mParentShortName,
                    DateEffective = item.DateEffective,
                    DateDiscontinued = item.DateDiscontinued,
                    KChangeID = item.KChangeID,
                    //FIconID = item.FIconID,
                    Children = new HierarchyListDataModel()
                };
                ud1.Children = ExpandHierarchyData(results, ud1.KCategoryID,ud1.ShortName);
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

        private void PerformSearch()
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
            //Ensure that selected element is the 'current' one
            {
                if (category.DateEffective == element.DateEffective)
                //move to new parent
                {
                    category.ParentCategoryID = element.ParentCategoryID;
                    mSearchText = element.KCategoryID;
                }
                else
                {
                    //Create a copy of the previous element with different parent
                    var mPersistElement = new HierarchyResultApiModel
                    {
                        ShortName = (element.ShortName.EditedText ?? element.ShortName.OriginalText),
                        Description = (element.Description.EditedText ?? element.Description.OriginalText),
                        FIconID = category.FIconID,
                        Frequency = category.Frequency,
                        FinHierarchyID = category.FinHierarchyID,
                        ParentCategoryID = mParentCategoryID,
                        DateEffective = element.DateDiscontinued,
                        KCategoryID = Guid.NewGuid().ToString().ToUpper(),
                        KChangeID = element.KChangeID
                    };
                    mPersist.Add(mPersistElement);
                    //terminate the previous position of the element, and link to the change control
                    category.DateDiscontinued = element.DateDiscontinued;
                    category.KChangeID = element.KChangeID;

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
                              where category.KCategoryID == element.KCategoryID && category.DateDiscontinued == new DateTime() && category.DateEffective <= DateTime.Today

                              select category;
            foreach (var category in matches)


            {
                    //mSearchText = category.ShortName;
                    var mPersistElement = new HierarchyResultApiModel
                    {
                        ShortName = "Copy Of " + category.ShortName,
                        Description = category.Description,
                        FIconID = category.FIconID,
                        Frequency = category.Frequency,
                        FinHierarchyID = category.FinHierarchyID,
                        ParentCategoryID = mParentCategoryID,
                        DateEffective = mElement.DateDiscontinued,
                        KCategoryID = Guid.NewGuid().ToString().ToUpper(),
                        KChangeID = mElement.KChangeID
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
                              where category.ParentCategoryID == mCategoryKId && category.DateDiscontinued == new DateTime() && category.DateEffective <= (DateTime.Today)
                              select category;
                foreach (var category in matches)


                {
                    var mPersistElement = new HierarchyResultApiModel
                    {
                        FIconID = category.FIconID,
                        Frequency = category.Frequency,
                        FinHierarchyID = category.FinHierarchyID,
                        ParentCategoryID = mParentKId,
                        KCategoryID = Guid.NewGuid().ToString().ToUpper(),
                        ShortName = "Copy Of " + category.ShortName,
                        Description = category.Description,
                        DateEffective = mElement.DateDiscontinued,
                        KChangeID = mElement.KChangeID
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

                //mSearchText = element.KCategoryID;

                //var sourceElement = from HierarchyDataModel in this
                //                    where KCategoryID
                var matches = from category in mPersist
                              where category.KCategoryID == element.KCategoryID
                              select category;
                foreach (var category in matches)


                {
                    //mSearchText = category.ShortName;

                    category.DateDiscontinued = mElement.DateDiscontinued;
                    mSearchText = mElement.KCategoryID;
                    DeleteElement1(mElement.KCategoryID);
                }


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
        public void DeleteElement1(string mCategoryKId)
        {
            try
            {

                //var sourceElement = from HierarchyDataModel in this
                //                    where KCategoryID
                var matches = from category in mPersist
                              where category.ParentCategoryID == mCategoryKId
                              select category;
                foreach (var category in matches)


                {
 
                    {
                        category.DateDiscontinued = mElement.DateDiscontinued;
                    };

                    DeleteElement1(category.KCategoryID );

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
            var mPersistElement = new HierarchyResultApiModel
            {
                ShortName = (element.ShortName.EditedText ?? element.ShortName.OriginalText),
                Description = (element.Description.EditedText ?? element.Description.OriginalText),
                ParentCategoryID = element.ParentCategoryID,
                KCategoryID = element.KCategoryID,
                KChangeID = element.KChangeID
            };
            mPersist.Add(mPersistElement);


            RefreshHierarchy();
            PerformKIdSearch();

        }

        /// <summary>
        /// Edit element in hiearchy tree

        /// </summary>
        /// <param name="element"></param>
        public void EditElement(HierarchyElementViewModel element)
        {
            mSearchText = element.KCategoryID;

            var matches = from category in mPersist
                          where category.ParentCategoryID == element.KCategoryID && category.DateDiscontinued == new DateTime() && category.DateEffective < element.DateDiscontinued
                          select category;
            foreach (var category in matches)
                if (category.ShortName != (element.ShortName.EditedText ?? element.ShortName.OriginalText) || category.ShortName != (element.Description.EditedText ?? element.Description.OriginalText)
                        || category.DateEffective == element.DateEffective )
                {
                    category.DateDiscontinued = element.DateDiscontinued;
                    category.KChangeID = element.KChangeID;

                    var mPersistElement = new HierarchyResultApiModel
                {
                    ShortName = (element.ShortName.EditedText ?? element.ShortName.OriginalText),
                    Description = (element.Description.EditedText ?? element.Description.OriginalText),
                    ParentCategoryID = element.ParentCategoryID,
                    KCategoryID = element.KCategoryID,
                    KChangeID = element.KChangeID
                };
                    mPersist.Add(mPersistElement);

                    RefreshHierarchy();
                    PerformKIdSearch();
                }

        }

        #endregion //Tree Manipulation





    }
}