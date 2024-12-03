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
namespace Fasetto.Word
{
    public class ExpenditureAdjustViewModel : BaseViewModel
    {
        #region Private Members

        /// <summary>
        /// The text to show while loading text
        /// </summary>
        private string mLoadingText = "...";

        #endregion

        #region Public Properties
        /// <summary>
        /// Field containing Adjustment to be mode
        /// </summary>
        public TextEntryViewModel BudgetAncestor { get; set; }



        /// <summary>
        /// Field containing Adjustment to be mode
        /// </summary>
        public TextEntryViewModel BudgetAncestorHierarchy { get; set; }

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
        /// A flag indicating whether adjustment appliess just to the selected month or for the remaining period of the budget
        /// </summary>
        /// 
        public bool IsMonthOnly { get; set; }


        /// <summary>
        /// API parameter model
        /// </summary>
        /// 
        public ParameterBudgetAdjustApiModel MAPI { get; set; }



        #region Transactional Properties

        /// <summary>
        /// Indicates if the node is being saved
        /// </summary>
        public bool NodeSaving { get; set; }

            /// <summary>
            /// Indicates if the first name is being saved
            /// </summary>
            public bool FirstNameIsSaving { get; set; }

            /// <summary>
            /// Indicates if the last name is current being saved
            /// </summary>
            public bool LastNameIsSaving { get; set; }

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
        public bool ExpenditureAdjustCompleted { get; set; }

        /// <summary>
        /// True to show the Updating of the adjustment changes have been completed
        /// </summary>
        public bool StockHoldingAdjustCompleted { get; set; }
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
        public ExpenditureAdjustViewModel(BudgetViewModel bvm)
        {
            // Create Node Name
            BudgetAncestor = new TextEntryViewModel
            {
                Label = "Direct Budget Amount for selected node: ",
                OriginalText = bvm.BudgetAmount.ToString("C", CultureInfo.CurrentCulture),
                CommitAction = UpdateBudgetAmountTotalAsync,
            };

            BudgetAncestorHierarchy = new TextEntryViewModel
            {
                Label = "Total Budget Amount for selected node with descendants: ",
                OriginalText = bvm.BudgetAmountTotal.ToString("C", CultureInfo.CurrentCulture),
                //CommitAction = SaveFirstNameAsync
            };

            // Display unique identifier for new node
            KCategoryID =bvm.KCategoryID;



            BudgetAmountStr = bvm.BudgetAmount.ToString("C", CultureInfo.CurrentCulture);

            BudgetAmountTotalStr = bvm.BudgetAmountTotal.ToString("C", CultureInfo.CurrentCulture);
            ActualAmountTotalStr = bvm.ActualAmountTotal.ToString("C", CultureInfo.CurrentCulture);
            BudgetAmountCumTotalStr = bvm.BudgetTotCum.ToString("C", CultureInfo.CurrentCulture);
            ActualAmountCumTotalStr = bvm.ActualTotCum.ToString("C", CultureInfo.CurrentCulture);
            DeviationStr = bvm.Deviation.ToString("C", CultureInfo.CurrentCulture);
            DeviationCumStr = bvm.DeviationCum.ToString("C", CultureInfo.CurrentCulture);

            BudgetAmountDec = bvm.BudgetAmount;
            BudgetAmountTotalDec = bvm.BudgetAmountTotal;
            BudgetAmountAdjDec = bvm.BudgetAmount;
            BudgetAmountTotalAdjDec = bvm.BudgetAmountTotal;
            // Display parent node name
            ParentShortName = "Parent Node";

            // Heading to be displayed on control
            HeadingText = "View Transaction Detail for Selected Node:  ";



            // Create commands
            CloseCommand = new RelayCommand(Close);
            IsStockTracked = bvm.IsStockTracked;
            ExpenditureReviewCommand = new RelayCommand(async () => await ExpenditureAdjustmentAsync());

            StockHoldingCommand = new RelayCommand(async () => await StockHoldingAsync());
            // TODO: Get from localization
            AdjustmentButtonText = "View Transaction Detail";
            StockHoldingButtonText = "View Stock Holding of Category";


            IsMonthOnly = true;

            Month = bvm.Month;
            DateEnd = DateTime.Now;
            DateStart = DateTime.Now.AddDays(-365);
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

            ViewModelApplication.CurrentPopupViewModel = PriorPopupViewModel;
            ViewModelApplication.CurrentPopupContent = PopupContent.ExpenditureReview;
            ViewModelApplication.CurrentPopupViewModel = PriorPopupViewModel;// ((ExpenditureAdjustViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel;
            ViewModelApplication.PopupVisible = true;


        }

