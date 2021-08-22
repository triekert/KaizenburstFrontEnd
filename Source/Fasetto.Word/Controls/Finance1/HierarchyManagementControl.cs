using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Fasetto.Word.Core;
using System.Threading.Tasks;
using Dna;
using static Fasetto.Word.DI;

namespace Fasetto.Word
{
    /// <summary>
    /// Interaction logic for HierarchyManagementControl.xaml
    /// </summary>
    public partial class HierarchyManagementControl : UserControl
    {
        private readonly HierarchyManagementTreeViewModel mCategoryTree;
        private string mSourceCategoryId;
        private string mSourceCategoryName;
        private string mDestinationCategoryId;
        private string mDestinationCategoryName;
        private bool mIsSourceObtained = false;
        private Point mLastMouseDown;
        private TreeViewItem mDraggedItem, mTarget;
        private HierarchyManagementViewModel mDraggedItemTest;
        //private readonly object mFamilyTree;
        private readonly HierarchyManagementViewModel mTargetTest;

        [System.Obsolete]
        public HierarchyManagementControl(HierarchyManagementTreeDataModel hierarchyManagementTreeDataModel)
        {
            InitializeComponent();
            //Retrieve categories from database
            var t = new TreeViewItem();
            //t.HeaderTemplate.hier
            var categories = new List<HierarchyManagementDataModel>();
            //populate DataTable with results of database query

            DataTable dt = new HomeBAL().GetAllCategories();
            //iterate through table and add raw CategoryData objects to List
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    categories.Add(
                        new HierarchyManagementDataModel
                        {
                            ShortName = row["ShortName"].ToString(),
                            KCategoryID = row["kCategoryID"].ToString(),
                            ParentCategoryId = row["ParentCategoryID"].ToString(),
                            Description = row["description"].ToString()
                        });
                }

            }
            var headerTree = FillRecursive(categories, "");

            var rootCategory = headerTree.ElementAt(0);

            mCategoryTree = new HierarchyManagementTreeViewModel(rootCategory);
            // Get raw family tree data from a database.
            //Person rootPerson = Database.GetFamilyTree();

            // Create UI-friendly wrappers around the 
            // raw data objects (i.e. the view-model).
            //_familyTree = new FamilyTreeViewModel(rootPerson);

