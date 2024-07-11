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
using static Fasetto.Word.DI;
using static Fasetto.Word.Core.CoreDI;

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

        /// <summary>
        /// Store View Model of current popup to allow reverse navigation
        /// </summary>
        public ParameterHierarchyItemSelectApiModel  root { get; set; }

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
            //Use the root of Clients
            root = new ParameterHierarchyItemSelectApiModel();


            //first check whether selection view model has already been populated for the relevant lookup
            //if ((ViewModelApplication.CurrentControlViewModel).GetType().Name == "HierarchyItemSelectionViewModel")
            //{ 

            switch (((HierarchyItemSelectionViewModel)ViewModelApplication.CurrentControlViewModel).Label)
            {
                case "Select Client":
                    //root.FHierarchyID = ((HierarchyItemSelectionViewModel)ViewModelApplication.CurrentControlViewModel).OriginalKid;
                    //root.RootID = ((HierarchyItemSelectionViewModel)ViewModelApplication.CurrentControlViewModel).OriginalKid;
                    //root.ClientID = "4766E825-1B58-410D-B06B-5A2639CA22C8";
                    //root.Level = ((HierarchyItemSelectionViewModel)ViewModelApplication.CurrentControlViewModel).Level;

                    root.FHierarchyID = null;
                    root.RootID = ((HierarchyItemSelectionViewModel)ViewModelApplication.CurrentControlViewModel).OriginalKid;
                    root.ClientID = null;
                    root.Level = ((HierarchyItemSelectionViewModel)ViewModelApplication.CurrentControlViewModel).Level;
                    //root.ClientID = "NULL";
                    //root.HierarchyTypeID = "NULL"; 
                    break;
                default:
                    if (((HierarchyItemSelectionViewModel)ViewModelApplication.CurrentControlViewModel).ClientID == null)
                    {
                        MessageBox.Show($"First select a valid Client to proceed...");
                        Close();
                        return;
                    }

                    root.ClientID = ((HierarchyItemSelectionViewModel)ViewModelApplication.CurrentControlViewModel).ClientID;
                    root.HierarchyTypeID = ((HierarchyItemSelectionViewModel)ViewModelApplication.CurrentControlViewModel).HierarchyTypeID;
                    root.FHierarchyID = ((HierarchyItemSelectionViewModel)ViewModelApplication.CurrentControlViewModel).HierarchyID;
                    root.Level = ((HierarchyItemSelectionViewModel)ViewModelApplication.CurrentControlViewModel).Level;
                    root.RootID = ((HierarchyItemSelectionViewModel)ViewModelApplication.CurrentControlViewModel).RootID;
                    //((HierarchyItemSelectionViewModel)ViewModelApplication.CurrentControlViewModel).OriginalKid = "00000000-0000-0000-0000-000000000000";
                    //root.FHierarchyID = "NULL"; 

                    break;
            }
            //}
            switch (((HierarchyItemSelectionViewModel)ViewModelApplication.CurrentControlViewModel).Label)
            {
                case "Select Client":
                    mHierarchyTree = new HierarchyTreeViewModel1(root);//root);
                    break;

                case "Select Cost Category":
                    if (
                        (ViewModelApplication.ControlPopupCostCategory != null)
                         &&
                         ((ParameterHierarchyItemSelectApiModel)((HierarchyTreeViewModel1)ViewModelApplication.ControlPopupCostCategory).mHierarchy).ClientID == root.ClientID
                         &&
                         ((ParameterHierarchyItemSelectApiModel)((HierarchyTreeViewModel1)ViewModelApplication.ControlPopupCostCategory).mHierarchy).HierarchyTypeID == root.HierarchyTypeID
                         )
                    {
                        mHierarchyTree = (HierarchyTreeViewModel1)ViewModelApplication.ControlPopupCostCategory;
                    }
                    else

                        mHierarchyTree = new HierarchyTreeViewModel1(root);
                    ViewModelApplication.ControlPopupCostCategory = mHierarchyTree;
                    break;

                case "Select Linked Party":
                    if (
                        (ViewModelApplication.ControlPopupParty != null)
                         &&
                         ((ParameterHierarchyItemSelectApiModel)((HierarchyTreeViewModel1)ViewModelApplication.ControlPopupParty).mHierarchy).ClientID == root.ClientID
                         &&
                         ((ParameterHierarchyItemSelectApiModel)((HierarchyTreeViewModel1)ViewModelApplication.ControlPopupParty).mHierarchy).HierarchyTypeID == root.HierarchyTypeID
                         )
                    {
                        mHierarchyTree = (HierarchyTreeViewModel1)ViewModelApplication.ControlPopupParty;
                    }
                    else

                        mHierarchyTree = new HierarchyTreeViewModel1(root);
                    ViewModelApplication.ControlPopupParty = mHierarchyTree;
                    break;

                case "Select Cost Hierarchy":
                    if (
                        (ViewModelApplication.ControlPopupCostHierarchy != null)
                         &&
                         ((ParameterHierarchyItemSelectApiModel)((HierarchyTreeViewModel1)ViewModelApplication.ControlPopupCostHierarchy).mHierarchy).ClientID == root.ClientID 
                         &&
                         ((ParameterHierarchyItemSelectApiModel)((HierarchyTreeViewModel1)ViewModelApplication.ControlPopupCostHierarchy).mHierarchy).HierarchyTypeID  == root.HierarchyTypeID                   
                         )
                    {
                        mHierarchyTree = (HierarchyTreeViewModel1)ViewModelApplication.ControlPopupCostHierarchy; 
                    }
                    else

                        mHierarchyTree = new HierarchyTreeViewModel1(root);
                        ViewModelApplication.ControlPopupCostHierarchy = mHierarchyTree;
                    break;

                default:
                    mHierarchyTree = new HierarchyTreeViewModel1(root);//root);
                    break;
            }

            //search for current original item on hierarchy
            //mHierarchyTree.mSearchText = ((HierarchyItemSelectionViewModel)ViewModelApplication.CurrentControlViewModel).OriginalKid;





            //ViewModelApplication.ControlParameter5 = ViewModelApplication.CurrentControlViewModel;
            //ViewModelApplication.CurrentPopupViewModel = this;
            PriorPopupViewModel = ViewModelApplication.CurrentPopupViewModel;
            DataContext = mHierarchyTree;
            //CloseCommand = new RelayCommand(Close);
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
                FClientID = item.FClientID,

                

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
                    else

                        if (Keyboard.IsKeyDown(Key.Insert))
                            {
                                AddHierarchyElement();
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
            ViewModelApplication.CurrentPopupContent = 0;
            RunSelectedItem();


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
            //((HierarchyItemSelectionViewModel)ViewModelApplication.CurrentControlViewModel).OriginalName   = mDraggedItem.ShortName;
            ((HierarchyItemSelectionViewModel)ViewModelApplication.CurrentControlViewModel).ClientID       = mDraggedItem.FClientID;
            //((HierarchyItemSelectionViewModel)ViewModelApplication.CurrentControlViewModel).OriginalName     = mDraggedItem.ShortName;
            //((HierarchyItemSelectionViewModel)ViewModelApplication.CurrentControlViewModel).OriginalKid = mDraggedItem.KCategoryID;
            //((HierarchyItemSelectionViewModel)ViewModelApplication.CurrentControlViewModel).Editing= true;
            //((HierarchyItemSelectionViewModel)ViewModelApplication.CurrentControlViewModel).Working = false;
            //ViewModelApplication.PopupVisible = false;
            //ViewModelApplication.CurrentPopupContent = PopupContent.AddElement;


            if (ViewModelApplication.SettingsMenuVisible)

            {

                ViewModelApplication.FClientID = mDraggedItem.KCategoryID;
                ViewModelApplication.ClientShortName = mDraggedItem.ShortName;
                ViewModelApplication.PopupVisible = false;

                ViewModelApplication.CurrentPopupContent = 0;
                return;
            }

            //var Poptype = ViewModelApplication.CurrentControlViewModel.PriorPopupViewModel.GetType().Name;

            //If the calling page is from the SWBilling function

            
            if ((ViewModelApplication.CurrentPageViewModel.GetType().Name == "SWBillingPageViewModel"))
                {
                ((SWBillingPageViewModel)ViewModelApplication.CurrentPageViewModel).BillingPeriod.mRequest = ((SWBillingPageViewModel)ViewModelApplication.CurrentPageViewModel).Client.EditedKid;
                ((SWBillingPageViewModel)ViewModelApplication.CurrentPageViewModel).PopulateAsync();
                }
            var Pgtype = ViewModelApplication.CurrentPageViewModel.GetType().Name; 
            if (ViewModelApplication.CurrentPopupViewModel == null || (ViewModelApplication.CurrentPopupViewModel.GetType().Name != "ManageClassificationViewModel"))
            {
                if ((string)Pgtype == "TransactionSelectionPageViewModel")
                {
                    if (ViewModelApplication.ControlParameter1 != null)
                    {
                        ViewModelApplication.CurrentPopupViewModel = ViewModelApplication.ControlParameter1;
                        ViewModelApplication.ControlParameter1 = null;
                        ViewModelApplication.CurrentPopupContent = PopupContent.Classify;
                        ViewModelApplication.PopupVisible = true;
                    }   
                    else
                    {
                        if (((HierarchyItemSelectionViewModel)ViewModelApplication.CurrentControlViewModel).Label == "Select Client")
                        {
                            if (((HierarchyItemSelectionViewModel)ViewModelApplication.CurrentControlViewModel).EditedKid != 
                                ((HierarchyItemSelectionViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy).ClientID
                                //If client selection has changed, nullify cost hierarchy selection
                                )
                                {
                                ((HierarchyItemSelectionViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy).EditedKid = null;
                                ((HierarchyItemSelectionViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy).EditedName = null;
                            }
                        }
                        ViewModelApplication.PopupVisible = false;
                        //ViewModelApplication.CurrentPopupContent = PopupContent.AddElement;
                        ViewModelApplication.CurrentPopupContent = 0;
                    }

                }
                else
                {
                    if ((string)Pgtype == "HierarchyPageViewModel")
                        //If the control is being called from the Hierarchy Page view model (and this is a hierarchy element of type hierarchy, then return to the element editing page after selection of hierarchy type
                    {
                        //((HierarchyItemSelectionViewModel)((HierarchyElementViewModel)ViewModelApplication.CurrentPopupViewModel).Type).EditedKid = mDraggedItem.KCategoryID;
                        ((HierarchyElementViewModel)ViewModelApplication.CurrentPopupViewModel).HierarchyType = mDraggedItem.ShortName;
                        //((HierarchyItemSelectionViewModel)((HierarchyElementViewModel)ViewModelApplication.CurrentPopupViewModel).Type).ClientID = mDraggedItem.FClientID;
                        ((HierarchyElementViewModel)ViewModelApplication.CurrentPopupViewModel).HierarchyTypeID = mDraggedItem.KCategoryID;
                        //var TempViewModel = (HierarchyElementViewModel)ViewModelApplication.CurrentPopupViewModel;
                        ViewModelApplication.CurrentPopupContent = PopupContent.AddElement;
                        ViewModelApplication.CurrentControlViewModel = ViewModelApplication.ControlParameter5;

                        ViewModelApplication.PopupVisible = true;
                        //ViewModelApplication.AddElementViewModel = TempViewModel;
                    }
                    else
                    { 
                        ViewModelApplication.PopupVisible = false;

                    ViewModelApplication.CurrentPopupContent = PopupContent.AddElement;
                    }
 

                }


            }
            else
            {
                ViewModelApplication.CurrentPopupContent = PopupContent.Classify;
                ViewModelApplication.PopupVisible = true;
            }

            //return;



        }


        #endregion
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
            mAddElementViewModel.DateDiscontinued = new DateTime(9999, 12, 31);
            mAddElementViewModel.AddNodeButtonText = "Add new Hierarchy Element";
            mAddElementViewModel.EditNodeButtonText = null;
            mAddElementViewModel.DeleteNodeButtonText = null;
            mAddElementViewModel.CopyNodeButtonText = null;
            mAddElementViewModel.MoveNodeButtonText = null;
            mAddElementViewModel.HierarchyType = mDraggedItem.HierarchyType;
            mAddElementViewModel.HierarchyTypeID = mDraggedItem.HierarchyTypeID;
            mAddElementViewModel.Type.OriginalKid = mDraggedItem.HierarchyTypeID;
            mAddElementViewModel.Type.OriginalName = mDraggedItem.HierarchyType;
            mAddElementViewModel.FClientID = ParentNodeClient;
            mAddElementViewModel.HeadingText = "Add new Hierarchy Element";

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
            mAddElementViewModel.DateDiscontinued = new DateTime(9999, 12, 31);
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
