using Fasetto.Word.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Forms.DataVisualization.Charting;

//using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
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
        private int MaxKeyCount = 3;
        private List<Key> PressedKeys = new List<Key>();
        private List<Key> AllowedKeys = new List<Key>();
        private string comboKeys;

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

            //Create dependency property
            //"ItemsSource is a dependency property, so it's easy enough to be notified when the property is changed to something else"
            //ItemsControl Represents a control that can be used to present a collection of items, ItemsSourceProperty is a dependency property which 
            //check this out
            var dpd = DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, typeof(DataGrid));
            dpd?.AddValueChanged(Transaction, ItemsPropertyIsChanged);

        }

        private void ItemsPropertyIsChanged(object sender, EventArgs e)
        {
            if (((TransactionTreeViewModel)ViewModelApplication.CurrentPopupViewModel).Trans_action.Count!= ((TransactionTreeViewModel)ViewModelApplication.CurrentPopupViewModel).mPersist.Count)
            {
                return;
            }
            SelectRowByIndex(Transaction, ((TransactionTreeViewModel)ViewModelApplication.CurrentPopupViewModel).Trans_actionRec);
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
                else
                if (Keyboard.IsKeyDown(Key.PageDown) && (Keyboard.IsKeyDown(Key.RightCtrl) || Keyboard.IsKeyDown(Key.LeftCtrl)))
                {
                    SelectRowByIndex(Transaction, Transaction.Items.Count -1);
                }
                else
                    if (Keyboard.IsKeyDown(Key.PageUp) && (Keyboard.IsKeyDown(Key.RightCtrl) || Keyboard.IsKeyDown(Key.LeftCtrl)))
                    {
                        SelectRowByIndex(Transaction, 0);
                    }
                    else
                        if (Keyboard.IsKeyDown(Key.Up))
                        {
                            SelectRowByIndex(Transaction, (Transaction.SelectedIndex -1 <0)?0:(Transaction.SelectedIndex -1));
                        }
                        else
                            if (Keyboard.IsKeyDown(Key.Down))
                            {
                                SelectRowByIndex(Transaction, (Transaction.SelectedIndex + 1 > Transaction.Items.Count - 1) ? Transaction.Items.Count - 1 : Transaction.SelectedIndex + 1);
                            }
                        else
                            if (Keyboard.IsKeyDown(Key.PageUp))
                            {
                                SelectRowByIndex(Transaction, (Transaction.SelectedIndex - 10 < 0) ? Transaction.Items.Count - 1 : Transaction.SelectedIndex - 10);
                            }
                        else
                            if (Keyboard.IsKeyDown(Key.PageDown))
                            {
                                SelectRowByIndex(Transaction, (Transaction.SelectedIndex + 10 > Transaction.Items.Count - 1) ? Transaction.Items.Count - 1 : Transaction.SelectedIndex + 10);
                            }

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


        private void DataGridRow_OnLoaded(object sender, RoutedEventArgs e)
        {


            //var mTimeStart = tempBR.OrderBy(x => x.TimeSlotStart).ToList().FirstOrDefault().TimeSlotStart;
            //var mTimeEnd = tempBR.OrderByDescending(x => x.TimeSlotStart).ToList().FirstOrDefault().TimeSlotStart.AddMinutes(30);
            //MessageBox.Show($" timeslot ends at {mTimeEnd}", $" The timeslot selected starts at {mTimeStart}");
        }
        private void DataGridRow_OnUnLoaded(object sender, RoutedEventArgs e)
        {
            //    var source = ((DataGridRow)sender).ItemsSource;
            //    var view = (IEditableCollectionView)CollectionViewSource.GetDefaultView(source);
            //    view.CommitEdit();

            //var mTimeStart = tempBR.OrderBy(x => x.TimeSlotStart).ToList().FirstOrDefault().TimeSlotStart;
            //var mTimeEnd = tempBR.OrderByDescending(x => x.TimeSlotStart).ToList().FirstOrDefault().TimeSlotStart.AddMinutes(30);
            //MessageBox.Show($" timeslot ends at {mTimeEnd}", $" The timeslot selected starts at {mTimeStart}");
        }

        private void DataGrid_OnUnLoaded(object sender, RoutedEventArgs e)
    {
        var source = ((DataGrid)sender).ItemsSource;
    var view = (IEditableCollectionView)CollectionViewSource.GetDefaultView(source);
    view.CommitEdit();


            //var mTimeStart = tempBR.OrderBy(x => x.TimeSlotStart).ToList().FirstOrDefault().TimeSlotStart;
            //var mTimeEnd = tempBR.OrderByDescending(x => x.TimeSlotStart).ToList().FirstOrDefault().TimeSlotStart.AddMinutes(30);
            //MessageBox.Show($" timeslot ends at {mTimeEnd}", $" The timeslot selected starts at {mTimeStart}");
        }

        private void DataGrid_OnLoaded(object sender, RoutedEventArgs e)
        {
            var source = ((DataGrid)sender).ItemsSource;
            var view = (IEditableCollectionView)CollectionViewSource.GetDefaultView(source);
            view.CommitEdit();
            SelectRowByIndex(Transaction, ((TransactionTreeViewModel)ViewModelApplication.CurrentPopupViewModel).Trans_actionRec);


            //var mTimeStart = tempBR.OrderBy(x => x.TimeSlotStart).ToList().FirstOrDefault().TimeSlotStart;
            //var mTimeEnd = tempBR.OrderByDescending(x => x.TimeSlotStart).ToList().FirstOrDefault().TimeSlotStart.AddMinutes(30);
            //MessageBox.Show($" timeslot ends at {mTimeEnd}", $" The timeslot selected starts at {mTimeStart}");
        }

        /// when called, this method will determine whether more detail is available for further selection and will either
        /// pass control to the Manage Classification window directly or first display transaction detail allocations made
        /// 
        /// </summary>
        /// 



        /// when called, this method will determine whether more detail is available for further selection and will either
        /// pass control to the Manage Classification window directly or first display transaction detail allocations made
        /// 
        /// </summary>
        /// 

        public static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T t)
                    return t;
                else
                {
                    var childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        public static void SelectRowByIndex(DataGrid dataGrid, int rowIndex)
        {
            if (!dataGrid.SelectionUnit.Equals(DataGridSelectionUnit.FullRow))
                throw new ArgumentException("The SelectionUnit of the DataGrid must be set to FullRow.");

            if (rowIndex < 0 || rowIndex > (dataGrid.Items.Count - 1))
                throw new ArgumentException(string.Format("{0} is an invalid row index.", rowIndex));

            dataGrid.SelectedItems.Clear();
            /* set the SelectedItem property */
            var item = dataGrid.Items[rowIndex]; // = Product X
            dataGrid.SelectedItem = item;

            if (!(dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) is DataGridRow row))
            {
                /* bring the data item (Product object) into view
                 * in case it has been virtualized away */
                dataGrid.ScrollIntoView(item);
                row = dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;
            }
            if (row != null)
            {
                var cell = GetCell(dataGrid, row, 0);
                cell?.Focus();
            }
            //TODO: Retrieve and focus a DataGridCell object
        }

        public static DataGridCell GetCell(DataGrid dataGrid, DataGridRow rowContainer, int column)
        {
            if (rowContainer != null)
            {
                var presenter = FindVisualChild<DataGridCellsPresenter>(rowContainer);
                if (presenter == null)
                {
                    /* if the row has been virtualized away, call its ApplyTemplate() method 
                     * to build its visual tree in order for the DataGridCellsPresenter
                     * and the DataGridCells to be created */
                    rowContainer.ApplyTemplate();
                    presenter = FindVisualChild<DataGridCellsPresenter>(rowContainer);
                }
                if (presenter != null)
                {
                    if (!(presenter.ItemContainerGenerator.ContainerFromIndex(column) is DataGridCell cell))
                    {
                        /* bring the column into view
                         * in case it has been virtualized away */
                        dataGrid.ScrollIntoView(rowContainer, dataGrid.Columns[column]);
                        cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
                    }
                    return cell;
                }
            }
            return null;
        }


    //    public static void SelectRowByIndex(DataGrid dataGrid, int rowIndex)
    //    {
    //        ...
    //DataGridRow row = dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;
    //        ...
    //if (row != null)
    //        {
    //            DataGridCell cell = GetCell(dataGrid, row, 0);
    //            if (cell != null)
    //                cell.Focus();
    //        }
    //    }


        private void NavigateOn()
        {

                var MKFinTranID = ((TransactionViewModel)Transaction.SelectedItem).KFinTranID;
                var RawTable = Transaction.Items;

            ((TransactionTreeViewModel)ViewModelApplication.CurrentPopupViewModel).Trans_actionRec = Transaction.SelectedIndex;

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
                        KHierarchyID = item.KHierarchyID,
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

        private void Datagrid_TargetUpdated(object sender, DataTransferEventArgs e)
        {

        }
    }
}
