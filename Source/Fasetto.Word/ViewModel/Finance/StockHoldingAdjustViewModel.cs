using Dna;
using Fasetto.Word.Core;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using static Fasetto.Word.DI;
using static Fasetto.Word.Core.CoreDI;
using System.Globalization;
using Fasetto.Word.Core.ApiModels.Controls;
using System.Linq;
using System.Threading.Channels;
using System.Security.Cryptography;
using System.Collections.ObjectModel;
using System.Security.Principal;
namespace Fasetto.Word
{
    public class StockHoldingAdjustViewModel : BaseViewModel
    {
        #region Private Members

        /// <summary>
        /// The text to show while loading text
        /// </summary>
        private string mLoadingText = "...";

        #endregion

        #region Public Properties


        /// <summary>
        /// Field containing current stock count for item
        /// </summary>
        public TextEntryViewModel StockUnits { get; set; }




        /// <summary>
        /// Field containing the current SOH for the selected item (as calculataed)
        /// </summary>

        public int SOH;


        public string KCategoryID { get; set; }

        /// <summary>
        /// Parent ID  of hierarchy item
        /// </summary>
        public string ParentCategoryID { get; set; }

        /// <summary>
        /// Parent ShortName of hierarchy item
        /// </summary>
        public string ParentShortName { get; set; }

        /// <summary>
        ///Amount budgeted directly for the selected element (excluding descendant aggregates)
        /// </summary>
        public string BudgetAmountStr { get; set; }

        /// <summary>
        /// Aggregate Total for element and all descendants
        /// </summary>
        public string BudgetAmountTotalStr { get; set; }
        /// <summary>
        /// Aggregate Total for element and all descendants
        /// </summary>
        public string ActualAmountTotalStr { get; set; }

        /// <summary>
        /// Aggregate Total for element and all descendants
        /// </summary>
        public string BudgetAmountCumTotalStr { get; set; }
        /// <summary>
        /// Aggregate Total for element and all descendants
        /// </summary>
        public string ActualAmountCumTotalStr { get; set; }

        /// <summary>
        /// Aggregate Total for element and all descendants
        /// </summary>
        public string DeviationStr { get; set; }
        /// <summary>
        /// Aggregate Total for element and all descendants
        /// </summary>
        public string DeviationCumStr { get; set; }

        /// <summary>
        ///Amount budgeted directly for the selected element (excluding descendant aggregates)
        /// </summary>
        public decimal BudgetAmountDec { get; set; }

        /// <summary>
        /// Aggregate Total for element and all descendants
        /// </summary>
        public decimal BudgetAmountTotalDec { get; set; }

        /// <summary>
        ///Amount budgeted directly for the selected element (excluding descendant aggregates)
        /// </summary>
        public decimal BudgetAmountAdjDec { get; set; }

        /// <summary>
        /// Aggregate Total for element and all descendants
        /// </summary>
        public decimal BudgetAmountTotalAdjDec { get; set; }

        /// <summary>
        /// Integer representing the budget month
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        ///Flag to indicate stock tracking on classification
        /// </summary>
        public bool IsStockTracked { get; set; }

        /// <summary>
        /// Calendar date from which Element is deactivated
        /// </summary>
        public DateTime DateStart { get; set; }


        /// <summary>
        /// Calendar date from which Element is deactivated
        /// </summary>
        public DateTime DateEnd { get; set; }

        /// <summary>
        /// Attach the current activity to a Change object
        /// </summary>
        public string KChangeID { get; set; }

        /// <summary>
        /// The text for the add Node button
        /// </summary>
        public string AdjustmentButtonText { get; set; }

        /// <summary>
        /// The text for the add Node button
        /// </summary>
        public string StockHoldingButtonText { get; set; }
        /// <summary>
        /// The text for the Edit Node button
        /// </summary>
        public string EditNodeButtonText { get; set; }
        /// <summary>
        /// The text for the Delete Node button
        /// </summary>
        public string DeleteNodeButtonText { get; set; }
        /// <summary>
        /// The text for the Copy Node button
        /// </summary>
        public string CopyNodeButtonText { get; set; }
        /// <summary>
        /// The text for the Move Node button
        /// </summary>
        public string MoveNodeButtonText { get; set; }


        public string HeadingText { get; set; } 

        /// <summary>
        /// A flag indicating whether processing of SOH adjustment has been completed
        /// </summary>
        /// 
        public bool UpdateSOHCompleted { get; set; }


        /// <summary>
        /// API parameter model
        /// </summary>
        /// 
        public  StockHoldingApiModel MRequest { get; set; }



        #region Transactional Properties

        /// <summary>
        /// Indicates if the the SOH is currently being retrieved
        /// </summary>
        public bool SOHRetrievalIsRunning { get; set; }

        ///// <summary>
        ///// Indicates if the first name is being saved
        ///// </summary>
        //public bool FirstNameIsSaving { get; set; }

        /// <summary>
        /// Indicates if the SOH  is currently being saved
        /// </summary>
        public bool SOHIsSaving { get; set; }

        /// <summary>
        /// Indicates if the username is current being saved
        /// </summary>
        public bool UsernameIsSaving { get; set; }

        /// <summary>
        /// Indicates if the email is current being saved
        /// </summary>
        public bool EmailIsSaving { get; set; }

        /// <summary>
        /// Indicates if the password is current being changed
        /// </summary>
        public bool PasswordIsChanging { get; set; }

        /// <summary>
        /// Indicates if the settings details are currently being loaded
        /// </summary>
        public bool SettingsLoading { get; set; }

        /// <summary>
        /// Indicates if the user is currently logging out
        /// </summary>
        public bool LoggingOut { get; set; }

