using Dna;
using Fasetto.Word.Core;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using static Fasetto.Word.DI;
using static Fasetto.Word.Core.CoreDI;
using System.Globalization;
namespace Fasetto.Word
{
    public class BudgetAdjustViewModel : BaseViewModel
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
        /// Parent ID  of hiearchy item
        /// </summary>
        public string ParentCategoryID { get; set; }

        /// <summary>
        /// Parent ShortName of hiearchy item
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
        /// Integer representing the budget month
        /// </summary>
        public int Month { get; set; }


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
        /// The command to add a new node and return to hierarchy navigation
        /// </summary>
        public ICommand BudgetAdjustmentCommand { get; set; }


        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public BudgetAdjustViewModel(BudgetViewModel bvm)
        {
            // Create Node Name
            BudgetAncestor = new TextEntryViewModel
            {
                Label = "Direct Budget Amount for selected node: ",
                OriginalText = bvm.BudgetAmount.ToString("C", CultureInfo.CurrentCulture),
                //CommitAction = SaveFirstNameAsync
            };

            BudgetAncestorHierarchy = new TextEntryViewModel
            {
                Label = "Total Budget Amount for selected node with descendants: ",
                OriginalText = bvm.BudgetAmountTotal.ToString("C", CultureInfo.CurrentCulture),
                //CommitAction = SaveFirstNameAsync
            };

            // Display unique identifier for new node
            KCategoryID = "132AB-AF1245-941QW";


            BudgetAmountStr = bvm.BudgetAmount.ToString("C", CultureInfo.CurrentCulture);

            BudgetAmountTotalStr = bvm.BudgetAmountTotal.ToString("C", CultureInfo.CurrentCulture);

            // Display parent node name
            ParentShortName = "Parent Node";

            // Heading to be displayed on control
            HeadingText = "Edit Budget figures for Selected Node:  ";



            // Create commands
            CloseCommand = new RelayCommand(Close);
            BudgetAdjustmentCommand = new RelayCommand(BudgetAdjustment);


            // TODO: Get from localization
            AdjustmentButtonText = "Apply Changes to Budget";

            IsMonthOnly = true;

            Month = bvm.Month;
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
            ViewModelApplication.CurrentPopupContent = PopupContent.BudgetDetailList;
            ViewModelApplication.CurrentPopupViewModel = PriorPopupViewModel;// ((BudgetAdjustViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel;
            ViewModelApplication.PopupVisible = true;


        }

