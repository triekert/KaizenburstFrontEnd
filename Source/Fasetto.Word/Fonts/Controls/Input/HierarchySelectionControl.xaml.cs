using Fasetto.Word.Core;
using Fasetto.Word.Core.ApiModels.Controls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Workflow.Activities;
using static Fasetto.Word.DI;

namespace Fasetto.Word
{
    /// <summary>
    /// Interaction logic for HierarchyManagementControl.xaml
    /// </summary>
    public partial class HierarchySelectionControl : UserControl
    {

        #region Public Properties

        //public string ControlTitle  = "Title of Control";
        public DateTime mTimer { get; set; } =DateTime.Now;

        /// <summary>
        /// Store View Model of current popup to allow reverse navigation
        /// </summary>
        public object PriorPopupViewModel { get; set; }

        #endregion//Public Properties

        #region Public Commands
        /// <summary>
        /// The command to close the settings menu
        /// </summary>
        //public ICommand CloseCommand { get; set; }
        #endregion//Public Commands


        private readonly HierarchyTreeViewModel1 mHierarchyTree;
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
            //var root = "2D7E4A7D-6F19-496E-8709-47E6A9ADDFA0";
            //Use the rootof Clients
            var root = new ParameterHierarchyItemSelectApiModel();

            //if ((ViewModelApplication.CurrentControlViewModel).GetType().Name == "HierarchyItemSelectionViewModel")
            //{ 
                if (((HierarchyItemSelectionViewModel)ViewModelApplication.CurrentControlViewModel).Label == "Select Client")
                {
                    root.FHierarchyID = ((HierarchyItemSelectionViewModel)ViewModelApplication.CurrentControlViewModel).OriginalKid ;
                    //root.ClientID = "NULL";
                    //root.HierarchyTypeID = "NULL"; 
                }
                else
                { 
                    if (((HierarchyItemSelectionViewModel)ViewModelApplication.CurrentControlViewModel).ClientID==null)
                    {
                        MessageBox.Show($"First select a valid Client to proceed...");
                        Close();
                        return;
                    }

                    root.ClientID = ((HierarchyItemSelectionViewModel)ViewModelApplication.CurrentControlViewModel).ClientID;
                    root.HierarchyTypeID = ((HierarchyItemSelectionViewModel)ViewModelApplication.CurrentControlViewModel).HierarchyTypeID;
                    root.FHierarchyID = ((HierarchyItemSelectionViewModel)ViewModelApplication.CurrentControlViewModel).HierarchyID;

                    //root.FHierarchyID = "NULL"; 
                }
            //}
            mHierarchyTree = new HierarchyTreeViewModel1(root);//root);
            //PriorPopupViewModel = ViewModelApplication.CurrentPopupViewModel;
            //ViewModelApplication.CurrentPopupViewModel = this;

            DataContext = mHierarchyTree;
            InitializeComponent();
            //mTimer = DateTime.Now;
            //ViewModelApplication.CurrentSideMenuViewModel = mHierarchyTree;
            //CloseCommand = new RelayCommand(Close);

        }

        /// <summary>
        /// the Overloading of MenuControl() with a parameter that selects the Menu Hierarchy for naviagion by passing the parameter
        /// </summary>
        /// <param name="root"></param>
        public HierarchySelectionControl(ParameterHierarchyItemSelectApiModel root)
        {
            mHierarchyTree = new HierarchyTreeViewModel1(root);//root);

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
            if  (mMilliSec>1000)
                { 
                if (e.ChangedButton == MouseButton.Right)
                    {
                     RunSelectedItem();
                    e.Handled = true;
                }
                    else
                        if (e.ChangedButton == MouseButton.Left )
                            {
                                //if (((TreeViewItem)sender).IsSelected)
                                //    {
                                        RunSelectedItem();
                    e.Handled = true;
                    //}
                }
                }
            else
            {
            e.Handled = e.Handled;
            }
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


 
                    if (Keyboard.IsKeyDown(Key.Enter))
                {

                ViewModelApplication.CurrentPageViewModel = ViewModelApplication.CurrentPageViewModel;

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
            ViewModelApplication.CurrentPopupContent = PopupContent.AddElement;


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
            //RootID = ((HierarchyItemSelectionViewModel)ViewModelApplication.CurrentControlViewModel).RootID;
            ((HierarchyItemSelectionViewModel)ViewModelApplication.CurrentControlViewModel).EditedKid      = mDraggedItem.KCategoryID;
            ((HierarchyItemSelectionViewModel)ViewModelApplication.CurrentControlViewModel).EditedName     = mDraggedItem.ShortName;
            ((HierarchyItemSelectionViewModel)ViewModelApplication.CurrentControlViewModel).OriginalName = mDraggedItem.ShortName;
            ((HierarchyItemSelectionViewModel)ViewModelApplication.CurrentControlViewModel).OriginalKid = mDraggedItem.KCategoryID;
            ((HierarchyItemSelectionViewModel)ViewModelApplication.CurrentControlViewModel).Editing = true;
            //((HierarchyItemSelectionViewModel)ViewModelApplication.CurrentControlViewModel).ClientID = mDraggedItem.fClientID;

            ViewModelApplication.CurrentPageViewModel = ViewModelApplication.CurrentPageViewModel;
            //ViewModelApplication.PopupVisible = false;
            //ViewModelApplication.CurrentPopupContent = PopupContent.AddElement;

            if (ViewModelApplication.CurrentPopupViewModel == null || (ViewModelApplication.CurrentPopupViewModel.GetType().Name != "ManageClassificationViewModel"))
            {
                ViewModelApplication.PopupVisible = false;
                ViewModelApplication.CurrentPopupContent = PopupContent.AddElement;
            }
            else
            {
                ViewModelApplication.CurrentPopupContent = PopupContent.Classify;
                ViewModelApplication.PopupVisible = true;
            }

            //return;
        }
 
  
        #endregion







    }
}
