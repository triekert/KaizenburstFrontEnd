using System.Threading.Tasks;
using System;
using System.Windows;

namespace Fasetto.Word
{
    /// <summary>
    /// The Hierarchy element as a view model
    /// </summary>
    public class BudgetPeriodDataModel 
    {
        //#region Data

        //public readonly BulkReconViewModel mParent;
        //private readonly BulkReconDataModel mElement;
        //public ObservableCollection<BulkReconViewModel> mChildren;
        //public bool mIsExpanded;
        //public bool mIsSelected;
        //public bool mIsAllowDrop;
        //#endregion // Data

        #region Public Properties

        /// <summary>
        /// String representation of GUID for Budget
        /// </summary>
        public string KBudgetID { get; set; }


        /// <summary>
        /// String representation of Name of Budget Period
        /// </summary>
        public string Name{ get; set; }


        /// <summary>
        /// Integer representing the start month for the budget
        /// </summary>
        public int MonthStart { get; set; }



        /// <summary>
        /// Integer representing the Last month for the budget
        /// </summary>
        public int MonthEnd { get; set; }








        #endregion







 


    }
}