            // Let the UI bind to the view-model.
            //base.DataContext = _familyTree;
            DataContext = mCategoryTree;
        }

        public HierarchyManagementControl(string destinationCategoryId)
        {
            mDestinationCategoryId = destinationCategoryId;
        }

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                mCategoryTree.SearchCommand.Execute(null);
        }

        private static List<HierarchyManagementTreeDataModel> FillRecursive(List<HierarchyManagementDataModel> flatObjects, string parentId)
        {
            return flatObjects.Where(x => x.ParentCategoryId.Equals(parentId)).Select(item => new HierarchyManagementTreeDataModel
            {
                ShortName = item.ShortName,
                Description = item.Description,

                KCategoryID = item.KCategoryID,
                Children = FillRecursive(flatObjects,
                                         item.KCategoryID)
            }).ToList();
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
                var res = ((sender.GetType().GetProperties()).Single(c => c.Name == "DataContext")).GetValue(sender);
                //myCatPropertyInfo = res.GetType().GetProperties();
                mSourceCategoryId = (string)((res.GetType().GetProperties()).Single(c => c.Name == "KCategoryId")).GetValue(res);
                mSourceCategoryName = (string)((res.GetType().GetProperties()).Single(c => c.Name == "ShortName")).GetValue(res);
                var cursorChanged = Mouse.SetCursor(Cursors.Wait);
                var result = MessageBox.Show("Would you like to edit " + mSourceCategoryName.ToString() + "(,Press 'Yes', or Add a Child item Press No", "Modifying selected Category - Add Child or Edit Category", MessageBoxButton.YesNoCancel);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        //Edit the selected Category
                        MessageBox.Show("Editing selected Category", "Category Hierarchy");
                        break;
                    case MessageBoxResult.No:
                        //Edit the selected Category
                        MessageBox.Show("Adding Child to selected Category", "Category Hierarchy");
                        break;
                    default:
                        break;
                }
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
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    var currentPosition = e.GetPosition(tvParameters);


                    if ((Math.Abs(currentPosition.X - mLastMouseDown.X) > 10.0) ||
                        (Math.Abs(currentPosition.Y - mLastMouseDown.Y) > 10.0))
                    {

                        mDraggedItemTest = (HierarchyManagementViewModel)tvParameters.SelectedItem;
                        //draggedItem = (TreeViewItem)tvParameters.SelectedItem;                       
                        if (mDraggedItemTest != null)
                        {
                            var finalDropEffect = DragDrop.DoDragDrop(tvParameters, tvParameters.SelectedValue,
                                DragDropEffects.Move);
                            //Checking target is not null and item is dragging(moving)
                            if ((finalDropEffect == DragDropEffects.Move) && (mTarget != null))
                            {
                                // A Move drop was accepted
                                if (!mDraggedItem.Header.ToString().Equals(mTarget.Header.ToString()))
                                {
                                    CopyItem(mDraggedItem, mTarget);
                                    mTarget = null;
                                    mDraggedItem = null;
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
        private void TreeView_DragOver(object sender, DragEventArgs e)
        {
            try
            {
                if (!mIsSourceObtained)
                {
                    //Use Reflection to identify the source Category from the Treeview object linked to the TreeView_Dragover event
                    // <T>.GetType().GetProperties() method returns all properties for the 'sender' object
                    //PropertyInfo[] myPropertyInfo;
                    //PropertyInfo[] myCatPropertyInfo;
                    //myPropertyInfo = sender.GetType().GetProperties();
                    ////use LINQ to identify "DataContext" property on 'sender' object, to be used for extracting the relevant "CategoryViewModel' object linked to the TreeView item
                    //var parentProperties = from propInf in myPropertyInfo
                    //                      where propInf.Name == "DataContext"
                    //                      select  propInf;
                    var res = ((sender.GetType().GetProperties()).Single(c => c.Name == "DataContext")).GetValue(sender);
                    //myCatPropertyInfo = res.GetType().GetProperties();
                    mSourceCategoryId = (string)((res.GetType().GetProperties()).Single(c => c.Name == "KCategoryId")).GetValue(res);
                    mSourceCategoryName = (string)((res.GetType().GetProperties()).Single(c => c.Name == "ShortName")).GetValue(res);
                    var cursorChanged = Mouse.SetCursor(Cursors.Wait);
                    e.Effects = DragDropEffects.Copy;
                    mIsSourceObtained = true;


                    //Stop event from bubbling up in WPF hiearchy
                    e.Handled = true;

                    ////if ((Math.Abs(currentPosition.X - _lastMouseDown.X) > 10.0) ||
                    ////    (Math.Abs(currentPosition.Y - _lastMouseDown.Y) > 10.0))
                    //{
                    //    // Verify that this is a valid drop and then store the drop target
                    //    TreeViewItem item = GetNearestContainer(e.OriginalSource as UIElement);
                    //    if (CheckDropTarget(draggedItem, item))
                    //    {
                    //        e.Effects = DragDropEffects.Move;
                    //    }
                    //    else
                    //    {
                    //        e.Effects = DragDropEffects.None;
                    //    }
                    //}
                    //e.Handled = true;
                }
            }
            catch (Exception)
            {
            }
        }
        private void TreeView_Drop(object sender, DragEventArgs e)
        {
            try
            {

                Mouse.SetCursor(Cursors.Wait);
                e.Effects = DragDropEffects.Move;
                var res1 = ((sender.GetType().GetProperties()).Single(c => c.Name == "DataContext")).GetValue(sender);
                //myCatPropertyInfo = res.GetType().GetProperties();
                mDestinationCategoryId = (string)((res1.GetType().GetProperties()).Single(c => c.Name == "KCategoryId")).GetValue(res1);
                mDestinationCategoryName = (string)((res1.GetType().GetProperties()).Single(c => c.Name == "ShortName")).GetValue(res1);


                //Asking user wether he want to drop the dragged TreeViewItem here or not
                if (MessageBox.Show("Would you like to drop " + mSourceCategoryName.ToString() + " into " + mDestinationCategoryName.ToString() + "", "Moving Hiearachy Element", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    try
                    {
                        MessageBox.Show("Moving " + mSourceCategoryName.ToString() + " into " + mDestinationCategoryName.ToString() + ", please wait while hierarchy is re-organised", "Moving Hierachy Element");
                    }
                    catch
                    {

                    }
                }




                //Stop event from bubbling up in WPF hiearchy

                e.Effects = DragDropEffects.None;
                mIsSourceObtained = false;
                e.Handled = true;


            }
            catch (Exception)
            {
            }



        }
        private bool CheckDropTarget(TreeViewItem _sourceItem, TreeViewItem _targetItem)
        {
            //Check whether the target item is meeting your condition
            var _isEqual = false;
            if (!_sourceItem.Header.ToString().Equals(_targetItem.Header.ToString()))
            {
                _isEqual = true;
            }
            return _isEqual;

        }
        private void CopyItem(TreeViewItem _sourceItem, TreeViewItem _targetItem)
        {

            //Asking user wether he want to drop the dragged TreeViewItem here or not
            if (MessageBox.Show("Would you like to drop " + _sourceItem.Header.ToString() + " into " + _targetItem.Header.ToString() + "", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    //adding dragged TreeViewItem in target TreeViewItem
                    AddChild(_sourceItem, _targetItem);

                    //finding Parent TreeViewItem of dragged TreeViewItem 
                    var ParentItem = FindVisualParent<TreeViewItem>(_sourceItem);
                    // if parent is null then remove from TreeView else remove from Parent TreeViewItem
                    if (ParentItem == null)
                    {
                        tvParameters.Items.Remove(_sourceItem);
                    }
                    else
                    {
                        ParentItem.Items.Remove(_sourceItem);
                    }
                }
                catch
                {

                }
            }

        }
        public void AddChild(TreeViewItem _sourceItem, TreeViewItem _targetItem)
        {
            // add item in target TreeViewItem 
            var item1 = new TreeViewItem
            {
                Header = _sourceItem.Header
            };
            _targetItem.Items.Add(item1);
            foreach (TreeViewItem item in _sourceItem.Items)
            {
                AddChild(item, item1);
            }
        }

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

    }
}
