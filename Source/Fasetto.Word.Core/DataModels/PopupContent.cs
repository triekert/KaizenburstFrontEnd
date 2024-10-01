namespace Fasetto.Word.Core
{
    /// <summary>
    /// The type of content in the side menu
    /// </summary>
    public enum PopupContent
    {
        /// <summary>
        /// Management of Hierarchy Element information
        /// </summary>
        AddElement = 1,
        /// <summary>
        /// A list of contacts
        /// </summary>
        Contacts = 2,

        /// <summary>
        /// A list of media from all chat messages
        /// </summary>
        Media = 3,

        /// <summary>
        /// A list of finance menu options for KaizenBurst
        /// </summary>
        Finance = 4,

        /// <summary>
        /// Form for editing User settings
        /// </summary>
        Settings = 5,


        /// <summary>
        /// Detail meter readings for water recon
        /// </summary>
        BulkReconDetail = 6,


        /// <summary>
        /// Detail meter readings for water recon
        /// </summary>
        MissingMeters = 7,


        /// <summary>
        /// Popup for selection of BulkMeter and time range
        /// </summary>
        BulkMeterSelectionControl = 8,


        /// <summary>
        /// Detail meter readings for water recon
        /// </summary>
        BulkRecon = 9,


        /// <summary>
        /// Hierarchy Selection Control
        /// </summary>
        HierarchyItemSelection = 10,



        /// <summary>
        ///Sewerage and Water Billing calculations
        /// </summary>
        SWBilling = 11,



        /// <summary>
        /// Adjustment to SW calculations
        /// </summary>
        SWAdjust = 12,



        /// <summary>
        /// Return transactions for selected period
        /// </summary>
        Transaction = 13,



        /// <summary>
        /// Drill down into specific transaction
        /// </summary>
        TransactionDetail = 14,




        /// <summary>
        /// Manage classification of a transaction
        /// </summary>
        Classify = 15,




        /// <summary>
        /// Manage classification of a transaction
        /// </summary>
        SWBillingDetail = 16,



        /// <summary>
        /// Manage budgets
        /// </summary>
        BudgetReview = 17,



        /// <summary>
        /// Manage budgets
        /// </summary>
        BudgetDetailList = 18,



        /// <summary>
        /// Manage budgets
        /// </summary>
        BudgetAdjust = 19
    }


}
