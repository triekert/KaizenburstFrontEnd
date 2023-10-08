using System;
using System.Collections.Generic;
using System.Linq;

namespace Fasetto.Word.Core
{
    /// <summary>
    /// Class representing each individual element of the hierarchy 
    /// </summary>
    /// 
    public class ParameterBillingApiModel
    {
        // <summary>
        /// GUID of Client selected for transaction management
        /// </summary>
        public string BillingPeriodID { get; set; }


        /// <summary>
        ///Effective Date to be used for billing calculation adjustments
        /// </summary>
        public DateTime DateEffective { get; set; }







    }

}


