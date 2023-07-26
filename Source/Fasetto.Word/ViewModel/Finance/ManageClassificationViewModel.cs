using Dna;
using Fasetto.Word.Core;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using static Fasetto.Word.DI;
using static Fasetto.Word.Core.CoreDI;
using System.Collections.ObjectModel;

namespace Fasetto.Word
{
    public class ManageClassificationViewModel : BaseViewModel
    {
        #region Private Members

        /// <summary>
        /// The text to show while loading text
        /// </summary>
        private string mLoadingText = "...";

        #endregion

        #region Public Properties
        /// <summary>
        /// Field containing actual allocation  of the total transaction to the selected cost category
        /// </summary>
        public TextEntryViewModel Allocation { get; set; }

        /// <summary>
        /// The Cost category to be used for the allocation
        /// </summary>
        public HierarchyItemSelectionViewModel Category { get; set; }



        ///// <summary>
        ///// Description of hierarchy item
        ///// </summary>
        //public TextEntryViewModel Description { get; set; }
        ///// <summary>
        ///// Page linked to  hierarchy item
        ///// </summary>
        //public string Page { get; set; }

        ///// <summary>
        ///// Page modifier linked to  menu item - in the case of Hierarchies, this is the 
        ///// </summary>
        //public TextEntryViewModel Root { get; set; }
        ///// <summary>
        ///// Property to indicate whether this element is a Menu Item or not..
        ///// </summary>
        //public bool IsMenuItem { get; set; }
        ///// <summary>
        ///// The Identifier of this hierarchy item
        ///// </summary>
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
        /// Calendar date from which Element is seen as active
        /// </summary>
        public DateTime DateAdjustment  { get; set; }

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

        /// <summary>
        /// The text for the control heading
        /// </summary>
        /// 
        public string HeadingText { get; set; }

        /// <summary>
        /// API parameter model
        /// </summary>
        /// 
        public ParameterBillingAdjustmentApiModel MAPI { get; set; }



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
        /// The command to add a new classification and allocation to the transaction
        /// If the allocation exceeds the 'unprocessed' balance, it will be taken from the remaining classification
        /// </summary>
        public ICommand AddClassificationCommand { get; set; }
        /// <summary>
        /// The command to edit an existing classification (and allocation) of a transaction
        /// If the amount allocated differs from the unallocated amount and one other classification remains, the balance is allocated to that one
        /// otherwise the balance is allocated to 'unprocessed'
        /// </summary>
        public ICommand EditClassificationCommand { get; set; }

         /// <summary>
        /// The command to remove a classification from the transaction, if only one classification remains the previous allocation reverts to that classification
        /// otherwise the amount is allocated to 'unprocessed'
        /// </summary>
        public ICommand DeleteClassificationCommand { get; set; }
        public ObservableCollection<TransactionDetailViewModel> Source { get; }

        /// <summary>
        /// The command to edit the selected node and return to hierarchy navigation
        /// </summary>
        //public ICommand EditNodeCommand { get; set; }

        ///// <summary>
        ///// The command to delete the selected node and return to hierarchy navigation
        ///// </summary>
        //public ICommand DeleteNodeCommand { get; set; }
        ///// <summary>
        ///// The command to edit the selected node and return to hierarchy navigation
        ///// </summary>
        //public ICommand MoveNodeCommand { get; set; }

