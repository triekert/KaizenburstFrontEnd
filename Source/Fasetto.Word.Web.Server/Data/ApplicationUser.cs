using Microsoft.AspNetCore.Identity;

namespace Fasetto.Word.Web.Server
{
    /// <summary>
    /// The user data and profile for our application
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        #region Public Properties

        /// <summary>
        /// The users first name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// The users last name
        /// </summary>
        public string LastName { get; set; }


        /// <summary>
        /// The users Root Client ID
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// The users default Client Setting
        /// </summary>
        public string CostHierarchyID { get; set; }


        /// <summary>
        /// The users default Client Setting Name
        /// </summary>
        public string ClientShortName { get; set; }

        /// <summary>
        /// The users default Client Setting Name
        /// </summary>
        public string CostHierarchyShortName { get; set; }

        #endregion
    }
}