        /// <summary>
        /// Used tp insert a new node with the currently selected node as parent
        /// </summary>
        public void BudgetAdjustment()
        {
            // Update billing record on database
            //TO DO: Integrate with change management, requiring approval of adjustment before committing...

            //var mViewModel = (HierarchyTreeViewModel)ViewModelApplication.CurrentControlViewModel;
            ViewModelApplication.CurrentPopupViewModel = ViewModelApplication.CurrentPopupViewModel;
            //var MDateAdj = ((BudgetAdjustViewModel)ViewModelApplication.CurrentPopupViewModel).DateAdjustment;
            IsMonthOnly = IsMonthOnly;
            MAPI = new ParameterBudgetAdjustApiModel
            {
                KCategoryID = KCategoryID,
                IsMonth = IsMonthOnly,
                Month = Month,
                //FPropertyID = ((BudgetAdjustViewModel)ViewModelApplication.CurrentPopupViewModel).KCategoryID,
                //Adjustment = Convert.ToDecimal(((BudgetAdjustViewModel)ViewModelApplication.CurrentPopupViewModel).BudgetAncestor.EditedText)/1000,
                ////DateStart = ((MDateAdj.Month ==
                ////            ((BudgetAdjustViewModel)ViewModelApplication.CurrentPopupViewModel).DateStart.Month) ?
                ////            ((BudgetAdjustViewModel)ViewModelApplication.CurrentPopupViewModel).DateStart :
                ////            new DateTime(MDateAdj.Year, MDateAdj.Month, 1)),
                //DateEffective = DateTime.Now,
                //FChangeID = new Guid().ToString(),
            };

        //    mElementViewModel.Description.OriginalText = null;
        //    mElementViewModel.Description.OriginalText = null;
        //}
        //mViewModel.AddElement(mElementViewModel);
        //ViewModelApplication.PopupVisible = false;
        TaskManager.RunAndForget(BudgetPeriodAdjustAsync);
            Close();
        }
        public async Task BudgetPeriodAdjustAsync()
        {
            await RunCommandAsync(() => IsRunning, async () =>
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

                var result = await WebRequests.PostAsync<ApiResponse>(
                // Set URL
                    RouteHelpers.GetAbsoluteRoute(ApiRoutes.BillingPeriodAdjustment),
                    MAPI ,
                    bearerToken: token);




                // If the response has an error...
                if (await result.HandleErrorIfFailedAsync("Capture of Adjustment failed"))
                    // We are done
                    return;





                ViewModelApplication.CurrentControlViewModel = ViewModelApplication.CurrentControlViewModel;


            });
        }
        ///// <summary>
        ///// Used tp insert a new node with the currently selected node as parent
        ///// </summary>
        //public void AddNode()
        //{
        //    // Close settings menu

        //    var mViewModel = (HierarchyTreeViewModel)ViewModelApplication.CurrentControlViewModel;
        //    var mElementViewModel = (HierarchyElementViewModel)ViewModelApplication.CurrentPopupViewModel;
        //    if (mElementViewModel.Description.OriginalText == "Description of New Element" || mElementViewModel.Description.EditedText == "Description of New Element") 
        //        { mElementViewModel.Description.OriginalText = null;
        //        mElementViewModel.Description.OriginalText = null;
        //    }
        //    mViewModel.AddElement(mElementViewModel);
        //    ViewModelApplication.PopupVisible = false;
        //}

        ///// <summary>
        ///// Used tp edit the currently selected node 
        ///// </summary>
        //public void EditNode()
        //{
        //    // Close settings menu
        //    //var mViewModel = (HierarchyTreeViewModel)ViewModelApplication.CurrentSideMenuViewModel;
        //    var mViewModel = (HierarchyTreeViewModel)ViewModelApplication.CurrentControlViewModel;
        //    var mElementViewModel = (HierarchyElementViewModel)ViewModelApplication.CurrentPopupViewModel;
        //    mViewModel.EditElement(mElementViewModel);
        //    ViewModelApplication.PopupVisible = false;
        //}


        ///// <summary>
        ///// Used tp edit the currently selected node 
        ///// </summary>
        //public void DeleteNode()
        //{
        //    // Close settings menu
        //    var mViewModel = (HierarchyTreeViewModel)ViewModelApplication.CurrentControlViewModel;
        //    var mElementViewModel = (HierarchyElementViewModel)ViewModelApplication.CurrentPopupViewModel;
        //    //Set discontinuation time to time of deletion
        //    mElementViewModel.DateDiscontinued = DateTime.Today; 
        //    mViewModel.DeleteElement(mElementViewModel);

        //    ViewModelApplication.PopupVisible = false;
        //}

        ///// <summary>
        ///// Used tp edit the currently selected node 
        ///// </summary>
        //public void MoveNode()
        //{
        //    // Close settings menu
        //    var mViewModel = (HierarchyTreeViewModel)ViewModelApplication.CurrentControlViewModel;
        //    var mElementViewModel = (HierarchyElementViewModel)ViewModelApplication.CurrentPopupViewModel;
        //    mViewModel.MoveElement(mElementViewModel);
        //    if (mElementViewModel.Description.OriginalText == "Description of New Element" && mElementViewModel.Description.EditedText == "Description of New Element")
        //    {
        //        mElementViewModel.Description.OriginalText = null;
        //        mElementViewModel.Description.OriginalText = null;
        //    }
        //    ViewModelApplication.PopupVisible = false;
        //}


        ///// <summary>
        ///// Used tp edit the currently selected node 
        ///// </summary>
        //public void CopyNode()
        //{
        //    // Close settings menu
        //    var mViewModel = (HierarchyTreeViewModel)ViewModelApplication.CurrentControlViewModel;
        //    var mElementViewModel = (HierarchyElementViewModel)ViewModelApplication.CurrentPopupViewModel;
        //    mViewModel.CopyElement(mElementViewModel);
        //    ViewModelApplication.PopupVisible = false;
        //}


        /// <summary>
        /// Clears any data specific to the current user
        /// </summary>
        //public void ClearUserData()
        //{
        //    // Clear all view models containing the users info
        //    FirstName.OriginalText = mLoadingText;
        //    LastName.OriginalText = mLoadingText;
        //    Username.OriginalText = mLoadingText;
        //    Email.OriginalText = mLoadingText;
        //}



        /// <summary>
        /// Saves the new First Name to the server
        /// </summary>
        /// <returns>Returns true if successful, false otherwise</returns>
        //public async Task<bool> SaveFirstNameAsync()
        //{
        //    // Lock this command to ignore any other requests while processing
        //    return await RunCommandAsync(() => FirstNameIsSaving, async () =>
        //    {
        //        // Update the First Name value on the server...
        //        return await UpdateUserCredentialsValueAsync(
        //            // Display name
        //            "First Name",
        //            // Update the first name
        //            (credentials) => credentials.FirstName,
        //            // To new value
        //            FirstName.OriginalText,
        //            // Set Api model value
        //            (apiModel, value) => apiModel.FirstName = value
        //            );
        //    });
        //}


        #endregion

    }
}
