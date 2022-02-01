
using System;
using System.Collections.Generic;

namespace Fasetto.Word.Core
{
    /// <summary>
    /// Return of selected  hierarchy item from hierarchy table on database 
    /// </summary>
    public class HierarchyResultApiModel
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

        public string ParentCategoryID { get; set; }
        /// <summary>
        /// Parent ShortName of hiearchy item
        /// </summary>
        public string ParentShortName { get; set; }

        /// <summary>
        //the link to tthe ICON used to depict this category
        /// </summary>
        public string FIconID { get; set; }

        /// <summary>
        /// Calendar date from which Element is seen as active
        /// </summary>
        public DateTime DateEffective { get; set; }

        /// <summary>
        /// Calendar date from which Element is deactivated
        /// </summary>
        public DateTime DateDiscontinued { get; set; }

        /// <summary>
        /// Attach the current activity to a Change object
        /// </summary>
        public string KChangeID { get; set; }
        /// <summary>
        /// Property to indicate whether element is being evaluated by a change request
        /// and whether it should be excluded from current operations
        /// </summary>
        public bool IsUnderReview { get; set; }
        /// <summary>
        /// Property to indicate whether this element has been newly added change request
        /// and whether it should be excluded from current operations
        /// </summary>
        public bool IsNewElement { get; set; }

        /// <summary>
        /// Property to indicate whether this element is to be removed from the persistence layer
        /// </summary>
        public bool IsDeleteElement { get; set; }

        #endregion       
    }
}
