using System;
using System.Collections.Generic;

namespace Fasetto.Word.Core
{
    /// <summary>
    /// Billing periods configured for selected Client on database
    /// </summary>
    public class CostHierarchyResultApiModel
    {
        #region Public Properties
     
        /// <summary>
        /// String representation of GUID for Billing Period
        /// </summary>
        public string KCategoryID { get; set; }

        /// <summary>
        /// Start time of Billing Period
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// String representation of GUID for Client owning Billing Period
        /// </summary>
        public string FClientID { get; set; }




    }


    #endregion
    
}
