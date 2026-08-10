using Fasetto.Word.Core;
using System;
using System.Collections.Generic;
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
    /// This class contains the code behind required for treeview management
    /// </summary>
    public partial class HierarchyControl : UserControl
    {

        #region Public Properties

        //public string ControlTitle { get; set; } = "Title of Control";

        #endregion//Public Properties

        //#region Public Commands
        ///// <summary>
        ///// The command to close the settings menu
        ///// </summary>
        ////public ICommand CloseCommand { get; set; }
        //#endregion//Public Commands


        public HierarchyTreeViewModel mHierarchyTree;
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
        public HierarchyControl()
        {

            //var root = "1C225789-3938-4480-86CB-071863DC5D33";
            //var root = "2D7E4A7D-6F19-496E-8709-47E6A9ADDFA0";

            //root is hardcoded to the GUID configured as the root of all menu options for the Kaizenburst framework
            //root = ViewModelApplication.ControlParameter;
            //ViewModelApplication.CurrentPopupViewModel = new HierarchyTreeViewModel(root);//root);
            //ViewModelApplication.CurrentPopupContent = 0;
            //ViewModelApplication.PopupVisible = false;
            DataContext = ViewModelApplication.CurrentPopupViewModel;
            InitializeComponent();
            //ViewModelApplication.CurrentPopupContent = PopupContent.Hierarchy;
            //ViewModelApplication.PopupVisible = true;

            //test what happens with controlviewmodel update

            //CloseCommand = new RelayCommand(Close);


        }

        public HierarchyControl(string root)
        {
            ViewModelApplication.CurrentPopupViewModel = new HierarchyTreeViewModel(root);//root);

            DataContext = ViewModelApplication.CurrentPopupViewModel;
            InitializeComponent();
            //ViewModelApplication.CurrentPopupViewModel = mHierarchyTree;   
        }


        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
             mHierarchyTree.SearchCommand.Execute(null) ;
        }



        /// <summary>
        /// When selected item changes, centre window on newly seleted item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewSelectedItemChanged(object sender, RoutedEventArgs e)
        {
            if (sender is TreeViewItem item)
            {
                item.BringIntoView(new Rect(100,100, 200, 200));
                //if (item.)
                e.Handled = true;
            }
        }

















        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeView_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                var item = GetNearestContainer(e.OriginalSource as UIElement);
                mDraggedItemTest = (HierarchyViewModel)item.Header;
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    var isCtrl = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
                    var isShift = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
                    var currentPosition = e.GetPosition(item);

                    //Check for dragging of treeview item
                    if ((Math.Abs(currentPosition.X - mLastMouseDown.X) > 10.0) ||
                        (Math.Abs(currentPosition.Y - mLastMouseDown.Y) > 10.0))
                    {

                        mDraggedItem = (HierarchyViewModel)((TreeViewItem)item).Header;
                        mSourceCategoryName = mDraggedItem.ShortName;
                        mLastMouseDown = currentPosition;
                        //draggedItem = (TreeViewItem)tvParameters.SelectedItem;
                        mSource = (TreeViewItem)sender;
                        if (mDraggedItem != null)
                        {
                            mTarget = null;//ensure target is reset
                            if (!isCtrl & !isShift)
                            {

                                var finalDropEffect = DragDrop.DoDragDrop(item, mDraggedItem,
                                  DragDropEffects.Move);
                                //Checking target is not null and item is dragging(moving)
                                if ((finalDropEffect == DragDropEffects.Move) && (mTarget != null))
                                {
                                    // A Move drop was accepted
                                    //if (!mSource.Header.ToString().Equals(mTargetT.Header.ToString()))
                                    //{
                                    //((HierarchyTreeViewModel)ViewModelApplication.CurrentPopupViewModel).MoveHierarchyElement(mDraggedItem,mTarget);// MoveItem();
                                    ((HierarchyTreeViewModel)ViewModelApplication.CurrentPopupViewModel).MoveHierarchyElement(mDraggedItem, mTarget);// MoveItem();

                                    mTargetT = null;
                                        mSource= null;
                                    //}

                                }
                            }
                            else
                            {
                                var finalDropEffect = DragDrop.DoDragDrop(tvParameters, tvParameters.SelectedValue,
                                  DragDropEffects.Copy);
                                if ((finalDropEffect == DragDropEffects.Copy) && (mTarget != null) && isCtrl)
                                {
                                    // A Copy drop was accepted
                                    //if (!mSource.Header.ToString().Equals(mTargetT.Header.ToString()))
                                    //{
                                    ((HierarchyTreeViewModel)ViewModelApplication.CurrentPopupViewModel).CopyHierarchyElement(mDraggedItem, mTarget);// CopyItem();
                                    mTargetT = null;
                                    mSource = null;
                                    //}

                                }
                                else
                               {
                                    finalDropEffect = DragDrop.DoDragDrop(tvParameters, tvParameters.SelectedValue,
                                   DragDropEffects.Link);
                                    // A Copy drop was accepted
                                    //if (!mSource.Header.ToString().Equals(mTargetT.Header.ToString()))
                                    //{
                                    ((HierarchyTreeViewModel)ViewModelApplication.CurrentPopupViewModel).CopyHierarchyElement(mDraggedItem, mTarget);// CopyItem();
                                    mTargetT = null;
                                    mSource = null;
                                    //}

                                }

                            }



                        }
                    }
                }

            }
            catch (Exception)
            {
            }
        }

       
        /// <summary>
        /// Handle event when tree view item is dragged over potential
        /// target objects
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeView_DragOver(object sender, DragEventArgs e)
        {
            try
            {
                var currentPosition = e.GetPosition(tvParameters);

                if ((Math.Abs(currentPosition.X - mLastMouseDown.X) > 10.0) ||
                   (Math.Abs(currentPosition.Y - mLastMouseDown.Y) > 10.0))
                {
                    // Verify that this is a valid drop and then store the drop target
                    var item = GetNearestContainer(e.OriginalSource as UIElement);
   
                    if (item == null)
                    { e.Effects = DragDropEffects.None; }
                    else
                    {
                        mTargetT = item;
                        mTarget = (HierarchyViewModel)item.GetType().GetProperties().Single(c => c.Name == "DataContext").GetValue(item);
                        if (e.Effects == DragDropEffects.Move)
                        { e.Effects = CheckDropTarget(mTarget, mDraggedItem) ? DragDropEffects.Move : DragDropEffects.None;}
                        else
                        { e.Effects =  DragDropEffects.Copy;}
                        //{ e.Effects = CheckDropTarget(mTarget, mDraggedItem) ? DragDropEffects.Copy : DragDropEffects.None; }
                    }
                }
                e.Handled = true;

            }
            catch (Exception)
            {
            }
        }
        private void TreeView_Drop(object sender, DragEventArgs e)
        {
            //try
            //{

            Mouse.SetCursor(Cursors.Wait);
            //CheckDropTarget(mTarget, mDraggedItem);
            e.Handled = true;
            return;
 
        }


        private bool CheckDropTarget(HierarchyViewModel mTargetN, HierarchyViewModel mDraggedN)
        {
            //Check whether the target item is meeting your condition


            //TO DO:

            //Check that move will not cause infinite loop(Ancestor-descendant - Ancestor)
            //Check that the item being moved is not an Ancestor of the item being moved to
            //the KCategoryID attribute of the item being moved may not be an ancestor of the
            //item being moved too.
            //If this constraint is met, the boolean is set to TRUE

            mDestinationID = mTargetN.KCategoryID;
            mSourceID = mDraggedN.KCategoryID;
            mParentID = mDraggedN.ParentCategoryID;
            //mSourceCategoryName = (string)res.GetType().GetProperties().Single(c => c.Name == "ShortName").GetValue(res);
            if (mSourceID == mDestinationID
                || mParentID == mDestinationID || TreeViewHelper.GetChildTreeViewItems(mSource, mTargetT))
                    { return false; }
                //var mDestinationID = (string)res.GetType().GetProperties().Single(c => c.Name == "KId").GetValue(res);

                MatchingKCategoryEnumerator = null;

            //return PerformKIdSearch();
            return true;

        }




        //public void AddChild(TreeViewItem _sourceItem, TreeViewItem _targetItem)
        //{
        //    // add item in target TreeViewItem 
        //    var item1 = new TreeViewItem
        //    {
        //        Header = _sourceItem.DataContext

        //    };
        //    _targetItem.Items.Add(item1);
        //    foreach (TreeViewItem item in _sourceItem.Items)
        //    {
        //        AddChild(item, item1);
        //    }

        //}

        private static TObject FindVisualParent<TObject>(UIElement child) where TObject : UIElement
        {
            if (child == null)
            {
                return null;
            }

            var parent = VisualTreeHelper.GetParent(child) as UIElement;

            while (parent != null)
            {
                if (parent is TObject found)
                {
                    return found;
                }
                else
                {
                    parent = VisualTreeHelper.GetParent(parent) as UIElement;
                }
            }

            return null;
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
            var elementItem =( (TreeViewItem)element).Header;
            // Figure out a relative position of the selected node to the scrollviewer
            //var relativePosition = element.TranslatePoint(new Point(0, 0), scrollViewer);
            //scrollViewer.ScrollToVerticalOffset(relativePosition.Y);
            //element.BringIntoView();
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


        #endregion // Search Logic
        #region Element manipulation




        public ItemsControl GetSelectedTreeViewItemParent(TreeViewItem item)
        {
            var parent = VisualTreeHelper.GetParent(item);
            while (!(parent is TreeViewItem || parent is TreeView))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent as ItemsControl;
        }


        public static class TreeViewHelper
        {
            public static bool GetChildTreeViewItems(FrameworkElement parent,FrameworkElement target)
            {
               var childItems = new List<FrameworkElement>();
                var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
                //if (childrenCount ==0)
                //    return false;

                for (var i = 0; i < childrenCount;)
                {
                    var child = VisualTreeHelper.GetChild(parent, i) as FrameworkElement;
                    i++;
                    var typeName = child.GetType().Name;
                    var parentType = parent.GetType().Name;

                    if (child!=null && child.GetType().Name == "TreeViewItem" && child == target) 
                    {
                        return true;
                    }                  
                    else
                    if (i == childrenCount)
                    {
                        if (GetChildTreeViewItems(child,target))
                            return true;
                    }



                }
                return false;

            }

        }


        #endregion




    }
}
