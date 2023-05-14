
using System;
using System.Collections.Generic;

namespace Fasetto.Word.Core
{
    /// <summary>
    /// Return of selected  hierarchy item from hierarchy table on database 
    /// </summary>
    public class BulkReconDetailResultApiModel
    {
        #region Public Properties


        /// <summary>
        /// GUID of BulkMeter linked Property
        /// </summary>
        public string BulkMeter { get; set; }


        /// <summary>
        ///name of property/Bulkmeter
        /// </summary>
        public string ShortName { get; set; }


        /// <summary>
        ///timestamp  beginning
        /// </summary>
        public DateTime TimeStart { get; set; }


        /// <summary>
        ///Consumption for meter over defined period
        /// </summary>
        public decimal Volume { get; set; }

        /// <summary>
        ///Meter reading (calculated) for start of measurement period
        /// </summary>
        public float MeterReadingCalc { get; set; }


        /// <summary>
        ///timestamp  of last meter reading prior to start of period
        /// </summary>
        public DateTime ReadingTimePrior { get; set; }

        /// <summary>
        /// Last Actual Meter reading obtained prior to start of measurement period
        /// </summary>
        public float ReadingPrior { get; set; }


        /// <summary>
        ///timestamp  of first meter reading after start of period
        /// </summary>
        public DateTime ReadingTimeNext { get; set; }

        /// <summary>
        /// First Actual Meter reading obtained after start of measurement period
        /// </summary>
        public float ReadingNext { get; set; }

        /// <summary>
        ///timestamp  end
        /// </summary>
        public DateTime TimeEnd { get; set; }

        /// <summary>
        ///Meter reading (calculated) for end of measurement period
        /// </summary>
        public float MeterReadingCalcE { get; set; }


        /// <summary>
        ///timestamp  of last meter reading prior to End of period
        /// </summary>
        public DateTime ReadingTimePriorE { get; set; }

        /// <summary>
        /// Last Actual Meter reading obtained prior to end of measurement period
        /// </summary>
        public float ReadingPriorE { get; set; }


        /// <summary>
        ///timestamp  of first meter reading after end of period
        /// </summary>
        public DateTime ReadingTimeNextE { get; set; }

        /// <summary>
        /// First Actual Meter reading obtained after start of measurement period
        /// </summary>
        public float ReadingNextE { get; set; }


        #endregion       
    }
}
