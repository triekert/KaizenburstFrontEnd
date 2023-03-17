using Fasetto.Word.Core;
using System;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
            //ViewModelApplication.CurrentPopupViewModel = new BulkReconDetailTreeViewModel();

            InitializeComponent();










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
            var bulkReconDetailRec = row.DataContext as BulkReconDetailViewModel;
            MessageBox.Show($"The timeslot selected is {bulkReconDetailRec.TimeStart}") ;
        }

        private void DataGridRow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //_ = (BulkReconViewModel)(BulkRecon.SelectedItems).OrderByDescending(x => x.TimeSlotStart).ToList().FirstOrDefault()).TimeSlotStart;
                ViewModelApplication.PopupVisible = false;

                var  tempBR = new ObservableCollection<BulkReconDetailViewModel>();
                foreach (var tBR in BulkReconDetail.SelectedItems)
                    tempBR.Add((BulkReconDetailViewModel)tBR);
                var TimeStart=tempBR.OrderBy(x=>x.TimeStart).ToList().FirstOrDefault().TimeStart;
                var TimeEnd = tempBR.OrderByDescending(x => x.TimeEnd).ToList().FirstOrDefault().TimeEnd;
                var BulkMeter = tempBR.OrderByDescending(x => x.TimeStart).ToList().FirstOrDefault().BulkMeter;
                var ShortName = tempBR.OrderByDescending(x => x.TimeStart).ToList().FirstOrDefault().ShortName;

                //ViewModelApplication.CurrentPopupContent = PopupContent.AddElement;

                ViewModelApplication.CurrentPopupViewModel = new BulkReconDetailTreeViewModel(BulkMeter, TimeStart, TimeEnd);

                ((BulkReconDetailTreeViewModel)ViewModelApplication.CurrentPopupViewModel).ControlTitle = "Bulk Meter Recon Detail: " + ShortName;
                var mCurrentPopupViewModel = ViewModelApplication.CurrentPopupViewModel;
                //New popup is only activated if name differs from current popup (irrespective of view model content)
                ViewModelApplication.CurrentPopupContent = PopupContent.AddElement;
                ViewModelApplication.CurrentPopupViewModel =  mCurrentPopupViewModel;
                ViewModelApplication.CurrentPopupContent = PopupContent.BulkReconDetail;
                ViewModelApplication.PopupVisible = true;
            }
        }
        private void BulkRecon_OnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            //var row = sender as DataGridRow;
            //var bulkReconRec = row.DataContext as BulkReconViewModel;
            //MessageBox.Show($"The timeslot selected is {bulkReconRec.TimeSlotStart}");
        }
        /// <summary>
        /// When right button is clicked calculate aggregate variance for all meters served by bulk meter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridRow_MouseRightClick(object sender, MouseButtonEventArgs e)
        {
            var tempBR = new ObservableCollection<BulkReconDetailViewModel>();
            foreach (var tBR in BulkReconDetail.ItemsSource)
                tempBR.Add((BulkReconDetailViewModel)tBR);
            var mmBulk = ((BulkReconDetailTreeViewModel)ViewModelApplication.CurrentPopupViewModel).mBulkMeter; //5249ffeb-6907-46aa-9204-d4527e11f9ce
            var prematch = tempBR.Where(x => x.BulkMeter == ((BulkReconDetailTreeViewModel)ViewModelApplication.CurrentPopupViewModel).mBulkMeter).ToList();
            var mBulkReading = tempBR.Where(x => x.BulkMeter == ((BulkReconDetailTreeViewModel)ViewModelApplication.CurrentPopupViewModel).mBulkMeter).ToList().FirstOrDefault().Volume;
            var matches = tempBR.Where(x => x.BulkMeter != ((BulkReconDetailTreeViewModel)ViewModelApplication.CurrentPopupViewModel).mBulkMeter).ToList();
            var mConsumerReading = 0.00;
            foreach (var category in matches)
                mConsumerReading += category.Volume;
            var timeDiff = (((BulkReconDetailTreeViewModel)ViewModelApplication.CurrentPopupViewModel).mTimeEnd - ((BulkReconDetailTreeViewModel)ViewModelApplication.CurrentPopupViewModel).mTimeStart);
            var mHrs =timeDiff.TotalHours;
            var mDifference = mBulkReading - mConsumerReading;
            var mDiscrepancyRate = Math.Round((mDifference / mHrs),2);
            MessageBox.Show($"Volume through bulk: {mBulkReading} \n Aggregate consumer volume:  {mConsumerReading}\n Volume difference:  {mDifference} \n Mismatch Rate per Hour: {mDiscrepancyRate}");
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
