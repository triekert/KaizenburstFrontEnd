using System;
using System.Collections.Generic;

namespace Fasetto.Word.Core
{
    /// <summary>
    /// Billing periods configured for selected Client on database
    /// </summary>
    public class BillingPeriodResultApiModel
    {
        #region Public Properties
     
        /// <summary>
        /// String representation of GUID for Billing Period
        /// </summary>
        public string KBillingPeriodID { get; set; }

        /// <summary>
        /// Start time of Billing Period
        /// </summary>
        public DateTime TimeStart { get; set; }

        /// <summary>
        /// String representation of GUID for Client owning Billing Period
        /// </summary>
        public string FClientID { get; set; }


        /// <summary>
        /// End time of Billing Period
        /// </summary>
        public DateTime TimeEnd { get; set; }

    }


    #endregion
    
}
