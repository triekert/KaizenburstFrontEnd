using Fasetto.Word.Core;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Fasetto.Word
{
    /// <summary>
    /// The settings state as a view model
    /// </summary>
    public class HierarchyManagementViewModel : BaseViewModel

    {
        #region Data

        private readonly HierarchyManagementViewModel mParent;
        private readonly HierarchyManagementTreeDataModel mCategory;

        //private readonly ReadOnlyCollection<HierarchyManagementViewModel> mChildren;
        private bool mIsExpanded;
        private bool mIsSelected;
        private bool mIsAllowDrop;
        #endregion // Data

        #region Constructors

        public HierarchyManagementViewModel(HierarchyManagementTreeDataModel mCategory)
            : this(mCategory, null)
        {
        }

        private HierarchyManagementViewModel(HierarchyManagementTreeDataModel category, HierarchyManagementViewModel parent)
        {
            mCategory = category;
            mParent = parent;

            Children = new ReadOnlyCollection<HierarchyManagementViewModel>(
                   //Order mCategory children alphabetically
                   (from child in mCategory.Children.OrderBy(x => x.ShortName)
                    select new HierarchyManagementViewModel(child, this))
                     .ToList());

        }

        #endregion // Constructors

        #region HierarchyManagementTreeDataModel Properties

        public ReadOnlyCollection<HierarchyManagementViewModel> Children { get; }


        public string ShortName => mCategory.ShortName;
        public string KCategoryID => mCategory.KCategoryID;

        #endregion // HierarchyManagementTreeDataModel Properties

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
                    OnPropertyChanged("IsExpanded");
                }

                // Expand all the way up to the root.
                if (mIsExpanded && mParent != null)
                    mParent.IsExpanded = true;
            }
        }

        #endregion // IsExpanded

        #region IsSelected

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is selected.
        /// </summary>
        public bool IsSelected
        {
            get => mIsSelected;
            set
            {
                if (value != mIsSelected)
                {
                    mIsSelected = value;
                    //int ndx =base.GetEnumerator();
                    var shortName = ShortName;
                    var name1 = shortName;
                    OnPropertyChanged("IsSelected");
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

        #endregion // IsSelected
        #region TreeView_MouseDown

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is selected.
        /// </summary>
        public void TreeView_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)

        {
            MessageBox.Show("You clicked me at tree view item level ");
        }



        #endregion // IsSelected

        #region NameContainsText

        public bool NameContainsText(string text)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(ShortName))
                return false;

            return ShortName.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) > -1;
        }

        #endregion // NameContainsText

        #region Parent

        public HierarchyManagementViewModel Parent => mParent;

        #endregion // Parent

        #endregion // Presentation Members        

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected new virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion // INotifyPropertyChanged Members
    }
}
