
using System;

namespace Fasetto.Word.Core
{
    /// <summary>
    /// Return of selected  hierarchy item from hierarchy table on database 
    /// </summary>
    public class HierarchyBillingResultApiModel
    {
        #region Public Properties

        ////string represetntation of UniqueIdentifier for a Category of hierarchy elements
        //public string FHierarchyID { get; set; }

        //name of Category element
        public string ShortName { get; set; }
        //description of Category element
        public string Description { get; set; }
        ////string representation of card where the expense category is determined by the linked card
        //public string Card { get; set; }
        ////integer indicating the number of months between expected occurrences of expense category
        //public int Frequency { get; set; }
        //string representation of GUID for a Category element
        public string KCategoryID { get; set; }
        //sstring representation of GUID for the Parent category of a Category element
        //the parent of all root elements will be NULL... any hierarchy will have at least one root element

        public string ParentCategoryID { get; set; }

        /// <summary>
        /// Calendar date from which Element is seen as active
        /// </summary>
        public DateTime DateEffective { get; set; }

        /// <summary>
        /// Calendar date from which Element is deactivated
        /// </summary>
        public DateTime DateDiscontinued { get; set; }

        /// <summary>
        /// Total Water Consumption for period
        /// </summary>

        public decimal TotalConsumption { get; set; }

        /// <summary>
        /// Water Bill
        /// </summary>
        public decimal WaterCost { get; set; }

        /// <summary>
        /// Sewerage Bill
        /// </summary>
        public decimal SewerCost { get; set; }

        /// <summary>
        /// Toal Cost
        /// </summary>
        public decimal TotalCost { get; set; }

        /// <summary>
        /// Period start
        /// </summary>
        public DateTime TimeStart { get; set; }

        /// <summary>
        /// Reading at start
        /// </summary>
        public decimal Startreading { get; set; }


        /// <summary>
        /// Period End
        /// </summary>
        public DateTime TimeEnd { get; set; }


        /// <summary>
        /// Reading at end of Period
        /// </summary>
        public decimal Endreading { get; set; }




        #endregion
    }
}
