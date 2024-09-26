
using System;
using System.Security.Cryptography;

namespace Fasetto.Word.Core
{
    /// <summary>
    /// Return of selected  hierarchy item from hierarchy table on database 
    /// </summary>
    public class BudgetResultApiModel
    {
        #region Public Properties

        //name of Category element
        public string ShortName { get; set; }
        //description of Category element
        public string Description { get; set; }
        ////string representation of card where the expense category is determined by the linked card
        //public string Card { get; set; }
        ////integer indicating the number of months between expected occurrences of expense category
        //public int Frequency { get; set; }
        //string representation of GUID for a Category element
        public string KCategoryID { get; set; }


        //sstring representation of GUID for the Parent category of a Category element
        //the parent of all root elements will be NULL... any hierarchy will have at least one root element

        public string ParentCategoryID { get; set; }


        /// <summary>
        /// Name of Category element
        /// </summary>
        public string ParentShortName { get; set; }


        /// <summary>
        /// Aggregate Total for element and all descendants
        /// </summary>
        public decimal BudgetAmountTotal { get; set; }

        /// <summary>
        ///Aggregate Total for all descendant elements
        /// </summary>
        public decimal BudgetAmountDescendants { get; set; }

        /// <summary>
        ///Amount budgeted directly for the selected element (excluding descendant aggregates)
        /// </summary>
        public decimal BudgetAmount { get; set; }

        /// <summary>
        /// Reading at start
        /// </summary>
        public int Month { get; set; }


        #endregion
    }
}
