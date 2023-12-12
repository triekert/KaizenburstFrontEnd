using System.Threading.Tasks;
using System;
using System.Windows;

namespace Fasetto.Word
{
    /// <summary>
    /// The Hierarchy element as a view model
    /// </summary>
    public class CostHierarchyViewModel : BaseViewModel

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
        /// String representation of GUID for Cost Category Structure
        /// </summary>
        public string KCategoryID { get; set; }


        /// <summary>
        /// String representation of GUID for Billing Period
        /// </summary>
        public string ShortName{ get; set; }


        /// <summary>
        /// String representation of GUID for Client owning Billing Period
        /// </summary>
        public string FClientID { get; set; }


 
        /// <summary>
        /// Title of Control
        /// </summary>
        public string Title { get; set; } = "Cost Hierarchy Selection";

        /// <summary>
        /// The action to run when initiating the control.
        /// Returns true if the prepaation was successful, or false otherwise.
        /// </summary>
        public Func<Task<bool>> PrepareAction { get; set; }

        /// <summary>
        /// Indicates if the current text is in edit mode
        /// </summary>
        public bool Editing { get; set; }



        /// <summary>
        /// TO DO: Determine the color of the text to be displayed depending
        ///on the specific hierarchy type being displayed. Default will be UI 
        ///default color.
        /// </summary>
        public string TextColor => "FF8B0000";


        /// <summary>
        /// Indicates if this item can be expanded
        /// </summary>

        //public bool CanExpand => Children?.Count(f => f != null) > 0;



        #endregion
        #region Data

        //public HierarchyListDataModel mHDML;
        #endregion
        #region Public Commands

        /// <summary>
        /// The command to expand this item
        /// </summary>
        //public ICommand ExpandCommand { get; set; }

        #endregion
        #region Constructors

        public CostHierarchyViewModel ()

        {

        }



        #endregion // Constructors







 

        #region TreeView_MouseDown

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is selected.
        /// </summary>
        public void TreeView_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)

        {
            _ = MessageBox.Show("You clicked me at tree view item level ");
        }



 

       

        #endregion // Presentation Members        
 


    }
}
