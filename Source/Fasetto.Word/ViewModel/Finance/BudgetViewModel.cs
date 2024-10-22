using Fasetto.Word.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Fasetto.Word
{
    /// <summary>
    /// The Hierarchy element as a view model
    /// </summary>
    public class BudgetViewModel : BaseViewModel

    {
        //#region Data

        public readonly BudgetViewModel mParent;
        private readonly BudgetDataModel mElement;
        public ObservableCollection<BudgetViewModel> mChildren;

        public bool mIsExpanded;
        public bool mIsSelected;
        public bool mIsAllowDrop;
        //#endregion // Data

        #region Public Properties
        /// <summary>
        /// Name of Category element
        /// </summary>
        public string ShortName  => mElement.ShortName;




        //string representation of GUID for a Category element
        public string KCategoryID => mElement.KCategoryID;
        //sstring representation of GUID for the Parent category of a Category element
        //the parent of all root elements will be NULL... any hierarchy will have at least one root element

        public string ParentCategoryID => mElement.ParentCategoryID;

        /// <summary>
        /// Name of Category element
        /// </summary>
        public string ParentShortName => mElement.ParentShortName;

        /// <summary>
        /// Aggregate Total for element and all descendants
        /// </summary>
        public decimal BudgetAmountTotal => mElement.BudgetAmountTotal;

        /// <summary>
        ///Aggregate Total for all descendant elements
        /// </summary>
        public decimal BudgetAmountDescendants => mElement.BudgetAmountDescendants;

        /// <summary>
        ///Amount budgeted directly for the selected element (excluding descendant aggregates)
        /// </summary>
        public decimal BudgetAmount   => mElement.BudgetAmount;


        /// <summary>
        /// Aggregate Total expenditure for element and all descendants
        /// </summary>
        public decimal ActualAmountTotal => mElement.ActualAmountTotal;

        /// <summary>
        ///Aggregate Total Expenditure for all descendant elements
        /// </summary>
        public decimal ActualAmountDescendants => mElement.ActualAmountDescendants;

        /// <summary>
        ///Actual expenditure for the selected element (excluding descendant aggregates)
        /// </summary>
        public decimal ActualAmount => mElement.ActualAmount;


        /// <summary>
        ///Current deviation from budget for the selected element (excluding descendant aggregates)
        /// </summary>
        public decimal Deviation => mElement.Deviation;


        /// <summary>
        ///Cumulative deviation from budget for the  for the selected element (excluding descendant aggregates)
        /// </summary>
        public decimal DeviationCum => mElement.DeviationCum;


        /// <summary>
        ///Cumulative budget amount so far
        /// </summary>
        public decimal BudgetTotCum => mElement.BudgetTotCum;


        /// <summary>
        ///Cumulative actual amount so far
        /// </summary>
        public decimal ActualTotCum => mElement.ActualTotCum;
        /// <summary>
        /// Reading at start
        /// </summary>
        public int Month => mElement.Month;



        //public List<BudgetViewModel> Children { get; set; }

        /// <summary>
        /// A list of all children contained inside this item
        /// </summary>
        public ObservableCollection<BudgetViewModel> Children => mChildren;

        /// <summary>
        /// Title of Control
        /// </summary>
        public string Title { get; set; } = "Budget Detail";


        /// <summary>
        /// TO DO: Determine the color of the text to be displayed depending
        ///on the specific hierarchy type being displayed. Default will be UI 
        ///default color.
        /// </summary>
        public string TextColor => "FF8B0000";


        /// <summary>
        /// Indicates if this item can be expanded
        /// </summary>

        public bool CanExpand => Children?.Count(f => f != null) > 0;



        #endregion
        #region Data

        public HierarchyListDataModel mHDML;
        #endregion
        #region Public Commands

        /// <summary>
        /// The command to expand this item
        /// </summary>
        public ICommand ExpandCommand { get; set; }

        #endregion
        #region Constructors

        public BudgetViewModel()

        {

        }
        public BudgetViewModel(BudgetDataModel element)

                 : this(element, null)
        {
        }

        private BudgetViewModel(BudgetDataModel element, BudgetViewModel parent)
        {
            mElement = element;
            mParent = parent;
            var exception = default(Exception);
            try
            { 
        
            mChildren = new ObservableCollection<BudgetViewModel>(
                    (from child in mElement.Children orderby(mElement.ShortName)
                     select new BudgetViewModel(child, this))
                     .ToList());
            }
            catch (Exception ex)
            {
                exception = ex;
            }

        }



        #endregion // Constructors


        #region Presentation Members

        #region IsExpanded

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get => mIsExpanded;
            set
            {
                if (value != mIsExpanded)
                {
                    mIsExpanded = value;
                    //OnPropertyChanged("IsExpanded");
                }

                // Expand all the way up to the root.
                if (mIsExpanded && mParent != null)
                    mParent.IsExpanded = true;
                //var mDescription = mElement.Description;
            }
        }

        #endregion // IsExpanded

        #region IsSelected

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is selected in the UI.
        /// </summary>
        public bool IsSelected
        {
            get => mIsSelected;
            set
            {
                if (value != mIsSelected)
                {
                    mIsSelected = value;
                    //var kCategoryID = KCategoryID;

                    var name1 = ShortName;
                    //OnPropertyChanged("IsSelected");
                }
            }
        }

        #endregion // IsSelected
        #region IsAllowDrop

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is selected.
        /// </summary>
        public bool IsAllowDrop
        {
            get => mIsAllowDrop;
            set =>
                //if (value != _isAllowDrop)
                //{
                //    _isAllowDrop = value;
                //    //int ndx =base.GetEnumerator();
                //    string name1 = this.ShortName;
                //    this.OnPropertyChanged("IsAllowDrop");
                //}
                mIsAllowDrop = true;
        }

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

        #region NameContainsText

        /// <summary>
        /// Check that the ShortName field contains data to enable the search
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool NameContainsText(string text)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(ShortName))
                return false;

            return ShortName.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) > -1;
        }

        #endregion // NameContainsText

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
        private void Expand()
        {



            //// Find all children
            //var children = from element in mHDML
            //               where element.ParentCategoryID == KCategoryID
            //               select (element.ShortName, element.Description, element.KCategoryID, element.ParentCategoryID);
            //// Hierarchy cannot be expanded
            //if (children.Count() == 0)
            //    return;
            //Children = new ObservableCollection<HierarchyViewModel>(
            //    children.Select(child => new HierarchyViewModel(child.ShortName, child.Description, child.KCategoryID, mHDML)));
        }



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
