using Dna;
using Fasetto.Word.Core;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows.Input;
using static Dna.FrameworkDI;
using static Fasetto.Word.DI;
namespace Fasetto.Word { 
    public class ExpenditureElementViewModel : BaseViewModel
    {
        #region Private Members

        /// <summary>
        /// The text to show while loading text
        /// </summary>
        private string mLoadingText = "...";

        #endregion

        #region Public Properties

        public TextEntryViewModel ShortName { get; set; }

        /// <summary>
        /// Description of hiearchy item
        /// </summary>
        public TextEntryViewModel Description { get; set; }
        /// <summary>
        /// Page linked to  hiearchy item
        /// </summary>
        public string Page { get; set; }

        /// <summary>
        /// Page modifier linked to  menu item - in the case of Hierarchies, this is the 
        /// </summary>
        public TextEntryViewModel Root { get; set; }
        /// <summary>
        /// Property to indicate whether this element is a Menu Item or not..
        /// </summary>
        public bool IsMenuItem { get; set; }
        /// <summary>
        /// The Identifier of this hierarchy item
        /// </summary>
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
        public DateTime DateEffective   { get; set; }

        /// <summary>
        /// Calendar date from which Element is deactivated
        /// </summary>
        public DateTime DateDiscontinued { get; set; }

        /// <summary>
        /// Attach the current activity to a Change object
        /// </summary>
        public string KChangeID { get; set; }

        /// <summary>
        /// The text for the add Node button
        /// </summary>
        public string AddNodeButtonText { get; set; }
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
        public ICommand AddNodeCommand { get; set; }

        /// <summary>
        /// The command to edit the selected node and return to hierarchy navigation
        /// </summary>
        public ICommand EditNodeCommand { get; set; }

        /// <summary>
        /// The command to delete the selected node and return to hierarchy navigation
        /// </summary>
        public ICommand DeleteNodeCommand { get; set; }
        /// <summary>
        /// The command to edit the selected node and return to hierarchy navigation
        /// </summary>
        public ICommand MoveNodeCommand { get; set; }

        /// <summary>
        /// The command to delete the selected node and return to hierarchy navigation
        /// </summary>
        public ICommand CopyNodeCommand { get; set; }
        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public ExpenditureElementViewModel()
        {
            // Create Node Name
            ShortName = new TextEntryViewModel
            {
                Label = "Node Name",
                OriginalText = mLoadingText,
                //CommitAction = SaveFirstNameAsync
            };

            // Create Node Description
            Description = new TextEntryViewModel
            {
                Label = "Node Description",
                OriginalText = mLoadingText,
                //CommitAction = SaveLastNameAsync
            };
            Page = "Login";

            // Create Node Description
            Root = new TextEntryViewModel
            {
                Label = "Page Modifer",
            OriginalText = mLoadingText,
                //CommitAction = SaveLastNameAsync
            };

            // Display unique identifier for new node
            KCategoryID = "132AB-AF1245-941QW"; 

            // Display parent node name
            ParentShortName = "Parent Node";

            // Heading to be displayed on control
            HeadingText = "Add Node to Hierarchy";



            // Create commands
            CloseCommand = new RelayCommand(Close);
            AddNodeCommand = new RelayCommand(AddNode);
            EditNodeCommand = new RelayCommand(EditNode);
            DeleteNodeCommand = new RelayCommand(DeleteNode);
            CopyNodeCommand = new RelayCommand(CopyNode);
            MoveNodeCommand = new RelayCommand(MoveNode);

            // TODO: Get from localization
            AddNodeButtonText = "Add Node to selected Parent";
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
            ViewModelApplication.PopupVisible = false;

        }

        /// <summary>
        /// Used tp insert a new node with the currently selected node as parent
        /// </summary>
        public void AddNode()
        {
            // Close settings menu

            var mViewModel = (HierarchyTreeViewModel)ViewModelApplication.CurrentControlViewModel;
            var mElementViewModel = (HierarchyElementViewModel)ViewModelApplication.CurrentPopupViewModel;
            if (mElementViewModel.Description.OriginalText == "Description of New Element" || mElementViewModel.Description.EditedText == "Description of New Element") 
                { mElementViewModel.Description.OriginalText = null;
                mElementViewModel.Description.OriginalText = null;
            }
            mViewModel.AddElement(mElementViewModel);
            ViewModelApplication.PopupVisible = false;
        }

        /// <summary>
        /// Used tp edit the currently selected node 
        /// </summary>
        public void EditNode()
        {
            // Close settings menu
            //var mViewModel = (HierarchyTreeViewModel)ViewModelApplication.CurrentSideMenuViewModel;
            var mViewModel = (HierarchyTreeViewModel)ViewModelApplication.CurrentControlViewModel;
            var mElementViewModel = (HierarchyElementViewModel)ViewModelApplication.CurrentPopupViewModel;
            mViewModel.EditElement(mElementViewModel);
            ViewModelApplication.PopupVisible = false;
        }


        /// <summary>
        /// Used tp edit the currently selected node 
        /// </summary>
        public void DeleteNode()
        {
            // Close settings menu
            var mViewModel = (HierarchyTreeViewModel)ViewModelApplication.CurrentControlViewModel;
            var mElementViewModel = (HierarchyElementViewModel)ViewModelApplication.CurrentPopupViewModel;
            //Set discontinuation time to time of deletion
            mElementViewModel.DateDiscontinued = DateTime.Today; 
            mViewModel.DeleteElement(mElementViewModel);

            ViewModelApplication.PopupVisible = false;
        }

        /// <summary>
        /// Used tp edit the currently selected node 
        /// </summary>
        public void MoveNode()
        {
            // Close settings menu
            var mViewModel = (HierarchyTreeViewModel)ViewModelApplication.CurrentControlViewModel;
            var mElementViewModel = (HierarchyElementViewModel)ViewModelApplication.CurrentPopupViewModel;
            mViewModel.MoveElement(mElementViewModel);
            if (mElementViewModel.Description.OriginalText == "Description of New Element" && mElementViewModel.Description.EditedText == "Description of New Element")
            {
                mElementViewModel.Description.OriginalText = null;
                mElementViewModel.Description.OriginalText = null;
            }
            ViewModelApplication.PopupVisible = false;
        }


        /// <summary>
        /// Used tp edit the currently selected node 
        /// </summary>
        public void CopyNode()
        {
            // Close settings menu
            var mViewModel = (HierarchyTreeViewModel)ViewModelApplication.CurrentControlViewModel;
            var mElementViewModel = (HierarchyElementViewModel)ViewModelApplication.CurrentPopupViewModel;
            mViewModel.CopyElement(mElementViewModel);
            ViewModelApplication.PopupVisible = false;
        }


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
