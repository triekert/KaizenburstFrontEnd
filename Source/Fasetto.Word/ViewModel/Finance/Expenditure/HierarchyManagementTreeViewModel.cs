
using Dna;
using Fasetto.Word.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static Fasetto.Word.DI;


namespace Fasetto.Word
{
    /// <summary>
    /// This is the view-model of the UI.  It provides a data source
    /// for the TreeView (the FirstGeneration property), a bindable
    /// SearchText property, and the SearchCommand to perform a search.
    /// </summary>
    public class HierarchyManagementTreeViewModel : BaseViewModel

    {
        #region Data

        private readonly ReadOnlyCollection<HierarchyManagementViewModel> mFirstGeneration;
        private readonly HierarchyManagementViewModel mRootCategory;
        private readonly ICommand mSearchCommand;
        //IEnumerator<HierarchyManagementViewModel> mMatchingCategoryEnumerator;

        //public HierarchyManagementTreeViewModel(IEnumerator<HierarchyManagementViewModel> matchingCategoryEnumerator)
        //{
        //    MatchingCategoryEnumerator = matchingCategoryEnumerator;
        //}

        private string mSearchText = string.Empty;

        #endregion // Data

        #region Constructor

        public HierarchyManagementTreeViewModel(HierarchyManagementTreeDataModel rootCategory)
        {
            mRootCategory = new HierarchyManagementViewModel(rootCategory);

            mFirstGeneration = new ReadOnlyCollection<HierarchyManagementViewModel>(
                new HierarchyManagementViewModel[]
                {
                    mRootCategory
                });

            mSearchCommand = new SearchCategoryTreeCommand(this);
        }

        #endregion // Constructor

        #region Properties
            #region Public Properties



            /// <summary>
            /// A flag indicating if the login command is running
            /// </summary>
            public bool HierarchyBuildIsRunning { get; set; }
            #endregion

        #region FirstGeneration

        /// <summary>
        /// Returns a read-only collection containing the first Category 
        /// in the Category tree, to which the TreeView can bind.
        /// </summary>
        public ReadOnlyCollection<HierarchyManagementTreeViewModel> FirstGeneration { get; }

        #endregion // FirstGeneration

        #region SearchCommand

        /// <summary>
        /// Returns the command used to execute a search in the Category tree.
        /// </summary>
        public ICommand SearchCommand => mSearchCommand;

        private class SearchCategoryTreeCommand : ICommand
        {
            private readonly HierarchyManagementTreeViewModel mCategoryTree;

            public SearchCategoryTreeCommand(HierarchyManagementTreeViewModel CategoryTree)
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

        /// <summary>
        /// Return Hierarchy of interest from Object persistance infrastructure
        /// User credentials are used to determine access authorisation
        /// </summary>
        /// <returns></returns>
        public async Task ExpenseHierarchyAsync()
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
                var result = await WebRequests.PostAsync<ApiResponse<ExpenseHierarchyResultsApiModel>>(
                // Set URL
                    RouteHelpers.GetAbsoluteRoute(ApiRoutes.ReturnExpenseHierarchy),
                    bearerToken: token);

                // If the response has an error...
                if (await result.HandleErrorIfFailedAsync("Hierarchy retrieval Failed"))
                    // We are done
                    return;

                // OK successfully registered (and logged in)... now get users data
                var expenseHiearchy = result.ServerResponse.Response;

                // Let the application view model handle what happens
                // with the successful login
                //await ViewModelApplication.HandleSuccessfulLoginAsync(loginResult);

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

        public IEnumerator<HierarchyManagementViewModel> MatchingCategoryEnumerator { get; private set; }

        #endregion // SearchText

        #endregion // Properties

        #region Search Logic

        private void PerformSearch()
        {
            if (MatchingCategoryEnumerator == null || !MatchingCategoryEnumerator.MoveNext())
                VerifyMatchingCategoryEnumerator();

            var Category = MatchingCategoryEnumerator.Current;

            if (Category == null)
                return;

            // Ensure that this Category is in view.
            if (Category.Parent != null)
                Category.Parent.IsExpanded = true;

            Category.IsSelected = true;
        }

        private void VerifyMatchingCategoryEnumerator()
        {
            var matches = FindMatches(mSearchText, mRootCategory);
            MatchingCategoryEnumerator = matches.GetEnumerator();

            if (!MatchingCategoryEnumerator.MoveNext())
            {
                MessageBox.Show(
                    "No matching names were found.",
                    "Try Again",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                    );
            }
        }

        private IEnumerable<HierarchyManagementViewModel> FindMatches(string searchText, HierarchyManagementViewModel Category)
        {
            if (Category.NameContainsText(searchText))
                yield return Category;

            foreach (var child in Category.Children)
                foreach (var match in FindMatches(searchText, child))
                    yield return match;
        }

        #endregion // Search Logic
    }
}