using System;
using System.Collections.Generic;
using System.Text;

namespace Fasetto.Word.Core
{
     public class MeterConsumptionResultApiModel
    {
        #region Public Properties
        /// <summary>
        /// Erf - property designation
        /// </summary>
        public string Erf { get; set; }

        /// <summary>
        /// Name of erf owner
        /// </summary>
            public string Customer { get; set; }

        /// <summary>
        /// start time of period
        /// </summary>

            public DateTime Timestart { get; set; }

        /// <summary>
        /// reading from meter at the start of theperiod
        /// </summary>
            public decimal MeterReading { get; set; }

        /// <summary>
        /// volume consumedfor the period (beteen start time of the priod and the start of the next period)
        /// 
        /// </summary>
        public decimal Volume { get; set; }

        #endregion   Public Properties

    }
}
