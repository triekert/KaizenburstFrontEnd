
using System;
using System.Collections.Generic;

namespace Fasetto.Word.Core
{
    /// <summary>
    /// Return of selected  hierarchy item from hierarchy table on database 
    /// </summary>
    public class TransactionResultApiModel
    {
        #region Public Properties

        /// <summary>
        /// timestamp of Transaction
        /// </summary>

        public DateTime Posted_Date { get; set; }

        /// <summary>
        /// integer conversion for Year_month of transaction
        /// </summary>
        public int Month { get; set; }


        /// <summary>
        ///Description linked to transaction
        /// </summary>

        public string Description { get; set; }


        /// <summary>
        /// Transaction value - source transaction
        /// </summary>

        public decimal TransAmount { get; set; }

        /// <summary>
        /// Transaction value allocated to specific cost item
        /// </summary>


        public decimal ActualAmount { get; set; }

        /// <summary>
        /// Name of cost classification
        /// </summary>

        public string ShortName { get; set; }

        /// <summary>
        /// String representation of GUID for linked category
        /// </summary>


        public string KCategoryID { get; set; }

        /// <summary>
        /// String representation of GUID for transaction allocation


        public string KFinActualID { get; set; }

        /// <summary>
        /// String representation of GUID for originating transaction
        /// </summary>

        public string KFinTranID { get; set; }


        /// <summary>
        /// String representation of GUID for Client
        /// </summary>

        public string KClientID { get; set; }



        /// <summary>
        /// String representation of GUID for CostHierarchy
        /// </summary>

        public string KHierarchyID { get; set; }

        /// <summary>
        /// Calendar date from which Element is seen as active
        /// </summary>
        public DateTime DateEffective { get; set; }

        /// <summary>
        /// Calendar date from which Element is deactivated
        /// </summary>
        public DateTime DateDiscontinued { get; set; }


        /// <summary>
        /// String representation of GUID for linked Party (legal person including supplier, membership, family etc)
        /// </summary>


        public string KPartyID { get; set; }


        /// <summary>
        /// Name of linked party
        /// </summary>
        public string KPartyName { get; set; }


        /// <summary>
        /// String representation of GUID for linked Plant (Including any portion of plant down to lowest BOM level)
        /// </summary>

        public string KPlantID { get; set; }


        /// <summary>
        /// Name of linked plant
        /// </summary>
        public string KPlantName { get; set; }

        /// <summary>
        /// Attach the current activity to a Change object
        /// </summary>
        public string KChangeID { get; set; }

        /// <summary>
        /// Flag to indicate the transaction classification has changed
        /// </summary>
        public bool IsChanged { get; set; }

        /// <summary>
        /// String representation of GUID for originating transaction
        /// </summary>

        public string FCatSrchID { get; set; }


        /// <summary>
        ///  Bool set true if template to be modified
        /// </summary>
        public bool IsTemplate { get; set; }




        /// <summary>
        /// Flag for record management - "d", remove existing
        /// "a" add a new record
        /// "c" change allocation and/or value on existing record
        /// --could use an enumerator, but as there are only 3 values, just use fixed values
        /// --if an audit trail is required for changing allocations, the full change management functionality
        /// --will need to be implemented
        /// </summary>
        /// 

        public string ChangeType { get; set;}


        #endregion       
    }
}
