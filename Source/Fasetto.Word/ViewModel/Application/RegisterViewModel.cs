using Dna;
using Fasetto.Word.Core;
using System.Threading.Tasks;
using System.Windows.Input;
using static Fasetto.Word.DI;

namespace Fasetto.Word
{
    /// <summary>
    /// The View Model for a register screen
    /// </summary>
    public class RegisterViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// The username of the user
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The email of the user
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// A flag indicating if the register command is running
        /// </summary>
        public bool RegisterIsRunning { get; set; }

        #endregion

        #region Commands

        /// <summary>
        /// The command to login
        /// </summary>
        public ICommand LoginCommand { get; set; }

        /// <summary>
        /// The command to register for a new account
        /// </summary>
        public ICommand RegisterCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public RegisterViewModel()
        {
            // Create commands
            RegisterCommand = new RelayParameterizedCommand(async (parameter) => await RegisterAsync(parameter));
            LoginCommand = new RelayCommand(async () => await LoginAsync());
        }

        #endregion

        /// <summary>
        /// Attempts to register a new user
        /// </summary>
        /// <param name="parameter">The <see cref="SecureString"/> passed in from the view for the users password</param>
        /// <returns></returns>
        public async Task RegisterAsync(object parameter)
        {
            await RunCommandAsync(() => RegisterIsRunning, async () =>
            {
                // Call the server and attempt to register with the provided credentials
                var result = await WebRequests.PostAsync<ApiResponse<RegisterResultApiModel>>(
                // Set URL
                    RouteHelpers.GetAbsoluteRoute(ApiRoutes.Register),
                    // Create api model
                    new RegisterCredentialsApiModel
                    {
                        Username = Username,
                        Email = Email,
                        Password = (parameter as IHavePassword).SecurePassword.Unsecure()
                    });

                // If the response has an error...
                if (await result.HandleErrorIfFailedAsync("Register Failed"))
                    // We are done
                    return;

                // OK successfully registered (and logged in)... now get users data
                var loginResult = result.ServerResponse.Response;

                var dataModel = loginResult.ToLoginCredentialsDataModel();
                //set changed flag to true if any changes detected in login credentials
                var changed = (
                    dataModel.ClientID != ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).ClientID ||
                    dataModel.ClientShortName != ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).ClientShortName ||
                    dataModel.CostHierarchyID != ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).CostHierarchyID ||
                    dataModel.CostHierarchyShortName != ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).CostHierarchyShortName ||
                    dataModel.Username != ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).Username ||
                    dataModel.LastName != ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).LastName ||
                    dataModel.FirstName != ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).FirstName ||
                    dataModel.Email != ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).Email ||
                    dataModel.Token != ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).Token
                    );

                if (changed)
                {
                    ViewModelApplication.CurrentCredential = dataModel;
                    await ViewModelApplication.HandleSuccessfulLoginAsync(loginResult);                    // Save the new information in the data store

                }
                // Let the application view model handle what happens
                // with the successful login

            });
        }

        /// <summary>
        /// Takes the user to the login page
        /// </summary>
        /// <returns></returns>
        public async Task LoginAsync()
        {
            // Go to register page?
            ViewModelApplication.GoToPage(ApplicationPage.Login);

            await Task.Delay(1);
        }
    }
}
