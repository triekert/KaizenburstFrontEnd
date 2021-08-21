using System.Collections.Generic;

namespace Fasetto.Word.Core
{
    /// <summary>
    /// Class representing each individual element of the hierarchy 
    /// </summary>
    public class HierarchyManagementDataModel
    {
        /// <summary>
        /// string representation of UniqueIdentifier for a Category element
        /// </summary>
        public string FinHierarchyID { get; set; }

        /// <summary>
        ///name of Category element
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        ///description of Category element
        /// </summary>
        /// 
        public string Description { get; set; }

        /// <summary>
        //string representation of card where the expense category is determined by the linked card
        /// </summary>
        /// 
        public string Card { get; set; }

        /// <summary>
        //integer indicating the number of months between expected occurrences of expense category
        /// </summary>
        public int Frequency { get; set; }

        /// <summary>
        //string representation of GUID for a Category element
        /// </summary>
        public string KCategoryID { get; set; }

        /// <summary>
        //sstring representation of GUID for the Parent category of a Category element
        //the parent of all root elements will be NULL... any hierarchy will have at least one root element
        /// </summary>
        public string ParentCategoryId { get; set; }

        /// <summary>
        //the link to tthe ICON used to depict this category
        /// </summary>
        public string FIconId { get; set; }

        /// <summary>
        //sub categories that are also categories in themself
        /// </summary>
        //public List<HierarchyManagementDataModel> Children { get; set; }
    }
}