        /// <summary>
        /// Update the total budget for the selected category hierarchy
        /// </summary>
        /// <returns></returns>
        public async Task<bool> UpdateBudgetAmountTotalAsync()
        {
            // Lock this command to ignore any other requests while processing

            return await RunCommandAsync(() =>UpdateTotalCompleted, async () =>
            {
                // Update the Category Classification value on the server...
                var decBudgAdj = BudgetAmountDec;
                if (!(BudgetAncestor.EditedText == null || BudgetAncestor.EditedText == ""))
                { decimal.TryParse(BudgetAncestor.EditedText, NumberStyles.Currency, CultureInfo.CurrentCulture, out decBudgAdj); }
                var decBudgTotAdj = BudgetAmountTotalDec;
                BudgetAncestorHierarchy.OriginalText =(BudgetAmountTotalDec - BudgetAmountDec + decBudgAdj).ToString("C", CultureInfo.CurrentCulture);

                return true;
            });

        }

        /// <summary>
        /// Used tp insert a new node with the currently selected node as parent
        /// </summary>
        public async Task ExpenditureAdjustmentAsync()
        {
            // Update billing record on database
            //TO DO: Integrate with change management, requiring approval of adjustment before committing...
            // Lock this command to ignore any other requests while processing

             await RunCommandAsync(() => ExpenditureAdjustCompleted, async () =>
                {


                    ViewModelApplication.CurrentPopupViewModel = new TransactionTreeViewModel(
                    ((HierarchyItemSelectionViewModel)((ExpenditureVSBudgetPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy).EditedKid,
                        DateStart,//SelectedBudgetMonth
                        DateEnd,//SelectedBudgetMonth -12 mo
                        KCategoryID,
                        ((BudgetPeriodViewModel)((ExpenditureVSBudgetPageViewModel)ViewModelApplication.CurrentPageViewModel).SelectedBudget).KBudgetID
                        );
                    ((TransactionTreeViewModel)ViewModelApplication.CurrentPopupViewModel).ControlTitle = 
                    ((ExpenditureAdjustViewModel)((TransactionTreeViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel).HeadingText ;
                    //force a reload of the BulkRecon Control
                    ViewModelApplication.CurrentPopupContent = 0;
                    ViewModelApplication.CurrentPopupContent = PopupContent.Transaction;
                    ViewModelApplication.PopupVisible = true;
                    return;
                }
             );
        }

        /// <summary>
        /// Used tp insert a new node with the currently selected node as parent
        /// </summary>
        public async Task StockHoldingAsync()
        {
            // Update billing record on database
            //TO DO: Integrate with change management, requiring approval of adjustment before committing...
            // Lock this command to ignore any other requests while processing

            await RunCommandAsync(() => StockHoldingAdjustCompleted, async () =>
            {
                var mCurrentPopupViewModel = ViewModelApplication.CurrentPopupViewModel;

                //var mSWAdjustViewModel = new SWAdjustViewModel();

                ViewModelApplication.CurrentPopupContent = PopupContent.StockHoldingAdjust;
                var MAdjustmentVM = (StockHoldingAdjustViewModel)ViewModelApplication.CurrentPopupViewModel;
                MAdjustmentVM.PriorPopupViewModel = mCurrentPopupViewModel;
                //MAdjustmentVM.KCategoryID = mDraggedItem.KCategoryID;
                //MAdjustmentVM.HeadingText = MAdjustmentVM.HeadingText + mDraggedItem.ShortName;
                ViewModelApplication.PopupVisible = true;
                //ViewModelApplication.SettingsMenuVisible = true;
            }
           );
        }

        //public async Task BudgetPeriodAdjustAsync()
        //{
        //    await RunCommandAsync(() => IsRunning, async () =>
        //    {

        //        // Store single transcient instance of client data store
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
        //            RouteHelpers.GetAbsoluteRoute(ApiRoutes.BudgetElementAdjustment),
        //            MAPI ,
        //            bearerToken: token);




        //        // If the response has an error...
        //        if (await result.HandleErrorIfFailedAsync("Capture of Adjustment failed"))
        //            // We are done
        //            return;
        //        var matches1 = result.ServerResponse.Response;
        //        var tmpList = ((BudgetTreeViewModel)((ExpenditureAdjustViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel).mPersist;
        //        var matches = tmpList.Where(x => x.ParentCategoryID == "00000000-0000-0000-0000-000000000000");
        //        foreach (var budgItem in matches1)
        //        {
        //            matches = tmpList.Where(x => x.KCategoryID ==budgItem.KCategoryID);
        //            foreach (var item in matches)
        //            {
        //                item.BudgetAmount = budgItem.BudgetAmount;
        //                item.BudgetAmountTotal = budgItem.BudgetAmountTotal;
        //                item.BudgetAmountDescendants = budgItem.BudgetAmountTotal - budgItem.BudgetAmount;
        //            }
        //        }

        //        ((BudgetTreeViewModel)((ExpenditureAdjustViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel).RefreshHierarchy();

        //    });
        //}



        #endregion

    }
}
