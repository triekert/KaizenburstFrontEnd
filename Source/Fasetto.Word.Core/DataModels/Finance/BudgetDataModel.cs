using System;
using System.Collections.Generic;
using System.Linq;

namespace Fasetto.Word.Core
{
    /// <summary>
    /// Class representing each individual element of the hierarchy 
    /// </summary>
    public class BudgetDataModel
    {
        /// <summary>
        /// Name of Category element
        /// </summary>
        public string ShortName { get; set; }


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



        public List<BudgetDataModel> Children { get; set; }





        #region KCategoryIdContainsText

        /// <summary>
        /// Check that the KCategoryId field contains data to enable the search
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool KCategoryIdContainsText(string text)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(KCategoryID))
                return false;

            return KCategoryID.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) > -1;
        }

        #endregion // NameContainsText


    }
}