        /// <summary>
        /// A flag indicating if the task is running
        /// </summary>
        public bool IsRunning { get; set; }


        /// <summary>
        /// True to show the Updating of the asjustment at the top level is being cascaded
        /// </summary>
        public bool UpdateTotalCompleted { get; set; }

        /// <summary>
        /// True to show the Updating of the adjustment changes have been completed
        /// </summary>
        public bool StockHoldingAdjustCompleted { get; set; }

        /// <summary>
        /// True to show the Updating of the adjustment changes have been completed
        /// </summary>
        public bool ExpenditureAdjustCompleted { get; set; }
        /// <summary>
        /// Store View Model of current popup to allow reverse navigation
        /// </summary>
        public object PriorPopupViewModel { get; set; }


        #endregion

        #endregion

        #region Public Commands


        /// <summary>
        /// The command to close the settings menu
        /// </summary>
        public ICommand CloseCommand { get; set; }

        /// <summary>
        /// The command View Transactions for the selected Category and Date range
        /// </summary>
        public ICommand ExpenditureReviewCommand { get; set; }

        /// <summary>
        /// The command to add a new node and return to hierarchy navigation
        /// </summary>
        public ICommand StockHoldingCommand { get; set; }
        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public StockHoldingAdjustViewModel(BudgetViewModel bvm)
        {
            SOH = 0;
            StockUnits = new TextEntryViewModel
            {
                Label = "Current Stock On Hand for selected node: " ,
                OriginalText = "Loading Calculated SOH...",
                //OriginalText = SOH.ToString(CultureInfo.CurrentCulture),
                CommitAction = UpdateSOHAsync,
            };






            //// Display unique identifier for new node
            //KCategoryID =bvm.KCategoryID;

            // Heading to be displayed on control
            HeadingText = "Stock Holding for Selected Category:  " + bvm.ShortName;



            // Create commands
            CloseCommand = new RelayCommand(Close);

            StockHoldingCommand = new RelayCommand(async () => await StockHoldingAdjustmentAsync());
            // TODO: Get from localization
            //AdjustmentButtonText = "View Stock Movements";
            StockHoldingButtonText = "Update Stock on Hand";

            KCategoryID = bvm.KCategoryID;

            MRequest = new StockHoldingApiModel
            {
                FCategoryID = bvm.KCategoryID
            };

            TaskManager.RunAndForget(ReturnSOHAsync);
        }


        #endregion

        #region Command Methods

        /// <summary>
        /// Open the settings menu
        /// </summary>
        //public void Open()
        //{
        //    // Close settings menu
        //    ViewModelApplication.PopupVisible = true;
        //}

        /// <summary>
        /// Closes the settings menu
        /// </summary>
        public void Close()
        {

            ViewModelApplication.CurrentPopupViewModel =PriorPopupViewModel;
            ViewModelApplication.CurrentPopupContent = PopupContent.ExpenditureAdjust;
            ViewModelApplication.CurrentPopupViewModel = PriorPopupViewModel;// ((StockHoldingAdjustViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel;
            ViewModelApplication.PopupVisible = true;



        }

        public async Task<bool> UpdateSOHAsync()
        {
            // Lock this command to ignore any other requests while processing

            return await RunCommandAsync(() => UpdateSOHCompleted, async () =>
            {
                if (StockUnits.EditedText != SOH.ToString(CultureInfo.CurrentCulture))
                      { int.TryParse(StockUnits.EditedText, out SOH); }
            return true;
            });


        }


        public async Task ReturnSOHAsync()
        {
            await RunCommandAsync(() => SOHRetrievalIsRunning, async () =>
            {

                // Store single transcient instance of client data store
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
                var result = await WebRequests.PostAsync<ApiResponse<StockHoldingApiModel>>(
                // Set URL
                    RouteHelpers.GetAbsoluteRoute(ApiRoutes.ReturnSOH),
                    //RouteHelpers.GetAbsoluteRoute(ApiRoutes.ReturnTransaction),
                    MRequest,
                    bearerToken: token);

                // If the response has an error...
                if (await result.HandleErrorIfFailedAsync("SOH retrieval Failed"))
                    // We are done
                    return;
                SOH = result.ServerResponse.Response.SOH;
                StockUnits.OriginalText = SOH.ToString(CultureInfo.CurrentCulture);

                ;
                try
                {
                    {
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }


            });
        }

        /// <summary>
        /// Used tp insert a new node with the currently selected node as parent
        /// </summary>
        public async Task StockHoldingAdjustmentAsync()
        {
            // Update billing record on database
            //TO DO: Integrate with change management, requiring approval of adjustment before committing...
            // Lock this command to ignore any other requests while processing

             await RunCommandAsync(() => StockHoldingAdjustCompleted, async () =>
                {
                    // Store single transcient instance of client data store
                    var scopedClientDataStore = ClientDataStore;
                    //
                    //return;
                    //
                    MRequest.SOH = SOH;

                    // Update values from local cache
                    // Get the user token
                                    var token = ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).Token;
                    // Call the server and attempt to register with the provided credentials
                    // If we don't have a token (then not logged in...)
                    if (string.IsNullOrEmpty(token))
                        // Then do nothing more
                        return;
                    var result = await WebRequests.PostAsync<ApiResponse<StockHoldingApiModel>>(
                        // Set URL
                        RouteHelpers.GetAbsoluteRoute(ApiRoutes.UpdateSOH),
                        //RouteHelpers.GetAbsoluteRoute(ApiRoutes.ReturnTransaction),
                        MRequest,
                        bearerToken: token);

                    // If the response has an error...
                    if (await result.HandleErrorIfFailedAsync("SOH Update Failed"))
                        // We are done
                        return;



                });
        }

        

     


        #endregion

    }
}
