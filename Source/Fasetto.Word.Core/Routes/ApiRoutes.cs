namespace Fasetto.Word.Core
{
    /// <summary>
    /// The relative routes to all Api calls in the server
    /// </summary>
    public static class ApiRoutes
    {
        #region Login / Register

        /// <summary>
        /// The route to the Register Api method
        /// </summary>
        public const string Register = "api/register";

        /// <summary>
        /// The route to the Login Api method
        /// </summary>
        public const string Login = "api/login";

        /// <summary>
        /// The route to the VerifyEmail Api method
        /// </summary>
        /// <remarks>
        ///     Pass the userId and emailToken as get parameters.
        ///     i.e. /api/verify/email?userId=...&emailToken=...
        /// </remarks>
        public const string VerifyEmail = "api/verify/email";

        #endregion

        #region User Profile

        /// <summary>
        /// The route to the GetUserProfile Api method
        /// </summary>
        public const string GetUserProfile = "api/user/profile";

        /// <summary>
        /// The route to the UpdateUserProfile Api method
        /// </summary>
        public const string UpdateUserProfile = "api/user/profile/update";

        /// <summary>
        /// The route to the UpdateUserPassword Api method
        /// </summary>
        public const string UpdateUserPassword = "api/user/password/update";

        #endregion

        #region Contacts

        /// <summary>
        /// The route to the SearchUsers Api method
        /// </summary>
        public const string SearchUsers = "api/users/search";

        #endregion

        #region Finance

        /// <summary>
        /// The route to the ReturnHierarchy Api method
        /// </summary>
        public const string ReturnHierarchy = "api/finance/hierarchy";


        /// <summary>
        /// The route to the PersitHierarchy Api method
        /// </summary>
        public const string PersistHierarchy = "api/finance/pershier";


        /// <summary>
        /// The route to the ReturnHierarchy Api method
        /// </summary>
        public const string ReturnExpenditure = "api/finance/expenditure";


        /// <summary>
        /// The route to the PersitHierarchy Api method
        /// </summary>
        public const string PersistExpenditure = "api/finance/expenditure";
        #endregion


        #region Services
        /// <summary>
        /// The route to the Loading of Water Meter Reading Api method
        /// </summary>
        /// 
        public const string LoadReadings = "api/Services/LoadReadings";
        //public const string LoadReadings = "api.netqedge.com/v1?From=2022-10-19T18%3A13%3A31.001&To=2022-10-19T20%3A13%3A31.000";

        /// <summary>
        /// The route to the accessing of Recon Data for Bulk Water Meters
        /// </summary>
        /// 
        public const string ReturnBulkRecon = "api/Services/BulkRecon";
        //public const string LoadReadings = "api.netqedge.com/v1?From=2022-10-19T18%3A13%3A31.001&To=2022-10-19T20%3A13%3A31.000";


        #endregion
    }
}
