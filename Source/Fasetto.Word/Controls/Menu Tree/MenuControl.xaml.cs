using Fasetto.Word.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static Fasetto.Word.DI;

namespace Fasetto.Word
{
    /// <summary>
    /// Interaction logic for HierarchyManagementControl.xaml
    /// </summary>
    public partial class MenuControl : UserControl
    {

        #region Public Properties

        //public string ControlTitle { get; set; } = "Title of Control";

        #endregion//Public Properties

        #region Public Commands
        /// <summary>
        /// The command to close the settings menu
        /// </summary>
        //public ICommand CloseCommand { get; set; }
        #endregion//Public Commands


        private readonly HierarchyTreeViewModel mHierarchyTree;
        private string mSourceCategory;
        private string mSourceCategoryName;
        private string mDestinationCategoryID, mDestinationID,mSourceID,mParentID;
        private string mDestinationCategoryName;
        private bool mIsSourceObtained = false, mIsEqual = false;
        private Point mLastMouseDown;
        private TreeViewItem mTargetT, mSource;
        private HierarchyViewModel mDraggedItemTest,mDraggedItem,mTarget;
        //private readonly object mFamilyTree;
        private readonly HierarchyViewModel mTargetTest;
        //public string mControlTitle = "testing";

        //[Obsolete]
        //public HierarchyManagementControl(HierarchyManagementTreeDataModel hierarchyManagementTreeDataModel)
        /// <summary>
        /// This initiation of the Menu Control tree
        /// </summary>
        public MenuControl()
        {

            //Set the root of the hierarchy to return the Menu structure
            var root = "2D7E4A7D-6F19-496E-8709-47E6A9ADDFA0";
            mHierarchyTree = new HierarchyTreeViewModel(root);//root);

            DataContext = mHierarchyTree;
            InitializeComponent();
            ViewModelApplication.CurrentSideMenuViewModel = mHierarchyTree;
            //CloseCommand = new RelayCommand(Close);

        }

        /// <summary>
        /// the Overloading of MenuControl() with a parameter that selects the Menu Hierarchy for navigation by passing the parameter
        /// </summary>
        /// <param name="root"></param>
        public MenuControl(string root)
        {
            mHierarchyTree = new HierarchyTreeViewModel(root);//root);

            DataContext = mHierarchyTree;
            InitializeComponent();
            ViewModelApplication.CurrentSideMenuViewModel = mHierarchyTree;
        }

        //private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Enter)
        //     mHierarchyTree.SearchCommand.Execute(null) ;
        //           }

        private static List<HierarchyTreeDataModel> FillRecursive(List<HierarchyDataModel> flatObjects, string parentId)
        {
            return flatObjects.Where(x => x.ParentCategoryID.Equals(parentId)).Select(item => new HierarchyTreeDataModel
            {
                ShortName = item.ShortName,
                Description = item.Description,

                KCategoryID = item.KCategoryID,
                Children = FillRecursive(flatObjects,
                                         item.KCategoryID)
            }).ToList();

        }

        private TreeViewItem GetNearestContainer(UIElement element)
        {
            //Walk up the element tree to the nearest tree view item.
           var container = element as TreeViewItem;
            while ((container == null) && (element != null))
            {
                element = VisualTreeHelper.GetParent(element) as UIElement;
                container = element as TreeViewItem;
            }
            return container;
        }
        /// <summary>
        /// This method will programmatically move the scrollbar to ensure that 
        /// a selected item is always in view in the scroll area
        /// It requires the use of the scrollViewer control prior to defining the 
        /// TreeView structure in XAML
        /// </summary>
        /// <param name="sender">the selected treeview item</param>
        /// <param name="e"></param>
        private void TreeView_Selected(object sender, RoutedEventArgs e)
        {
            var element = e.OriginalSource as FrameworkElement;
            //var tmp1 = element.GetType().Name;
            //var element = sender as FrameworkElement;
            // Figure out a relative position of the selected node to the scrollviewer
            //var relativePosition = element.TranslatePoint(new Point(0, 0), scrollViewer);
            //scrollViewer.ScrollToVerticalOffset(relativePosition.Y);
            element.BringIntoView();
            element.Focus();

        }

        //public void Close()
        //{
        //    // Close settings menu
        //    ViewModelApplication.PopupVisible = false;

        //}
        #region Search Logic //KCategoryID
        public IEnumerator<HierarchyViewModel> MatchingKCategoryEnumerator { get; private set; }

        #endregion // SearchKCategoryID



