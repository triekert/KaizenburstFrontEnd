
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



        //private void UpdateHierarchyDataModel(HierarchyResultListApiModel result)
        //{
        //    var rootElement = from element in mHDML
        //                      where element.ParentCategoryID == ""
        //                      select (element.ShortName, element.Description, element.KCategoryID, element.ParentCategoryID);
        //    mRootCategory = new HierarchyViewModel(rootElement.First().ShortName, rootElement.First().Description, rootElement.First().KCategoryID, mHDML);

        //    // Create the view models from the data
        //    FirstGeneration = new ObservableCollection<HierarchyViewModel>(
        //        rootElement.Select(root => new HierarchyViewModel(root.ShortName, root.Description, root.KCategoryID, mHDML)));
        //}

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
                //convert response into HierarchyListDataModel
                mHDML.Clear();
                mHDML.AddRange(ExpandHierarchyData(expenseHierarchy, ""));
                //Update the viewModel with the returned values

                UpdateTreeViewElements();


            });
        }

        public void RefreshHierarchy()
        {
            //await RunCommandAsync(() => HierarchyBuildIsRunning, async () =>
            //{

                //Store single transcient instance of client data store
                //var scopedClientDataStore = ClientDataStore;

                // Update values from local cache
                // Get the user token
                //var token = (await scopedClientDataStore.GetLoginCredentialsAsync())?.Token;
                //// Call the server and attempt to register with the provided credentials
                //// If we don't have a token (then not logged in...)
                //if (string.IsNullOrEmpty(token))
                //    // Then do nothing more
                //    return;
                //var result = await WebRequests.PostAsync<ApiResponse<HierarchyResultListApiModel>>(
                //// Set URL
                //    RouteHelpers.GetAbsoluteRoute(ApiRoutes.ReturnHierarchy),
                //    mTableName,
                //    bearerToken: token);

                // If the response has an error...
                //if (await result.HandleErrorIfFailedAsync("Hierarchy retrieval Failed"))
                //    // We are done
                //    return;

                // OK successfully registered (and logged in)... now get users data
                //var expenseHierarchy = result.ServerResponse.Response;
                //convert response into HierarchyListDataModel
                mHDML.Clear();
                mHDML.AddRange(ExpandHierarchyData(mPersist, ""));
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
        private HierarchyListDataModel ExpandHierarchyData(HierarchyResultListApiModel results, string KCategoryID)
        {
            mPersist = results;
            // Find all children
            var children = results.Where(x => x.ParentCategoryID == KCategoryID).ToList();

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
                    Card = item.Card,
                    Frequency = item.Frequency,
                    KCategoryID = item.KCategoryID,
                    ParentCategoryID = item.ParentCategoryID,
                    FIconID = item.FIconID,
                    Children = new HierarchyListDataModel()
                };
                ud1.Children = ExpandHierarchyData(results, ud1.KCategoryID);
                elements.Add(ud1);
            }

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
        public void MoveElement(string mCategoryKId, string mParentKId)
        {
            mSearchText = mCategoryKId;
            mParentCategoryID = mParentKId;
            //var sourceElement = from HierarchyDataModel in this
            //                    where KCategoryID
            var matches = from category in mPersist
                          where category.KCategoryID == mCategoryKId
                          select category;
            foreach (var category in matches)

            { category.ParentCategoryID = mParentCategoryID;
                mSearchText = category.KCategoryID;
            }


            RefreshHierarchy();
            PerformKIdSearch();

        }
        /// <summary>
        /// Copy Hierarchy Element from one location to another (allocate to a different parent Element)
        /// Simultaneously, copies must be made of all descendents and these 2 must be inserted as descendents
        /// of the newly copied apex element
        /// </summary>
        /// <param name="mCategoryKId"></param>
        /// <param name="mParentKId"></param>
        public void CopyElement(string mCategoryKId, string mParentKId)
        {
            try
            { 
            //mSearchText = mCategoryKId;
            mParentCategoryID = mParentKId;
                mPersistTmp = new HierarchyResultListApiModel();

                //var sourceElement = from HierarchyDataModel in this
                //                    where KCategoryID
                var matches = from category in mPersist
                          where category.KCategoryID == mCategoryKId
                          select category;
            foreach (var category in matches)


            {
                    mSearchText = category.ShortName;
                    var mPersistElement = new HierarchyResultApiModel
                    {
                        ShortName = category.ShortName,
                        Description = category.Description,
                        FIconID = category.FIconID,
                        Frequency = category.Frequency,
                        FinHierarchyID = category.FinHierarchyID,
                        ParentCategoryID = mParentKId,
                        DateEffective = DateTime.Now,
                        KCategoryID = Guid.NewGuid().ToString().ToUpper()
                    };
                    mPersistTmp.Add(mPersistElement);
                    CopyElement1(mCategoryKId, mPersistElement.KCategoryID);
            }

            mPersist.AddRange(mPersistTmp);
            RefreshHierarchy();
            PerformSearch();
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
                          where category.ParentCategoryID == mCategoryKId
                          select category;
            foreach (var category in matches)


            {
                    var mPersistElement = new HierarchyResultApiModel
                    {
                        ShortName = category.ShortName,
                        Description = category.Description,
                        FIconID = category.FIconID,
                        Frequency = category.Frequency,
                        FinHierarchyID = category.FinHierarchyID,
                        ParentCategoryID = mParentKId,
                        DateEffective = DateTime.Now,
                        KCategoryID = Guid.NewGuid().ToString().ToUpper()
                    };
                    mPersistTmp.Add(mPersistElement);
                CopyElement1(category.KCategoryID, mPersistElement.KCategoryID);

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
        public void AddElement(HierarchyElementViewModel mNewElement)
        {
            mSearchText = mNewElement.KCategoryID;
            var mPersistElement = new HierarchyResultApiModel
            {
                ShortName = mNewElement.ShortName.EditedText,
                Description = mNewElement.Description.EditedText,
                ParentCategoryID = mNewElement.ParentCategoryID,
                KCategoryID = mNewElement.KCategoryID
            };
            mPersist.Add(mPersistElement);


            RefreshHierarchy();
            PerformKIdSearch();

        }

        #endregion //Tree Manipulation





    }
}