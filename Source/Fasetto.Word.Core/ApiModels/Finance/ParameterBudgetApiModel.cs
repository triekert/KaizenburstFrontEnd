namespace Fasetto.Word.Core
{
    /// <summary>
    /// Class representing each individual element of the hierarchy 
    /// </summary>
    /// 
    public class ParameterBudgetApiModel
    {
        // <summary>
        /// GUID of Budget selected for transaction management
        /// </summary>
        public string BudgetID { get; set; }


        /// <summary>
        ///Selected Month for review of budget
        /// </summary>
        public int BMonth { get; set; }


        /// <summary>
        /// Name of Selected Budget
        /// </summary>
        public string BudgetName { get; set; }




        /// <summary>
        ///Flag to return Expenditure if true
        /// </summary>
        public bool IsExpenditureReturn { get; set; }

    }

}