        #region Search Logic //KCategoryID
        public bool PerformKIdSearch()
        {

            if (MatchingKCategoryEnumerator == null || !MatchingKCategoryEnumerator.MoveNext())
                VerifyMatchingKCategoryEnumerator();
            var KCategory = MatchingKCategoryEnumerator.Current;

            if (KCategory == null)
                return true;

            return false;
        }

        private void VerifyMatchingKCategoryEnumerator()
        {
            //var matchK = FindKMatches(mParentID, mTarget);
            var matchK = FindKMatches(mDestinationID, mDraggedItem);
            MatchingKCategoryEnumerator = matchK.GetEnumerator();
            _ = !MatchingKCategoryEnumerator.MoveNext();

        }

        public IEnumerable<HierarchyViewModel> FindKMatches(string searchText, HierarchyViewModel Category)
        {
            //var mSearchText = searchText;
            if (Category.KCategoryIdContainsText(searchText))
                yield return Category;

            foreach (var child in Category.Children)
                foreach (var matchK in FindKMatches(searchText, child))
                    yield return matchK;
        }

        #endregion // Search Logic
        #region Element manipulation
        /// <summary>
        /// Use Popup View to add a Hierarchy Element
        /// </summary>
        //private void RunSelectedMenu()
        //{
        //    //Prepopulate
        //    //Only allow one execution of  the function per event
        //    if (!ViewModelApplication.SideMenuVisible)
        //        return;
      
        //     mDraggedItem = (HierarchyViewModel)tvParameters.SelectedItem;
        //    if (mDraggedItem == null || mDraggedItem.Children.Count > 0 || ((string)mDraggedItem.Page).Length == 0)
        //        return;
        //    ViewModelApplication.OpenMenu(mDraggedItem.Root,mDraggedItem.Page);
        //       }
        /// <summary>
        /// Use Popup view to edit existing Hiearchy Element
        /// </summary>
        //private void EditHierarchyElement()
        //{
        //    //Prepopulate
        //    mDraggedItem = (HierarchyViewModel)tvParameters.SelectedItem;
        //    if (mDraggedItem == null)
        //        return;
        //    var mAddElementViewModel = (HierarchyElementViewModel)ViewModelApplication.CurrentPopupViewModel;
        //    mAddElementViewModel.ShortName.OriginalText = mDraggedItem.ShortName;
        //    mAddElementViewModel.ShortName.EditedText = mDraggedItem.ShortName;
        //    mAddElementViewModel.Description.OriginalText = mDraggedItem.Description;
        //    mAddElementViewModel.Description.EditedText = mDraggedItem.Description;
        //    mAddElementViewModel.ParentShortName = mDraggedItem.ParentShortName;
        //    mAddElementViewModel.ParentCategoryID = mDraggedItem.ParentCategoryID;
        //    mAddElementViewModel.KCategoryID = mDraggedItem.KCategoryID;
        //    mAddElementViewModel.DateEffective = mDraggedItem.DateEffective;
        //    mAddElementViewModel.DateDiscontinued = mDraggedItem.DateDiscontinued;
        //    mAddElementViewModel.AddNodeButtonText = null;
        //    mAddElementViewModel.EditNodeButtonText = "Update Selected Element";
        //    mAddElementViewModel.DeleteNodeButtonText = null;
        //    mAddElementViewModel.CopyNodeButtonText = null;
        //    mAddElementViewModel.MoveNodeButtonText = null;
        //    mAddElementViewModel.HeadingText = "Update Selected Element";
            
        //    //ViewModelApplication.CurrentPopupContent = PopupContent.AddElement;
        //    ViewModelApplication.PopupVisible = true;
        //    //ViewModelApplication.SettingsMenuVisible = true;
        //}
        //private void DeleteHierarchyElement()
        //{
        //    //Prepopulate
        //    mDraggedItem = (HierarchyViewModel)tvParameters.SelectedItem;
        //    if (mDraggedItem == null)
        //        return;
        //    var mAddElementViewModel = (HierarchyElementViewModel)ViewModelApplication.CurrentPopupViewModel;
        //    mAddElementViewModel.ShortName.OriginalText = mDraggedItem.ShortName;
        //    mAddElementViewModel.Description.OriginalText = mDraggedItem.Description;
        //    mAddElementViewModel.ParentShortName = mDraggedItem.ParentShortName;
        //    mAddElementViewModel.ParentCategoryID = mDraggedItem.ParentCategoryID;
        //    mAddElementViewModel.KCategoryID = mDraggedItem.KCategoryID;
        //    mAddElementViewModel.DateEffective = mDraggedItem.DateEffective;
        //    mAddElementViewModel.DateDiscontinued = mDraggedItem.DateDiscontinued;
        //    mAddElementViewModel.AddNodeButtonText = null;
        //    mAddElementViewModel.EditNodeButtonText = null;
        //    mAddElementViewModel.CopyNodeButtonText = null;
        //    mAddElementViewModel.MoveNodeButtonText = null;
        //    mAddElementViewModel.DeleteNodeButtonText = "Delete Selected Element";
        //    mAddElementViewModel.HeadingText = "Delete Selected Element";
            
