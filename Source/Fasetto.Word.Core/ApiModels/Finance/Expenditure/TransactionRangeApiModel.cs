namespace Fasetto.Word.Core
{
    /// <summary>
    /// The credentials for  an API client to register on the server 
    /// </summary>
    public class TransactionRangeApiModel
    {
        #region Public Properties

        /// <summary>
        /// The budget identifier
        /// </summary>
        public string FBudgetID { get; set; }

        /// <summary>
        /// Version of budget
        /// </summary>
        public int FVersion { get; set; }

        /// <summary>
        /// Start Month of extract
        /// </summary>
        public int MonthBegin { get; set; }


        /// <summary>
        /// End Month of extract
        /// </summary>
        public int MonthEnd { get; set; }



        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public TransactionRangeApiModel()
        {
            
        }

        #endregion
    }
}
