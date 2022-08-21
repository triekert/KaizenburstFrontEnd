using Dna;
using Fasetto.Word.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static Dna.FrameworkDI;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;


namespace Fasetto.Word.Web.Server
{
    /// <summary>
    /// Manages the Web API calls
    /// </summary>
    [AuthorizeToken]
    public class ApiController : Controller
    {
        #region Protected Members

        /// <summary>
        /// The scoped Application context
        /// </summary>
        protected ApplicationDbContext mContext;

        /// <summary>
        /// The manager for handling user creation, deletion, searching, roles etc...
        /// </summary>
        protected UserManager<ApplicationUser> mUserManager;

        /// <summary>
        /// The manager for handling signing in and out for our users
        /// </summary>
        protected SignInManager<ApplicationUser> mSignInManager;



        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="context">The injected context</param>
        /// <param name="signInManager">The Identity sign in manager</param>
        /// <param name="userManager">The Identity user manager</param>
        public ApiController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            mContext = context;
            mUserManager = userManager;
            mSignInManager = signInManager;
        }

        #endregion

        #region Login / Register / Verify

        /// <summary>
        /// Tries to register for a new account on the server
        /// </summary>
        /// <param name="registerCredentials">The registration details</param>
        /// <returns>Returns the result of the register request</returns>
        [AllowAnonymous]
        [Route(ApiRoutes.Register)]
        public async Task<ApiResponse<RegisterResultApiModel>> RegisterAsync([FromBody]RegisterCredentialsApiModel registerCredentials)
        {
            // TODO: Localize all strings
            // The message when we fail to login
            var invalidErrorMessage = "Please provide all required details to register for an account";

            // The error response for a failed login
            var errorResponse = new ApiResponse<RegisterResultApiModel>
            {
                // Set error message
                ErrorMessage = invalidErrorMessage
            };

            // If we have no credentials...
            if (registerCredentials == null)
                // Return failed response
                return errorResponse;

            // Make sure we have a user name
            if (string.IsNullOrWhiteSpace(registerCredentials.Username))
                // Return error message to user
                return errorResponse;

            // Create the desired user from the given details
            var user = new ApplicationUser
            {
                UserName = registerCredentials.Username,
                FirstName = registerCredentials.FirstName,
                LastName = registerCredentials.LastName,
                Email = registerCredentials.Email
            };

            // Try and create a user
            var result = await mUserManager.CreateAsync(user, registerCredentials.Password);

            // If the registration was successful...
            if (result.Succeeded)
            {
                // Get the user details
                var userIdentity = await mUserManager.FindByNameAsync(user.UserName);

                // Send email verification
                await SendUserEmailVerificationAsync(user);

                // Return valid response containing all users details
                return new ApiResponse<RegisterResultApiModel>
                {
                    Response = new RegisterResultApiModel
                    {
                        FirstName = userIdentity.FirstName,
                        LastName = userIdentity.LastName,
                        Email = userIdentity.Email,
                        Username = userIdentity.UserName,
                        Token = userIdentity.GenerateJwtToken()
                    }
                };
            }
            // Otherwise if it failed...
            else
                // Return the failed response
                return new ApiResponse<RegisterResultApiModel>
                {
                    // Aggregate all errors into a single error string
                    ErrorMessage = result.Errors.AggregateErrors()
                };
        }

