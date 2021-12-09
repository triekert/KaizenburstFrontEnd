using Fasetto.Word.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Data.SqlTypes;
using System.Threading.Tasks;
using static Dna.FrameworkDI;
using System.Security.Cryptography.X509Certificates;
using System;
using System.Linq;

namespace Fasetto.Word.Web.Server
{
    /// <summary>
    /// Manages the standard web server pages
    /// </summary>
    public class HomeController : Controller
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
        public HomeController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            mContext = context;
            mUserManager = userManager;
            mSignInManager = signInManager;
        }

        #endregion

        /// <summary>
        /// Basic welcome page
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Creates our single user for now
        /// </summary>
        /// <returns></returns>
        [Route(WebRoutes.CreateUser)]
        public async Task<IActionResult> CreateUserAsync()
        {
            var result = await mUserManager.CreateAsync(new ApplicationUser
            {
                UserName = "angelsix",
                Email = "contact@angelsix.com",
                FirstName = "Luke",
                LastName = "Malpass"
            }, "password");

            if (result.Succeeded)
                return Content("User was created", "text/html");

            return Content("User creation failed", "text/html");
        }

        /// <summary>
        /// Log the user out
        /// </summary>
        /// <returns></returns>
        [Route(WebRoutes.Logout)]
        public async Task<IActionResult> SignOutAsync()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return Content("done");
        }

        /// <summary>
        /// An auto-login page for testing
        /// </summary>
        /// <param name="returnUrl">The url to return to if successfully logged in</param>
        /// <returns></returns>
        [Route(WebRoutes.Login)]
        public async Task<IActionResult> LoginAsync(string returnUrl)
        {
            // Sign out any previous sessions
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

            // Sign user in with the valid credentials
            var result = await mSignInManager.PasswordSignInAsync("angelsix", "password", true, false);

            // If successful...
            if (result.Succeeded)
            {
                // If we have no return URL...
                if (string.IsNullOrEmpty(returnUrl))
                    // Go to home
                    return RedirectToAction(nameof(Index));

                // Otherwise, go to the return url
                return Redirect(returnUrl);
            }

            return Content("Failed to login", "text/html");
        }
        [AuthorizeToken]
        [Route(WebRoutes.Private)]
        public async Task<IActionResult> PrivateAsync()
        {
            var test = HttpContext.User.Identity.Name;
            var user = await mUserManager.GetUserAsync(HttpContext.User);
            return Content($"testing of cookie translation. Welcome {HttpContext.User.Identity.Name}", "text/html");
        }


        [Route(WebRoutes.Hierarchy)]
        public async Task<IActionResult> HierarchyAsync()
        
        
        
        {


            var SqlString = "SELECT  [ShortName],coalesce([Description],'') Description,coalesce([Card],'')Card,coalesce([Frequency],'')Frequency,coalesce(convert(nvarchar(50),[KCategoryID]),'') KCategoryID,coalesce(convert(nvarchar(50),[ParentCategoryID]),'') ParentCategoryID,coalesce(convert(nvarchar(50),[fIconID]),'') Icon FROM [Kaizen].[Finance].[vwFinHierarchy]";

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
                    Card = (string)row[2],
                    Frequency = (int)row[3],
                    KCategoryID = (string)row[4],
                    ParentCategoryID = (string)row[5],
                    FIconID = (string)row[6]
                };

                results.Add(u);

            }



            //  pass the root item through for extracting the hierarchy
            var mHDML = new HierarchyListDataModel();
            mHDML.Clear();
            mHDML.AddRange(ExpandHierarchyData(results, ""));


            var hierarchyListDataModel = ExpandHierarchyData(results, "");


            return Content($"testing of Hieararch extract", "text/html");
        }

        /// <summary>
        /// This funtion builds a hierarchy of elements based on a
        /// a Hierarchy result returned when querying a database structure
        /// on which the hierarchy structures are persisted
        /// </summary>
        /// <param name="results"></param>
        /// This is a class of <HierarchyResultListApiModel></HierarchyResultListApiModel>
        /// <param name="KCategoryID"></param>
        /// the ID of the parent for finding descendants is passsed through as a string
        /// <returns></returns>
        private HierarchyListDataModel ExpandHierarchyData(HierarchyResultListApiModel results, string KCategoryID)
        {

            // Find all children
            var children = results.Where(x => x.ParentCategoryID == KCategoryID).ToList();

            // Hierarchy cannot be expanded
            if (children.Count() == 0)
                return null;
            //...otherwise, return all descendants recursively
            var elements = new HierarchyListDataModel();
            foreach (var item in children)
            { 
                var ud1 = new HierarchyDataModel
                {
                    //var u = hierarchyDataModel;
                    ShortName = item.ShortName,
                    Description = item.Description,
                    Card = item.Card,
                    Frequency = item.Frequency,
                    KCategoryID = item.KCategoryID,
                    ParentCategoryID = item.ParentCategoryID,
                    FIconID = item.FIconID
                };
                    ud1.Children = ExpandHierarchyData(results, ud1.KCategoryID);
                    elements.Add(ud1);
                }

            return elements;
        }

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
                    var ctr = mySQLAdapter.Fill(myDataSet);


                    
                    return myDataSet;
                }
            });
        }

    }
}
