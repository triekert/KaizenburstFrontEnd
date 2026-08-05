using Fasetto.Word.Core;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using static Fasetto.Word.DI;

namespace Fasetto.Word
{
    /// <summary>
    /// This is the view-model of the UI.  It provides a data source
    /// for the TreeView (the FirstGeneration property), a bindable
    /// SearchText property, and the SearchCommand to perform a search.
    /// </summary>
    public class TransactionDetailTreeViewModel : BaseViewModel

    {

        #region Public Properties

        /// <summary>
        /// A set of Bulk Meter Recon records for the selected period
        /// </summary>
        public ObservableCollection<TransactionViewModel>TransactionDetail{ get; set; }
        private object mStocksLock = new object();
        public string ControlTitle { get; set; }
        //public ObservableCollection<HierarchyViewModel> FirstGeneration1 { get; set; }

        #endregion

        #region Data

        //private readonly ReadOnlyCollection<HierarchyViewModel> mFirstGeneration;
        //protected BulkReconViewModel mRootHierarchyElement;
        //protected BulkReconViewModel mRootHierarchyElement1;
        //private readonly ICommand mSearchCommand;
        //public BulkReconListDataModel mBRDML;
        //public BulkReconResultListApiModel mPersist, mPersistTmp,mOriginal;
        //public BulkReconViewModel mBRVM;
        //public ParameterBulkReconApiModel mRequest;
        //public string mBulkMeter;
        //public DateTime mTimeStart;
        //public DateTime mTimeEnd;
        //public int mTODStart;
        //public int mTODEnd;
        /// <summary>
        /// 
        /// Store View Model of current popup to allow reverse navigation
        /// </summary>
        public object PriorPopupViewModel { get; set; }
        //public HierarchyElementViewModel mElement;

        //IEnumerator<HierarchyManagementViewModel> mMatchingCategoryEnumerator;

        //public HierarchyManagementTreeViewModel(IEnumerator<HierarchyManagementViewModel> matchingCategoryEnumerator)
        //{
        //    MatchingCategoryEnumerator = matchingCategoryEnumerator;
        //}

        private string mSearchText = "", mSearchKCategoryID = string.Empty, mParentCategoryID = string.Empty;

        #endregion // Data
        #region Public Commands
        /// <summary>
        /// The command to close the settings menu
        /// </summary>
        public ICommand CloseCommand { get; set; }
        #endregion//Public Commands

        #region Constructor
        /// <summary>
        /// The HierarchyTreeViewModel is a visual inteface for interacting with hiearchical
        /// Structures persisted on the database linked to the application
        /// Generic hierarchy structures with parent-child relationships may be used to represent
        /// appropriate data sets
        /// </summary>
        /// <param name="hierarchyTable"></param>
        /// The hierarchyTable passed through as a paremeter identifies the specific hierarchy set to be retrieved
        /// from persistent s
        public TransactionDetailTreeViewModel(string mKFinTranID)
        {
            #region Build TransactionDetailCollection

            TransactionDetail = new ObservableCollection<TransactionViewModel>();


            //var TDVM = new TransactionDetailViewModel
            //{

            //    ShortName = "Loading...Please be patient",
            //    //TimeSlotStart = new DateTime(2023, 1, 22, 0, 0, 0),
            //    //Missing = 3,
            //    //ChildMeters = 87,
            //    //VolumeIn = 3145.342F,
            //    //VolumeOut = 3215.124F


            //};
            //TransactionDetail.Add(TDVM);

            //var matches = ((TransactionTreeViewModel)ViewModelApplication.CurrentPopupViewModel).Trans_action.Where(x => x.KFinTranID == mKFinTranID).ToList();
            //var matches = ((TransactionTreeViewModel)ViewModelApplication.CurrentPopupViewModel).mPersist.Where(x => x.KFinTranID == mKFinTranID).ToList();

            //foreach (var item in matches)
            //{

            //    var mTDVM = new TransactionViewModel

            //    {
            //        Posted_Date = item.Posted_Date,
            //        Month = item.Month,
            //        Description = item.Description,
            //        TransAmount = item.TransAmount,
            //        ActualAmount = item.ActualAmount,
            //        ShortName = item.ShortName,
            //        KCategoryID = item.KCategoryID,
            //        KFinActualID = item.KFinActualID,
            //        KFinTranID = item.KFinTranID,
            //        KPartyName = item.KPartyName,
            //        KPartyID = item.KPartyID,
            //        FCatSrchID = item.FCatSrchID,
            //        KHierarchyID = item.KHierarchyID,
            //        Notes = item.Notes,
            //        Units = item.Units,


            //    };
            //    TransactionDetail.Add(mTDVM);
            //}
            RefreshTransactionList(mKFinTranID, ((TransactionTreeViewModel)ViewModelApplication.CurrentPopupViewModel).MPersist);
  
            PriorPopupViewModel = ViewModelApplication.CurrentPopupViewModel;
            //TaskManager.RunAndForget(TransactionDetailAsync);



            // Get the OptFinHierarchies currently configured - first populate 'root hierarchy' variable with all configured root hierarchy elements currently available


            //UpdateTreeViewElements();
            CloseCommand = new RelayCommand(Close);
            //mSearchCommand = new SearchCategoryTreeCommand(this);
        }



