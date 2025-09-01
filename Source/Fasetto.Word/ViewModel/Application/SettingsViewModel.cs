using Dna;
using Fasetto.Word.Core;
using Fasetto.Word.Core.ApiModels.Controls;
//using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
using static Dna.FrameworkDI;
using static Fasetto.Word.DI;
using static Fasetto.Word.Core.CoreDI;
namespace Fasetto.Word
{
    /// <summary>
    /// The settings state as a view model
    /// </summary>
    public class SettingsViewModel : BaseViewModel
    {
        #region Private Members

        /// <summary>
        /// The text to show while loading text
        /// </summary>
        private string mLoadingText = "...";

        #endregion

        #region Public Properties

        /// <summary>
        /// The current users first name
        /// </summary>
        public TextEntryViewModel FirstName { get; set; }

        /// <summary>
        /// The current users last name
        /// </summary>
        public TextEntryViewModel LastName { get; set; }

        /// <summary>
        /// The current users username
        /// </summary>
        public TextEntryViewModel Username { get; set; }

        /// <summary>
        /// The current users password
        /// </summary>
        public PasswordEntryViewModel Password { get; set; }

        /// <summary>
        /// The current users email
        /// </summary>
        public TextEntryViewModel Email { get; set; }


        /// <summary>
        /// The Client for which Bulk Meter reconciliation is to be processed
        /// </summary>
        public HierarchyItemSelectionViewModel Client { get; set; }


        /// <summary>
        /// The current users selected cost hierarchy for the Selected Client
        /// </summary>
        public HierarchyItemSelectionViewModel CostHierarchy { get; set; }


        /// <summary>
        /// Populate parameters for retrieval of required hierarchy tree
        /// </summary>
        public ParameterHierarchyItemSelectApiModel HierarchyParam { get; set; }



        /// <summary>
        /// The current LoginCredentials for the current user
        /// </summary>
        public LoginCredentialsDataModel LoginCredentials { get; set; }

        /// <summary>
        /// The text for the logout button
        /// </summary>
        public string LogoutButtonText { get; set; }


        /// <summary>
        /// The text for the logout button
        /// </summary>
        public string UpdateButtonText { get; set; }

        #region Transactional Properties

        /// <summary>
        /// Indicates if the first name is current being saved
        /// </summary>
        public bool FirstNameIsSaving { get; set; }


        /// <summary>
        /// Indicates if the meter lookup has been configured
        /// </summary>
        public bool MeterSelectionIsConfigured { get; set; }


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
        /// True to show the Hierarchy has been selected
        /// </summary>
        public bool SetHierarchyCompleted { get; set; }

        /// <summary>
        /// True to show if Client Selection completed
        /// </summary>
        public bool UpdateHierarchyCompleted { get; set; }


        /// <summary>
        /// True to show the Hierarchy has been selected
        /// </summary>
        public bool SetCostHierarchyCompleted { get; set; }

        /// <summary>
        /// True to show if Client Selection completed
        /// </summary>
        public bool UpdateCostHierarchyCompleted { get; set; }
        #endregion

#endregion

        #region Public Commands

        /// <summary>
        /// The command to open the settings menu
        /// </summary>
        public ICommand OpenCommand { get; set; }

        /// <summary>
        /// The command to close the settings menu
        /// </summary>
        public ICommand CloseCommand { get; set; }

        /// <summary>
        /// The command to logout of the application
        /// </summary>
        public ICommand LogoutCommand { get; set; }


        /// <summary>
        /// The command to force changes to the login credentials on the backend
        /// </summary>
        public ICommand UpdateCommand { get; set; }

        /// <summary>
        /// The command to clear the users data from the view model
        /// </summary>
        public ICommand ClearUserDataCommand { get; set; }

        /// <summary>
        /// Loads the settings data from the client data store
        /// </summary>
        public ICommand LoadCommand { get; set; }

        /// <summary>
        /// Saves the current first name to the server
        /// </summary>
        public ICommand SaveFirstNameCommand { get; set; }

        /// <summary>
        /// Saves the current last name to the server
        /// </summary>
        public ICommand SaveLastNameCommand { get; set; }

