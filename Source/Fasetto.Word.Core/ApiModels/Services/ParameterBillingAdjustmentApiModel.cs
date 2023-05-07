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
        public string KCategoryID { get; set; }

        //string representation of GUID for Property requiring adjustment
        public string KBillingPeriodID { get; set; }


       /// <summary>
        ///Date related to the  consumption being adjusted
        /// </summary>
        public DateTime DateStart { get; set; }



        /// <summary>
        ///Adjustment related to the portion of consumption in the first month
        /// </summary>
        public decimal Adjustment { get; set; }


        /// <summary>
        ///Adjustment related to the portion of consumption in the Next month
        /// </summary>
        public decimal AdjustmentN { get; set; }

    }

}


