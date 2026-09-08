
using CsvHelper;
using Dna;
using Fasetto.Word.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using static Fasetto.Word.Core.CoreDI;
using static Fasetto.Word.DI;

namespace Fasetto.Word
{
    /// <summary>
    /// This is the view-model of the UI.  It provides a data source
    /// for the TreeView (the FirstGeneration property), a bindable
    /// SearchText property, and the SearchCommand to perform a search.
    /// </summary>
    public class TransactionTreeViewModel : BaseViewModel

    {

        #region Public Properties

        /// <summary>
        /// A set of Bulk Meter Recon records for the selected period
        /// </summary>
        public ObservableCollection<TransactionViewModel> Trans_action { get; set; }
        public ObservableCollection<TransactionViewModel> OrgTransaction { get; set; }
        public ObservableCollection<TransactionViewModel> MPersist { get; set; }

        /// <summary>
        /// Place holder for storing selected item when calling transaction detail
        /// </summary>
        public int TransSelector { get; set; }

        public int Trans_actionRec { get; set; }
        //public ObservableCollection<HierarchyViewModel> FirstGeneration1 { get; set; }
        /// <summary>
        /// A flag indicating if the login command is running
        /// </summary>
        public bool TransClassBuildIsRunning { get; set; }

        /// <summary>
        /// Create placeholder for Datagrid used in View
        /// </summary>
        public DataGrid Transaction { get; set; }

        /// <summary>
        /// Lock for updating observable collection data
        /// </summary>
        private object mStocksLock = new object();


        /// <summary>
        /// API model for retrieving transaction data

        /// </summary>
        public ParameterTransactionApiModel mRequest { get; set; }


        #endregion

        #region Data

        //private readonly ReadOnlyCollection<HierarchyViewModel> mFirstGeneration;
        //protected TransactionViewModel mRootHierarchyElement;
        //protected TransactionViewModel mRootHierarchyElement1;
        //private readonly ICommand mSearchCommand;
        public TransactionListDataModel mTDML;

        public TransactionResultListApiModel mChange;

        public TransactionViewModel mTVM;
        //public ParameterTransactionApiModel mRequest;
        public string mClient;
        public int mMonthStart;
        public int mMonthEnd;
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
        public ICommand GestureHandlerCommand { get; set; }

        //public ActionCommand<DragEventArgs> DropCommand { get; private set; }


        #region Constructor
        /// <summary>
        /// The HierarchyTreeViewModel is a visual interface for interacting with hierarchical
        /// Structures persisted on the database linked to the application
        /// Generic hierarchy structures with parent-child relationships may be used to represent
        /// appropriate data sets
        /// </summary>
        /// <param name="hierarchyTable"></param>
        /// The hierarchyTable passed through as a parameter identifies the specific hierarchy set to be retrieved
        /// from persistent s
        //public TransactionTreeViewModel(string client, DateTime timeStart, DateTime timeEnd, string category, string budget)
        public TransactionTreeViewModel(ParameterTransactionApiModel MRequest)
        {
            #region Build HierarchyViewCollection
            mRequest = MRequest;

            Trans_action = new ObservableCollection<TransactionViewModel>();
            BindingOperations.EnableCollectionSynchronization(Trans_action, mStocksLock);


            mTVM = new TransactionViewModel
            {

                ShortName = "Loading...Please be patient",
                //TimeSlotStart = new DateTime(2023, 1, 22, 0, 0, 0),
                //Missing = 3,
                //ChildMeters = 87,
                //VolumeIn = 3145.342F,
                //VolumeOut = 3215.124F


            };
            Trans_action.Add(mTVM);

            //mRequest = new ParameterTransactionApiModel
            //{
            //    Client = client,
            //    MonthStart = int.Parse(timeStart.ToString("yyyyMMdd")),
            //    MonthEnd = int.Parse(timeEnd.ToString("yyyyMMdd")),
            //    Category = category,
            //    Budget = budget,
            //};

            //var MMmonth = timeStart.ToString("MM");
            //mTableName = hierarchyTable;
            #endregion
            //retrieve hierarchy from persistent storage on server
            //To Do: Add mTableName as parameter when calling HierarchyAsync to populate hierarchy

            //mTODStart = TODStart;
            //mTODEnd = TODEnd;
            PriorPopupViewModel = ViewModelApplication.CurrentPopupViewModel;
            TaskManager.RunAndForget(TransactionAsync);


            // Get the OptFinHierarchies currently configured - first populate 'root hierarchy' variable with all configured root hierarchy elements currently available

            Transaction = new DataGrid();
            //UpdateTreeViewElements();

            CloseCommand = new RelayCommand(Close);
            GestureHandlerCommand = new DelegateCommand<ContextualEventArgs>(GestureHandler);
            //mSearchCommand = new SearchCategoryTreeCommand(this);
        }





