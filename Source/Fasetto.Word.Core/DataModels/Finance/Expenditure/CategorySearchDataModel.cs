using System;
using System.Collections.Generic;
using System.Linq;

namespace Fasetto.Word.Core
{
    /// <summary>
    /// Class representing each individual element of the hierarchy 
    /// </summary>
    public class CategorySearchDataModel
    {

        /// <summary>
        /// Date transaction was posted
        /// </summary>
        public string DescrLookup { get; set; }
        /// <summary>
        ///Proportion of transaction allocated to costCategory
        /// </summary>
        public int Proportion { get; set; }
        /// <summary>
        ///Card No linked to transaction
        /// </summary>
        public string Card { get; set; }

        /// <summary>
        ///Unique Identifier
        /// </summary>
        public string KCatSrchID { get; set; }

        /// <summary>
        /// The linked category for the expenses
        /// </summary>
        public string FFinCategoryID { get; set; }


        /// <summary>
        /// The linked category for the expenses
        /// </summary>
        public string FPartyID { get; set; }

        /// <summary>
        /// Description of hiearchy item
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        //index of category
        /// </summary>
        /// 
        public int Index { get; set; }

        /// <summary>
        ///Fixed portion of transaction allocated to this category
        /// </summary>
        public decimal FixedAmount { get; set; }
        /// <summary>
        /// The Identifier of this hierarchy item
        /// </summary>
        /// 
        public string KCategoryID { get; set; }

        /// <summary>
        /// The Identifier of the Hierarchy Type for this item
        /// </summary>
        public string FHierarchyID { get; set; }
        /// <summary>
        /// Parent ID  of hiearchy item
        /// </summary>
        public string ParentCategoryID { get; set; }


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
