using System;
using System.Collections.Generic;

namespace Fasetto.Word.Core
{
    /// <summary>
    /// Billing periods configured for selected Client on database
    /// </summary>
    public class BudgetPeriodResultApiModel
    {
        #region Public Properties


        /// <summary>
        /// String representation of GUID for Budget
        /// </summary>
        public string KBudgetID { get; set; }


        /// <summary>
        /// String representation of GUID for Cost hierarchy
        /// </summary>
        public string CostHierarchy { get; set; }


        /// <summary>
        /// String representation of GUID for Billing Period
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// Integer representing the start month for the budget
        /// </summary>
        public int MonthStart { get; set; }


        /// <summary>
        /// Integer representing the last month for the budget
        /// </summary>
        public int MonthEnd { get; set; }



    }


    #endregion

}
