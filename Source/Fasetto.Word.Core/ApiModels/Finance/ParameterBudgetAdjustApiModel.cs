
using System;
using System.Security.Cryptography;

namespace Fasetto.Word.Core
{
    /// <summary>
    /// Return of selected  hierarchy item from hierarchy table on database 
    /// </summary>
    public class ParameterBudgetAdjustApiModel
    {
        #region Public Properties


        ////string representation of card where the expense category is determined by the linked card
        //public string Card { get; set; }
        ////integer indicating the number of months between expected occurrences of expense category
        //public int Frequency { get; set; }
        //string representation of GUID for a Category element
        public string KCategoryID { get; set; }

        /// <summary>
        /// The actual budget to be adjusted
        /// </summary>
        public string KBudgetID { get; set; }


        /// <summary>
        /// Aggregate Total for element and all descendants
        /// </summary>
        public decimal BudgetAmountTotal { get; set; }



        /// <summary>
        ///Amount budgeted directly for the selected element (excluding descendant aggregates)
        /// </summary>
        public decimal BudgetAmount { get; set; }

        /// <summary>
        /// Aggregate Total (Adjusted) for element and all descendants
        /// </summary>
        public decimal BudgetAmountTotalAdj { get; set; }


        /// <summary>
        ///Amount budgeted (adjusted)directly for the selected element (excluding descendant aggregates)
        /// </summary>
        public decimal BudgetAmountAdj { get; set; }

        /// <summary>
        /// Integer representing the budget month
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// variable set true if budget should only be adjusted for month in question, false is rest of period to be updated...
        /// 
        /// </summary>
        public bool IsMonth { get; set; }


        #endregion
    }
}