        /// <summary>
        /// Saves the current username to the server
        /// </summary>
        public ICommand SaveUsernameCommand { get; set; }

        /// <summary>
        /// Saves the current email to the server
        /// </summary>
        public ICommand SaveEmailCommand { get; set; }

        /// <summary>
        /// Update client selection for current session
        /// </summary>
        public ICommand UpdateClientSelectionCommand { get; set; }
        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public SettingsViewModel()
        {
            LoginCredentials = new LoginCredentialsDataModel();
            // Create First Name
            FirstName = new TextEntryViewModel
            {
                Label = "First Name",
                OriginalText = mLoadingText,
                CommitAction = SaveFirstNameAsync
            };

            // Create Last Name
            LastName = new TextEntryViewModel
            {
                Label = "Last Name",
                OriginalText = mLoadingText,
                CommitAction = SaveLastNameAsync
            };

            // Create Username
            Username = new TextEntryViewModel
            {
                Label = "Username",
                OriginalText = mLoadingText,
                CommitAction = SaveUsernameAsync
            };

            // Create Password
            Password = new PasswordEntryViewModel
            {
                Label = "Password",
                FakePassword = "********",
                CommitAction = SavePasswordAsync
            };

            // Create Email
            Email = new TextEntryViewModel
            {
                Label = "Email",
                OriginalText = mLoadingText,
                CommitAction = SaveEmailAsync
            };


            Client = new HierarchyItemSelectionViewModel
            {
                Label = "Client",
                //EditedName = mLoadingText,
                EditedName = (string)ViewModelApplication.ClientShortName ?? "Client",
                OriginalName = (string)ViewModelApplication.ClientShortName ?? "Client Lookup",
                OriginalKid = (string)ViewModelApplication.FClientID ?? "4766E825-1B58-410D-B06B-5A2639CA22C8",
                EditedKid = (string)ViewModelApplication.FClientID,
                HierarchyTypeID = "1A8CCEE0-52D1-454B-8165-23EDB2241058",
                PrepareAction = SetClientHierarchySelectionAsync,
                PriorPopupViewModel = ViewModelApplication.CurrentPopupViewModel,
                CommitAction = UpdateClientSelectionAsync,
                //OriginalKid = (await ClientDataStore.GetLoginCredentialsAsync() ?).ClientID,

            };

            //CostCategoryID = new HierarchyItemSelectionViewModel
            //{
            //    Label = "Select Cost Category",
            //    //EditedName = mLoadingText,
            //    EditedName = (string)ViewModelApplication.CostHierarchyShortName ?? "Cost Category",
            //    OriginalName = (string)ViewModelApplication.CostHierarchyShortName ?? "Cost Category Lookup",
            //    OriginalKid = (string)ViewModelApplication.FCostHierarchyID,
            //    EditedKid = (string)ViewModelApplication.FCostHierarchyID,
            //    HierarchyTypeID = "1A8CCEE0-52D1-454B-8165-23EDB2241058",
            //    PrepareAction = SetHierarchySelectionAsync,
            //    CommitAction = UpdateClientSelectionAsync,
            //};

            CostHierarchy = new HierarchyItemSelectionViewModel
            {

                Label = "Cost Hierarchy",
                EditedName = (string)ViewModelApplication.CostHierarchyShortName ?? "Cost Hierarchy Name",
                OriginalName = (string)ViewModelApplication.CostHierarchyShortName ?? "Cost Hierarchy",
                OriginalKid = (string)ViewModelApplication.FCostHierarchyID,
                EditedKid = (string)ViewModelApplication.FCostHierarchyID,
                HierarchyTypeID = "64413ae7-822f-4866-9ebe-433083d699ac",
                PrepareAction = SetCostHierarchySelectionAsync,
                Level = 1,
                PriorPopupViewModel = ViewModelApplication.CurrentPopupViewModel,
                CommitAction = UpdateCostHierarchySelectionAsync,
            };



            // Create commands
            OpenCommand = new RelayCommand(Open);
            CloseCommand = new RelayCommand(async () => await CloseAsync());
            UpdateCommand = new RelayCommand(Update);
            LogoutCommand = new RelayCommand(async () => await LogoutAsync());
            ClearUserDataCommand = new RelayCommand(ClearUserData);
            LoadCommand = new RelayCommand(async () => await LoadAsync());
            SaveFirstNameCommand = new RelayCommand(async () => await SaveFirstNameAsync());
            SaveLastNameCommand = new RelayCommand(async () => await SaveLastNameAsync());
            SaveUsernameCommand = new RelayCommand(async () => await SaveUsernameAsync());
            SaveEmailCommand = new RelayCommand(async () => await SaveEmailAsync());
            UpdateClientSelectionCommand = new RelayCommand(async () => await UpdateClientSelectionAsync());
            // TODO: Get from localization
            LogoutButtonText = "Logout";
            UpdateButtonText = "Save Changes to User Credentials";
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// Opens the settings menu
        /// </summary>
        public void Open()
        {
            // Open settings menu
            ViewModelApplication.SettingsMenuVisible = true;
        }

        /// <summary>
        /// Closes the settings menu
        /// </summary>
        /// 

        public async Task CloseAsync()
        {
            await RunCommandAsync(() => PasswordIsChanging, async () =>
            {
                // Roll back any changes to fields if not proceeding with them
            
                Client.OriginalKid = ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).ClientID;
                Client.OriginalName = ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).ClientShortName ;
                CostHierarchy.OriginalKid = ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).CostHierarchyID ;
                CostHierarchy.OriginalName = ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).CostHierarchyShortName ;
                ViewModelApplication.FClientID = ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).ClientID;
                ViewModelApplication.ClientShortName = ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).ClientShortName;
                ViewModelApplication.FCostHierarchyID = ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).CostHierarchyID;
                ViewModelApplication.CostHierarchyShortName = ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).CostHierarchyShortName;
                Username.OriginalText = ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).Username ;
                LastName.OriginalText = ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).LastName ;
                FirstName.OriginalText = ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).FirstName ;
                Email.OriginalText = ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).Email;

            ViewModelApplication.SettingsMenuVisible = false;
            return true;
            });

        }

        public void Update()
        {
            TaskManager.RunAndForget(UpdateAsync);
        }

        /// <summary>
        /// Updates the backend with modified login credentials
        /// </summary>
        /// 

        public async Task UpdateAsync()
        {
            await RunCommandAsync(() => PasswordIsChanging, async () =>
            {
                // Close settings menu
                ViewModelApplication.SettingsMenuVisible = false;
                //LoginCredentials = LoginCredentials;
                var changed = (
                LoginCredentials.ClientID != ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).ClientID ||
                LoginCredentials.ClientShortName != ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).ClientShortName ||
                LoginCredentials.CostHierarchyID != ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).CostHierarchyID ||
                LoginCredentials.CostHierarchyShortName != ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).CostHierarchyShortName ||

                LoginCredentials.Username != ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).Username ||
                LoginCredentials.LastName != ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).LastName ||
                LoginCredentials.FirstName != ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).FirstName ||
                LoginCredentials.Email != ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).Email
            );
                if (changed)
                {
                    var scopedClientDataStore = ClientDataStore;
                    var updateApiModel = new UpdateUserProfileApiModel
                    {

                        ClientID = (LoginCredentials.ClientID != ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).ClientID)? LoginCredentials.ClientID : null,
                        ClientShortName = (LoginCredentials.ClientShortName != ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).ClientShortName) ? LoginCredentials.ClientShortName : null,
                        CostHierarchyID = (LoginCredentials.CostHierarchyID!= ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).CostHierarchyID) ? LoginCredentials.CostHierarchyID: null,
                        CostHierarchyShortName = (LoginCredentials.CostHierarchyShortName!= ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).CostHierarchyShortName) ? LoginCredentials.CostHierarchyShortName: null,
                        Username = (LoginCredentials.Username!= ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).Username) ? LoginCredentials.Username: null,
                        LastName = (LoginCredentials.LastName != ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).LastName) ? LoginCredentials.LastName : null,
                        FirstName = (LoginCredentials.FirstName != ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).FirstName) ? LoginCredentials.FirstName : null,
                        Email = (LoginCredentials.Email!= ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).Email) ? LoginCredentials.Email: null
                    };
                    var result = await WebRequests.PostAsync<ApiResponse>(
                        // Set URL
                        RouteHelpers.GetAbsoluteRoute(ApiRoutes.UpdateUserProfile),
                        // Pass the Api model
                        updateApiModel,
                    //Pass in user Token

                    bearerToken: ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).Token);

                    if (await result.HandleErrorIfFailedAsync("Load User Details Failed"))
                        // We are done
                        return true;


                    // Store the new user credentials the data store
                    await ClientDataStore.SaveLoginCredentialsAsync(LoginCredentials);

                    // Return successful
                    return true;

                }
                ////else
                //{
                return true;
            });

        }



        /// <summary>
        /// Logs the user out
        /// </summary>
        public async Task LogoutAsync()
        {
            // Lock this command to ignore any other requests while processing
            await RunCommandAsync(() => LoggingOut, async () =>
            {
                // TODO: Confirm the user wants to logout

                // Clear any user data/cache
                await ClientDataStore.ClearAllLoginCredentialsAsync();

                // Clean all application level view models that contain
                // any information about the current user
                ClearUserData();

                // Go to login page
                ViewModelApplication.GoToPage(ApplicationPage.Login);
            });
        }

        /// <summary>
        /// Clears any data specific to the current user
        /// </summary>

        public void ClearUserData()
        {
            // Clear all view models containing the users info
            FirstName.OriginalText = mLoadingText;
            LastName.OriginalText = mLoadingText;
            Username.OriginalText = mLoadingText;
            Email.OriginalText = mLoadingText;
        }

        /// <summary>
        /// Sets the settings view model properties based on the data in the client data store
        /// </summary>
        public async Task LoadAsync()
        {
            // Lock this command to ignore any other requests while processing
            await RunCommandAsync(() => SettingsLoading, async () =>
            {
            // Store single transient instance of client data store
            var scopedClientDataStore = ClientDataStore;

            // Update values from local cache
            await UpdateValuesFromLocalStoreAsync(scopedClientDataStore);

                // Get the user token
                //var token = (await scopedClientDataStore.GetLoginCredentialsAsync())?.Token;
                //var token = ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).Token;
                    var token = "";
                if (ViewModelApplication.CurrentCredential != null)
                { token = ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).Token; }
 
            //var test1 = (await scopedClientDataStore.GetLoginCredentialsAsync());
            //reduce wait time for token when required in calls to back end
            //ViewModelApplication.MToken = token;

            // If we don't have a token (so we are not logged in...)
            if (string.IsNullOrEmpty(token))
                    // Then do nothing more
                    return;

            //Add default client for selection of models
            //ViewModelApplication.FClientID =  (await scopedClientDataStore.GetLoginCredentialsAsync())?.ClientID;
            // Load user profile details from server
            //var path = RouteHelpers.GetAbsoluteRoute(WebRoutes.Private);
            var result = await WebRequests.PostAsync<ApiResponse<UserProfileDetailsApiModel>>(
                // Set URL
                RouteHelpers.GetAbsoluteRoute(ApiRoutes.GetUserProfile),
                //RouteHelpers.GetAbsoluteRoute(WebRoutes.Private),

                // Pass in user Token
                bearerToken: token);

            // If the response has an error...
            if (await result.HandleErrorIfFailedAsync("Load User Details Failed"))
                // We are done
                return;

                // TODO: Should we check if the values are different before saving?

                // Create data model from the response
                LoginCredentials = result.ServerResponse.Response.ToLoginCredentialsDataModel();
                //set changed flag to true if any changes detected in login credentials
                var changed = (
                LoginCredentials.ClientID != ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).ClientID ||
                LoginCredentials.ClientShortName != ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).ClientShortName ||
                LoginCredentials.CostHierarchyID != ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).CostHierarchyID ||
                LoginCredentials.CostHierarchyShortName != ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).CostHierarchyShortName ||

                LoginCredentials.Username != ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).Username||
                LoginCredentials.LastName != ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).LastName ||
                LoginCredentials.FirstName != ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).FirstName ||
                LoginCredentials.Email != ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).Email
                );
                LoginCredentials.Token = token; 

                // Re-add our known token




                if (changed)
                {
                    ViewModelApplication.CurrentCredential =
                new LoginCredentialsDataModel
                {
                    ClientID = LoginCredentials.ClientID,
                    ClientShortName = LoginCredentials.ClientShortName,
                    CostHierarchyID = LoginCredentials.CostHierarchyID,
                    Username = LoginCredentials.Username,
                    LastName = LoginCredentials.LastName,
                    FirstName = LoginCredentials.FirstName,
                    Email = LoginCredentials.Email,
                    Token = LoginCredentials.Token
                };

                        // Save the new information in the data store
                        await scopedClientDataStore.SaveLoginCredentialsAsync(LoginCredentials);

                        // Update values from local cache
                        await UpdateValuesFromLocalStoreAsync(scopedClientDataStore);
                }
            });
        }

        /// <summary>
        /// Saves the new First Name to the server
        /// </summary>
        /// <returns>Returns true if successful, false otherwise</returns>
        public async Task<bool> SaveFirstNameAsync()
        {
            // Lock this command to ignore any other requests while processing
            return await RunCommandAsync(() => FirstNameIsSaving, async () =>
            {
                // Update the First Name value on the server...
                //return await UpdateUserCredentialsValueAsync(
                //    // Display name
                //    "First Name",
                //    // Update the first name
                //    (credentials) => credentials.FirstName,
                //    // To new value
                //    FirstName.OriginalText,
                //    // Set Api model value
                //    (apiModel, value) => apiModel.FirstName = value
                //    );
                LoginCredentials.FirstName = FirstName.EditedText;
                return true;
            });
        }

        /// <summary>
        /// Saves the new Last Name to the server
        /// </summary>
        /// <returns>Returns true if successful, false otherwise</returns>
        public async Task<bool> SaveLastNameAsync()
        {
            // Lock this command to ignore any other requests while processing
            return await RunCommandAsync(() => LastNameIsSaving, async () =>
            {
                // Update the Last Name value on the server...
                //return await UpdateUserCredentialsValueAsync(
                    //// Display name
                    //"Last Name",
                    //// Update the last name
                    //(credentials) => credentials.LastName,
                    //// To new value
                    //LastName.OriginalText,
                    //// Set Api model value
                    //(apiModel, value) => apiModel.LastName = value
                    //);
                LoginCredentials.LastName = LastName.EditedText;
                return true;
            });
        }

        /// <summary>
        /// Saves the new Username to the server
        /// </summary>
        /// <returns>Returns true if successful, false otherwise</returns>
        public async Task<bool> SaveUsernameAsync()
        {
            // Lock this command to ignore any other requests while processing
            return await RunCommandAsync(() => UsernameIsSaving, async () =>
            {
                // Update the Username value on the server...
                //return await UpdateUserCredentialsValueAsync(
                //    // Display name
                //    "Username",
                //    // Update the first name
                //    (credentials) => credentials.Username,
                //    // To new value
                //    Username.OriginalText,
                //    // Set Api model value
                //    (apiModel, value) => apiModel.Username = value
                //    );
                LoginCredentials.Username = Username.EditedText;
                return true;
            });
        }

        /// <summary>
        /// Saves the new Email to the server
        /// </summary>
        /// <returns>Returns true if successful, false otherwise</returns>
        public async Task<bool> SaveEmailAsync()
        {
            // Lock this command to ignore any other requests while processing
            return await RunCommandAsync(() => EmailIsSaving, async () =>
            {
                // Update the Email value on the server...
                //return await UpdateUserCredentialsValueAsync(
                //    // Display name
                //    "Email",
                //    // Update the email
                //    (credentials) => credentials.Email,
                //    // To new value
                //    Email.OriginalText,
                //    // Set Api model value
                //    (apiModel, value) => apiModel.Email = value
                //    );
                LoginCredentials.Email = Email.EditedText;
                return true;
            });
        }


        /// <summary>
        /// Saves the new Password to the server
        /// </summary>
        /// <returns>Returns true if successful, false otherwise</returns>
        public async Task<bool> SavePasswordAsync()
        {
            // Lock this command to ignore any other requests while processing
            return await RunCommandAsync(() => PasswordIsChanging, async () =>
            {
                // Log it
                Logger.LogDebugSource($"Changing password...");

                // Get the current known credentials
                var credentials = await ClientDataStore.GetLoginCredentialsAsync();

                // Make sure the user has entered the same password
                if (Password.NewPassword.Unsecure() != Password.ConfirmPassword.Unsecure())
                {
                    // Display error
                    await UI.ShowMessage(new MessageBoxDialogViewModel
                    {
                        // TODO: Localize
                        Title = "Password Mismatch",
                        Message = "New password and confirm password must match"
                    });

                    // Return fail
                    return false;
                }

                // Update the server with the new password
                var result = await WebRequests.PostAsync<ApiResponse>(
                    // Set URL
                    RouteHelpers.GetAbsoluteRoute(ApiRoutes.UpdateUserPassword),
                    // Create API model
                    new UpdateUserPasswordApiModel
                    {
                        CurrentPassword = Password.CurrentPassword.Unsecure(),
                        NewPassword = Password.NewPassword.Unsecure()
                    }, 
                    // Pass in user Token
                    bearerToken: credentials.Token);

                // If the response has an error...
                if (await result.HandleErrorIfFailedAsync($"Change Password"))
                {
                    // Log it
                    Logger.LogDebugSource($"Failed to change password. {result.ErrorMessage}");

                    // Return false
                    return false;
                }

                // Otherwise, we succeeded...

                // Log it
                Logger.LogDebugSource($"Successfully changed password");

                // Return successful
                return true;
            });
        }


        /// <summary>
        /// Prepare Hierarchy Control for selection of Client
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SetClientHierarchySelectionAsync()
        {
            // Lock this command to ignore any other requests while processing

            return await RunCommandAsync(() => SetHierarchyCompleted, async () =>
            {
                // Update the First Name value on the server...

                ViewModelApplication.CurrentControlViewModel = Client;
                HierarchyParam = new ParameterHierarchyItemSelectApiModel
                {

                    //Level = 1
                    //Always allow the user to reset the default client
                    RootID = "4766E825-1B58-410D-B06B-5A2639CA22C8",
                    Level = 1
                };
                ViewModelApplication.CurrentPopupViewModel = new HierarchyTreeViewModel1(HierarchyParam);
                return true;
            });

        }


        public async Task<bool> SetCostHierarchySelectionAsync()
        {
            // Lock this command to ignore any other requests while processing

            return await RunCommandAsync(() => SetHierarchyCompleted, async () =>
            {
                // Update the First Name value on the server...

                //((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy.ClientID = ((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).Client.EditedKid;
                //((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy.RootID = ((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).Client.RootID;
                ViewModelApplication.CurrentControlViewModel = CostHierarchy;
                HierarchyParam = new ParameterHierarchyItemSelectApiModel
                {

                    ClientID = ViewModelApplication.FClientID,
                    Level = 1,
                    HierarchyTypeID = CostHierarchy.HierarchyTypeID
                };
                ViewModelApplication.CurrentPopupViewModel = new HierarchyTreeViewModel1(HierarchyParam);
                return true;
            });

        }


        public async Task<bool> UpdateClientSelectionAsync()
                {
                    // Lock this command to ignore any other requests while processing

                    return await RunCommandAsync(() => UpdateHierarchyCompleted, async () =>
                    {
                        // Update the First Name value on the server...
                        if (ViewModelApplication.FClientID != Client.EditedKid)
                        {
                            CostHierarchy.OriginalKid = null;
                            CostHierarchy.OriginalName = null;
                            ViewModelApplication.FCostHierarchyID = null;
                            ViewModelApplication.CostHierarchyShortName = null;
                        }
                        ViewModelApplication.FClientID = Client.EditedKid;
                        ViewModelApplication.ClientShortName = Client.EditedName;
                        Client.OriginalName = Client.EditedName;
                        Client.OriginalKid = Client.EditedKid;
                        LoginCredentials.ClientID = Client.EditedKid;
                        LoginCredentials.ClientShortName = Client.EditedName;


                        ViewModelApplication.PopupVisible = false;
                        ViewModelApplication.CurrentPopupViewModel = null;

                        ViewModelApplication.CurrentPopupContent = 0;

                        // Update the Client value on the server...
                        //var Test = await UpdateUserCredentialsValueAsync(
                        //// Display name
                        //"Client",
                        //// Update the first name
                        //propertyToUpdate: (credentials) => credentials.ClientID,
                        //// To new value
                        //newValue: Client.EditedKid,
                        //// Set Api model value
                        //setApiModel: (apiModel, value) => apiModel.ClientID = value
                        //);


                        //// Update the Client value on the server...
                        //return await UpdateUserCredentialsValueAsync(
                        //// Display name
                        //"ClientShortName",
                        //// Update the first name
                        //propertyToUpdate: (credentials) => credentials.ClientShortName,
                        //// To new value
                        //newValue: Client.EditedName,
                        //// Set Api model value
                        //setApiModel: (apiModel, value) => apiModel.ClientShortName = value
                        //); 
                        return true;

                    });

        }

        ///<summary>
        /// Update Client selection for current session
        /// </summary>
        /// <returns></returns>
        public async Task<bool> UpdateCostHierarchySelectionAsync()
        {
            // Lock this command to ignore any other requests while processing

            return await RunCommandAsync(() => UpdateHierarchyCompleted, async () =>
            {
                // Update the First Name value on the server...

                ViewModelApplication.FCostHierarchyID = CostHierarchy.EditedKid;
                ViewModelApplication.CostHierarchyShortName = CostHierarchy.EditedName;
                ViewModelApplication.CurrentPopupContent = 0;
                CostHierarchy.OriginalName = CostHierarchy.EditedName;
                Client.OriginalName = Client.EditedName;
                LoginCredentials.CostHierarchyID = CostHierarchy.EditedKid;
                LoginCredentials.CostHierarchyShortName = CostHierarchy.EditedName;
                ViewModelApplication.PopupVisible = false;
                ViewModelApplication.CurrentPopupViewModel = null;

                ViewModelApplication.CurrentPopupContent = 0;

            // Update the Client value on the server...
            //var Test = await UpdateUserCredentialsValueAsync(
            //// Display name
            //"CostHierarchy",
            //// Update the first name
            //propertyToUpdate: (credentials) => credentials.CostHierarchyID,
            //// To new value
            //newValue: CostHierarchy .EditedKid,
            //// Set Api model value
            //setApiModel: (apiModel, value) => apiModel.CostHierarchyID = value
            //);


            //// Update the Client value on the server...
            //return await UpdateUserCredentialsValueAsync(
            //// Display name
            //"CostHierarchyShortName",
            //// Update the first name
            //propertyToUpdate: (credentials) => credentials.CostHierarchyShortName,
            //// To new value
            //newValue: CostHierarchy.EditedName,
            //// Set Api model value
            //setApiModel: (apiModel, value) => apiModel.CostHierarchyShortName = value
                return true;
                //);


            });

        }




                //   return await UpdateUserCredentialsValueAsync(
                //// Display name
                //"Client Name",
                //   // Update the first name
                //   propertyToUpdate: (credentials) => credentials.ClientShortName,
                //   // To new value
                //   newValue: FClientID.EditedName,
                //   // Set Api model value
                //   setApiModel: (apiModel, value) => apiModel.ClientShortName = value
                //   );
            //    return true;


            //});

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Loads the settings from the local data store and binds them 
        /// to this view model
        /// </summary>
        /// <returns></returns>
        private async Task UpdateValuesFromLocalStoreAsync(IClientDataStore clientDataStore)
        {
            // Get the stored credentials

            var storedCredentials = await clientDataStore.GetLoginCredentialsAsync();

            if (storedCredentials != null)
            {
                ViewModelApplication.CurrentCredential =
                new LoginCredentialsDataModel
                {
                    ClientID = storedCredentials.ClientID,
                    ClientShortName = storedCredentials.ClientShortName,
                    CostHierarchyID = storedCredentials.CostHierarchyID,
                    CostHierarchyShortName = storedCredentials.CostHierarchyShortName,
                    Username = storedCredentials.Username,
                    LastName = storedCredentials.LastName,
                    FirstName = storedCredentials.FirstName,
                    Email = storedCredentials.Email,
                    Token = storedCredentials.Token
                };

            }


            // Set first name
            FirstName.OriginalText = storedCredentials?.FirstName;

            // Set last name
            LastName.OriginalText = storedCredentials?.LastName;

            // Set username
            Username.OriginalText = storedCredentials?.Username;

            // Set email
            Email.OriginalText = storedCredentials?.Email;

            //Set Client Root
            ViewModelApplication.ClientShortName = storedCredentials?.ClientShortName;
            ViewModelApplication.FClientID = storedCredentials?.ClientID;
            Client.OriginalName = storedCredentials?.ClientShortName;
            Client.OriginalKid = storedCredentials?.ClientID;



            //Set CostHierarchy
            ViewModelApplication.CostHierarchyShortName = storedCredentials?.CostHierarchyShortName;
            ViewModelApplication.FCostHierarchyID = storedCredentials?.CostHierarchyID;
            CostHierarchy.OriginalName = storedCredentials?.CostHierarchyShortName;
            CostHierarchy.OriginalKid = storedCredentials?.CostHierarchyID;
            

        }

        /// <summary>
        /// Updates a specific value from the client data store for the user profile details
        /// and attempts to update the server to match those details.
        /// For example, updating the first name of the user.
        /// </summary>
        /// <param name="displayName">The display name for logging and display purposes of the property we are updating</param>
        /// <param name="propertyToUpdate">The property from the <see cref="LoginCredentialsDataModel"/> to be updated</param>
        /// <param name="newValue">The new value to update the property to</param>
        /// <param name="setApiModel">Sets the correct property in the <see cref="UpdateUserProfileApiModel"/> model that this property maps to</param>
        /// <returns></returns>
        private async Task<bool> UpdateUserCredentialsValueAsync(string displayName, Expression<Func<LoginCredentialsDataModel, string>> propertyToUpdate, string newValue, Action<UpdateUserProfileApiModel, string> setApiModel)
        {
            // Log it
            Logger.LogDebugSource($"Saving {displayName}...");

            // Get the current known credentials
            var credentials = await ClientDataStore.GetLoginCredentialsAsync();

            // Get the property to update from the credentials
            var toUpdate = propertyToUpdate.GetPropertyValue(credentials);

            // Log it
            Logger.LogDebugSource($"{displayName} currently {toUpdate}, updating to {newValue}");

            // Check if the value is the same. If so...
            if (toUpdate == newValue)
            {
                // Log it
                Logger.LogDebugSource($"{displayName} is the same, ignoring");

                // Return true
                return true;
            }

            // Set the property
            propertyToUpdate.SetPropertyValue(newValue, credentials);

            // Create update details
            var updateApiModel = new UpdateUserProfileApiModel();

            // Ask caller to set appropriate value
            setApiModel(updateApiModel, newValue);

            // Update the server with the details
            var result = await WebRequests.PostAsync<ApiResponse>(
                // Set URL
                RouteHelpers.GetAbsoluteRoute(ApiRoutes.UpdateUserProfile),
                // Pass the Api model
                updateApiModel,
                // Pass in user Token
                bearerToken: credentials.Token);

            // If the response has an error...
            if (await result.HandleErrorIfFailedAsync($"Update {displayName}"))
            {
                // Log it
                Logger.LogDebugSource($"Failed to update {displayName}. {result.ErrorMessage}");

                // Return false
                return false;
            }

            // Log it
            Logger.LogDebugSource($"Successfully updated {displayName}. Saving to local database cache...");

            // Store the new user credentials the data store
            await ClientDataStore.SaveLoginCredentialsAsync(credentials);

            // Return successful
            return true;
        }

        #endregion
    }
}
