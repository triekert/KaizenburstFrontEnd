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
    public class TransactionViewModel : BaseViewModel

    {
        //#region Data

        //public readonly BulkReconViewModel mParent;
        private readonly TransactionDataModel mElement;
        //public ObservableCollection<BulkReconViewModel> mChildren;
        //public bool mIsExpanded;
        //public bool mIsSelected;
        //public bool mIsAllowDrop;
        //#endregion // Data

        #region Public Properties

        /// <summary>
        /// timestamp of Transaction
        /// </summary>

        public DateTime Posted_Date { get; set; }

        /// <summary>
        /// integer conversion for Year_month of transaction
        /// </summary>
        public int Month { get; set; }


        /// <summary>
        ///Description linked to transaction
        /// </summary>

        public string Description { get; set; }


        /// <summary>
        /// Transaction value - source transaction
        /// </summary>

        public decimal TransAmount { get; set; }

        /// <summary>
        /// Transaction value allocated to specific cost item
        /// </summary>


        public decimal ActualAmount { get; set; }

        /// <summary>
        /// Name of cost classification
        /// </summary>

        public string ShortName { get; set; }

        /// <summary>
        /// String representation of GUID for linked category
        /// </summary>


        public string KCategoryID { get; set; }

        /// <summary>
        /// String representation of GUID for transaction allocation


        public string KFinActualID { get; set; }

        /// <summary>
        /// String representation of GUID for originating transaction
        /// </summary>


        public string KFinTranID { get; set; }


        /// <summary>
        /// Calendar date from which Element is seen as active
        /// </summary>
        public DateTime DateEffective { get; set; }

        /// <summary>
        /// Calendar date from which Element is deactivated
        /// </summary>
        public DateTime DateDiscontinued { get; set; }


        /// <summary>
        /// String representation of GUID for linked Party (legal person including supplier, membership, family etc)
        /// </summary>


        public string KPartyID { get; set; }


        /// <summary>
        /// Name of linked party
        /// </summary>
        public string KPartyName { get; set; }

        /// <summary>
        /// String representation of GUID for linked Account
        /// </summary>


        public string KAccountID { get; set; }


        /// <summary>
        /// Name of linked account
        /// </summary>
        public string KAccountName { get; set; }



        /// <summary>
        /// String representation of GUID for linked Plant (Including any portion of plant down to lowest BOM level)
        /// </summary>

        public string KPlantID { get; set; }


        /// <summary>
        /// Name of linked plant
        /// </summary>
        public string KPlantName { get; set; }

        /// <summary>
        /// Attach the current activity to a Change object
        /// </summary>
        public string KChangeID { get; set; }

        /// <summary>
        /// Flag to indicate the transaction classification has changed
        /// </summary>
        public bool IsChanged { get; set; }


        /// <summary>
        /// Flag to indicate at least one document linked 
        /// </summary>
        public bool IsDocLinked { get; set; }



        /// <summary>
        /// Flag to indicate at least one document linked 
        /// </summary>
        public bool IsTemplate { get; set; }

        /// <summary>
        /// String representation of GUID for originating transaction
        /// </summary>

        public string FCatSrchID { get; set; }


        /// <summary>
        /// String representation of GUID for CostHierarchy
        /// </summary>

        public string KHierarchyID { get; set; }



        /// <summary>
        /// Ad hoc notes linked to processed transaction
        /// </summary>

        public string Notes { get; set; }

        /// <summary>
        /// A document, including images etc, linked to the transaction
        /// </summary>
        public DocDataViewModel Document{ get; set; }

        ///// <summary>
        ///// String representation of GUID for linked document
        ///// </summary>
        //public string KDocID { get; set; }

        ///// <summary>
        /////  Name of Doc linked to Transaction
        ///// </summary>
        //public string DocName { get; set; }


        ///// <summary>
        /////  Image of  Doc linked to Transaction
        ///// </summary>
        //public byte[] DocImage { get; set; }


        ///// <summary>
        /////  URL of  Doc linked to Transaction
        ///// </summary>
        //public string DocURL { get; set; }




        /// <summary>
        /// Title of Control
        /// </summary>
        public string Title { get; set; } = "Financial Transactions";


        /// <summary>
        /// TO DO: Determine the color of the text to be displayed depending
        ///on the specific hierarchy type being displayed. Default will be UI 
        ///default color.
        /// </summary>
        public string TextColor => "FF8B0000";


  



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

        public TransactionViewModel()

        {

        }
        //public BulkReconViewModel(BulkReconDataModel element)

        //         : this(element, null)
        //{
        //}

        //private BulkReconViewModel(BulkReconDataModel element, BulkReconViewModel parent)
        //{
        //    mElement = element;
        //    mParent = parent;
        //    var exception = default(Exception);
        //    try
        //    { 
        
        //    mChildren = new ObservableCollection<BulkReconViewModel>(
        //            (from child in mElement.Children orderby(mElement.ShortName)
        //             select new BulkReconViewModel(child, this))
        //             .ToList());
        //    }
        //    catch (Exception ex)
        //    {
        //        exception = ex;
        //    }

        //}



        #endregion // Constructors


        #region Presentation Members

        #region IsExpanded

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is expanded.
        /// </summary>
        //public bool IsExpanded
        //{
        //    get => mIsExpanded;
        //    set
        //    {
        //        if (value != mIsExpanded)
        //        {
        //            mIsExpanded = value;
        //            //OnPropertyChanged("IsExpanded");
        //        }

        //        // Expand all the way up to the root.
        //        if (mIsExpanded && mParent != null)
        //            mParent.IsExpanded = true;
        //        var mDescription = mElement.Description;
        //    }
        //}

        #endregion // IsExpanded

        //#region IsSelected

        ///// <summary>
        ///// Gets/sets whether the TreeViewItem 
        ///// associated with this object is selected in the UI.
        ///// </summary>
        //public bool IsSelected
        //{
        //    get => mIsSelected;
        //    set
        //    {
        //        if (value != mIsSelected)
        //        {
        //            mIsSelected = value;
        //            //var kCategoryID = KCategoryID;

        //            var name1 = ShortName;
        //            //OnPropertyChanged("IsSelected");
        //        }
        //    }
        //}

        //#endregion // IsSelected
        #region IsAllowDrop

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is selected.
        /// </summary>
        //public bool IsAllowDrop
        //{
        //    get => mIsAllowDrop;
        //    set =>
        //        //if (value != _isAllowDrop)
        //        //{
        //        //    _isAllowDrop = value;
        //        //    //int ndx =base.GetEnumerator();
        //        //    string name1 = this.ShortName;
        //        //    this.OnPropertyChanged("IsAllowDrop");
        //        //}
        //        mIsAllowDrop = true;
        //}

        #endregion // IsAllowDreop
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

        //#region NameContainsText

        ///// <summary>
        ///// Check that the ShortName field contains data to enable the search
        ///// </summary>
        ///// <param name="text"></param>
        ///// <returns></returns>
        //public bool NameContainsText(string text)
        //{
        //    if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(ShortName))
        //        return false;

        //    return ShortName.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) > -1;
        //}

        //#endregion // NameContainsText

        //#region KCategoryIdContainsText

        ///// <summary>
        ///// Check that the KCategoryId field contains data to enable the search
        ///// </summary>
        ///// <param name="text"></param>
        ///// <returns></returns>
        //public bool KCategoryIdContainsText(string text)
        //{
        //    if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(KCategoryID))
        //        return false;

        //    return KCategoryID.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) > -1;
        //}

        //#endregion // NameContainsText



        /// <summary>
        /// Check that the KCategoryId field contains data to enable the search
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>


        //#region Parent

        ////public HierarchyViewModel Parent => mParent;

        //#endregion // Parent

        #endregion // Presentation Members        
        #region Helper Method

        /// <summary>
        /// Removes all children from the list, adding a dummy item to show the expand icon if required
        /// </summary>
        //public void ClearChildren()
        //{
        //    // Clear Items
        //    Children = new ObservableCollection<HierarchyViewModel>();

        //    //check for children
        //    var children = from element in mHDML
        //                   where element.ParentCategoryID == KCategoryID
        //                   select (element.ShortName, element.Description, element.KCategoryID, element.ParentCategoryID);
        //    // Show the expand arrow if we are not a file
        //    if (children.Count() > 0)
        //       Children.Add(null);
        //}

        #endregion

        /// <summary>
        /// Epands this directory and finds all the children
        /// </summary>
        //private void Expand()
        //{



        //    //// Find all children
        //    //var children = from element in mHDML
        //    //               where element.ParentCategoryID == KCategoryID
        //    //               select (element.ShortName, element.Description, element.KCategoryID, element.ParentCategoryID);
        //    //// Hierarchy cannot be expanded
        //    //if (children.Count() == 0)
        //    //    return;
        //    //Children = new ObservableCollection<HierarchyViewModel>(
        //    //    children.Select(child => new HierarchyViewModel(child.ShortName, child.Description, child.KCategoryID, mHDML)));
        //}



        //    #region INotifyPropertyChanged Members

        //    public event PropertyChangedEventHandler PropertyChanged;

        //    protected new virtual void OnPropertyChanged(string propertyName)
        //    {
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //    }

        //    #endregion // INotifyPropertyChanged Members
        //
    }
}
