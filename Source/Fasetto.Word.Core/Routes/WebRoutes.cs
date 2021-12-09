namespace Fasetto.Word.Core
{
    /// <summary>
    /// The relative routes to all normal (non-API) calls in the server
    /// </summary>
    public static class WebRoutes
    {
        /// <summary>
        /// The route to the CreateUser method
        /// </summary>
        public const string CreateUser = "/user/create";

        /// <summary>
        /// The route to the Logout method
        /// </summary>
        public const string Logout = "/logout";

        /// <summary>
        /// The route to the Login method
        /// </summary>
        public const string Login = "/login";

        /// <summary>
        /// The route to the test the Private area
        /// </summary>
        public const string Private = "/private";


        /// <summary>
        /// The route to the test the Hiearchy
        /// </summary>
        public const string Hierarchy = "/hierarchy";
    }
}