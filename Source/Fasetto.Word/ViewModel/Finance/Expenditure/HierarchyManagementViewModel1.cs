using Dna;
using Fasetto.Word.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using static Fasetto.Word.DI;

namespace Fasetto.Word
{
    /// <summary>
    /// The settings state as a view model
    /// </summary>
    public class HierarchyManagementViewModel1 : BaseViewModel
   
    {

        #region Public Properties

        ///
        /// <summary>
        /// string representation of UniqueIdentifier for a Category element
        /// </summary>
        public string FinHierarchyID { get; set; }

        /// <summary>
        ///name of Category element
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        ///description of Category element
        /// </summary>
        /// 
        public string Description { get; set; }

        /// <summary>
        //string representation of card where the expense category is determined by the linked card
        /// </summary>
        /// 
        public string Card { get; set; }

        /// <summary>
        //integer indicating the number of months between expected occurrences of expense category
        /// </summary>
        public int Frequency { get; set; }

        /// <summary>
        //string representation of GUID for a Category element
        /// </summary>
        public string KCategoryID { get; set; }

        /// <summary>
        //sstring representation of GUID for the Parent category of a Category element
        //the parent of all root elements will be NULL... any hierarchy will have at least one root element
        /// </summary>
        public string ParentCategoryId { get; set; }

        /// <summary>
        //the link to tthe ICON used to depict this category
        /// </summary>
        public string FIconId { get; set; }

        /// <summary>
        //sub categories, each of which is also a category itself
        /// </summary>
        public List<HierarchyManagementViewModel> Children { get; set; }


        /// <summary>
        /// A flag indicating if the register command is running
        /// </summary>
        public bool HiearachyBuildIsRunning { get; set; }

        #endregion
        #region Commands

        /// <summary>
        /// The command to login
        /// </summary>
        public ICommand RetrieveExpenseHierarchy{ get; set; }

        /// <summary>
        /// The command for when the user clicks the send button
        /// </summary>
        public ICommand SendCommand { get; set; }

        #endregion
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public HierarchyManagementViewModel1()
        {
            // Create commands

            RetrieveExpenseHierarchy = new RelayCommand(async () => await ExpenseHierarchyAsync());
        }

        #endregion

        /// <summary>
        /// Attempts to return a HierarchyManagement data set
        /// </summary>
        /// <param name="parameter">The <see cref="SecureString"/> passed in from the view for the users password</param>
        /// <returns></returns>
        public async Task ExpenseHierarchyAsync()
        {
            await RunCommandAsync(() => HiearachyBuildIsRunning, async () =>
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



    }

}
