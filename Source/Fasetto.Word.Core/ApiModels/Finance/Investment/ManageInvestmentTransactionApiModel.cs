using System;

namespace Fasetto.Word.Core
{
    /// <summary>
    /// Details used to search for investment transactions
    /// </summary>
    public class ManageInvestmentTransactionApiModel
    {
        #region Public Properties

        /// <summary>
        /// Start Date for Transaction Search inclusive)
        /// </summary>
        public DateTimeOffset StartDateOfTransaction { get; set; }

        /// <summary>
        /// End Date for Transaction Search (inclusive)
        /// </summary>
        public DateTimeOffset EndDateOfTransaction { get; set; }

        /// <summary>
        /// Instrument involved in transaction
        /// '*' - If all instruments to be included, concataenate specific instruments into single string
        /// if one or more instruments to be returned
        /// </summary>
        public string Instrument { get; set; }

        /// <summary>
        /// Type of transaction to be retuned
        /// '*' - If all types to be included, concataenate specific types into single string
        /// if one or more type to be returned 
        /// </summary>
        public string TransactionType { get; set; }

        /// <summary>
        /// Account Number transactions to be returned
        /// '*' - If all accounts to be included, concatenate specific account numbers into single string
        /// if one or more accounts to be returned         /// 
        /// </summary>
        public string Account { get; set; }

   
        #endregion
    }
}
