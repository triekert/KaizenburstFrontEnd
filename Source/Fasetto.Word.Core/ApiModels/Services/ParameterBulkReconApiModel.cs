using System;
using System.Collections.Generic;
using System.Linq;

namespace Fasetto.Word.Core
{
    /// <summary>
    /// Class representing each individual element of the hierarchy 
    /// </summary>
    /// 
    public class ParameterBulkReconApiModel
    {
        // <summary>
        /// GUID of BulkMeter Name
        /// </summary>
        public string BulkMeter { get; set; }


        /// <summary>
        ///Start Time for reconciliaition data
        /// </summary>
        public DateTime TimeStart { get; set; }

        /// <summary>
        ///End Time for reconciliaition data
        /// </summary>
        public DateTime TimeEnd { get; set; }
    }

}