        /// <summary>
        /// Logs in a user using token-based authentication
        /// </summary>
        /// <returns>Returns the result of the login request</returns>
        [AllowAnonymous]
        [Route(ApiRoutes.Login)]
        public async Task<ApiResponse<UserProfileDetailsApiModel>> LogInAsync([FromBody]LoginCredentialsApiModel loginCredentials)
        {
            // TODO: Localize all strings
            // The message when we fail to login
            var invalidErrorMessage = "Invalid username or password";

            // The error response for a failed login
            var errorResponse = new ApiResponse<UserProfileDetailsApiModel>
            {
                // Set error message
                ErrorMessage = invalidErrorMessage
            };

            // Make sure we have a user name
            if (loginCredentials?.UsernameOrEmail == null || string.IsNullOrWhiteSpace(loginCredentials.UsernameOrEmail))
                // Return error message to user
                return errorResponse;

            // Validate if the user credentials are correct...

            // Is it an email?
            var isEmail = loginCredentials.UsernameOrEmail.Contains("@");

            // Get the user details
            var user = isEmail ? 
                // Find by email
                await mUserManager.FindByEmailAsync(loginCredentials.UsernameOrEmail) : 
                // Find by username
                await mUserManager.FindByNameAsync(loginCredentials.UsernameOrEmail);

            // If we failed to find a user...
            if (user == null)
                // Return error message to user
                return errorResponse;

            // If we got here we have a user...
            // Let's validate the password

            // Get if password is valid
            var isValidPassword = await mUserManager.CheckPasswordAsync(user, loginCredentials.Password);

            // If the password was wrong
            if (!isValidPassword)
                // Return error message to user
                return errorResponse;

            // If we get here, we are valid and the user passed the correct login details

            // Get username
            var username = user.UserName;

            // Return token to user
            return new ApiResponse<UserProfileDetailsApiModel>
            {
                // Pass back the user details and the token
                Response = new UserProfileDetailsApiModel
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Username = user.UserName,
                    Token = user.GenerateJwtToken()
                }
            };
        }

        [AllowAnonymous]
        [Route(ApiRoutes.VerifyEmail)]
        [HttpGet]
        public async Task<ActionResult> VerifyEmailAsync(string userId, string emailToken)
        {
            // Get the user
            var user = await mUserManager.FindByIdAsync(userId);

            // If the user is null
            if (user == null)
                // TODO: Nice UI
                return Content("User not found");

            // If we have the user...

            // Verify the email token
            var result = await mUserManager.ConfirmEmailAsync(user, emailToken);

            // If succeeded...
            if (result.Succeeded)
                // TODO: Nice UI
                return Content("Email Verified :)");

            // TODO: Nice UI
            return Content("Invalid Email Verification Token :(");
        }

        #endregion

        #region User Profile

        /// <summary>
        /// Returns the users profile details based on the authenticated user
        /// </summary>
        /// <returns></returns>
        [Route(ApiRoutes.GetUserProfile)]
        public async Task<ApiResponse<UserProfileDetailsApiModel>> GetUserProfileAsync()
        {
            // Get user claims
            var user = await mUserManager.GetUserAsync(HttpContext.User);

            // If we have no user...
            if (user == null)
                // Return error
                return new ApiResponse<UserProfileDetailsApiModel>()
                {
                    // TODO: Localization
                    ErrorMessage = "User not found"
                };

            // Return token to user
            return new ApiResponse<UserProfileDetailsApiModel>
            {
                // Pass back the user details and the token
                Response = new UserProfileDetailsApiModel
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Username = user.UserName
                }
            };
        }

        /// <summary>
        /// Attempts to update the users profile details
        /// </summary>
        /// <param name="model">The user profile details to update</param>
        /// <returns>
        ///     Returns successful response if the update was successful, 
        ///     otherwise returns the error reasons for the failure
        /// </returns>
        [Route(ApiRoutes.UpdateUserProfile)]
        public async Task<ApiResponse> UpdateUserProfileAsync([FromBody]UpdateUserProfileApiModel model)
        {
            #region Declare Variables

            // Make a list of empty errors
            var errors = new List<string>();

            // Keep track of email change
            var emailChanged = false;

            #endregion

            #region Get User

            // Get the current user
            var user = await mUserManager.GetUserAsync(HttpContext.User);

            // If we have no user...
            if (user == null)
                return new ApiResponse
                {
                    // TODO: Localization
                    ErrorMessage = "User not found"
                };

            #endregion

            #region Update Profile

            // If we have a first name...
            if (model.FirstName != null)
                // Update the profile details
                user.FirstName = model.FirstName;

            // If we have a last name...
            if (model.LastName != null)
                // Update the profile details
                user.LastName = model.LastName;

            // If we have a email...
            if (model.Email != null &&
                // And it is not the same...
                !string.Equals(model.Email.Replace(" ", ""), user.NormalizedEmail))
            {
                // Update the email
                user.Email = model.Email;

                // Un-verify the email
                user.EmailConfirmed = false;

                // Flag we have changed email
                emailChanged = true;
            }

            // If we have a username...
            if (model.Username != null)
                // Update the profile details
                user.UserName = model.Username;

            #endregion

            #region Save Profile

            // Attempt to commit changes to data store
            var result = await mUserManager.UpdateAsync(user);

            // If successful, send out email verification
            if (result.Succeeded && emailChanged)
                // Send email verification
                await SendUserEmailVerificationAsync(user);

            #endregion

            #region Respond

            // If we were successful...
            if (result.Succeeded)
                // Return successful response
                return new ApiResponse();
            // Otherwise if it failed...
            else
                // Return the failed response
                return new ApiResponse
                {
                    ErrorMessage = result.Errors.AggregateErrors()
                };

            #endregion
        }

        /// <summary>
        /// Attempts to update the users password
        /// </summary>
        /// <param name="model">The user password details to update</param>
        /// <returns>
        ///     Returns successful response if the update was successful, 
        ///     otherwise returns the error reasons for the failure
        /// </returns>
        [Route(ApiRoutes.UpdateUserPassword)]
        public async Task<ApiResponse> UpdateUserPasswordAsync([FromBody]UpdateUserPasswordApiModel model)
        {
            #region Declare Variables

            // Make a list of empty errors
            var errors = new List<string>();

            #endregion

            #region Get User

            // Get the current user
            var user = await mUserManager.GetUserAsync(HttpContext.User);

            // If we have no user...
            if (user == null)
                return new ApiResponse
                {
                    // TODO: Localization
                    ErrorMessage = "User not found"
                };

            #endregion

            #region Update Password

            // Attempt to commit changes to data store
            var result = await mUserManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            #endregion

            #region Respond

            // If we were successful...
            if (result.Succeeded)
                // Return successful response
                return new ApiResponse();
            // Otherwise if it failed...
            else
                // Return the failed response
                return new ApiResponse
                {
                    ErrorMessage = result.Errors.AggregateErrors()
                };

            #endregion
        }

        #endregion

        #region Contacts

        /// <summary>
        /// Searches all users for any users that match the search credentials
        /// </summary>
        /// <param name="model">The search credentials</param>
        /// <returns>
        ///     Returns a list of found contact details if successful, 
        ///     otherwise returns the error reasons for the failure
        /// </returns>
        [Route(ApiRoutes.SearchUsers)]
        public async Task<ApiResponse<SearchUsersResultsApiModel>> SearchUsersAsync([FromBody]SearchUsersApiModel model)
        {
            #region Get User

            // Get the current user
            var user = await mUserManager.GetUserAsync(HttpContext.User);

            // If we have no user...
            if (user == null)
                return new ApiResponse<SearchUsersResultsApiModel>
                {
                    // TODO: Localization
                    ErrorMessage = "User not found"
                };

            #endregion

            #region Check Valid Search Credentials

            // Check if the user provided both a first and last name
            var firstOrLastNameMissing = string.IsNullOrEmpty(model?.FirstName) || string.IsNullOrEmpty(model?.LastName);

            // Check if enough details are provided for a search
            var notEnoughSearchDetails =
                // First and last name
                firstOrLastNameMissing &&
                // Username
                string.IsNullOrEmpty(model?.Username) &&
                // Phone number
                string.IsNullOrEmpty(model?.PhoneNumber) &&
                // Email
                string.IsNullOrEmpty(model?.Email);

            // If we don't have enough details for a search...
            if (notEnoughSearchDetails)
                // Return error
                return new ApiResponse<SearchUsersResultsApiModel>
                {
                    // TODO: Localization
                    ErrorMessage = "Please provide a first and last name, or an email, username or phone number"
                };

            #endregion

            #region Find Users

            // Create a found user variable
            var foundUser = default(ApplicationUser);

            // If we have a username...
            if (!string.IsNullOrEmpty(model.Username))
                // Find the user by username
                foundUser = await mUserManager.FindByNameAsync(model.Username);

            // If we have an email...
            if (foundUser == null && !string.IsNullOrEmpty(model.Email))
                // Find the user by email
                foundUser = await mUserManager.FindByEmailAsync(model.Email);

            // If we have a phone number...
            if (foundUser == null && !string.IsNullOrEmpty(model.PhoneNumber))
            {
                // Find the user by phone number
                foundUser = mUserManager.Users.FirstOrDefault(u => 
                                // Phone number is confirmed
                                u.PhoneNumberConfirmed &&
                                // Phone number must match exactly 
                                // including country code if provided
                                u.PhoneNumber == model.PhoneNumber);
            }

            // If we found a user...
            if (foundUser != null)
            {
                // Return that users details
                return new ApiResponse<SearchUsersResultsApiModel>
                {
                    Response = new SearchUsersResultsApiModel
                        {
                            new SearchUsersResultApiModel
                            {
                                Username = foundUser.UserName,
                                FirstName = foundUser.FirstName,
                                LastName = foundUser.LastName
                            }
                        }
                };
            }

            // Create a new list of results
            var results = new SearchUsersResultsApiModel();

            // If we have a first and last name...
            if (!firstOrLastNameMissing)
            {
                // Search for users...
                var foundUsers = mUserManager.Users.Where(u =>
                                    // With the same first name
                                    u.FirstName == model.FirstName &&
                                    // And same last name
                                    u.LastName == model.LastName)
                                    // And for now, limit to 100 results
                                    // TODO: Add pagination
                                    .Take(100);

                // If we found any users...
                if (foundUsers.Any())
                {
                    // Add each users details
                    results.AddRange(foundUsers.Select(u => new SearchUsersResultApiModel
                    {
                        Username = u.UserName,
                        FirstName = u.FirstName,
                        LastName = u.LastName
                    }));
                }
            }

            // Return the results
            return new ApiResponse<SearchUsersResultsApiModel>
            {
                Response = results
            };

            #endregion
        }


        #endregion


        #region Finance

        #region Hiearachy

        /// <summary>
        /// Returns Hierarchy for Navigation
        /// </summary>
        /// <param name="model">The search credentials</param>
        /// <returns>
        ///     Returns a list of hiearchy items if successful, 
        ///     otherwise returns the error reasons for the failure
        /// </returns>


        [Route(ApiRoutes.ReturnHierarchy)]

        public async Task<ApiResponse<HierarchyResultListApiModel>> ReturnHierarchyAsync([FromBody]string model)
        {
            #region Get User

            // Get the current user
            var user = await mUserManager.GetUserAsync(HttpContext.User);

            // If we have no user...
            if (user == null)
                return new ApiResponse<HierarchyResultListApiModel>
                {
                    // TODO: Localization
                    ErrorMessage = "User not found"
                };

            #endregion //Get User

            #region sql query
            var SqlString = "SELECT  c.[ShortName],coalesce(c.[Description],'') Description,coalesce(convert(nvarchar(50),c.[KCategoryID]),'') KCategoryID, coalesce(convert(nvarchar(50),c.[ParentCategoryID]),'') ParentCategoryID," +
                "coalesce(convert(nvarchar(50),c.[fIconID]),'') Icon,coalesce(c.DateEffective,convert(datetime,'1753/1/1'))DateEffective,coalesce(c.DateDiscontinued,convert(datetime,'9999/12/31'))DateDiscontinued,coalesce(convert(nvarchar(50),c.[fChangeID]),'') fChangeID,c.[isUnderReview],c.[isNewElement]," +
                "coalesce(c.[Page],'') Page, coalesce(c.[Root],'') Root,p.[isMenuItem] FROM [Admin].[HierarchyGeneric] c INNER JOIN  [Admin].[HierarchyGeneric] p on p.kCategoryID = c.fHierarchyID  WHERE c.fHierarchyID = " +
                "'" + model + "'";
                ;// " + model;
            try
            {
                // Try and run the task
                var dataset = await GetDataSetAsync(SqlString);
                var dt = dataset.Tables[0];
                var hierarchyResultListApiModel = new HierarchyResultListApiModel();
                var results = hierarchyResultListApiModel;


                foreach (DataRow row in dt.Rows)
                {
                    var u = new HierarchyResultApiModel
                    {
                        ShortName = (string)(row[0]),
                        Description = (string)row[1],
                        KCategoryID = (string)row[2],
                        ParentCategoryID = (string)row[3],
                        FHierarchyID = model,
                        FIconID = (string)row[4],
                        DateEffective =  (DateTime)row[5],
                        DateDiscontinued =  (DateTime)row[6],
                        KChangeID = (string)row[7],

                        IsUnderReview =  (row[8] != DBNull.Value) ?   (bool)row[8] :false ,
                        IsNewElement = false,
                        Page = (string)row[10],
                        Root = (string)row[11],
                        IsMenuItem = (row[12] != DBNull.Value) ? (bool)row[12] : false,

                    };
                    var mShortName = u.ShortName;
                    results.Add(u);

                }
                var matches = results.Where(x => x.ShortName == "Brendon Snakes").ToList();
                return new ApiResponse<HierarchyResultListApiModel>
                {

                    Response = results
                };
                #endregion //sql query


            }
            catch (Exception ex)
            {
                // Log error
                //Logger.LogErrorSource(ex.ToString(), origin: origin, filePath: filePath, lineNumber: lineNumber);

                // Throw it as normal
                throw;
            }
            //var SqlString1 =  "SELECT  [ShortName],coalesce([Description],'') Description,coalesce([Card],'')Card,coalesce([Frequency],'')Frequency,coalesce(convert(nvarchar(50),[KCategoryID]),'') KCategoryID,coalesce(convert(nvarchar(50),[ParentCategoryID]),'') ParentCategoryID,coalesce(convert(nvarchar(50),[fIconID]),'') Icon FROM [Kaizen]." + model;// [Finance].[vwFinHierarchy]";

            #region Find Users


            //convert response into HierarchyListDataModel
            #endregion //Find Users
        }


        [Route(ApiRoutes.PersistHierarchy)]
        /// <summary>
        /// Persist hierarchy changes made on front end
        /// </summary>
        /// <param name="mPersist"></param>
        /// <returns></returns>
        public async Task<ApiResponse<HierarchyResultListApiModel>> PersistHierarchyAsync([FromBody]HierarchyResultListApiModel mPersist)

        {
                #region Get User

                // Get the current user
                var user = await mUserManager.GetUserAsync(HttpContext.User);

                // If we have no user...
                if (user == null)
                    return new ApiResponse<HierarchyResultListApiModel>
                    {
                        // TODO: Localization
                        ErrorMessage = "User not found"
                    };

            #endregion //Get User   
                //TO Do: solve the problem of identifying a group of elements by the root ID, when auto generating first element  of hierarchy menu elements - won't work unless root is also included...
            var results = mPersist.Where(x => x.ParentCategoryID == "00000000-0000-0000-0000-000000000000").OrderBy(x => x.ShortName).ToList();

            var mHierarchyLink = results.FirstOrDefault().KCategoryID;


            var para = new SqlParameter[14];
            para[0] = new SqlParameter("@ShortName", SqlDbType.NVarChar);
            para[1] = new SqlParameter("@Description", SqlDbType.NVarChar);
            para[2] = new SqlParameter("@kCategoryID", SqlDbType.UniqueIdentifier);
            para[3] = new SqlParameter("@ParentCategoryID", SqlDbType.UniqueIdentifier);
            para[4] = new SqlParameter("@fIconID", SqlDbType.UniqueIdentifier);
            para[5] = new SqlParameter("@DateEffective", SqlDbType.DateTime);
            para[6] = new SqlParameter("@DateDiscontinued", SqlDbType.DateTime);
            para[7] = new SqlParameter("@fChangeID", SqlDbType.UniqueIdentifier);
            para[8] = new SqlParameter("@isUnderReview", SqlDbType.Bit);
            para[9] = new SqlParameter("@isNewElement", SqlDbType.Bit);
            para[10] = new SqlParameter("@Page", SqlDbType.NVarChar);
            para[11] = new SqlParameter("@Root", SqlDbType.NVarChar);
            para[12] = new SqlParameter("@isMenuItem", SqlDbType.Bit);
            para[13] = new SqlParameter("@fHierarchyID", SqlDbType.UniqueIdentifier);


            //var SqlString = "INSERT INTO [Admin].[HierarchyGeneric]  (ShortName,Description,kCategoryID,ParentCategoryID,fIconID,DateEffective,DateDiscontinued,fChangeID,isUnderReview,isNewElement)" +// ) " +
            //    "VALUES (@ShortName,@Description,@kCategoryID,@ParentCategoryID,@fIconID,@DateEffective,@DateDiscontinued,@fChangeID,@isUnderReview,@isNewElement)";//)";
            var SqlString = "INSERT INTO [Admin].[HierarchyGeneric]  (fHierarchyID,ShortName,Description,kCategoryID,ParentCategoryID,fIconID,DateEffective,DateDiscontinued,fChangeID,isUnderReview,isNewElement,Page,Root,isMenuItem)" +// ) " +

                "VALUES (@fHierarchyID,@ShortName,@Description,@kCategoryID,@ParentCategoryID,@fIconID,@DateEffective,@DateDiscontinued,@fChangeID,@isUnderReview,@isNewElement,@Page,@Root,@isMenuItem)";//)";
            //If elements are to be added, insert into backend
            results = mPersist.Where(x => x.IsNewElement == true).OrderBy(x => x.ShortName).ToList();//
            if (results.Count >0)

            
            { foreach (var row in results)
                {
                    para[0].Value =  row.ShortName;
                    para[1].Value = row.Description;
                    para[2].Value = new Guid(row.KCategoryID);
                    if (row.ParentCategoryID == null || row.ParentCategoryID == "")
                        para[3].Value = new Guid();
                    else
                        para[3].Value = new Guid(row.ParentCategoryID);

                    if (row.FIconID == null||row.FIconID =="")
                    para[4].Value = new Guid();
                    else
                    para[4].Value = new Guid(row.FIconID); 
                    if (row.DateEffective != Convert.ToDateTime("0001/01/01 00:00:00"))
                        para[5].Value = row.DateEffective;
                    else
                        para[5].Value = Convert.ToDateTime("1753/01/01 00:00:00");
                    if (row.DateDiscontinued != Convert.ToDateTime("0001/01/01 00:00:00"))
                        para[6].Value = row.DateDiscontinued;
                    else
                        para[6].Value = Convert.ToDateTime("9999/12/31 00:00:00");

                    if (row.KChangeID != null)
                    { para[7].Value = new Guid(row.KChangeID); }
                    else
                        para[7].Value = new Guid();

                    para[8].Value = row.IsUnderReview;
                    para[9].Value = row.IsNewElement;
                    if (row.Page == null)
                    { para[10].Value = DBNull.Value; }
                    else
                    { para[10].Value = row.Page; }
                    if (row.Root == null)
                    { para[11].Value = DBNull.Value; }
                    else
                    { para[11].Value = row.Root; }

                    para[12].Value = row.IsMenuItem;
                    para[13].Value = new Guid(row.FHierarchyID);
                    try
                    {
                        // Try and run the task
                        _ = await ExecuteAsync(SqlString, para);

                    }
                    catch (Exception ex)
                    {
                        // Log error
                        //Logger.LogErrorSource(ex.ToString(), origin: origin, filePath: filePath, lineNumber: lineNumber);

                        // Throw it as normal
                        throw;
                    }
 
                }
             }

            SqlString = "UPDATE [Admin].[HierarchyGeneric] SET ShortName = @ShortName,Description = @Description,kCategoryID = @kCategoryID," +
                "ParentCategoryID = @ParentCategoryID,fIconID = @fIconID,DateEffective = @DateEffective,DateDiscontinued = @DateDiscontinued,fChangeID = @fChangeID," +
                "isUnderReview = @isUnderReview,isNewElement = @isNewElement,Page = @Page,Root = @Root,isMenuItem =@isMenuItem WHERE kCategoryID = @kCategoryID AND DateEffective = @DateEffective"; 

            //If elements are to be updated, insert into backend
            results = mPersist.Where(x => x.IsNewElement != true && x.IsUnderReview == true).OrderBy(x => x.ShortName).ToList();//
            if (results.Count > 0)

            {
                foreach (var row in results)
                {
                    para[0].Value = row.ShortName;
                    para[1].Value = row.Description;
                    para[2].Value = new Guid(row.KCategoryID);
                    if (row.ParentCategoryID == null || row.ParentCategoryID == "")
                        para[3].Value = new Guid();
                    else
                        para[3].Value = new Guid(row.ParentCategoryID);

                    if (row.FIconID == null || row.FIconID == "")
                        para[4].Value = new Guid();
                    else
                        para[4].Value = new Guid(row.FIconID);
                    if (row.DateEffective != Convert.ToDateTime("0001/01/01 00:00:00"))
                        para[5].Value = row.DateEffective;
                    else
                        para[5].Value = Convert.ToDateTime("1753/01/01 00:00:00");
                    if (row.DateDiscontinued != Convert.ToDateTime("0001/01/01 00:00:00"))
                        para[6].Value = row.DateDiscontinued;
                    else
                        para[6].Value = Convert.ToDateTime("9999/12/31 00:00:00");

                    if (row.KChangeID != null)
                    { para[7].Value = new Guid(row.KChangeID); }
                    else
                        para[7].Value = new Guid();

                    para[8].Value = row.IsUnderReview;
                    para[9].Value = row.IsNewElement;
                    if (row.Page == null)
                    { para[10].Value = DBNull.Value; }
                    else
                    { para[10].Value = row.Page; }
                    if (row.Root == null)
                    { para[11].Value = DBNull.Value; }
                    else
                    { para[11].Value = row.Root; }
                    para[12].Value = row.IsMenuItem;
                    para[13].Value = new Guid(row.FHierarchyID);
                    try
                    
                    {
                        // Try and run the task
                        _ = await ExecuteAsync(SqlString, para);

                    }
                    catch (Exception ex)
                    {
                        // Log error
                        //Logger.LogErrorSource(ex.ToString(), origin: origin, filePath: filePath, lineNumber: lineNumber);

                        // Throw it as normal
                        throw;
                    }


                }
            }

            SqlString = "Delete from [Admin].[HierarchyGeneric]  WHERE kCategoryID = @kCategoryID AND DateEffective = @DateEffective";

            //If elements are to be deleted, find and remove
            results = mPersist.Where(x => x.IsDeleteElement == true && x.IsUnderReview == true).OrderBy(x => x.ShortName).ToList();//
            if (results.Count > 0)

            {
                foreach (var row in results)
                {
 
                    para[2].Value = new Guid(row.KCategoryID);

                    if (row.DateEffective != Convert.ToDateTime("0001/01/01 00:00:00"))
                        para[5].Value = row.DateEffective;
                    else
                        para[5].Value = Convert.ToDateTime("1753/01/01 00:00:00");



                    try
                    {
                        // Try and run the task
                        _ = await ExecuteAsync(SqlString, para);

                    }
                    catch (Exception ex)
                    {
                        // Log error
                        //Logger.LogErrorSource(ex.ToString(), origin: origin, filePath: filePath, lineNumber: lineNumber);

                        // Throw it as normal
                        throw;
                    }
                }
            }



                    #region sql query



                    //var dt = dataset.Tables[0];
                    //var hierarchyResultListApiModel = new HierarchyResultListApiModel();
                    //var results = hierarchyResultListApiModel;


                    //foreach (DataRow row in dt.Rows)
                    //{
                    //    var u = new HierarchyResultApiModel
                    //    {
                    //        ShortName = (string)(row[0]),
                    //        Description = (string)row[1],
                    //        Card = (string)row[2],
                    //        Frequency = (int)row[3],
                    //        KCategoryID = (string)row[4],
                    //        ParentCategoryID = (string)row[5],
                    //        FIconID = (string)row[6]
                    //    };

                    //    results.Add(u);

                    //}
                    return new ApiResponse<HierarchyResultListApiModel>
            {
                    //Response = results
                };
            #endregion //sql query

            #region Find Users


            #endregion //Find Users
        }
      
        #endregion


        #endregion


        #region Private Helpers

        /// <summary>
        /// Sends the given user a new verify email link
        /// </summary>
        /// <param name="user">The user to send the link to</param>
        /// <returns></returns>
        private async Task SendUserEmailVerificationAsync(ApplicationUser user)
        {
            // Get the user details
            var userIdentity = await mUserManager.FindByNameAsync(user.UserName);

            // Generate an email verification code
            var emailVerificationCode = await mUserManager.GenerateEmailConfirmationTokenAsync(user);

            // TODO: Replace with APIRoutes that will contain the static routes to use
            var confirmationUrl = $"http://{Request.Host.Value}/api/verify/email/?userId={HttpUtility.UrlEncode(userIdentity.Id)}&emailToken={HttpUtility.UrlEncode(emailVerificationCode)}";

            // Email the user the verification code
            await FasettoEmailSender.SendUserVerificationEmailAsync(user.UserName, userIdentity.Email, confirmationUrl);
        }


        //EXECUTE ASYNC
        public Task<int> ExecuteAsync(string sSQL, params SqlParameter[] parameters) => Task.Run(() =>
                {
                    using (var newConnection = new SqlConnection(Configuration["ConnectionStrings:DefaultConnection"]))
                    using (var newCommand = new SqlCommand(sSQL, newConnection))
                    {
                        newCommand.CommandType = CommandType.Text;
                        if (parameters != null) newCommand.Parameters.AddRange(parameters);

                        newConnection.Open();
                        var feedback = newCommand.ExecuteNonQuery();
                        newCommand.Parameters.Clear();
                        return feedback;
                    }
                });

        // RETURN DATASET
        public Task<DataSet> GetDataSetAsync(string sSQL, params SqlParameter[] parameters)
            {
                return Task.Run(() =>
                {
                    using (var newConnection = new SqlConnection(Configuration["ConnectionStrings:DefaultConnection"]))
                    using (var mySQLAdapter = new SqlDataAdapter(sSQL, newConnection))
                    {
                        mySQLAdapter.SelectCommand.CommandType = CommandType.Text;
                        if (parameters != null) mySQLAdapter.SelectCommand.Parameters.AddRange(parameters);

                        var myDataSet = new DataSet();
                        mySQLAdapter.Fill(myDataSet);
                        return myDataSet;
                    }
                });
            }
        //}

        #endregion
    }
}

//con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);

//cmd = new SqlCommand();

//cmd.Connection = con;



//cmd.Parameters.Add(new SqlParameter("@RollNo", SqlDbType.Int));

//cmd.Parameters["@RollNo"].Value = textBox1.Text;



//cmd.Parameters.Add(new SqlParameter("@Name", SqlDbType.VarChar));

//cmd.Parameters["@Name"].Value = textBox2.Text;



//cmd.Parameters.Add(new SqlParameter("@Fees", SqlDbType.Float));

//cmd.Parameters["@Fees"].Value = textBox3.Text;



//cmd.CommandText = "insert into student values(@RollNo, @Name, @Fees)";



//con.Open();

//cmd.ExecuteNonQuery();