        #endregion // TransactionDetailCollection

        #endregion // Constructor

        public void RefreshTransactionList(string mKFinTranID, ObservableCollection<TransactionViewModel> mPersist)
        {
            lock (mStocksLock)
            {
                TransactionDetail.Clear();
            }

            //mPersist = result.ServerResponse.Response;
            //if (matches.Count>0)
            var matches = mPersist.Where(x => x.KFinTranID == mKFinTranID).ToList();

            foreach (var item in matches)
            {

                var mTVM = new TransactionViewModel

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
                    KPartyID = item.KPartyID,
                    KPartyName = item.KPartyName,
                    IsChanged = false,
                    FCatSrchID = item.FCatSrchID,
                    KHierarchyID = item.KHierarchyID,
                    IsDocLinked = item.IsDocLinked,
                    Notes = item.Notes,
                    KAccountID = item.KAccountID,
                    KAccountName = item.KAccountName,
                    KProjectID = item.KProjectID,
                    KProjectName = item.KProjectName,
                    KAssetID = item.KAssetID,
                    KAssetName = item.KAssetName,
                    KPersonID = item.KPersonID,
                    KPersonName = item.KPersonName,
                    Units = item.Units,
                    KClientID = item.KClientID,

                };

                //Lock collection to prevent contention with UI


                AddItem(mTVM);

            }


        }

        /// <summary>
        /// This function allows the addition of an TransactionViewModel to the Trans_action collection
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(TransactionViewModel item)
        {
            lock (mStocksLock)
            {
                TransactionDetail.Add(item);

            }
        }


        public void Close()
        {
            var mType = ((TransactionDetailTreeViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel.GetType().Name;           
            ViewModelApplication.CurrentPopupViewModel = ((TransactionDetailTreeViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel;
            var mTransactionDetailTreeViewModel = ViewModelApplication.CurrentPopupViewModel;
            ViewModelApplication.PopupVisible = false;
            //ViewModelApplication.ControlParameter1 = null;

            //Give control back to parent 'Popup view model'


            if (mType == "TransactionDetailTreeViewModel")
            {
                ViewModelApplication.CurrentPopupContent = PopupContent.AddElement;
                ViewModelApplication.CurrentPopupViewModel = mTransactionDetailTreeViewModel;
                ViewModelApplication.CurrentPopupContent = PopupContent.TransactionDetail;
            }
            else
            {

                ViewModelApplication.CurrentPopupContent = PopupContent.Transaction;
            }

            ViewModelApplication.PopupVisible = true;


        }


        ~TransactionDetailTreeViewModel()
        {
            Console.WriteLine("Destructor: Object Destroyed!");
        }

    }

}