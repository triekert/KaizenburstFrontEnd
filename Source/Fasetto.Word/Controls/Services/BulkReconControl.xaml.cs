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
    public partial class BulkReconControl : UserControl
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
        public BulkReconTreeViewModel mBulkReconTreeView;
        //public BulkReconListDataModel mBRDML;
        public string mBulkMeter;


        //private readonly HierarchyTreeViewModel mHierarchyTree;
        //private string mSourceCategory;
        //private string mSourceCategoryName;
        //private string mDestinationCategoryID, mDestinationID, mSourceID, mParentID;
        //private string mDestinationCategoryName;
        //private bool mIsSourceObtained = false, mIsEqual = false;
        //private Point mLastMouseDown;
        //private TreeViewItem mTargetT, mSource;
        //private HierarchyViewModel mDraggedItemTest, mDraggedItem, mTarget;
        ////private readonly object mFamilyTree;
        //private readonly HierarchyViewModel mTargetTest;
        public string DisplayTitle { get; set; }


        //[Obsolete]
        //public HierarchyManagementControl(HierarchyManagementTreeDataModel hierarchyManagementTreeDataModel)
        public BulkReconControl()
        {

            //var root = "1C225789-3938-4480-86CB-071863DC5D33";
            mBulkReconTreeView = new BulkReconTreeViewModel("5249FFEB-6907-46AA-9204-D4527E11F9CE", DateTime.Now.AddDays(-1), DateTime.Now);
            mBulkMeter = " - Tre Donne Estate Main Feed";
            DataContext = mBulkReconTreeView;
            ((BulkReconPageViewModel)ViewModelApplication.CurrentPageViewModel).DisplayTitle = ((BulkReconPageViewModel)ViewModelApplication.CurrentPageViewModel).DisplayTitle+mBulkMeter;




            InitializeComponent();
            mBulkReconTreeView.mBulkMeter = "5249FFEB-6907-46AA-9204-D4527E11F9CE";
            ViewModelApplication.CurrentControlViewModel = mBulkReconTreeView;

            //CloseCommand = new RelayCommand(Close);


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
                var mTimeStart = tempBR.OrderBy(x => x.TimeSlotStart).ToList().FirstOrDefault().TimeSlotStart;
                var mTimeEnd = tempBR.OrderByDescending(x => x.TimeSlotStart).ToList().FirstOrDefault().TimeSlotStart.AddMinutes(30);

                ViewModelApplication.CurrentPopupViewModel = new BulkReconDetailTreeViewModel(mBulkReconTreeView.mBulkMeter, mTimeStart, mTimeEnd);



                ViewModelApplication.CurrentPopupContent = PopupContent.BulkReconDetail;

                
                ViewModelApplication.PopupVisible = true;

            }
        }

    }
}
