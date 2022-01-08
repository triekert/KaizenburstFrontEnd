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
    public class HierarchyViewModel : BaseViewModel

    {
        //#region Data

        public readonly HierarchyViewModel mParent;
        private readonly HierarchyDataModel mElement;
        public ObservableCollection<HierarchyViewModel> mChildren;
        public bool mIsExpanded;
        public bool mIsSelected;
        public bool mIsAllowDrop;
        //#endregion // Data

        #region Public Properties



        /// <summary>
        /// The name of this hierarchy item
        /// </summary>
        public string ShortName => mElement.ShortName;

        /// <summary>
        /// Description of hiearchy item
        /// </summary>
        public string Description =>mElement.Description;
        /// <summary>
        /// The Identifier of this hierarchy item
        /// </summary>
        public string KCategoryID => mElement.KCategoryID;

        /// <summary>
        /// Parent ID  of hiearchy item
        /// </summary>
        public string ParentCategoryID =>mElement.ParentCategoryID;

        /// <summary>
        /// Parent ShortName of hiearchy item
        /// </summary>
        public string ParentShortName => mElement.ParentShortName;

        /// <summary>
        /// Calendar date from which Element is seen as active
        /// </summary>
        public DateTime DateEffective => mElement.DateEffective;

        /// <summary>
        /// Calendar date from which Element is deactivated
        /// </summary>
        public DateTime DateDiscontinued => mElement.DateDiscontinued;

        /// <summary>
        /// Attach the current activity to a Change object
        /// </summary>
        public string KChangeID   => mElement.KChangeID;

        /// <summary>
        /// A list of all children containd inside this item
        /// </summary>
        public ObservableCollection<HierarchyViewModel> Children => mChildren;

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

        public HierarchyViewModel()

        {

        }
        public HierarchyViewModel(HierarchyDataModel element)

                 : this(element, null)
        {
        }

        private HierarchyViewModel(HierarchyDataModel element, HierarchyViewModel parent)
        {
            mElement = element;
            mParent = parent;
            var exception = default(Exception);
            try
            { 
        
            mChildren = new ObservableCollection<HierarchyViewModel>(
                    (from child in mElement.Children
                     select new HierarchyViewModel(child, this))
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
                var mDescription = mElement.Description;
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

                    var name1 = this.ShortName;
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
