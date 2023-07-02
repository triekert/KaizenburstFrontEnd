using System;
using System.Collections.Generic;
using System.Linq;

namespace Fasetto.Word.Core
{
    /// <summary>
    /// Class representing each individual element of the hierarchy 
    /// </summary>
    public class TransactionDataModel
    {

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





    }
}