        #endregion // Constructor

        #region Properties
        #region Public Properties



        /// <summary>
        /// A flag indicating if the login command is running
        /// </summary>
        public bool TransactionBuildIsRunning { get; set; }

        /// <summary>
        /// Title to be published on Control
        /// </summary>
        public string ControlTitle { get; set; }
        //{get => mTableName;
        //    set
        //    {
        //        if (value == mTableName)
        //            return;

        //        mTableName = value;

        //    } }
        #endregion//Public Properties



        #region SearchCommand

        /// <summary>
        /// Returns the command used to execute a search in the Category tree.
        /// </summary>
        //public ICommand SearchCommand => mSearchCommand;

        //private class SearchCategoryTreeCommand : ICommand
        //{
        //    private readonly TransactionTreeViewModel mCategoryTree;

        //    public SearchCategoryTreeCommand(TransactionTreeViewModel CategoryTree)
        //    {
        //        mCategoryTree = CategoryTree;
        //    }

        //    public bool CanExecute(object parameter)
        //    {
        //        return true;
        //    }

        //    event EventHandler ICommand.CanExecuteChanged
        //    {
        //        // I intentionally left these empty because
        //        // this command never raises the event, and
        //        // not using the WeakEvent pattern here can
        //        // cause memory leaks.  WeakEvent pattern is
        //        // not simple to implement, so why bother.
        //        add { }
        //        remove { }
        //    }

        //    public void Execute(object parameter)
        //    {
        //        mCategoryTree.PerformSearch();
        //    }
        //}

        #endregion // SearchCommand

        #endregion //Properties

