using Fasetto.Word.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
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
    public partial class BulkReconDetailControl : UserControl
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


        public bool BulkReconBuildIsRunning { get; set; }
        private BulkReconDataModel mBulkReconItem;
        private BulkReconListDataModel mBulkRecon = new BulkReconListDataModel();
        public BulkReconResultListApiModel mBulkReconApi = new BulkReconResultListApiModel();
        public BulkReconDetailTreeViewModel mBulkReconDetailTreeView;
        //public BulkReconListDataModel mBRDML;
        private string mTableName;


        private readonly HierarchyTreeViewModel mHierarchyTree;
        private string mSourceCategory;
        private string mSourceCategoryName;
        private string mDestinationCategoryID, mDestinationID, mSourceID, mParentID;
        private string mDestinationCategoryName;
        private bool mIsSourceObtained = false, mIsEqual = false;
        private Point mLastMouseDown;
        private TreeViewItem mTargetT, mSource;
        private HierarchyViewModel mDraggedItemTest, mDraggedItem, mTarget;
        //private readonly object mFamilyTree;
        private readonly HierarchyViewModel mTargetTest;
        public string DisplayTitle { get; set; }


        //[Obsolete]
        //public HierarchyManagementControl(HierarchyManagementTreeDataModel hierarchyManagementTreeDataModel)
        public BulkReconDetailControl()
        {

            //var root = "1C225789-3938-4480-86CB-071863DC5D33";
            var mReturnBulkReconDetail = (BulkReconDetailTreeViewModel)ViewModelApplication.CurrentPopupViewModel;
            //mBulkReconDetailTreeView = new BulkReconDetailTreeViewModel("5249FFEB-6907-46AA-9204-D4527E11F9CE", mReturnBulkReconDetail.TimeStart, mReturnBulkReconDetail.TimeEnd);
            DataContext = mReturnBulkReconDetail;
            InitializeComponent();

            //ViewModelApplication.CurrentPopupViewModel = new BulkReconDetailViewModel();








        }

        //public HierarchyControl(string root)
        //{
        //    mHierarchyTree = new HierarchyTreeViewModel(root);//root);

        //    DataContext = mHierarchyTree;
        //    InitializeComponent();
        //    ViewModelApplication.CurrentControlViewModel = mHierarchyTree;   
        //}


        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var row = sender as DataGridRow;
            var bulkReconRec = row.DataContext as BulkReconViewModel;
            MessageBox.Show($"The timeslot selected is {bulkReconRec.TimeSlotStart}") ;
        }

        private void DataGridRow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //_ = (BulkReconViewModel)(BulkRecon.SelectedItems).OrderByDescending(x => x.TimeSlotStart).ToList().FirstOrDefault()).TimeSlotStart;
               var  tempBR = new ObservableCollection<BulkReconViewModel>();
                foreach (var tBR in BulkRecon.SelectedItems)
                    tempBR.Add((BulkReconViewModel)tBR);
                var TimeStart=tempBR.OrderBy(x=>x.TimeSlotStart).ToList().FirstOrDefault().TimeSlotStart;
                var TimeEnd = tempBR.OrderByDescending(x => x.TimeSlotStart).ToList().FirstOrDefault().TimeSlotStart;
                ViewModelApplication.CurrentPopupContent = PopupContent.BulkReconDetail;
            }
        }
        private void BulkRecon_OnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            //var row = sender as DataGridRow;
            //var bulkReconRec = row.DataContext as BulkReconViewModel;
            //MessageBox.Show($"The timeslot selected is {bulkReconRec.TimeSlotStart}");
        }

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



  









    





       






    }
}
