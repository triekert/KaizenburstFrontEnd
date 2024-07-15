using Fasetto.Word.Core;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
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
    public partial class HierarchyControl : UserControl
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
        public HierarchyControl()
        {

            //var root = "1C225789-3938-4480-86CB-071863DC5D33";
            var root = "2D7E4A7D-6F19-496E-8709-47E6A9ADDFA0";
            root = ViewModelApplication.ControlParameter;
            mHierarchyTree = new HierarchyTreeViewModel(root);//root);
            ViewModelApplication.CurrentPopupContent = PopupContent.AddElement;
            ViewModelApplication.PopupVisible = false;
            DataContext = mHierarchyTree;
            InitializeComponent();
            ViewModelApplication.CurrentControlViewModel = mHierarchyTree;
            //CloseCommand = new RelayCommand(Close);


        }

        public HierarchyControl(string root)
        {
            mHierarchyTree = new HierarchyTreeViewModel(root);//root);

            DataContext = mHierarchyTree;
            InitializeComponent();
            ViewModelApplication.CurrentControlViewModel = mHierarchyTree;   
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



        //
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
        /// The TreeView_MouseDown event does not cater for the left mouse button on Tree View Items
        /// A solution is to use the PreViewMouseDown event and to allow it to bubble down to the selected treeview item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //var tst = e.OriginalSource;
            //if (e.ChangedButton == MouseButton.Left)

            //{
            //    if (((TreeViewItem)sender).IsSelected)
            //    {
            //        if (!((TreeViewItem)sender).IsExpanded)
            //        {
            //            ((TreeViewItem)sender).IsExpanded = true;
            //            e.Handled = true;
            //        }
            //        else 
            //        { 
            //        
            //        EditHierarchyElement();
            //        
            //    }
            //}

            //    mDraggedItem = null;
            //mSource =(TreeViewItem)sender;
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
        private void TreeView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                if (((TreeViewItem)sender).IsSelected)
                {
                    AddHierarchyElement();
                }
            }
            else
                if (e.ChangedButton == MouseButton.Middle)
            {
                if (((TreeViewItem)sender).IsSelected)
                {
                    EditHierarchyElement();
                }
            }
            e.Handled = true;
        }
        private void TreeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        { if (e.ChangedButton == MouseButton.Right)
            {
             AddHierarchyElement();                            
            }
            else
                if (e.ChangedButton == MouseButton.Left)
                    {
                    EditHierarchyElement(); 
                    }
            var clkcnt = e.ClickCount;
            e.Handled = true;

        }
        /// <summary>
        /// Monitor keyboard for use of Insert key
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeView_KeyBoard(object sender, KeyboardEventArgs e)
        {
            //check to determine whether user would like to add an item to the hierarchy
            
            if (ViewModelApplication.PopupVisible == false)
            { 

                if (Keyboard.IsKeyDown(Key.Insert))
                {
                    AddHierarchyElement();                    
                    e.Handled= true;
                }
                else
                    if (Keyboard.IsKeyDown(Key.Enter))
                    {
                    EditHierarchyElement();
                    e.Handled = true;

                    }
                    else
                        if (Keyboard.IsKeyDown(Key.Delete))
                        {
                            DeleteHierarchyElement();
                        e.Handled = true;

                        }


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
                    var currentPosition = e.GetPosition(tvParameters);

                    //Check for dragging of treeview item
                    if ((Math.Abs(currentPosition.X - mLastMouseDown.X) > 10.0) ||
                        (Math.Abs(currentPosition.Y - mLastMouseDown.Y) > 10.0))
                    {

                        mDraggedItem = (HierarchyViewModel)tvParameters.SelectedItem;
                        mSourceCategoryName = mDraggedItem.ShortName;
                        //draggedItem = (TreeViewItem)tvParameters.SelectedItem;
                        //mSource = (TreeViewItem)tvParameters.SelectedItem;
                        if (mDraggedItem != null)
                        {
                            mTarget = null;//ensure target is reset
                            if (!isCtrl)
                            {

                                var finalDropEffect = DragDrop.DoDragDrop(tvParameters, tvParameters.SelectedValue,
                                  DragDropEffects.Move);
                                //Checking target is not null and item is dragging(moving)
                                if ((finalDropEffect == DragDropEffects.Move) && (mTarget != null))
                                {
                                    // A Move drop was accepted
                                    //if (!mSource.Header.ToString().Equals(mTargetT.Header.ToString()))
                                    //{
                                    MoveHierarchyElement();// MoveItem();
                                        mTargetT = null;
                                        mSource= null;
                                    //}

                                }
                            }
                            else
                            {
                                var finalDropEffect = DragDrop.DoDragDrop(tvParameters, tvParameters.SelectedValue,
                                  DragDropEffects.Copy);
                                if ((finalDropEffect == DragDropEffects.Copy) && (mTarget != null))
                                {
                                    // A Copy drop was accepted
                                    //if (!mSource.Header.ToString().Equals(mTargetT.Header.ToString()))
                                    //{
                                    CopyHierarchyElement();// CopyItem();
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
                        { e.Effects = CheckDropTarget(mTarget, mDraggedItem) ? DragDropEffects.Copy : DragDropEffects.None;}
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
                    e.Handled = true;
                    return;

        }
        //private void TreeView_MouseEnter(object sender, MouseEventArgs e)
        //{
        //    //try
        //    //{
   
        //    ((HierarchyViewModel)((TreeViewItem)sender).DataContext).IsSelected = true;

        //    e.Handled = true;
        //    return;
        //}
        //private void TreeView_MouseLeave(object sender, MouseEventArgs e)
        //{
        //    //try
        //    //{
        //    ((HierarchyViewModel)((TreeViewItem)sender).DataContext).IsSelected = false;


        //    e.Handled = true;
        //    return;

        //}

        /// <summary>
        /// Check whether it is possible to allow drop into the current
        /// Desitination view model
        /// </summary>
        /// <param name="mTargetN"></param>
        /// <param name="mDraggedN"></param>
        /// <returns></returns>

        private bool CheckDropTarget(HierarchyViewModel mTargetN, HierarchyViewModel mDraggedN)
        {
            //Check whether the target item is meeting your condition


            //TO DO:

            //Check that move will not cause infinite loop(ascendant-descendant - ascendant)
            //Check that the item being moved is not an ascendant of the item being moved to
            //the KCategoryID attribute of the item being moved may not be an ancestor of the
            //item being moved too.
            //If this constraint is met, the boolean is set to TRUE

            mDestinationID = mTargetN.KCategoryID;
            mSourceID = mDraggedItem.KCategoryID;
            mParentID = mDraggedItem.ParentCategoryID;
                //mSourceCategoryName = (string)res.GetType().GetProperties().Single(c => c.Name == "ShortName").GetValue(res);
                if (mSourceID == mDestinationID
                    || mParentID == mDestinationID)
                    { return false; }
                //var mDestinationID = (string)res.GetType().GetProperties().Single(c => c.Name == "KId").GetValue(res);

                MatchingKCategoryEnumerator = null;
 
                return PerformKIdSearch();




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
        private void AddHierarchyElement()
        {
            //Prepopulate
            mDraggedItem = (HierarchyViewModel)tvParameters.SelectedItem;
            if (mDraggedItem == null)
                return;
            var results = mHierarchyTree.mPersist.Where(x => x.KCategoryID == mDraggedItem.ParentCategoryID).OrderBy(x => x.ShortName).ToList();
            var mPage = "";
            if (results.Count > 0)
             mPage = results.FirstOrDefault().Page;
            var ParentNodeClient = results.FirstOrDefault().FClientID;
            var mAddElementViewModel = (HierarchyElementViewModel)ViewModelApplication.CurrentPopupViewModel;
            mAddElementViewModel.ShortName.OriginalText = "New Element Name";
            mAddElementViewModel.Description.OriginalText = "Description of New Element";
            mAddElementViewModel.ShortName.EditedText = "New Element Name";
            mAddElementViewModel.Description.EditedText = "Description of New Element";
            //if (mPage == "Hierarchy")
            //    mAddElementViewModel.Page = mPage;
            //else
            mAddElementViewModel.Page = "";
            mAddElementViewModel.Root.OriginalText = "Element Root";
            mAddElementViewModel.Root.EditedText = "Element Root";
            mAddElementViewModel.IsMenuItem = mDraggedItem.IsMenuItem;
            mAddElementViewModel.ParentShortName = mDraggedItem.ShortName;
            mAddElementViewModel.ParentCategoryID = mDraggedItem.KCategoryID;
            mAddElementViewModel.KCategoryID = Guid.NewGuid().ToString().ToUpper();
            mAddElementViewModel.DateEffective = DateTime.Today;
            mAddElementViewModel.DateDiscontinued = new DateTime(9999,12,31);
            mAddElementViewModel.AddNodeButtonText = "Add new Hierarchy Element";
            mAddElementViewModel.EditNodeButtonText = null;
            mAddElementViewModel.DeleteNodeButtonText = null;
            mAddElementViewModel.CopyNodeButtonText = null;
            mAddElementViewModel.MoveNodeButtonText = null;
            mAddElementViewModel.HierarchyType = mDraggedItem.HierarchyType;
            mAddElementViewModel.HierarchyTypeID = mDraggedItem.HierarchyTypeID;
            mAddElementViewModel.Type.OriginalKid = mDraggedItem.HierarchyTypeID;
            mAddElementViewModel.Type.OriginalName = mDraggedItem.HierarchyType;
            mAddElementViewModel.FClientID = mDraggedItem.FClientID;
            mAddElementViewModel.HeadingText= "Add new Hierarchy Element";
            
            //ViewModelApplication.CurrentPopupContent = PopupContent.AddElement;
            ViewModelApplication.PopupVisible = true;
            //ViewModelApplication.SettingsMenuVisible = true;
        }
        /// <summary>
        /// Use Popup view to edit existing Hiearchy Element
        /// </summary>
        private void EditHierarchyElement()
        {

            //Prepopulate
            mDraggedItem = (HierarchyViewModel)tvParameters.SelectedItem;
            if (mDraggedItem == null)
                return;
            var mAddElementViewModel = (HierarchyElementViewModel)ViewModelApplication.CurrentPopupViewModel;
            mAddElementViewModel.ShortName.OriginalText = mDraggedItem.ShortName;
            mAddElementViewModel.ShortName.EditedText = mDraggedItem.ShortName;
            mAddElementViewModel.Description.OriginalText = mDraggedItem.Description;
            mAddElementViewModel.Description.EditedText = mDraggedItem.Description;
            mAddElementViewModel.Page = mDraggedItem.Page;
            mAddElementViewModel.Root.OriginalText = mDraggedItem.Root;
            mAddElementViewModel.Root.EditedText = mDraggedItem.Root;
            mAddElementViewModel.IsMenuItem = mDraggedItem.IsMenuItem;
            mAddElementViewModel.ParentShortName = mDraggedItem.ParentShortName;
            mAddElementViewModel.ParentCategoryID = mDraggedItem.ParentCategoryID;
            mAddElementViewModel.KCategoryID = mDraggedItem.KCategoryID;
            mAddElementViewModel.DateEffective = mDraggedItem.DateEffective;
            mAddElementViewModel.DateDiscontinued = mDraggedItem.DateDiscontinued;
            mAddElementViewModel.AddNodeButtonText = null;
            mAddElementViewModel.EditNodeButtonText = "Update Selected Element";
            mAddElementViewModel.DeleteNodeButtonText = null;
            mAddElementViewModel.CopyNodeButtonText = null;
            mAddElementViewModel.MoveNodeButtonText = null;
            mAddElementViewModel.HeadingText = "Update Selected Element";
            mAddElementViewModel.HierarchyType = mDraggedItem.HierarchyType;
            mAddElementViewModel.HierarchyTypeID = mDraggedItem.HierarchyTypeID;
            mAddElementViewModel.FHierarchyID = mDraggedItem.HierarchyTypeID;
            mAddElementViewModel.Type.OriginalKid = mDraggedItem.HierarchyTypeID;
            mAddElementViewModel.Type.OriginalName = mDraggedItem.HierarchyType;
            mAddElementViewModel.Type.EditedKid = mDraggedItem.HierarchyTypeID;
            mAddElementViewModel.Type.EditedName = mDraggedItem.HierarchyType;
            //ViewModelApplication.CurrentPopupContent = PopupContent.AddElement;
            //ViewModelApplication.CurrentPopupViewModel = null;
            //ViewModelApplication.CurrentPopupContent = PopupContent.SWBilling;
            ////ViewModelApplication.CurrentPopupContent = PopupContent.AddElement;
            ViewModelApplication.PopupVisible = true;
            //ViewModelApplication.SettingsMenuVisible = true;
        }
        private void DeleteHierarchyElement()
        {
            //Prepopulate
            mDraggedItem = (HierarchyViewModel)tvParameters.SelectedItem;
            if (mDraggedItem == null)
                return;
            var mAddElementViewModel = (HierarchyElementViewModel)ViewModelApplication.CurrentPopupViewModel;
            mAddElementViewModel.ShortName.OriginalText = mDraggedItem.ShortName;
            mAddElementViewModel.ShortName.EditedText = mDraggedItem.ShortName;
            mAddElementViewModel.Description.OriginalText = mDraggedItem.Description;
            mAddElementViewModel.Description.EditedText = mDraggedItem.Description;
            mAddElementViewModel.ParentShortName = mDraggedItem.ParentShortName;
            mAddElementViewModel.ParentCategoryID = mDraggedItem.ParentCategoryID;
            mAddElementViewModel.Page = mDraggedItem.Page;
            mAddElementViewModel.Root.OriginalText = mDraggedItem.Root;
            mAddElementViewModel.Root.EditedText = mDraggedItem.Root;
            mAddElementViewModel.IsMenuItem = mDraggedItem.IsMenuItem;
            mAddElementViewModel.KCategoryID = mDraggedItem.KCategoryID;
            mAddElementViewModel.DateEffective = mDraggedItem.DateEffective;
            mAddElementViewModel.DateDiscontinued = mDraggedItem.DateDiscontinued;
            mAddElementViewModel.AddNodeButtonText = null;
            mAddElementViewModel.EditNodeButtonText = null;
            mAddElementViewModel.CopyNodeButtonText = null;
            mAddElementViewModel.MoveNodeButtonText = null;
            mAddElementViewModel.DeleteNodeButtonText = "Delete Selected Element";
            mAddElementViewModel.HeadingText = "Delete Selected Element";
            mAddElementViewModel.HierarchyType = mDraggedItem.HierarchyType;
            mAddElementViewModel.HierarchyTypeID = mDraggedItem.HierarchyTypeID;
            mAddElementViewModel.FHierarchyID = mDraggedItem.HierarchyTypeID;
            mAddElementViewModel.Type.OriginalKid = mDraggedItem.HierarchyTypeID;
            mAddElementViewModel.Type.OriginalName = mDraggedItem.HierarchyType;


            //ViewModelApplication.CurrentPopupContent = PopupContent.AddElement;
            ViewModelApplication.PopupVisible = true;
            //ViewModelApplication.SettingsMenuVisible = true;
        }
        /// <summary>
        /// Use Popup view to move existing Hiearchy Element
        /// </summary>
        private void MoveHierarchyElement()
        {
            //Prepopulate
            mDraggedItem = (HierarchyViewModel)tvParameters.SelectedItem;

            var mAddElementViewModel = (HierarchyElementViewModel)ViewModelApplication.CurrentPopupViewModel;
            mAddElementViewModel.ShortName.OriginalText = mDraggedItem.ShortName;
            mAddElementViewModel.Description.OriginalText = mDraggedItem.Description; 
            mAddElementViewModel.ShortName.EditedText = mDraggedItem.ShortName;
            mAddElementViewModel.Description.EditedText = mDraggedItem.Description;
            mAddElementViewModel.Page = mDraggedItem.Page;
            mAddElementViewModel.Root.OriginalText = mDraggedItem.Root;
            mAddElementViewModel.Root.EditedText = mDraggedItem.Root;
            mAddElementViewModel.IsMenuItem = mDraggedItem.IsMenuItem;
            mAddElementViewModel.ParentShortName = mTarget.ShortName;
            mAddElementViewModel.ParentCategoryID = mTarget.KCategoryID;
            mAddElementViewModel.KCategoryID = mDraggedItem.KCategoryID;
            mAddElementViewModel.DateEffective = mDraggedItem.DateEffective;
            mAddElementViewModel.DateDiscontinued = new DateTime(9999, 12, 31);
            mAddElementViewModel.AddNodeButtonText = null;
            mAddElementViewModel.MoveNodeButtonText = "Move Selected Element";
            mAddElementViewModel.DeleteNodeButtonText = null;
            mAddElementViewModel.CopyNodeButtonText = null;
            mAddElementViewModel.EditNodeButtonText = null;
            mAddElementViewModel.HierarchyType = mDraggedItem.HierarchyType;
            mAddElementViewModel.HierarchyTypeID = mDraggedItem.HierarchyTypeID;
            mAddElementViewModel.HeadingText = "Move Selected Element (with descendants)";
            mAddElementViewModel.FHierarchyID = mDraggedItem.HierarchyTypeID;
            mAddElementViewModel.Type.OriginalKid = mDraggedItem.HierarchyTypeID;
            mAddElementViewModel.Type.OriginalName = mDraggedItem.HierarchyType;

            //ViewModelApplication.CurrentPopupContent = PopupContent.AddElement;
            ViewModelApplication.PopupVisible = true;
            //ViewModelApplication.SettingsMenuVisible = true;
        }
        /// <summary>
        /// Copy the selected hierarchy (with all descendants) to the element selected as the destination
        /// "Copy Of " is used as a prefix for all elements in the element family being copied
        /// </summary>
        private void CopyHierarchyElement()
        {
            //Prepopulate
            mDraggedItem = (HierarchyViewModel)tvParameters.SelectedItem;
            var mAddElementViewModel = (HierarchyElementViewModel)ViewModelApplication.CurrentPopupViewModel;
            mAddElementViewModel.ShortName.OriginalText = mDraggedItem.ShortName;
            mAddElementViewModel.Description.OriginalText = mDraggedItem.Description;
            mAddElementViewModel.ShortName.EditedText = mDraggedItem.ShortName;
            mAddElementViewModel.Description.EditedText = mDraggedItem.Description;
            mAddElementViewModel.Page = mDraggedItem.Page;
            mAddElementViewModel.Root.OriginalText = mDraggedItem.Root;
            mAddElementViewModel.Root.EditedText = mDraggedItem.Root;
            mAddElementViewModel.IsMenuItem = mDraggedItem.IsMenuItem;
            mAddElementViewModel.ParentShortName = mTarget.ShortName;
            mAddElementViewModel.ParentCategoryID = mTarget.KCategoryID;
            mAddElementViewModel.KCategoryID = mDraggedItem.KCategoryID;
            mAddElementViewModel.DateEffective = DateTime.Today;
            mAddElementViewModel.DateDiscontinued = new DateTime(9999,12,31);
            mAddElementViewModel.AddNodeButtonText = null;
            mAddElementViewModel.EditNodeButtonText = null;
            mAddElementViewModel.MoveNodeButtonText = null;
            mAddElementViewModel.CopyNodeButtonText = "Copy Selected Element";
            mAddElementViewModel.DeleteNodeButtonText = null;
            mAddElementViewModel.HierarchyType = mDraggedItem.HierarchyType;
            mAddElementViewModel.HierarchyTypeID = mDraggedItem.HierarchyTypeID;
            mAddElementViewModel.HeadingText = "Copy Selected Element (with descendants)";
            mAddElementViewModel.FHierarchyID = mDraggedItem.HierarchyTypeID;
            mAddElementViewModel.Type.OriginalKid = mDraggedItem.HierarchyTypeID;
            mAddElementViewModel.Type.OriginalName = mDraggedItem.HierarchyType;

            //ViewModelApplication.CurrentPopupContent = PopupContent.AddElement;
            ViewModelApplication.PopupVisible = true;
            //ViewModelApplication.SettingsMenuVisible = true;
        }
        #endregion







    }
}