        /// <summary>
        /// Return Hierarchy of interest from Object persistence infrastructure
        /// User credentials are used to determine access authorisation
        /// </summary>
        /// <returns></returns>
        public async Task TransactionAsync()
        {
            await RunCommandAsync(() => TransactionBuildIsRunning, async () =>
            {

                // Store single transcient instance of client data store
                var scopedClientDataStore = ClientDataStore;
                //
                //return;
                //

                // Update values from local cache
                // Get the user token
                                var token = ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).Token;
                // Call the server and attempt to register with the provided credentials
                // If we don't have a token (then not logged in...)
                if (string.IsNullOrEmpty(token))
                    // Then do nothing more
                    return;
                var result = await WebRequests.PostAsync<ApiResponse<TransactionResultListApiModel>>(
                // Set URL
                    RouteHelpers.GetAbsoluteRoute(ApiRoutes.ReturnTransaction),
                    mRequest,
                    bearerToken: token);

                // If the response has an error...
                if (await result.HandleErrorIfFailedAsync("Transaction retrieval Failed"))
                    // We are done
                    return;

                // OK successfully registered (and logged in)... now get appropriate tree view data
                //for now; keep a snapshot of persisted data
                //mOriginal = result.ServerResponse.Response;

                ;

                try
                {
                    //var hierarchyResultApiModels = mOriginal.ToList();
                    //make a clone of the persisted data for manipulation on front end
                    //mPersist = new TransactionResultListApiModel();
                    //mPersist.Clone(mOriginal, mPersist);
                    //Transaction.Clear();
                    //lock (mStocksLock) {

                    //    Trans_action = new ObservableCollection<TransactionViewModel>();
                    //}

                    //BindingOperations.EnableCollectionSynchronization(Trans_action, mStocksLock);
                    OrgTransaction = new ObservableCollection<TransactionViewModel>();



                    mChange = new TransactionResultListApiModel();




                    MPersist = new ObservableCollection<TransactionViewModel>();
                    var matches = result.ServerResponse.Response.OrderByDescending(x => x.Posted_Date).ThenBy(x => x.KFinTranID).ThenBy(x => x.ShortName).ToList();
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
                            KClientID = item.KClientID,
                            KAccountID = item.KAccountID,
                            KAccountName = item.KAccountName,
                            KPersonID = item.KPersonID,
                            KPersonName = item.KPersonName,
                            KAssetID = item.KAssetID,
                            KAssetName = item.KAssetName,
                            KProjectID = item.KProjectID,
                            KProjectName = item.KProjectName,
                            Units = item.Units,
                        };

                        //Lock collection to prevent contention with UI

                        MPersist.Add(mTVM);
                    }


                    RefreshTransactionList();
                    //Clone(Trans_action, OrgTransaction);

                    Trans_actionRec = 0;

                    //if (Trans_action.Count != mPersist.Count)
                    //{ 
                    //};


                }
                catch (Exception e)
                {
                    throw e;
                }


            });
        }


        #region SearchText

        /// <summary>
        /// Gets/sets a fragment of the name to search for.
        /// </summary>
        public string SearchText
        {
            get => mSearchText;
            set
            {
                if (value == mSearchText)
                    return;

                mSearchText = value;

                MatchingCategoryEnumerator = null;
            }
        }

        public IEnumerator<TransactionViewModel> MatchingCategoryEnumerator { get; private set; }

        #endregion // SearchText

        //#endregion // Properties



        #region Search Logic //KCategoryID
        public IEnumerator<TransactionViewModel> MatchingKCategoryEnumerator { get; private set; }

        #endregion // SearchKCategoryID



        public void Close()
        {
            // Close settings menu

            //Call API to persist changes if any...

            var UpPersist = ((TransactionTreeViewModel)ViewModelApplication.CurrentPopupViewModel).Trans_action;
            var OPersist = ((TransactionTreeViewModel)ViewModelApplication.CurrentPopupViewModel).OrgTransaction;
            //create record set of CRUD members of mPersist based on changes appearing in Trans_act -> Delete, Add and change flags



            if (ViewModelApplication.CurrentPageViewModel.GetType().Name == "BudgetSelectionPageViewModel" || ViewModelApplication.CurrentPageViewModel.GetType().Name == "ExpenditureVSBudgetPageViewModel")
            {
                if (ViewModelApplication.CurrentPageViewModel.GetType().Name == "BudgetSelectionPageViewModel")
                {
                    ViewModelApplication.CurrentPopupViewModel = PriorPopupViewModel;
                    ViewModelApplication.CurrentPopupContent = PopupContent.BudgetReview;
                    ViewModelApplication.CurrentPopupViewModel = PriorPopupViewModel;// ((BudgetAdjustViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel;
                    ViewModelApplication.PopupVisible = true;
                }
                else
                {
                    ViewModelApplication.CurrentPopupViewModel = PriorPopupViewModel;
                    if (ViewModelApplication.CurrentPopupViewModel.GetType().Name == "ExpenditureAdjustViewModel")

                    {

                        ViewModelApplication.CurrentPopupContent = PopupContent.ExpenditureAdjust;
                        ViewModelApplication.PopupVisible = true;
                    }
                    else
                    {

                        ViewModelApplication.CurrentPopupContent = PopupContent.ExpenditureReview;
                        ViewModelApplication.CurrentPopupViewModel = PriorPopupViewModel;// ((BudgetAdjustViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel;
                        ViewModelApplication.PopupVisible = true;
                    }
                }
            }
            else
            {
                if (ViewModelApplication.CurrentPageViewModel.GetType().Name == "TransactionSelectionPageViewModel")
                {
                    ViewModelApplication.PopupVisible = false;
                    ViewModelApplication.CurrentControlViewModel = ((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).Client;
                }
                else
                {
                    ViewModelApplication.PopupVisible = false;
                    ViewModelApplication.CurrentControlViewModel = ((TransactionSelectionAnalysisPageViewModel)ViewModelApplication.CurrentPageViewModel).Client;
                }
                if (UpPersist == null || OPersist == null) { return; }
                var except = UpPersist.Except(OPersist);
                //TO DO: Map PopupViewModel to PopupContent with converter
                //ViewModelApplication.CurrentPopupContent = PopupContent.HierarchyItemSelection;
                var tmpTbl = (TransactionResultListApiModel)((TransactionTreeViewModel)ViewModelApplication.CurrentPopupViewModel).mChange;
                var res = from c in tmpTbl
                          group c by new { c.KFinActualID, c.ChangeType } into transApi
                          select transApi.OrderByDescending(x => x.DateEffective)
                                          .FirstOrDefault();



                ViewModelApplication.CurrentPopupViewModel = ((TransactionTreeViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel;

            }


        }

        public async Task PersistTransClassAsync()
        {
            await RunCommandAsync(() => TransClassBuildIsRunning, async () =>
            {

                // Store single transcient instance of client data store
                var scopedClientDataStore = ClientDataStore;

                // Update values from local cache
                // Get the user token
                                var token = ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).Token;
                // Call the server and attempt to register with the provided credentials
                // If we don't have a token (then not logged in...)
                if (string.IsNullOrEmpty(token))
                    // Then do nothing more
                    return;
                var result = await WebRequests.PostAsync<ApiResponse<TransactionResultListApiModel>>(
                // Set URL
                    RouteHelpers.GetAbsoluteRoute(ApiRoutes.PersistClassification),
                    mChange,
                    bearerToken: token);

                // If the response has an error...
                if (await result.HandleErrorIfFailedAsync("Classification Update Failed"))
                    // We are done
                    return;
                mChange.Clear();

                // return to menu


            });
        }





        /// <summary>
        /// This function allows the addition of an TransactionViewModel to the Trans_action collection
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(TransactionViewModel item)
        {
            lock (mStocksLock)
            {
                Trans_action.Add(item);

            }
        }


        /// <summary>
        /// This function allows the removal of a TransactionViewModel from the Trans_action collection
        /// </summary>
        /// <param name="item"></param>
        public void RemoveItem(TransactionViewModel item)
        {
            lock (mStocksLock)
            {
                Trans_action.Remove(item);

            }
        }



        /// <summary>
        /// This function removes items from the target list included in the source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public void Remove(ObservableCollection<TransactionViewModel> source, ObservableCollection<TransactionViewModel> target)
        {
            foreach (var item in source)
                target.Remove(item);
        }

        /// <summary>
        /// This method will make a clone of the source List of objects
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public void Clone(ObservableCollection<TransactionViewModel> source, ObservableCollection<TransactionViewModel> target)
        {
            foreach (var item in source)
            {
                var mTR = new TransactionViewModel
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
                    KHierarchyID = item.KHierarchyID,
                    DateEffective = item.DateEffective,
                    KPartyID = item.KPartyID,
                    KPartyName = item.KPartyName,
                    FCatSrchID = item.FCatSrchID,
                    IsTemplate = item.IsTemplate,
                    Notes = item.Notes,
                    KClientID = item.KClientID,
                    KPersonID = item.KPersonID,
                    KPersonName = item.KPersonName,
                    KAssetID = item.KAssetID,
                    KAssetName = item.KAssetName,
                    KProjectID = item.KProjectID,
                    KProjectName = item.KProjectName,
                };
                target.Add(mTR);
            }

        }

        public void RefreshTransactionList()
        {
            lock (mStocksLock)
            {
                Trans_action.Clear();
            }
            var mTest = MPersist.GroupBy(x => x.KFinTranID)
            .Select(g => g.First()).ToList();
            var matches = mTest.OrderByDescending(x => x.Posted_Date).ThenBy(x => x.KFinTranID).ThenBy(x => x.ShortName).ToList();
            //mPersist = result.ServerResponse.Response;
            //if (matches.Count>0)

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
                    Units = item.Units,
                    KClientID = item.KClientID,
                    KPersonID = item.KPersonID,
                    KPersonName = item.KPersonName,
                    KAssetID = item.KAssetID,
                    KAssetName = item.KAssetName,
                    KProjectID = item.KProjectID,
                    KProjectName = item.KProjectName,
                };

                //Lock collection to prevent contention with UI


                AddItem(mTVM);

            }
        }


        //Interpret Keyboard Gestures
        public void GestureHandler(object parameter)
        {
            var tmp = ((ContextualEventArgs)parameter).OriginalEventArgs;
            var eventTmp = tmp.GetType().Name;
            //Get datagrid from relevent event args
            if (eventTmp == "KeyEventArgs")
            {
                Transaction = ((KeyEventArgs)tmp).Source as DataGrid;
             }
            else
            if (eventTmp == "MouseEventArgs")
            {
                Transaction = ((MouseEventArgs)tmp).Source as DataGrid;
            }
            else
            if (eventTmp == "MouseButtonEventArgs")
            {
                Transaction = ((MouseButtonEventArgs)tmp).Source as DataGrid;
            }

            if (eventTmp == "MouseEventArgs" && ((MouseEventArgs)tmp).RoutedEvent.Name == "PreviewMouseMove")
            {
                var TmpTmp = ((MouseEventArgs)tmp).OriginalSource as UIElement;


                ((MouseEventArgs)tmp).Handled = true;
            }
            else
            {
                var tmp1 = ((ContextualEventArgs)parameter).Context.GetType().Name;


                if (ViewModelApplication.SideMenuVisible && ViewModelApplication.CurrentPopupViewModel == null)
                //if (mTableName == "2D7E4A7D-6F19-496E-8709-47E6A9ADDFA0")

                {
                    //if Gesture handler is triggered from Text Search Box...               
                    if (tmp1 == "String")
                    {
                        SearchText = SearchText;
                        if (((KeyEventArgs)tmp).Key == Key.Enter)
                        { }
                        //((KeyEventArgs)tmp).Handled = true;
                        //{ SearchCommand.Execute(null); }
                    }
                    else
                    {

                        //mSelectedTreeItem = (HierarchyViewModel)(((ContextualEventArgs)parameter).Context);
                        //ViewModelApplication.SideMenuVisible = true;

                        if (eventTmp == "MouseButtonEventArgs")
                        {
                            if ((((MouseButtonEventArgs)tmp).RightButton == MouseButtonState.Pressed) || (((MouseButtonEventArgs)tmp).LeftButton == MouseButtonState.Pressed))
                            {
                                ((MouseButtonEventArgs)tmp).Handled = true;
                                //RunSelectedMenu();
                            }
                        }
                        else
                        if (eventTmp == "KeyEventArgs")
                        {
                            if ((((KeyEventArgs)tmp).Key == Key.Enter) || (((KeyEventArgs)tmp).Key == Key.Insert) || (((KeyEventArgs)tmp).Key == Key.Delete))
                            {
                                ((KeyEventArgs)tmp).Handled = true;
                                //RunSelectedMenu();
                            }
                            ((KeyEventArgs)tmp).Handled = true;
                        }
                        //}
                    }
                }
                else
                //enable editing of hierarchy menu structure
                //if Gesture handler is triggered from Text Search Box...
                //

                {

                    if (tmp1 == "String")
                    {
                        SearchText = SearchText;
                        if (((KeyEventArgs)tmp).Key == Key.Enter)
                        { }
                        //((KeyEventArgs)tmp).Handled = true;
                        //{ SearchCommand.Execute(null); }
                    }
                    else
                    {
                        //mSelectedTreeItem = (HierarchyViewModel)(((ContextualEventArgs)parameter).Context);
                        //ViewModelApplication.SideMenuVisible = true;
                        if (eventTmp == "MouseButtonEventArgs")
                        {
                            if ((((MouseButtonEventArgs)tmp).RightButton == MouseButtonState.Pressed) || (((MouseButtonEventArgs)tmp).LeftButton == MouseButtonState.Pressed))
                            {
                                ((MouseButtonEventArgs)tmp).Handled = true;
                                NavigateOnAsync(Transaction);
                                //EditHierarchyElement(mSelectedTreeItem);
                            }
                        }
                        else
                        if (eventTmp == "KeyEventArgs")

                        //Edit element
                        {


                                //var Transaction = ((KeyEventArgs)tmp).Source as DataGrid;
                                //var ttype = Transaction.GetType().Name;
                                if (((KeyEventArgs)tmp).Key == Key.Enter)
                                {
                                    ((KeyEventArgs)tmp).Handled = true;
                                    NavigateOnAsync(Transaction);
                                    //EditHierarchyElement(mSelectedTreeItem);
                                }
                                else
                                    if (((KeyEventArgs)tmp).Key == Key.Insert)
                                    {
                                        ((KeyEventArgs)tmp).Handled = true;
                                        Insert();
                                    }
                                    else
                                        if (((KeyEventArgs)tmp).Key == Key.F2)
                                        {
                                            ((KeyEventArgs)tmp).Handled = true;
                                            Generate();
                                        }
                                        else
                                            if (((KeyEventArgs)tmp).Key == Key.F3)
                                            {
                                                ((KeyEventArgs)tmp).Handled = true;
                                                LookupMain();
                                            }
                                            else
                                                if (((KeyEventArgs)tmp).Key == Key.PageDown & ((KeyboardDevice)((KeyEventArgs)tmp).Device).Modifiers == ModifierKeys.Control)
                                                {

                                                    SelectRowByIndex(Transaction, Transaction.Items.Count - 1);
                                                    ((KeyEventArgs)tmp).Handled = true;
                                                }
                                                else
                                                    if (((KeyEventArgs)tmp).Key == Key.PageUp & ((KeyboardDevice)((KeyEventArgs)tmp).Device).Modifiers == ModifierKeys.Control)
                                                    {

                                                        SelectRowByIndex(Transaction, 0);
                                                        ((KeyEventArgs)tmp).Handled = true;
                                                    }
                                                    else
                                                        if ((((KeyEventArgs)tmp).Key == Key.Down))
                                                        {
                                                            SelectRowByIndex(Transaction, (Transaction.SelectedIndex + 1 > Transaction.Items.Count - 1) ? Transaction.Items.Count - 1 : Transaction.SelectedIndex + 1);
                                                            ((KeyEventArgs)tmp).Handled = true;
                                                        }
                                                    else
                                                        if ((((KeyEventArgs)tmp).Key == Key.Up))
                                                        {
                                                                SelectRowByIndex(Transaction, (Transaction.SelectedIndex - 1 < 0) ? 0 : (Transaction.SelectedIndex - 1));
                                                                ((KeyEventArgs)tmp).Handled = true;
                                                        }
                                                                else




                                                                {
                                                                    var VisibleRows = 0;
                                                                    foreach (var Item in Transaction.Items)
                                                                    {
                                                                        var Row = (DataGridRow)Transaction.ItemContainerGenerator.ContainerFromItem(Item);

                                                                        if (Row != null)
                                                                        {
                                                                            if (Row.TransformToVisual(Transaction).Transform(new Point(0, 0)).Y + Row.ActualHeight > Transaction.ActualHeight)
                                                                            {
                                                                                break;
                                                                            }

                                                                            VisibleRows++;
                                                                        }

                                                                    }
                                                                VisibleRows--;
                                                                    if ((((KeyEventArgs)tmp).Key == Key.PageUp))
                                                                    {
                                                                        SelectRowByIndex(Transaction, (Transaction.SelectedIndex - VisibleRows <= 0) ? 0 : Transaction.SelectedIndex - VisibleRows);
                                                                        ((KeyEventArgs)tmp).Handled = true;
                                                                    }
                                                                    else
                                                                        if ((((KeyEventArgs)tmp).Key == Key.PageDown))
                                                                        {
                                                                            SelectRowByIndex(Transaction, (Transaction.SelectedIndex + VisibleRows > Transaction.Items.Count - 1) ? Transaction.Items.Count - 1 : Transaction.SelectedIndex + VisibleRows);
                                                                            ((KeyEventArgs)tmp).Handled = true;
                                                                        }

                                                                    else
                                                                        if (((KeyEventArgs)tmp).Key == Key.F4)
                                                                        {
                                                                            ((KeyEventArgs)tmp).Handled = true;
                                                                            ViewModelApplication.CurrentPopupContent = 0;
                                                                            ViewModelApplication.CurrentPopupContent = PopupContent.Transaction;
                                                                            var mSelectedIndex = ((TransactionTreeViewModel)ViewModelApplication.CurrentPopupViewModel).Transaction.SelectedIndex;
                                                                            //Generate();
                                                                        }
                                                            }


                            }
                        //((KeyEventArgs)tmp).Handled = true;
                    }

                }
            }
        }


        //}
        private async void NavigateOnAsync(DataGrid Transaction)
        {

            var MKFinTranID = ((TransactionViewModel)Transaction.SelectedItem).KFinTranID;
            var RawTable = Transaction.Items;
            var Merge = Transaction.SelectedItems;

            if (Merge.Count == 2)
            {
                //If 2 items have been selected, merge the first transaction with the second,moving all the allocations from the second to the first
                //and deleting the second transaction thereafter
                //var matches = Merge.
                if (((TransactionViewModel)Merge[0]).TransAmount != ((TransactionViewModel)Merge[1]).TransAmount || ((TransactionViewModel)Merge[0]).KAccountID != ((TransactionViewModel)Merge[1]).KAccountID)
                {
                    //if transaction totals differ, or if the account is different, they cannot be merged
                    System.Windows.MessageBox.Show($"Only transactions having the same transaction value and Account Name may be merged!");
                    return;
                }
                //Confirm with user that 2 transactions are to be merged irreversibly...

                if (MessageBox.Show("Merging of Transactions - Irreversible!", "Confirm",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {


                    foreach (var item in Merge)
                    {
                        var u = new TransactionResultApiModel
                        {
                            Posted_Date = ((TransactionViewModel)item).Posted_Date,
                            Month = ((TransactionViewModel)item).Month,
                            Description = ((TransactionViewModel)item).Description,
                            TransAmount = ((TransactionViewModel)item).TransAmount,
                            ActualAmount = ((TransactionViewModel)item).ActualAmount,
                            ShortName = ((TransactionViewModel)item).ShortName,
                            KCategoryID = ((TransactionViewModel)item).KCategoryID,
                            KFinActualID = ((TransactionViewModel)item).KFinActualID,
                            KFinTranID = ((TransactionViewModel)item).KFinTranID,
                            KPartyName = ((TransactionViewModel)item).KPartyName,
                            KPartyID = ((TransactionViewModel)item).KPartyID,
                            FCatSrchID = ((TransactionViewModel)item).FCatSrchID,
                            KHierarchyID = ((TransactionViewModel)item).KHierarchyID,
                            KAccountID = ((TransactionViewModel)item).KAccountID,
                            KAccountName = ((TransactionViewModel)item).KAccountName,
                            KProjectName = ((TransactionViewModel)item).KProjectName,
                            KProjectID = ((TransactionViewModel)item).KProjectID,
                            KAssetName = ((TransactionViewModel)item).KAssetName,
                            KAssetID = ((TransactionViewModel)item).KAssetID,
                            KPersonName = ((TransactionViewModel)item).KPersonName,
                            KPersonID = ((TransactionViewModel)item).KPersonID,
                            Notes = ((TransactionViewModel)item).Notes,
                            Units = ((TransactionViewModel)item).Units,
                            ChangeType = "m",
                            KClientID = ((TransactionViewModel)item).KClientID,
                        };

                        ((TransactionTreeViewModel)ViewModelApplication.CurrentPopupViewModel).mChange.Add(u);
                    }
                    await ((TransactionTreeViewModel)ViewModelApplication.CurrentPopupViewModel).PersistTransClassAsync();
                    ((TransactionTreeViewModel)ViewModelApplication.CurrentPopupViewModel).mChange.Clear();


                }
                else
                {
                    return;
                }

            }
            else
            if (Merge.Count == 1)
            {
                ((TransactionTreeViewModel)ViewModelApplication.CurrentPopupViewModel).Trans_actionRec = Transaction.SelectedIndex;

                ViewModelApplication.PopupVisible = false;

                    ViewModelApplication.CurrentPopupViewModel = new TransactionDetailTreeViewModel(MKFinTranID);
                ((TransactionDetailTreeViewModel)ViewModelApplication.CurrentPopupViewModel).ControlTitle = "Financial Transaction Allocation ";


                    //If only one allocation linked to the Transaction, bypass the 'detail' window...

                    if (((TransactionDetailTreeViewModel)ViewModelApplication.CurrentPopupViewModel).TransactionDetail.Count > 1)
                {                 //((TransactionDetailTreeViewModel)ViewModelApplication.CurrentPopupViewModel).ControlTitle = "Bulk Meter Recon Detail: " + ShortName;
                    ViewModelApplication.PopupVisible = false;
                        ViewModelApplication.CurrentPopupContent = PopupContent.TransactionDetail;
                        ViewModelApplication.PopupVisible = true;

                        //ViewModelApplication.CurrentPopupViewModel = ((TransactionDetailTreeViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel;
                        //ViewModelApplication.CurrentPopupContent = PopupContent.Transaction;
                        //ViewModelApplication.PopupVisible = true;
                        //return;




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
                            KAccountID = item.KAccountID,
                            KAccountName = item.KAccountName,
                            Notes = item.Notes,
                            Units = item.Units,
                            KClientID = item.KClientID,
                            KPersonID = item.KPersonID,
                            KPersonName = item.KPersonName,
                            KAssetID = item.KAssetID,
                            KAssetName = item.KAssetName,
                            KProjectID = item.KProjectID,
                            KProjectName = item.KProjectName,
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




        /// <summary>
        /// Add a new Transaction based on the selected existing transaction
        /// </summary>
        private void Insert()
        {

            var NewTransaction = new TransactionViewModel()
            {
                KFinTranID = "00000000-0000-0000-0000-000000000001",
                KFinActualID = Guid.NewGuid().ToString().ToUpper(),
                Posted_Date = ((TransactionViewModel)Transaction.SelectedItem).Posted_Date,
                KHierarchyID = ((TransactionViewModel)Transaction.SelectedItem).KHierarchyID,
                Month = ((TransactionViewModel)Transaction.SelectedItem).Month,
                KClientID = ((TransactionViewModel)Transaction.SelectedItem).KClientID,
                ActualAmount = -1M,
            };
            ((TransactionTreeViewModel)ViewModelApplication.CurrentPopupViewModel).AddItem(NewTransaction);
            ((TransactionTreeViewModel)ViewModelApplication.CurrentPopupViewModel).MPersist.Add(NewTransaction);

            Transaction.SelectedItem = NewTransaction;

            //((TransactionTreeViewModel)ViewModelApplication.CurrentPopupViewModel).Trans_action.SelectedItem = (TransactionViewModel)Transaction[Transaction.Items.Count()];

            var RawTable = Transaction.Items;


            NavigateOnAsync(Transaction);
        }

        /// <summary>
        /// Generate CSV file from selection of transactions
        /// </summary>
        private void Generate()
        {

            var fileName = @"C:\Temp\Transaction Records "
            //+
            //    ((SWBillingPageViewModel)ViewModelApplication.CurrentPageViewModel).SelectedBillingPeriod.TimeStart.ToString("d_MM_yyyy")
            //+ " TO " + ((SWBillingPageViewModel)ViewModelApplication.CurrentPageViewModel).SelectedBillingPeriod.TimeEnd.ToString("d_MM_yyyy")
            + ".csv";
            try
            {
                using (var writer = new StreamWriter(fileName))
                {
                    using (var csvOut = new CsvWriter(writer, CultureInfo.InvariantCulture))
                    {
                        csvOut.WriteRecords(((TransactionTreeViewModel)ViewModelApplication.CurrentPopupViewModel).MPersist);
                    }
                }
            }
            catch (Exception exp)
            {
                Console.Write(exp.Message);
            }
        }



        private void LookupMain()
        {

            var fileName = @"C:\Temp\Transaction Records "
    //+
    //    ((SWBillingPageViewModel)ViewModelApplication.CurrentPageViewModel).SelectedBillingPeriod.TimeStart.ToString("d_MM_yyyy")
    //+ " TO " + ((SWBillingPageViewModel)ViewModelApplication.CurrentPageViewModel).SelectedBillingPeriod.TimeEnd.ToString("d_MM_yyyy")
    + ".csv";
            try
            {
                using (var writer = new StreamWriter(fileName))
                {
                    using (var csvOut = new CsvWriter(writer, CultureInfo.InvariantCulture))
                    {
                        csvOut.WriteRecords(((TransactionTreeViewModel)ViewModelApplication.CurrentPopupViewModel).MPersist);
                    }
                }
            }
            catch (Exception exp)
            {
                Console.Write(exp.Message);
            }
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

    }
}