using Fasetto.Word.Core;
using System.Threading.Tasks;
using static Fasetto.Word.DI;
using static Fasetto.Word.Core.CoreDI;
using System.Windows.Input;
using System;

namespace Fasetto.Word
{
    /// <summary>
    /// The application state as a view model
    /// </summary>
    public class ApplicationViewModel : BaseViewModel
    {
        #region Private Members

        /// <summary>
        /// True if the settings menu should be shown
        /// </summary>
        private bool mSettingsMenuVisible;

        #endregion

        #region Public Properties

        /// <summary>
        /// String representation of the GUID for the currently selected Client
        /// </summary>
        public string FClientID { get; set; }

        /// <summary>
        ///The name of the currently selected Client
        /// </summary>
        public string ClientShortName { get; set; }

        /// <summary>
        /// String representation of the GUID for the currently selected Client
        /// </summary>
        public string FCostHierarchyID { get; set; }

        /// <summary>
        ///The name of the currently selected Cost Hierarchy
        /// </summary>
        public string CostHierarchyShortName { get; set; }


        /// <summary>
        /// The current page of the application
        /// </summary>
        public ApplicationPage CurrentPage { get; private set; } = ApplicationPage.Login;

        /// <summary>
        /// The view model to use for the current Hierarchy Element
        /// when the PopupControl is called to add a new Hierarchy Element
        /// </summary>
        //public HierarchyElementViewModel AddElementViewModel { get; set; } = new HierarchyElementViewModel();

        /// <summary>
        /// The view model to use for the current page when the CurrentPage changes
        /// NOTE: This is not a live up-to-date view model of the current page
        ///       it is simply used to set the view model of the current page 
        ///       at the time it changes
        /// </summary>
        public BaseViewModel CurrentPageViewModel { get; set; }

        /// <summary>
        /// True if the side menu should be shown
        /// </summary>
        public bool SideMenuVisible { get; set; } = false;

        /// <summary>
        /// True if the PopupScreen should be shown
        /// </summary>
        public bool PopupVisible { get; set; } = false;

        /// <summary>
        /// True if the settings menu should be shown
        /// </summary>
        public bool SettingsMenuVisible
        {
            get => mSettingsMenuVisible;
            set
            {
                // If property has not changed...
                if (mSettingsMenuVisible == value)
                    // Ignore
                    return;

                // Set the backing field
                mSettingsMenuVisible = value;

                // If the settings menu is now visible...
                if (value)
                    // Reload settings
                    TaskManager.RunAndForget(ViewModelSettings.LoadAsync);
            }
        }

        /// <summary>
        /// Determines the currently visible side menu content
        /// </summary>
        public SideMenuContent CurrentSideMenuContent { get; set; } = SideMenuContent.Chat;

        /// <summary>
        /// Determines the currently visible popup content
        /// </summary>
        public PopupContent CurrentPopupContent { get; set; }
            //= PopupContent.AddElement;

        /// <summary>
        /// Points to the currently visible popup content view model
        /// </summary>
        public object CurrentPopupViewModel { get; set; }


        /// <summary>
        /// Points to the currently visible side menu content (or page if control deployed to page) view model
        /// </summary>
        public object CurrentSideMenuViewModel { get; set; }


        /// <summary>
        /// Points to the control eleMent of the page content (or page if control deployed to page) view model
        /// </summary>
        public object CurrentControlViewModel { get; set; }

        /// <summary>
        /// Points to the currently visible page content view model
        /// </summary>
        //public object CurrentPageViewModel { get; set; }

        /// <summary>
        /// Determines if the application has network access to the fasetto server
        /// </summary>
        public bool ServerReachable { get; set; } = true;

        /// <summary>
        /// Make provision for a stage parameter that could be passed through to adjust a page
        /// </summary>
        public string PageParameter { get; set; }
        /// <summary>
        /// Make provision for a popup parameter that could be passed through to adjust a popup
        /// </summary>
        public string PopupParameter { get; set; }
        /// <summary>
        /// Make provision for a control parameter that could be passed through to adjust a control
        /// </summary>
        public string ControlParameter { get; set; }
        /// <summary>
        /// Make provision for another control parameter that could be passed through to adjust a control
        /// </summary>
        public object ControlParameter1 { get; set; }

        /// <summary>
        /// Make provision for another control parameter  for communication betwween parent and child
        /// </summary>
        public string ControlParameter2 { get; set; }

