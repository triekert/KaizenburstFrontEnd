using Fasetto.Word.Core;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Fasetto.Word
{
    /// <summary>
    /// The Hierarchy element as a view model
    /// </summary>
    public class BillingPeriodViewModel : BaseViewModel

    {

        #region Public Properties

        /// <summary>
        /// String representation of GUID for Billing Period
        /// </summary>
        public string KBillingPeriodID { get; set; }

        /// <summary>
        /// Start time of Billing Period
        /// </summary>
        public DateTime TimeStart { get; set; }

        /// <summary>
        /// String representation of GUID for Client owning Billing Period
        /// </summary>
        public string FClientID { get; set; }


        /// <summary>
        /// End time of Billing Period
        /// </summary>
        public DateTime TimeEnd { get; set; }

        /// <summary>
        /// Title of Control
        /// </summary>
        public string Title { get; set; } = "Billing Period Selection";


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

        public BillingPeriodViewModel ()

        {

        }


        #endregion // Constructors


        #region Presentation Members

  
        #region TreeView_MouseDown

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is selected.
        /// </summary>
        public void TreeView_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)

        {
            _ = MessageBox.Show("You clicked me at tree view item level ");
        }



        #endregion // IsSelected

 

        #endregion // Presentation Members        


    }
}
