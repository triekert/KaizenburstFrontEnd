using System;
using System.Collections.Generic;
using System.Linq;

namespace Fasetto.Word.Core
{
    /// <summary>
    /// Class representing each individual element of the hierarchy 
    /// </summary>
    /// 
    public class ParameterBillingAdjustmentApiModel
    {
        //string representation of GUID for Property requiring adjustment
        public string FPropertyID { get; set; }

        //string representation of GUID for Property requiring adjustment
        public string FBillingPeriodID { get; set; }


       /// <summary>
        ///Date related to the  consumption being adjusted
        /// </summary>
        public DateTime DateStart { get; set; }



        /// <summary>
        ///Adjustment related to the portion of consumption in the first month
        /// </summary>
        public decimal Adjustment { get; set; }



        /// <summary>
        /// The effective date for this adjustment
        /// </summary>
        public DateTime DateEffective { get; set; }


        //string representation of GUID for Change request linked to adjustment
        public string FChangeID { get; set; }


    }

}