        //    //ViewModelApplication.CurrentPopupContent = PopupContent.AddElement;
        //    ViewModelApplication.PopupVisible = true;
        //    //ViewModelApplication.SettingsMenuVisible = true;
        //}
        /// <summary>
        /// Use Popup view to move existing Hiearchy Element
        /// </summary>
        //private void MoveHierarchyElement()
        //{
        //    //Prepopulate
        //    mDraggedItem = (HierarchyViewModel)tvParameters.SelectedItem;

        //    var mAddElementViewModel = (HierarchyElementViewModel)ViewModelApplication.CurrentPopupViewModel;
        //    mAddElementViewModel.ShortName.OriginalText = mDraggedItem.ShortName;
        //    mAddElementViewModel.Description.OriginalText = mDraggedItem.Description; 
        //    mAddElementViewModel.ShortName.EditedText = mDraggedItem.ShortName;
        //    mAddElementViewModel.Description.EditedText = mDraggedItem.Description;
        //    mAddElementViewModel.ParentShortName = mTarget.ShortName;
        //    mAddElementViewModel.ParentCategoryID = mTarget.KCategoryID;
        //    mAddElementViewModel.KCategoryID = mDraggedItem.KCategoryID;
        //    mAddElementViewModel.DateEffective = mDraggedItem.DateEffective;
        //    mAddElementViewModel.DateDiscontinued = new DateTime(9999, 12, 31);
        //    mAddElementViewModel.AddNodeButtonText = null;
        //    mAddElementViewModel.MoveNodeButtonText = "Move Selected Element";
        //    mAddElementViewModel.DeleteNodeButtonText = null;
        //    mAddElementViewModel.CopyNodeButtonText = null;
        //    mAddElementViewModel.EditNodeButtonText = null;
        //    mAddElementViewModel.HeadingText = "Move Selected Element (with descendants)";
            
        //    //ViewModelApplication.CurrentPopupContent = PopupContent.AddElement;
        //    ViewModelApplication.PopupVisible = true;
        //    //ViewModelApplication.SettingsMenuVisible = true;
        //}
        /// <summary>
        /// Copy the selected hierarchy (with all descendants) to the element selected as the destination
        /// "Copy Of " is used as a prefix for all elements in the element family being copied
        /// </summary>
        //private void CopyHierarchyElement()
        //{
        //    //Prepopulate
        //    mDraggedItem = (HierarchyViewModel)tvParameters.SelectedItem;
        //    var mAddElementViewModel = (HierarchyElementViewModel)ViewModelApplication.CurrentPopupViewModel;
        //    mAddElementViewModel.ShortName.OriginalText = mDraggedItem.ShortName;
        //    mAddElementViewModel.Description.OriginalText = mDraggedItem.Description;
        //    mAddElementViewModel.ShortName.EditedText = mDraggedItem.ShortName;
        //    mAddElementViewModel.Description.EditedText = mDraggedItem.Description;
        //    mAddElementViewModel.ParentShortName = mTarget.ShortName;
        //    mAddElementViewModel.ParentCategoryID = mTarget.KCategoryID;
        //    mAddElementViewModel.KCategoryID = mDraggedItem.KCategoryID;
        //    mAddElementViewModel.DateEffective = DateTime.Today;
        //    mAddElementViewModel.DateDiscontinued = new DateTime(9999,12,31);
        //    mAddElementViewModel.AddNodeButtonText = null;
        //    mAddElementViewModel.EditNodeButtonText = null;
        //    mAddElementViewModel.MoveNodeButtonText = null;
        //    mAddElementViewModel.CopyNodeButtonText = "Copy Selected Element";
        //    mAddElementViewModel.DeleteNodeButtonText = null;
        //    mAddElementViewModel.HeadingText = "Copy Selected Element (with descendants)";
            
        //    //ViewModelApplication.CurrentPopupContent = PopupContent.AddElement;
        //    ViewModelApplication.PopupVisible = true;
        //    //ViewModelApplication.SettingsMenuVisible = true;
        //}
        #endregion







    }
}
