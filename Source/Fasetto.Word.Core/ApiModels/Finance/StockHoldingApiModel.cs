
using System;
using System.Collections.Generic;

namespace Fasetto.Word.Core
{
    /// <summary>
    /// Return of selected  document item from web server repository table on database 
    /// </summary>
    /// 
            #region Public Properties


    public class StockHoldingApiModel
    {



        /// <summary>
        /// String representation of GUID for selected category
        /// </summary>
        public string FCategoryID { get; set; }

        /// <summary>
        ///  SOH for selected category in Units
        /// </summary>
        public int SOH { get; set; }


        /// <summary>
        /// Timestamp for latest SOH reading
        /// </summary>
        public DateTime DateOfTransaction { get; set; }



#endregion Public Properties

    }
}
