using System;
using System.Collections.Generic;

namespace Fasetto.Word.Core
{
    /// <summary>
    /// Class representing each complete tree of the hierarchy o
    /// </summary>
    public class ExpenditureTreeDataModel
    {
        /// <summary>
        /// Date transaction was posted
        /// </summary>
        public DateTime PostedDate { get; set; }

        /// <summary>
        ///Total Expenditure against the transaction
        /// </summary>
        public decimal ActualAmount { get; set; }

        /// <summary>
        ///Allocated Expenditure of this  transaction against budget item
        /// </summary>
        public decimal TransactionAmount { get; set; }

        /// <summary>
        ///Budget amount allocated against budget item
        /// </summary>
        public decimal BudgetedlAmount { get; set; }

        /// <summary>
        ///Budget category
        /// </summary>
        /// 
        public string ShortName { get; set; }

        /// <summary>
        ///description linked to transaction
        /// </summary>
        /// 
        public string Description { get; set; }

        /// <summary>
        //string representation of card where the expense category is determined by the linked card
        /// </summary>
        /// 
        public string Card { get; set; }

        /// <summary>
        ///Month of transaction
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        //string representation of GUID for a Category element
        /// </summary>
        public string KCategoryID { get; set; }

        /// <summary>
        //string representation of GUID for the Parent category of a Category element
        //the parent of all root elements will be NULL... any hierarchy will have at least one root element
        /// </summary>
        public string ParentCategoryID { get; set; }

        /// <summary>
        //string representation of GUID for a specific Cost Structures
        /// </summary>
        public string FHierarchyID { get; set; }

        /// <summary>
        //string representation of GUID for a transaction
        /// </summary>
        public string KFinTranID { get; set; }

        /// <summary>
        //string representation of GUID for a transaction allocation
        /// </summary>
        public string KFinActualID { get; set; }

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



        /// <summary>
        //sub categories that are also categories in themself
        /// </summary>
        public List<ExpenditureTreeDataModel> Children { get; set; }
    }
}
