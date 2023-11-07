using Fasetto.Word.Core;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static Fasetto.Word.DI;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Fasetto.Word
{
    /// <summary>
    /// Interaction logic for HierarchyManagementControl.xaml
    /// </summary>
    public partial class SWBillingDetailControl : UserControl
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


        public bool TransactionBuildIsRunning { get; set; }
        //private BulkReconDataModel mBulkReconItem;
        //private BulkReconListDataModel mBulkRecon = new BulkReconListDataModel();
        //public BulkReconResultListApiModel mBulkReconApi = new BulkReconResultListApiModel();
        public SWBillingDetailTreeViewModel mSWBillingDetailTreeView;
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
        public SWBillingDetailControl()
        {

            //var root = "1C225789-3938-4480-86CB-071863DC5D33";
            var mReturnSWBillingDetail = (SWBillingDetailTreeViewModel)ViewModelApplication.CurrentPopupViewModel;
            //mSWBillingDetailTreeView = new SWBillingDetailTreeViewModel("5249FFEB-6907-46AA-9204-D4527E11F9CE", mReturnSWBillingDetail.TimeStart, mReturnSWBillingDetail.TimeEnd);
            DataContext = mReturnSWBillingDetail;
            //ViewModelApplication.CurrentPopupViewModel = new SWBillingDetailTreeViewModel();

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
            var SWBillingDetailRec = row.DataContext as SWBillingDetailViewModel;
            //MessageBox.Show($"The timeslot selected is {SWBillingDetailRec.TimeStart}") ;
        }

        private void DataGridRow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
  
                ViewModelApplication.PopupVisible = false;
                var MSelected = (TransactionViewModel)SWBillingDetail.SelectedItem;
                var RawTable = SWBillingDetail.Items.SourceCollection;
                var tempTDList = new ObservableCollection<TransactionViewModel>();
                foreach (var tBR in RawTable)
                    tempTDList.Add((TransactionViewModel)tBR);

                //ViewModelApplication.CurrentPopupContent = PopupContent.AddElement;

                ViewModelApplication.CurrentPopupViewModel = new ManageClassificationViewModel(tempTDList, MSelected);
                ViewModelApplication.CurrentPopupContent = PopupContent.Classify;
                ViewModelApplication.CurrentPageViewModel = ViewModelApplication.CurrentPageViewModel;

                //((SWBillingDetailTreeViewModel)ViewModelApplication.CurrentPopupViewModel).ControlTitle = "Bulk Meter Recon Detail: " + ShortName;
                //var mCurrentPopupViewModel = ViewModelApplication.CurrentPopupViewModel;
                ////New popup is only activated if name differs from current popup (irrespective of view model content)
                //ViewModelApplication.CurrentPopupContent = PopupContent.AddElement;
                //ViewModelApplication.CurrentPopupViewModel =  mCurrentPopupViewModel;
                //ViewModelApplication.CurrentPopupContent = PopupContent.SWBillingDetail;
                ViewModelApplication.PopupVisible = true;

                //var Trans = ((TransactionTreeViewModel)PriorPopup).Trans_action;

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
         //   var tempBR = new ObservableCollection<SWBillingDetailViewModel>();
         //   foreach (var tBR in SWBillingDetail.ItemsSource)
         //       tempBR.Add((SWBillingDetailViewModel)tBR);
         //   var mmBulk = ((SWBillingDetailTreeViewModel)ViewModelApplication.CurrentPopupViewModel).mBulkMeter; //5249ffeb-6907-46aa-9204-d4527e11f9ce
         //   var prematch = tempBR.Where(x => x.BulkMeter == ((SWBillingDetailTreeViewModel)ViewModelApplication.CurrentPopupViewModel).mBulkMeter).ToList();
         //   var mBulkReading = tempBR.Where(x => x.BulkMeter == ((SWBillingDetailTreeViewModel)ViewModelApplication.CurrentPopupViewModel).mBulkMeter).ToList().FirstOrDefault().Volume;
         //   var matches = tempBR.Where(x => x.BulkMeter != ((SWBillingDetailTreeViewModel)ViewModelApplication.CurrentPopupViewModel).mBulkMeter).ToList();
         //   decimal mConsumerReading = 0;
         //   foreach (var category in matches)
         //       mConsumerReading +=category.Volume;
         //   var timeDiff = (((SWBillingDetailTreeViewModel)ViewModelApplication.CurrentPopupViewModel).mTimeEnd - ((SWBillingDetailTreeViewModel)ViewModelApplication.CurrentPopupViewModel).mTimeStart);
         //   var mHrs = (decimal)timeDiff.TotalHours;
         //   if (timeDiff.TotalHours > 24)

         //   { mHrs = (((decimal)(((SWBillingDetailTreeViewModel)ViewModelApplication.CurrentPopupViewModel).mTODEnd - ((SWBillingDetailTreeViewModel)ViewModelApplication.CurrentPopupViewModel).mTODStart)) +1) / 24 * mHrs;
            
         //   };

         //var mDifference = mBulkReading - mConsumerReading;
         //   var mDiscrepancyRate = Math.Round((mDifference / mHrs),2);
         //   MessageBox.Show($"Volume through bulk: {mBulkReading} \n Aggregate consumer volume:  {mConsumerReading}\n Volume difference:  {mDifference}\n Hours of Consumption: {Math.Round(mHrs,2)} \n Mismatch Rate per Hour: {mDiscrepancyRate}");
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
