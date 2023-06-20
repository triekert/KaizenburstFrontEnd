using System;
using System.Collections.Generic;
using System.Linq;

namespace Fasetto.Word.Core
{
    /// <summary>
    /// Class for reauesting meter reading detail
    /// </summary>
    /// 
    public class ParameterMeterReadingApiModel
    {
        // <summary>
        /// GUID of Property 
        /// </summary>
        public string PropertyID { get; set; }


        /// <summary>
        ///Start Time for meter reading extract
        /// </summary>
        public DateTime TimeStart { get; set; }

        /// <summary>
        ///End Time for meter reading  data
        /// </summary>
        public DateTime TimeEnd { get; set; }

        /// <summary>
        ///Type of meter
        /// </summary>
        public string MeterType { get; set; }


        /// <summary>
        /// Reference time used for data extraction based on data state at that time
        /// </summary>
        public DateTime DateReference { get; set; }       

    }

}


