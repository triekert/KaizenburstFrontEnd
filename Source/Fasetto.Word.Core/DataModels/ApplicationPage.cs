namespace Fasetto.Word.Core
{
    /// <summary>
    /// A page of the application
    /// </summary>
    public enum ApplicationPage
    {
        /// <summary>
        /// The initial login page
        /// </summary>
        Login = 0,

        /// <summary>
        /// The main chat page
        /// </summary>
        Chat = 1,

        /// <summary>
        /// The register page
        /// </summary>
        Register = 2,

        /// <summary>
        /// The Finance Menu page
        /// </summary>
        Finance = 3,

        /// <summary>
        /// The Hierarchy page
        /// </summary>
        Hierarchy = 4,

        /// <summary>
        /// Re-allocation/adjustment of transactions
        /// </summary>
        Actuals = 5,


        /// <summary>
        /// A Dummy (Folder) page - no Menu
        /// </summary>
        Folder = 6,

        /// <summary>
        ///Load meter readings for services
        /// </summary>
       LoadMeters = 7,

        /// <summary>
        ///Bulk meter recons
        /// </summary>
        BulkRecon = 8,

        /// <summary>
        ///Selection of Bulk Meter
        /// </summary>
        MeterSelection = 9,

        /// <summary>
        ///Inspection of Water and Sewerage Billing
        /// </summary>
        SWBilling = 10,


        /// <summary>
        ///Management of captured transactions
        /// </summary>
        Transactions = 11,
    }

}
