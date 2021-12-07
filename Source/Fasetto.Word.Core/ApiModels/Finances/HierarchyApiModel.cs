using System;
using System.Collections.Generic;

namespace Fasetto.Word.Core
{
    /// <summary>
    /// Persistance of expense hierarchy item on database
    /// </summary>
    public class HierarchyApiModel
    {
        #region Public Properties

        //string represetntation of UniqueIdentifier for a Category element
        public string FinHierarchyID { get; set; }

        //name of Category element
        public string ShortName { get; set; }
        //description of Category element
        public string Description { get; set; }
        //string representation of card where the expense category is determined by the linked card
        public string Card { get; set; }
        //integer indicating the number of months between expected occurrences of expense category
        public int Frequency { get; set; }
        //string representation of GUID for a Category element
        public string KCategoryID { get; set; }
        //sstring representation of GUID for the Parent category of a Category element
        //the parent of all root elements will be NULL... any hierarchy will have at least one root element
        public string ParentCategoryId { get; set; }
        ////sub categories that are also categories in themself
        //public List<ExpenseHierarchy> Children { get; set; }


        #endregion
    }
}