        ///// <summary>
        ///// The command to delete the selected node and return to hierarchy navigation
        ///// </summary>
        //public ICommand CopyNodeCommand { get; set; }
        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public ManageClassificationViewModel(ObservableCollection<TransactionDetailViewModel> source, TransactionDetailViewModel selected)
        {
            // Create Node Name
            Allocation = new TextEntryViewModel
            {
                Label = "Allocation",
                OriginalText = "0",
                //CommitAction = SaveFirstNameAsync
            };

            Category = new HierarchyItemSelectionViewModel
            {
                Label = "Select Client",
                //EditedName = mLoadingText,
                EditedName = "Selected Client",
                OriginalName = selected.ShortName,
                OriginalKid = selected.KCategoryID,
                EditedKid = "4766E825-1B58-410D-B06B-5A2639CA22C8",
                HierarchyTypeID = "1A8CCEE0-52D1-454B-8165-23EDB2241058",

                //CommitAction = SaveFirstNameAsync
            };
            // Create Node Description
            //Description = new TextEntryViewModel
            //{
            //    Label = "Node Description",
            //    OriginalText = mLoadingText,
            //    //CommitAction = SaveLastNameAsync
            //};
            //Page = "Login";

            //// Create Node Description
            //Root = new TextEntryViewModel
            //{
            //    Label = "Page Modifer",
            //OriginalText = mLoadingText,
            //    //CommitAction = SaveLastNameAsync
            //};

            // Display unique identifier for new node
            KCategoryID = "132AB-AF1245-941QW"; 

            // Display parent node name
            ParentShortName = "Parent Node";

            // Heading to be displayed on control
            HeadingText = "Manage classification of selected transaction :";



            // Create commands
            CloseCommand = new RelayCommand(Close);
            AddClassificationCommand = new RelayCommand(AddClassification);
            EditClassificationCommand = new RelayCommand(AddClassification);
            DeleteClassificationCommand = new RelayCommand(AddClassification);


            // TODO: Get from localization
            AdjustmentButtonText = "Add Adjustment to Water Consumption for :";
            Source = source;
        }
        //private void TreeView_KeyBoard(object sender, KeyboardEventArgs e)
        //{
        //    //check to determine whether user would like to add an item to the hierarchy

        //    if (Keyboard.IsKeyDown(Key.Escape))
        //    {
        //        Close();

        //    }
        //    e.Handled = true;
        //}

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
            // Close settings menu
            //var mHierarchyBillingTreeViewModel = ((SWAdjustViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel;
            //var mHierarchyBillingTreeViewModel = ViewModelApplication.CurrentPopupViewModel;
            //ViewModelApplication.CurrentPopupContent = PopupContent.SWBilling;

            //ViewModelApplication.CurrentPopupViewModel = mHierarchyBillingTreeViewModel;
            ViewModelApplication.PopupVisible = true;


        }

        /// <summary>
        /// Used tp insert a new node with the currently selected node as parent
        /// </summary>
        public void AddClassification()
        {
            // Update billing record on database
            //TO DO: Integrate with change management, requiring approval of adjustment before committing...

            //var mViewModel = (HierarchyTreeViewModel)ViewModelApplication.CurrentControlViewModel;
            ViewModelApplication.CurrentPopupViewModel = ViewModelApplication.CurrentPopupViewModel;
            //var MDateAdj = ((SWAdjustViewModel)ViewModelApplication.CurrentPopupViewModel).DateAdjustment;
            //MAPI = new ParameterBillingAdjustmentApiModel
            //{
            //    FBillingPeriodID = ((SWBillingPageViewModel)ViewModelApplication.CurrentPageViewModel).SelectedBillingPeriod.KBillingPeriodID,
            //    FPropertyID = ((SWAdjustViewModel)ViewModelApplication.CurrentPopupViewModel).KCategoryID,
            //    Adjustment = Convert.ToDecimal(((SWAdjustViewModel)ViewModelApplication.CurrentPopupViewModel).Adjustment.EditedText)/1000,
            //    DateStart = ((MDateAdj.Month ==
            //                ((SWAdjustViewModel)ViewModelApplication.CurrentPopupViewModel).DateStart.Month) ?
            //                ((SWAdjustViewModel)ViewModelApplication.CurrentPopupViewModel).DateStart :
            //                new DateTime(MDateAdj.Year, MDateAdj.Month, 1)),
            //    DateEffective = DateTime.Now,
            //    FChangeID = new Guid().ToString(),
            //};

        //    mElementViewModel.Description.OriginalText = null;
        //    mElementViewModel.Description.OriginalText = null;
        //}
        //mViewModel.AddElement(mElementViewModel);
        //ViewModelApplication.PopupVisible = false;
        //TaskManager.RunAndForget(BillingPeriodAdjustAsync);
            Close();
        }
        public async Task BillingPeriodAdjustAsync()
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

                // OK successfully registered (and logged in)... now get aprpropriate tree view data
                //for now; keep a snapshot of persisted data
                //mOriginal = result.ServerResponse.Response;
                //;

                //try
                //{
                //    //var hierarchyResultApiModels = mOriginal.ToList();
                //    //make a clone of the persisted data for manipulation on front end
                //    mPersist = new HierarchyResultListApiModel();
                //    mPersist.Clone(mOriginal, mPersist);
                //}
                //catch (Exception e)
                //{
                //    throw e;
                //}



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
