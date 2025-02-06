using Dna;
using Fasetto.Word.Core;
using System.Threading.Tasks;
using System.Windows.Input;
using static Fasetto.Word.DI;
using System.Net.Http;
using System.Security.Policy;
using System.Windows.Forms;
using System.Net;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace Fasetto.Word
{
    /// <summary>
    /// The View Model for a register screen
    /// </summary>
    public class LoadReadingsPageViewModel : BaseViewModel
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
        /// A flag indicating if the LoadReadings command is running
        /// </summary>
        public bool LoadReadingsIsRunning { get; set; }

        #endregion

        #region Commands

        /// <summary>
        /// The command to login
        /// </summary>
        public ICommand CloseCommand { get; set; }

        /// <summary>
        /// The command to LoadReadings for a new account
        /// </summary>
        public ICommand LoadReadingsCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public LoadReadingsPageViewModel()
        {
            // Create commands

            //var request1 = WebRequest.CreateHttp("http://api.netqedge.com/v1? From = 2022 - 10 - 19T18 % 3A13 % 3A31.001 & To = 2022 - 10 - 19T20 % 3A13 % 3A31.000l"); /// v1//RouteHelpers.GetAbsoluteRoute(ApiRoutes.LoadReadings));
            //request1.Method = HttpMethod.Get.ToString();
            //request1.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {"UbCZyYRin01xwXdFwda4Z901Qax0OywBzzHTDSA5"}");
            //" ? From = 2022 - 10 - 19T18 % 3A13 % 3A31.001 & To = 2022 - 10 - 19T20 % 3A13 % 3A31.000l"); ;
            //var result = await request1.GetResponseAsync() ;
            LoadReadingsCommand = new RelayParameterizedCommand(async (parameter) => await LoadReadingsAsync(parameter));
            CloseCommand = new RelayCommand(Close);
            //configureRequest?.Invoke(request1);
        }

        #endregion

            /// <summary>
            /// Attempts to LoadReadings a new user
            /// </summary>
            /// <param name="parameter">The <see cref="SecureString"/> passed in from the view for the users password</param>
            /// <returns></returns>
        public async Task LoadReadingsAsync(object parameter)
        {
            await RunCommandAsync(() => LoadReadingsIsRunning, async () =>
            {

                // Store single transient instance of client data store
                var scopedClientDataStore = ClientDataStore;

                //// Update values from local cache
                //// Get the user token
                var token = (await scopedClientDataStore.GetLoginCredentialsAsync())?.Token;
                //// Call the server and attempt to register with the provided credentials
                //// If we don't have a token (then not logged in...)
                if (string.IsNullOrEmpty(token))
                    // Then do nothing more
                    return;
                var result = await WebRequests.PostAsync<ApiResponse>(
                // Set URL
                    RouteHelpers.GetAbsoluteRoute(ApiRoutes.LoadReadings), null
                    ,
                    bearerToken: token);
                  
                return;

            });
        }

        public static async Task<HttpWebResponse> Get1Async()
        {
            var request1 = WebRequest.CreateHttp("https://api.netqedge.com/v1?From=2022-10-13%2021%3A11%3A28&To=2022-10-14%2021%3A11%3A28");//?From=2022-10-19T18%3A13%3A31.001&To=2022-10-20T20%3A13%3A31.000; /// v1//RouteHelpers.GetAbsoluteRoute(ApiRoutes.LoadReadings));
            request1.Method = HttpMethod.Get.ToString();
            request1.Headers.Add("x-api-key: UbCZyYRin01xwXdFwda4Z901Qax0OywBzzHTDSA5");

            //" ? From = 2022 - 10 - 19T18 % 3A13 % 3A31.001 & To = 2022 - 10 - 19T20 % 3A13 % 3A31.000l"); ;
            //var result = await request1.GetResponseAsync();
            try
            {
                // Return the raw server response
                return await request1.GetResponseAsync() as HttpWebResponse;
            }
            // Catch Web Exceptions (which throw for things like 401)
            catch (WebException ex)
            {
                // If we got a response...
                if (ex.Response is HttpWebResponse httpResponse)
                    // Return the response
                    return httpResponse;

                // Otherwise, we don't have any information to be able to return
                // So re-throw
                throw;
            }


        }
        /// <summary>
        /// Takes the user to the login page
        /// </summary>
        /// <returns></returns>
        public void Close()
        {
            // Close settings menu
            ViewModelApplication.SideMenuVisible = true;
            //ViewModelApplication.CurrentSideMenuViewModel = null;
            ViewModelApplication.GoToPage(ApplicationPage.Chat);


        }
    }
}
