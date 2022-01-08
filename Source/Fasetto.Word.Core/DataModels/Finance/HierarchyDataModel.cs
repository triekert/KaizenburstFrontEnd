using System;
using System.Collections.Generic;
using System.Linq;

namespace Fasetto.Word.Core
{
    /// <summary>
    /// Class representing each individual element of the hierarchy 
    /// </summary>
    public class HierarchyDataModel
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
        //string representation of GUID for the Parent category of a Category element
        //the parent of all root elements will be NULL... any hierarchy will have at least one root element
        /// </summary>
        public string ParentCategoryID { get; set; }

        /// <summary>
        /// Parent ShortName of hiearchy item
        /// </summary>
        public string ParentShortName { get; set; }

        /// <summary>
        //the link to tthe ICON used to depict this category
        /// </summary>
        public string FIconID { get; set; }

        /// <summary>
        /// Calendar date from which Element is seen as active
        /// </summary>
        public DateTime DateEffective { get; set; }

        /// <summary>
        /// Calendar date from which Element is deactivated
        /// </summary>
        public DateTime DateDiscontinued { get; set; }

        /// <summary>
        /// Attach the current activity to a Change object
        /// </summary>
        public string KChangeID { get; set; }

        /// <summary>
        //sub categories that are also categories in themself
        /// </summary>
        public List<HierarchyDataModel> Children { get; set; }

        #region KCategoryIdContainsText

        /// <summary>
        /// Check that the KCategoryId field contains data to enable the search
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool KCategoryIdContainsText(string text)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(KCategoryID))
                return false;

            return KCategoryID.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) > -1;
        }

        #endregion // NameContainsText

        //#region Constructor
        //public HierarchyDataModel(HierarchyResultListApiModel result)
        //{

        //    //var rootElement = from element in result
        //    //                  where element.ParentCategoryID == ""
        //    //                  select (element.ShortName, element.Description, element.KCategoryID, element.ParentCategoryID);
        //    //mRootCategory = new HierarchyViewModel(rootElement.First().ShortName, rootElement.First().Description, rootElement.First().KCategoryID, mHDML);

        //    //// Create the view models from the data
        //    //FirstGeneration = new ObservableCollection<HierarchyDataModel>(
        //    //rootElement.Select(root => new HierarchyDataModel(root.ShortName, root.Description, root.KCategoryID, result)));

        //}
        //public HierarchyDataModel()
        //{ }

        //#endregion //Constructor
    }
}