        /// <summary>
        /// Make provision for another control parameter for communication parent and child
        /// </summary>
        public bool ControlParameter3 { get; set; }
        /// <summary>
        /// Make provision for another control parameter that could be passed through to adjust a control
        /// </summary>
        public string ControlParameter4 { get; set; }

        /// <summary>
        /// Make provision for another control parameter  for communication betwween parent and child
        /// </summary>
        public object ControlParameter5 { get; set; }

        /// <summary>
        /// Persistence of view model for CostHierarchy lookup
        /// </summary>
        public object ControlPopupCostHierarchy { get; set; }

        /// <summary>
        /// Persistence of view model for CostHierarchy lookup
        /// </summary>
        public object ControlPopupCostCategory { get; set; }

        /// <summary>
        /// Persistence of view model for Party lookup
        /// </summary>
        public object ControlPopupParty { get; set; }
        #endregion

        #region Public Commands

        /// <summary>
        /// The command to change the side menu to the Chat
        /// </summary>
        public ICommand OpenChatCommand { get; set; }

        /// <summary>
        /// The command to change the side menu to the Contacts
        /// </summary>
        public ICommand OpenContactsCommand { get; set; }

        /// <summary>
        /// The command to change the side menu to the Finance Menu selection
        /// </summary>
        public ICommand OpenFinanceCommand { get; set; }

        /// <summary>
        /// The command to change the side menu to Media
        /// </summary>
        public ICommand OpenMediaCommand { get; set; }

        /// <summary>
        /// The command to change the side menu to Menu
        /// </summary>
        public ICommand OpenMenuCommand { get; set; }

        /// <summary>
        /// The command to change the side menu to Menu
        /// </summary>
        public ICommand OpenActualsCommand { get; set; }


        /// <summary>
        /// The command to change the side menu to Loading Meter readings
        /// </summary>
        public ICommand OpenLoadMetersCommand { get; set; }



        /// <summary>
        /// The command to change the side menu to Loading Meter readings
        /// </summary>
        public ICommand OpenBulkReconCommand { get; set; }

#endregion

        #region Constructor

