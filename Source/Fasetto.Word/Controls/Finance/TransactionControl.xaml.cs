using Fasetto.Word.Core;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static Fasetto.Word.DI;

namespace Fasetto.Word
{
    /// <summary>
    /// Interaction logic for HierarchyManagementControl.xaml
    /// </summary>
    public partial class TransactionControl : UserControl
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
        private TransactionDataModel mTransactionItem;
        private TransactionListDataModel mTransaction = new TransactionListDataModel();
        public TransactionResultListApiModel mTransactionApi = new TransactionResultListApiModel();
        public TransactionTreeViewModel mTransactionTreeView;
        //public TransactionListDataModel mBRDML;
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
        public TransactionControl()
        {

            //var root = "1C225789-3938-4480-86CB-071863DC5D33";
            mTransactionTreeView = (TransactionTreeViewModel)ViewModelApplication.CurrentPopupViewModel;
            //mBulkMeter = "Tre Donne Estate Main Feed";
            DataContext = mTransactionTreeView;
            //((TransactionPageViewModel)ViewModelApplication.CurrentPageViewModel).DisplayTitle = ((TransactionPageViewModel)ViewModelApplication.CurrentPageViewModel).DisplayTitle + mBulkMeter;

            InitializeComponent();
            //mTransactionTreeView.mBulkMeter = "5249ffeb-6907-46aa-9204-d4527e11f9ce";
            //ViewModelApplication.CurrentControlViewModel = mTransactionTreeView;

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
            var TransactionRec = row.DataContext as TransactionViewModel;
            NavigateOn();
            //MessageBox.Show($"The timeslot selected is {TransactionRec.TimeSlotStart}", $"The timeslot selected is {TransactionRec.TimeSlotStart}");
        }

        private void TreeView_KeyBoard(object sender, KeyboardEventArgs e)
        {
            //check to determine whether user would like to add an item to the hierarchy

            if (Keyboard.IsKeyDown(Key.Escape))
            { }

               
        }


        private void DataGridRow_KeyDown(object sender, KeyEventArgs e)
        
        {
            if (e.Key == Key.Enter)
            {
                NavigateOn();
            }
            else
                if (e.Key == Key.F2)
                { }
            e.Handled = true;
        }
        private void DataGridRow_MouseRightClick(object sender, MouseButtonEventArgs e)
        {
            var tempT = new ObservableCollection<TransactionViewModel>();
            foreach (var tT in Transaction.ItemsSource)
                tempT.Add((TransactionViewModel)tT);

            //var mTimeStart = tempBR.OrderBy(x => x.TimeSlotStart).ToList().FirstOrDefault().TimeSlotStart;
            //var mTimeEnd = tempBR.OrderByDescending(x => x.TimeSlotStart).ToList().FirstOrDefault().TimeSlotStart.AddMinutes(30);
            //MessageBox.Show($" timeslot ends at {mTimeEnd}", $" The timeslot selected starts at {mTimeStart}");
        }


        /// when called, this method will determine whether more detail is available for further selection and will either
        /// pass control to the Manage Classification window directly or first display transaction detail allocations made
        /// 
        /// </summary>

        private void NavigateOn()
        {

                var MKFinTranID = ((TransactionViewModel)Transaction.SelectedItem).KFinTranID;
                var RawTable = Transaction.Items;

                ViewModelApplication.PopupVisible = false;
            ViewModelApplication.CurrentPopupViewModel = new TransactionDetailTreeViewModel(MKFinTranID);
            ((TransactionDetailTreeViewModel)ViewModelApplication.CurrentPopupViewModel).ControlTitle = "Financial Transaction Allocation ";

            //If only one allocation linked to the Transaction, bypass the 'detail' window...

            if (((TransactionDetailTreeViewModel)ViewModelApplication.CurrentPopupViewModel).TransactionDetail.Count > 1)
            {                 //((TransactionDetailTreeViewModel)ViewModelApplication.CurrentPopupViewModel).ControlTitle = "Bulk Meter Recon Detail: " + ShortName;
                ViewModelApplication.PopupVisible = false;
                //ViewModelApplication.CurrentPopupContent = Null;
                ViewModelApplication.CurrentPopupContent = PopupContent.TransactionDetail;
                ViewModelApplication.PopupVisible = true;
            }
            else
            {            //((TransactionDetailTreeViewModel)ViewModelApplication.CurrentPopupViewModel).ControlTitle = "Bulk Meter Recon Detail: " + ShortName;
                ViewModelApplication.PopupVisible = false;
                //ViewModelApplication.CurrentPopupContent = Null;
                ViewModelApplication.CurrentPopupContent = PopupContent.TransactionDetail;

                var MSelected = new TransactionViewModel();

                var TransactionDetail = new ObservableCollection<TransactionViewModel>();
                //(TransactionViewModel)(TransactionDetail.SelectedItem;
                var matches = ((TransactionTreeViewModel)((TransactionDetailTreeViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel).Trans_action.Where(x => x.KFinTranID == MKFinTranID).ToList();

                foreach (var item in matches)
                {

                    var mTDVM = new TransactionViewModel

                    {
                        Posted_Date = item.Posted_Date,
                        Month = item.Month,
                        Description = item.Description,
                        TransAmount = item.TransAmount,
                        ActualAmount = item.ActualAmount,
                        ShortName = item.ShortName,
                        KCategoryID = item.KCategoryID,
                        KFinActualID = item.KFinActualID,
                        KFinTranID = item.KFinTranID,
                        KPartyName = item.KPartyName,
                        KPartyID = item.KPartyID,
                        FCatSrchID = item.FCatSrchID,
                    };
                    TransactionDetail.Add(mTDVM);
                }


                //var RawTable = ((ObservableCollection<TransactionViewModel>)((TransactionDetailTreeViewModel)(ViewModelApplication.CurrentPopupViewModel)).TransactionDetail).Items;
                var tempTDList = new ObservableCollection<TransactionViewModel>();
                foreach (var tBR in RawTable)
                    tempTDList.Add((TransactionViewModel)tBR);
                ViewModelApplication.CurrentPopupViewModel = new ManageClassificationViewModel(TransactionDetail, TransactionDetail[0]);

                //ViewModelApplication.CurrentPopupContent = PopupContent.Classify;
                ViewModelApplication.CurrentPageViewModel = ViewModelApplication.CurrentPageViewModel;
                ViewModelApplication.CurrentPopupContent = PopupContent.Classify;
                ViewModelApplication.PopupVisible = true;

            }
        }
    }
}
