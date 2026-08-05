
using Dna;
using Fasetto.Word.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using static Fasetto.Word.Core.CoreDI;
using System.Windows.Forms;
using static Fasetto.Word.DI;
using System.Windows;


namespace Fasetto.Word
{
    /// <summary>
    /// This is the view-model of the UI.  It provides a data source
    /// for the TreeView (the FirstGeneration property), a bindable
    /// SearchText property, and the SearchCommand to perform a search.
    /// </summary>
    public class BudgetPeriodListViewModel : BaseViewModel

    {



        #region Public Properties

        /// <summary>
        /// The label to identify what this value is for
        /// </summary>
        public string Label { get; set; }



        /// <summary>
        /// Indicates if the current text is in edit mode
        /// </summary>
        public bool Editing { get; set; }

        /// <summary>
        /// Indicates if the current control is pending an update (in progress)
        /// </summary>
        public bool Working { get; set; }

        /// <summary>
        /// Store View Model of current popup to allow reverse navigation
        /// </summary>
        public object PriorPopupViewModel { get; set; }


        // <summary>
        /// Level limit for hierarchy's to be returned (1 = top level only...)
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// The action to run when saving the text.
        /// Returns true if the commit was successful, or false otherwise.
        /// </summary>
        public Func<Task<bool>> CommitAction { get; set; }


        /// <summary>
        /// The action to run when initiating the control.
        /// Returns true if the preparation was successful, or false otherwise.
        /// </summary>
        public Func<Task<bool>> PrepareAction { get; set; }

        #endregion

        #region Public Commands

        /// <summary>
        /// Puts the control into edit mode
        /// </summary>
        public ICommand EditCommand { get; set; }

        /// <summary>
        /// Cancels out of edit mode
        /// </summary>
        public ICommand CancelCommand { get; set; }

        /// <summary>
        /// Commits the edits and saves the value
        /// as well as goes back to non-edit mode
        /// </summary>
        public ICommand SaveCommand { get; set; }
        /// <summary>
        /// The command to close the settings menu
        /// </summary>
        public ICommand CloseCommand { get; set; }





        /// <summary>
        /// A set of Bulk Meter Recon records for the selected period
        /// </summary>
        public ObservableCollection<BudgetPeriodDataModel> BudgetPeriodList{ get; set; }

        /// <summary>
        /// The selected Billing Period view model
        /// </summary>
        public BudgetPeriodDataModel MSelectedBudgetPeriod{ get; set; }

        //public ObservableCollection<HierarchyViewModel> FirstGeneration1 { get; set; }

        #endregion
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





        #endregion //Properties

        #region Data

        public BudgetPeriodDataModel mCHVM;
        public BudgetPeriodResultApiModel mRequest;




        private string mSearchText = "", mSearchKCategoryID = string.Empty, mParentCategoryID = string.Empty;

        #endregion // Data

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
        public BudgetPeriodListViewModel(string costHierarchy)
        {
            #region Build HierarchyViewCollection
            BudgetPeriodList = new ObservableCollection<BudgetPeriodDataModel> {

             new BudgetPeriodDataModel
            {

                Name = "Loading Budgets for selected cost hierarchy...Please be patient",
                KBudgetID = "Test1",
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
            if (costHierarchy == null)
            {
                System.Windows.MessageBox.Show(
                          "No Cost Hierarchy has been selected.",
                          "Search for Tree Item failed",
                          MessageBoxButton.OK,
                          MessageBoxImage.Information
                          );
                return;

            }
            mRequest = new BudgetPeriodResultApiModel
            {
                CostHierarchy = costHierarchy,
                //Set Generic Root lookup to return hierarchies of type 'Cost Hierarchy'
                //HierarchyTypeID = "64413ae7-822f-4866-9ebe-433083d699ac"
            };
            TaskManager.RunAndForget(BudgetPeriodAsync);
            CloseCommand = new RelayCommand(Close);

        }






        #endregion // Constructor

        /// <summary>
        /// Return Hierarchy of interest from Object persistence infrastructure
        /// User credentials are used to determine access authorisation
        /// </summary>
        /// <returns></returns>
        public async Task BudgetPeriodAsync()
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
                var token = ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).Token;
                // Call the server and attempt to register with the provided credentials
                // If we don't have a token (then not logged in...)
                if (string.IsNullOrEmpty(token))
                    // Then do nothing more
                    return;
                var result = await WebRequests.PostAsync<ApiResponse<BudgetPeriodResultListApiModel>>(
                // Set URL
                    RouteHelpers.GetAbsoluteRoute(ApiRoutes.ReturnBudgetsList),
                    mRequest,
                    bearerToken: token);

                // If the response has an error...
                if (await result.HandleErrorIfFailedAsync("Budget List retrieval Failed"))
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
                    BudgetPeriodList = new ObservableCollection<BudgetPeriodDataModel>();
                    //BulkRecon.Clear();
                    var matches = result.ServerResponse.Response.ToList();


                    foreach (var item in matches)
                    {

                    var mCHVM = new BudgetPeriodDataModel

                        {
                            KBudgetID = item.KBudgetID,
                            Name = item.Name,
                            MonthStart = item.MonthStart,
                            MonthEnd = item.MonthEnd,
                        };
                        BudgetPeriodList.Add(mCHVM); 
                    }


                }
                 catch (Exception e)
                {
                    throw e;
                }


            });
        }

 

        public void Close()
        {
            // Close settings menu


            ViewModelApplication.PopupVisible = false;



        }




    }

}