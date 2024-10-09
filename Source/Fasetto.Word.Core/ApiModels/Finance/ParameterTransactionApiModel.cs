using System;
using System.Collections.Generic;
using System.Linq;

namespace Fasetto.Word.Core
{
    /// <summary>
    /// Class representing each individual element of the hierarchy 
    /// </summary>
    /// 
    public class ParameterTransactionApiModel
    {
        // <summary>
        /// GUID of Client selected for transaction management
        /// </summary>
        public string Client { get; set; }

        /// <summary>
        ///Start Month for managing transactions
        /// </summary>
        public int MonthStart { get; set; }

        /// <summary>
        ///End Month for managing transactions
        /// </summary>
        public int MonthEnd { get; set; }

        // <summary>
        /// GUID of Category selected for transaction management- (Optional, will only return transactions for desired category if populated )
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        ///Start time for retrieving TOD data
        /// </summary>
        public int TODStart { get; set; }


   


    }

}


