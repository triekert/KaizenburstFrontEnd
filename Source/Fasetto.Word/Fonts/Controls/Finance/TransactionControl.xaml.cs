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
            //MessageBox.Show($"The timeslot selected is {TransactionRec.TimeSlotStart}", $"The timeslot selected is {TransactionRec.TimeSlotStart}");
        }

        private void DataGridRow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //_ = (TransactionViewModel)(Transaction.SelectedItems).OrderByDescending(x => x.TimeSlotStart).ToList().FirstOrDefault()).TimeSlotStart;
                //ViewModelApplication.PopupVisible = false;
                //ViewModelApplication.CurrentPopupContent = PopupContent.AddElement;
                var MKFinTranID= ((TransactionViewModel)Transaction.SelectedItem).KFinTranID;
                var RawTable = Transaction.Items;

                //var tempst = cellInfos[0].Column.Header;
                //var tempBR = new ObservableCollection<TransactionViewModel>();
                //
                //    //foreach (var tBR in Transaction.SelectedItems)
                //    tempBR.Add((TransactionViewModel)tBR.Item);


                //var tempT = new ObservableCollection<TransactionViewModel>();
                //var tempT = new TransactionViewModel();

                ViewModelApplication.CurrentPopupViewModel = new TransactionDetailTreeViewModel(MKFinTranID);
                ((TransactionDetailTreeViewModel)ViewModelApplication.CurrentPopupViewModel).ControlTitle = "Financial Transaction Allocation " ;

                //((TransactionDetailTreeViewModel)ViewModelApplication.CurrentPopupViewModel).ControlTitle = "Bulk Meter Recon Detail: " + ShortName;
                ViewModelApplication.PopupVisible = false;
                //ViewModelApplication.CurrentPopupContent = Null;
                ViewModelApplication.CurrentPopupContent = PopupContent.TransactionDetail;
                ViewModelApplication.PopupVisible = true;



            }
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
    }
}
