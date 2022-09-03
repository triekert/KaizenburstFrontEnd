using System;

namespace Fasetto.Word.Core
{
    /// <summary>
    /// Details used to search for a user
    /// </summary>
    public class ManageInvestmentTransactionResultApiModel
    {
        #region Public Properties

        /// <summary>
        /// Date of transaction
        /// </summary>
        public DateTimeOffset DateOfTransaction { get; set; }

        /// <summary>
        /// Instrument involved in transaction
        /// </summary>
        public string Instrument { get; set; }

        /// <summary>
        /// Type of transaction
        /// </summary>
        public string TransactionType { get; set; }

        /// <summary>
        /// Transaction Description
        /// </summary>
        public string Description { get; set; }


        /// <summary>
        /// Account Number
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// Quantity of instrument involved in transaction
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Total value of transaction
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// Price per unit of Instrument in transaction
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Counter Instrument involved in transaction, where transaction involves an exchange between 2 instruments
        /// </summary>
        public string InstrumentCtr { get; set; }

        /// <summary>
        /// Total cost of Transation
        /// </summary>
        public decimal Cost { get; set; }

        #endregion       
    }
}