        /// <summary>
        /// The default constructor
        /// </summary>
        public ApplicationViewModel()
        {
            // Create the commands
            OpenChatCommand = new RelayCommand(OpenChat);
            OpenContactsCommand = new RelayCommand(OpenContacts);
            OpenMediaCommand = new RelayCommand(OpenMedia);
            OpenFinanceCommand = new RelayCommand(OpenFinance);
            OpenMenuCommand = new RelayCommand(OpenMenu);
            OpenActualsCommand = new RelayCommand(OpenActuals);
            OpenLoadMetersCommand = new RelayCommand(OpenLoadMeters);
            OpenBulkReconCommand = new RelayCommand(OpenBulkRecon);
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// Changes the current side menu to the Chat
        /// </summary>
        public void OpenChat()
        {
            // Set the current side menu to Chat
            ViewModelApplication.GoToPage(ApplicationPage.Chat);
            CurrentSideMenuContent = SideMenuContent.Chat;
        }

        /// <summary>
        /// Changes the current side menu to the Contacts
        /// </summary>
        public void OpenContacts()
        {
            // Set the current side menu to Chat
            ViewModelApplication.GoToPage(ApplicationPage.Chat);
            CurrentSideMenuContent = SideMenuContent.Contacts;
        }

        /// <summary>
        /// Changes the current side menu to Media
        /// </summary>
        public void OpenMedia()
        {
            // Set the current side menu to Media
            CurrentSideMenuContent = SideMenuContent.Media;
        }

        /// <summary>
        /// Changes the current side menu to Finance
        /// </summary>
        public void OpenFinance()
        {
            // Set the current side menu to Finance
            ViewModelApplication.ControlParameter = "7E669DCA-D356-43F0-BB64-5DF6D1499C99";
            //ViewModelApplication.GoToPage(ApplicationPage.Finance);
            //ViewModelApplication.GoToPage(ApplicationPage.Chat);
            CurrentSideMenuContent = SideMenuContent.Finance;
            SideMenuVisible = true;
        }
        public void OpenFinance(string root, string page)
        {
            // Set the current side menu to Kaizenburst Menu Options
            ViewModelApplication.ControlParameter = root;
            //convert from string to the appropriate Enum Application Page
            var applicationPage = (ApplicationPage)Enum.Parse(typeof(ApplicationPage), page);
            ViewModelApplication.GoToPage(applicationPage);
            //CurrentSideMenuContent = SideMenuContent.Menu;

            //ViewModelApplication.GoToPage(ApplicationPage.Hierarchy);
            SideMenuVisible = false;
        }


        public void OpenMenu()
        {
            // Set the current side menu to KaizenBurst Menu
            ViewModelApplication.ControlParameter = "2D7E4A7D-6F19-496E-8709-47E6A9ADDFA0";
            //ViewModelApplication.GoToPage(ApplicationPage.Hierarchy);
            CurrentSideMenuContent = SideMenuContent.Menu;

            //ViewModelApplication.GoToPage(ApplicationPage.Hierarchy);
            SideMenuVisible = true;
        }
        public void OpenMenu(string root,string page)
        {
            // Set the current side menu to Kaizenburst Menu Options
            ViewModelApplication.ControlParameter = root;
            //convert from string to the appropriate Enum Application Page
            var applicationPage = (ApplicationPage)Enum.Parse(typeof(ApplicationPage), page);

            ViewModelApplication.GoToPage(applicationPage);
            //ViewModelApplication.CurrentPageViewModel = ViewModelApplication.CurrentPageViewModel;
            //CurrentSideMenuContent = SideMenuContent.Menu;

            //ViewModelApplication.GoToPage(ApplicationPage.Hierarchy);
            SideMenuVisible = false;
        }
        public void OpenActuals()
        {
            // Set the current side menu to KaizenBurst Menu
            ViewModelApplication.ControlParameter = "2D7E4A7D-6F19-496E-8709-47E6A9ADDFA0";
            //ViewModelApplication.GoToPage(ApplicationPage.Hierarchy);
            CurrentSideMenuContent = SideMenuContent.Menu;

            //ViewModelApplication.GoToPage(ApplicationPage.Hierarchy);
            SideMenuVisible = true;
        }
        public void OpenLoadMeters()
        {
            // Set the current side menu to KaizenBurst Menu
            ViewModelApplication.ControlParameter = "2D7E4A7D-6F19-496E-8709-47E6A9ADDFA0";
            //ViewModelApplication.GoToPage(ApplicationPage.Hierarchy);
            //CurrentSideMenuContent = SideMenuContent.Menu;

            //ViewModelApplication.GoToPage(ApplicationPage.Hierarchy);
            SideMenuVisible = false;
        }

        public void OpenBulkRecon()
        {
            // Set the current side menu to KaizenBurst Menu
            ViewModelApplication.ControlParameter = "2D7E4A7D-6F19-496E-8709-47E6A9ADDFA0";
            //ViewModelApplication.GoToPage(ApplicationPage.Hierarchy);
            //CurrentSideMenuContent = SideMenuContent.Menu;

            //ViewModelApplication.GoToPage(ApplicationPage.Hierarchy);
            SideMenuVisible = false;
        }

        #endregion

        #region Public Helper Methods

        /// <summary>
        /// Navigates to the specified page
        /// </summary>
        /// <param name="page">The page to go to</param>
        /// <param name="viewModel">The view model, if any, to set explicitly to the new page</param>
        public void GoToPage(ApplicationPage page, BaseViewModel viewModel = null)
        {
            // Always hide settings page if we are changing pages
            SettingsMenuVisible = false;

            // Set the view model
            CurrentPageViewModel = viewModel;

            // See if page has changed
            var different = CurrentPage != page;

            // Set the current page
            CurrentPage = page;

            // If the page hasn't changed, fire off notification
            // So pages still update if just the view model has changed
            if (!different)
                OnPropertyChanged(nameof(CurrentPage));

            // Show side menu or not?
            SideMenuVisible = page == ApplicationPage.Chat || page == ApplicationPage.Finance || page == ApplicationPage.Hierarchy;

        }

        /// <summary>
        /// Handles what happens when we have successfully logged in
        /// </summary>
        /// <param name="loginResult">The results from the successful login</param>
        public async Task HandleSuccessfulLoginAsync(UserProfileDetailsApiModel loginResult)
        {
            // Store this in the client data store
            await ClientDataStore.SaveLoginCredentialsAsync(loginResult.ToLoginCredentialsDataModel());

            // Load new settings
            await ViewModelSettings.LoadAsync();

            // Go to chat page
            ViewModelApplication.GoToPage(ApplicationPage.Chat);
        }

        #endregion
    }
}
