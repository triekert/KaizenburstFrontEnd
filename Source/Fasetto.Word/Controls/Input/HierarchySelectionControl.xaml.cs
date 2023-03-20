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
    public partial class HierarchySelectionControl : UserControl
    {

        #region Public Properties

        //public string ControlTitle { get; set; } = "Title of Control";
        public DateTime mTimer;

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
        public HierarchySelectionControl()
        {

            //Set the root of the hierarchy to return the Menu structure
            var root = "2D7E4A7D-6F19-496E-8709-47E6A9ADDFA0";
            if (!ViewModelApplication.ControlParameter3)
            { 
            root = ViewModelApplication.ControlParameter;
            }
            else
            root = ViewModelApplication.ControlParameter1;
            mHierarchyTree = new HierarchyTreeViewModel(root);//root);

            DataContext = mHierarchyTree;
            InitializeComponent();
            mTimer = DateTime.Now;
            //ViewModelApplication.CurrentSideMenuViewModel = mHierarchyTree;
            //CloseCommand = new RelayCommand(Close);

        }

        /// <summary>
        /// the Overloading of MenuControl() with a parameter that selects the Menu Hierarchy for naviagion by passing the parameter
        /// </summary>
        /// <param name="root"></param>
        public HierarchySelectionControl(string root)
        {
            mHierarchyTree = new HierarchyTreeViewModel(root);//root);

            DataContext = mHierarchyTree;
            InitializeComponent();
            //ViewModelApplication.CurrentSideMenuViewModel = mHierarchyTree;
        }

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
             mHierarchyTree.SearchCommand.Execute(null) ;
                   }

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
        /// <summary>
        /// The TreeView_MouseDown event does not cater for the left mouse button on Tree View Items
        /// A soulution is to use the PreViewMouseDown event and to allow it to bubble down to the selected treeview item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (e.ChangedButton == MouseButton.Left)
            {
                
                if (((TreeViewItem)sender).IsSelected  && (((TreeViewItem)sender).IsExpanded ||(((HierarchyViewModel)((TreeViewItem)sender).DataContext).Children.Count() == 0)))
                {

                    //e.Handled = true;
                    //EditHierarchyElement();
                }
                //
            }

            //mDraggedItem = (HierarchyViewModel)tvParameters.SelectedItem;
            //mSourceCategoryName = mDraggedItem.ShortName;

            //e.Handled = true; This cannot be set if the correct object is to be retrieved
        }



        /// <summary>
        /// This method responds to the MouseDown event and evaluates for the right click event
        /// -If the event is not handled at the treeview item level, it will pass through again at the treeview root level...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// TreeView_MouseLeftButtonDown
        private void TreeView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                //if (((TreeViewItem)sender).IsSelected)
                //{
                    RunSelectedItem();
                //}
            }
            else
                if (e.ChangedButton == MouseButton.Middle)
                {
                    if (((TreeViewItem)sender).IsSelected)
                    {
                        RunSelectedItem();
                    }
                }
            //    else
            //    if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            //{
            //    if (((TreeViewItem)sender).IsSelected)
            //    {
            //        RunSelectedItem();
            //    }
            //}

            e.Handled = true;
        }
        private void TreeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var timeDiff = (DateTime.Now -mTimer);
            var mMilliSec = timeDiff.TotalMilliseconds;
            mTimer= DateTime.Now;
            if  (mMilliSec>3500)
                { 
                if (e.ChangedButton == MouseButton.Right)
                    {
                     RunSelectedItem();
                        //e.Handled = true;
                    }
                    else
                        if (e.ChangedButton == MouseButton.Left )
                            {
                                //if (((TreeViewItem)sender).IsSelected)
                                //    {
                                        RunSelectedItem();
                                    //}
                            }
                }
            e.Handled = true;
            ViewModelApplication.ControlParameter3 = ViewModelApplication.ControlParameter3;
        }
        /// <summary>
        /// Monitor keyboard for use of Insert key
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeView_KeyBoard(object sender, KeyboardEventArgs e)
        {
            //check to determine whether user would like to add an item to the hierarchy


 
                    if (Keyboard.IsKeyDown(Key.Enter))
                {
                    RunSelectedItem();
                    e.Handled = true;

                }


        }

   
        private TreeViewItem GetNearestContainer(UIElement element)
        {
            // Walk up the element tree to the nearest tree view item.
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
            // Figure out a relative position of the selected node to the scrollviewer
            var relativePosition = element.TranslatePoint(new Point(0, 0), scrollViewer);
            element.BringIntoView();
            scrollViewer.ScrollToVerticalOffset(relativePosition.Y);
        }

        public void Close()
        {
            // Close settings menu
            ViewModelApplication.PopupVisible = false;


        }
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
        private void RunSelectedItem()
        {
            //Prepopulate
            //Only allow one execution of  the function per event
            //if (!ViewModelApplication.SideMenuVisible)
            //    return;
      
             mDraggedItem = (HierarchyViewModel)tvParameters.SelectedItem;
            if (mDraggedItem == null)
                return;
            //Toggle hierarchy navigation 
            if (!ViewModelApplication.ControlParameter3)
            { 
                ViewModelApplication.ControlParameter1 = mDraggedItem.KCategoryID;
                ViewModelApplication.ControlParameter2 = mDraggedItem.ShortName;
            }
            else
                {
                    ViewModelApplication.ControlParameter4 = mDraggedItem.KCategoryID;
                    ViewModelApplication.ControlParameter5= mDraggedItem.ShortName;
                }

                ViewModelApplication.ControlParameter3 = !ViewModelApplication.ControlParameter3;
                ViewModelApplication.PopupVisible = false;
                ViewModelApplication.CurrentPopupContent = PopupContent.AddElement;


        }
 
  
        #endregion







    }
}
