using Dna;
using Fasetto.Word.Core;
using Fasetto.Word.Core.ApiModels.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using static Fasetto.Word.Core.CoreDI;
using static Fasetto.Word.DI;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;


namespace Fasetto.Word
{
    public partial class ManageClassificationViewModel : BaseViewModel
    {
        #region Private Members

        /// <summary>
        /// The text to show while loading text
        /// </summary>
        private string mLoadingText = "...";

        #endregion

        #region Public Properties


        /// <summary>
        /// Field containing notes added to the transaction allocation (a transaction may have one or more allocations linked)
        /// </summary>
        public TextEntryViewModel TransactionNotes { get; set; }

        /// <summary>
        /// Field containing description of the transaction
        /// </summary>
        public TextEntryViewModel TransactionDescription { get; set; }

        /// <summary>
        /// Field containing actual allocation  of the total transaction to the selected cost category
        /// </summary>
        public TextEntryViewModel Allocation { get; set; }

        /// <summary>
        /// The Cost category to be used for the allocation
        /// </summary>
        public HierarchyItemSelectionViewModel Category { get; set; }

        /// <summary>
        /// The Party to be linked for the allocation
        /// </summary>
        public HierarchyItemSelectionViewModel Party { get; set; }


        public string KCategoryID { get; set; }

        /// <summary>
        /// Parent ID  of hiearchy item
        /// </summary>
        public string ParentCategoryID { get; set; }

        /// <summary>
        /// Parent ShortName of hiearchy item
        /// </summary>
        public string ParentShortName { get; set; }

        /// <summary>
        /// Calendar date from which Element is seen as active
        /// </summary>
        public DateTime DateAdjustment  { get; set; }

        /// <summary>
        /// Calendar date from which Element is deactivated
        /// </summary>
        public DateTime DateStart { get; set; }


        /// <summary>
        /// Calendar date from which Element is deactivated
        /// </summary>
        public DateTime DateEnd { get; set; }

        /// <summary>
        /// Attach the current activity to a Change object
        /// </summary>
        public string KChangeID { get; set; }

        /// <summary>
        /// The text for the add Node button
        /// </summary>
        public string AddClassificationButtonText { get; set; }
        /// <summary>
        /// The text for the add Node button
        /// </summary>
        public string AlterTemplateButtonText { get; set; }
        /// <summary>
        /// The text for the Edit Node button
        /// </summary>
        public string EditNodeButtonText { get; set; }
        /// <summary>
        /// The text for the Delete Node button
        /// </summary>
        public string DeleteNodeButtonText { get; set; }
        /// <summary>
        /// The text for the Copy Node button
        /// </summary>
        public string CopyNodeButtonText { get; set; }
        /// <summary>
        /// The text for the Move Node button
        /// </summary>
        public string MoveNodeButtonText { get; set; }

        /// <summary>
        /// The text for the control heading
        /// </summary>
        /// 
        public string HeadingText { get; set; }

        /// <summary>
        /// The date of the transaction
        /// </summary>
        /// 
        public string TransactionDate { get; set; }

        /// <summary>
        /// The text for the full transaction description
        /// </summary>
        /// 
        public string TransactionDetail { get; set; }


        /// <summary>
        /// A document, including images etc, linked to the transaction
        /// </summary>
        public DocDataViewModel Document { get; set; }

        /// <summary>
        /// A document, including images etc, linked to the transaction
        /// </summary>
        public ObservableCollection<DocDataViewModel> DocumentList { get; set; }


        /// <summary>
        /// String representation of GUID for linked document
        /// </summary>
        public string KDocID { get; set; }

        /// <summary>
        ///  Bool set true if template to be modified
        /// </summary>
        public bool IsTemplate { get; set; }


        /// <summary>
        /// Populate parameters for retrieval of required hierarchy tree
        /// </summary>
        public ParameterHierarchyItemSelectApiModel HierarchyParam { get; set; }

        ///// <summary>
        /////  Image of  Doc linked to Transaction
        ///// </summary>
        //public byte[] DocImage { get; set; }


        ///// <summary>
        /////  URL of  Doc linked to Transaction
        ///// </summary>
        //public string DocURL { get; set; }


        /// <summary>
        /// API parameter model
        /// </summary>
        /// 
        public ParameterBillingAdjustmentApiModel MAPI { get; set; }

        /// <summary>
        /// True to show the hierarchy retrieval command is running
        /// </summary>
        public bool SetHierarchyCompleted { get; set; }


        /// <summary>
        /// The action to run when saving the text.
        /// Returns true if the commit was successful, or false otherwise.
        /// </summary>
        public Func<Task<bool>> CommitAction { get; set; }


        /// <summary>
        /// A flag indicating if the document retrieval command is running
        /// </summary>
        public bool DocumentRetrievalIsRunning { get; set; }



        /// <summary>
        /// A flag indicating if the document storage  command is running
        /// </summary>
        public bool DocumentStorageIsRunning { get; set; }

        /// <summary>
        /// A flag indicating if the cost category selection is complete
        /// </summary>
        public bool SelectCategoryCompleted { get; set; }


        /// <summary>
        /// A flag indicating if the party selection is complete
        /// </summary>
        public bool SelectPartyCompleted { get; set; }


        #region Transactional Properties

        /// <summary>
        /// Indicates if the Cost Hierarchy Search is currently being loaded
        /// </summary>
        public bool CostClassificationIsSaving { get; set; }

        /// <summary>
        /// Indicates if the node is being saved
        /// </summary>
        public bool NodeSaving { get; set; }


        /// <summary>
        /// Indicates if the settings details are currently being loaded
        /// </summary>
        public bool SettingsLoading { get; set; }

        /// <summary>
        /// Indicates if the user is currently logging out
        /// </summary>
        public bool LoggingOut { get; set; }

        /// <summary>
        /// A flag indicating if the task is running
        /// </summary>
        public bool IsRunning { get; set; }

        /// <summary>
        /// Store View Model of current popup to allow reverse navigation
        /// </summary>
        public object PriorPopupViewModel { get; set; }

        /// <summary>
        /// All allocations linked to the selected transaction
        /// </summary>
        public ObservableCollection<TransactionViewModel> Source { get; }

        /// <summary>
        /// The selected allocation
        /// </summary>
        public TransactionViewModel Selected { get; }

        /// <summary>
        /// The selected allocation
        /// </summary>
        public TransactionViewModel Selected1 { get; }

        /// <summary>
        /// The selected allocation
        /// </summary>
        //public TransactionDetailViewModel New1 { get; set; }

        public DocDataResultListApiModel mRequest;

        #endregion

        #endregion

        #region Public Commands


        /// <summary>
        /// The command to close the settings menu
        /// </summary>
        public ICommand CloseCommand { get; set; }

        /// <summary>
        /// The command to add a new classification and allocation to the transaction
        /// If the allocation exceeds the 'unprocessed' balance, it will be taken from the remaining classification
        /// </summary>
        public ICommand AddClassificationCommand { get; set; }
        /// <summary>
        /// The command to edit an existing classification (and allocation) of a transaction
        /// If the amount allocated differs from the unallocated amount and one other classification remains, the balance is allocated to that one
        /// otherwise the balance is allocated to 'unprocessed'
        /// </summary>
        public ICommand AlterTemplateCommand { get; set; }

         /// <summary>
        /// The command to remove a classification from the transaction, if only one classification remains the previous allocation reverts to that classification
        /// otherwise the amount is allocated to 'unprocessed'
        /// </summary>
        public ICommand DeleteClassificationCommand { get; set; }


        /// <summary>
        /// The command to search a new image for loading into the image window
        /// </summary>
        public ICommand BrowseImageCommand { get; set; }



        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public ManageClassificationViewModel(ObservableCollection<TransactionViewModel> source, TransactionViewModel selected)
        {
            //Retrieve any documents from file server on the web server
            //If no documents linked previously, add dummy fields to facilitate selection
            //mRequest = new DocDataResultListApiModel();
            //var matches = selected.Document.ToList();

            //foreach (var item in selected.Document)
            //{

            //}
            Selected = selected;
            Selected.Document = new DocDataViewModel()

            {
            DocURL = "\\somepath\\filename.jpg",
            KDocID = "00000000 - 0000 - 0000 - 0000 - 000000000000",
            DocName = "Name of Document.",
            DocDescription = "Name of Document in plain language"
            };
            //Document = selected.Document;
            //if (selected.Document !=null)
            //{ 
            mRequest = new DocDataResultListApiModel();
            var mRqst = new DocDataResultApiModel
            {
                FFintranID = selected.Document.FFintranID,
                DocImage = selected.Document.DocImage,
                DocName = selected.Document.DocName,
                DocURL = selected.Document.DocURL,
                KDocID = selected.Document.KDocID,
                DocDescription = selected.Document.DocDescription
            };
            mRequest.Add(mRqst);

            TaskManager.RunAndForget(DocumentRetrievalAsync);
            DocumentList = new ObservableCollection<DocDataViewModel>()
            { Selected.Document};

            TransactionNotes = new TextEntryViewModel
            {
                Label = "Notes linked to transaction (allocation)",
                OriginalText = selected.Notes,
                EditedText = selected.Notes,
                //CommitAction = SaveFirstNameAsync
            };

            TransactionDescription = new TextEntryViewModel
            {
                Label = "Transaction Description",
                OriginalText = selected.Description,
                //CommitAction = SaveFirstNameAsync
            };


            // Create Node Name
            Allocation = new TextEntryViewModel
            {
                Label = "Allocation",
                OriginalText = selected.ActualAmount.ToString("C", CultureInfo.CurrentCulture),
                //CommitAction = SaveFirstNameAsync
            };

            Category = new HierarchyItemSelectionViewModel
            {
                Label = "Select Cost Category",
                //EditedName = mLoadingText,
                EditedName = "Selected Category",
                OriginalName = selected.ShortName,
                OriginalKid = selected.KCategoryID,
                EditedKid = null,
                ClientID = ((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).Client.EditedKid,
                HierarchyID = ((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy.EditedKid,
                //HierarchyTypeID = "64413ae7-822f-4866-9ebe-433083d699ac",
                RootID = ((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy.EditedKid,
                PrepareAction = SetCostCategorySelectionAsync,
                PriorPopupViewModel = ViewModelApplication.CurrentPopupViewModel,

                //HierarchyID = ((CostHierarchyViewModel)((CostHierarchyListViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy).MSelectedCostHierarchy).KCategoryID,

                CommitAction = SelectCategoryAsync,           
            };
            Party = new HierarchyItemSelectionViewModel
            {
                Label = "Select Linked Party",
                //EditedName = mLoadingText,
                EditedName = "Selected Party",
                OriginalKid = selected.KPartyID,
                OriginalName = selected.KPartyName,
                EditedKid = null,
                ClientID = ((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).Client.EditedKid,
                HierarchyTypeID = "ADEEBB16-F553-48F8-955F-663227A4886C",
                PrepareAction = SetPartyHierarchySelectionAsync,
                //HierarchyTypeID = ((CostHierarchyListViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy).f,
                //HierarchyID = ((CostHierarchyViewModel)((CostHierarchyListViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy).MSelectedCostHierarchy).KCategoryID,
                PriorPopupViewModel = ViewModelApplication.CurrentPopupViewModel,
                CommitAction = SelectPartyAsync,
            };



            TransactionDate = (selected.Posted_Date).ToString();

            TransactionDetail = selected.Description;

            HeadingText = "Manage classification of selected Transaction";

            Document = selected.Document;


            // Create commands
            CloseCommand = new RelayCommand(Close);
            AddClassificationCommand = new RelayCommand(AddClassification);
            AlterTemplateCommand = new RelayCommand(AlterTemplate);
            DeleteClassificationCommand = new RelayCommand(AddClassification);
            BrowseImageCommand = new RelayCommand(BrowseImage);

            // TODO: Get from localization
            AddClassificationButtonText = "Update Transaction Classification:";
            AlterTemplateButtonText = "Alter Classification Template:";
            Source = source;

            Selected1= new TransactionViewModel();
            PriorPopupViewModel = ViewModelApplication.CurrentPopupViewModel;
        }
        //private void TreeView_KeyBoard(object sender, KeyboardEventArgs e)
        //{
        //    //check to determine whether user would like to add an item to the hierarchy

        //    if (Keyboard.IsKeyDown(Key.Escape))
        //    {
        //        Close();

        //    }
        //    e.Handled = true;
        //}

        #endregion

        public async Task DocumentRetrievalAsync()
        {
            await RunCommandAsync(() => DocumentRetrievalIsRunning, async () =>
            {

                // Store single transient instance of client data store
                var scopedClientDataStore = ClientDataStore;
                //
                //return;
                //

                // Update values from local cache
                // Get the user token
                var token = (await scopedClientDataStore.GetLoginCredentialsAsync())?.Token;
                // Call the server and attempt to register with the provided credentials
                // If we don't have a token (then not logged in...)
                if (string.IsNullOrEmpty(token))
                    // Then do nothing more
                    return;
                var result = await WebRequests.PostAsync<ApiResponse<DocDataResultListApiModel>>(
                // Set URL
                    RouteHelpers.GetAbsoluteRoute(ApiRoutes.ReturnDocument),
                    Selected.KFinTranID,
                    bearerToken: token);

                // If the response has an error...
                if (await result.HandleErrorIfFailedAsync("Transaction retrieval Failed"))
                    // We are done
                    return;

                // OK successfully registered (and logged in)... 
                //If data returned, modify data on classification view model

                if (result.ServerResponse.Response.Count > 0)
                {



                    try
                    {
                        DocumentList = new ObservableCollection<DocDataViewModel>();
                        foreach (var doc in result.ServerResponse.Response)
                        {
                            var mRqst = new DocDataViewModel
                            {
                                DocImage = doc.DocImage,
                                DocName = doc.DocName,
                                DocURL = doc.DocURL,
                                KDocID = doc.KDocID,
                                DocDescription = doc.DocDescription,
                                FFintranID = doc.FFintranID
                            };

                            Selected.Document = mRqst;
                            Document = mRqst;
                            DocumentList.Add(mRqst);

                        }


                     }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }

            });
        }


        public async Task DocumentStorageAsync()
        {
            await RunCommandAsync(() => DocumentStorageIsRunning, async () =>
            {

                // Store single transient instance of client data store
                var scopedClientDataStore = ClientDataStore;
                //
                //return;
                //

                // Update values from local cache
                // Get the user token
                var token = (await scopedClientDataStore.GetLoginCredentialsAsync())?.Token;
                // Call the server and attempt to register with the provided credentials
                // If we don't have a token (then not logged in...)
                if (string.IsNullOrEmpty(token))
                    // Then do nothing more
                    return;
                var result = await WebRequests.PostAsync<ApiResponse<DocDataResultApiModel>>(
                // Set URL
                    RouteHelpers.GetAbsoluteRoute(ApiRoutes.AddDocument),
                    mRequest,
                    bearerToken: token);

                // If the response has an error...
                if (await result.HandleErrorIfFailedAsync("Failed to add documents on web server"))
                    // We are done
                    return;


                ////((TransactionTreeViewModel)ViewModelApplication.CurrentPopupViewModel).Trans_action

                ;

                try
                {



                }
                catch (Exception e)
                {
                    throw e;
                }


            });
        }

        #region Command Methods

        /// <summary>
        /// Open the settings menu
        /// </summary>
        //public void Open()
        //{
        //    // Close settings menu
        //    ViewModelApplication.PopupVisible = true;
        //}

        /// <summary>
        /// Closes the settings menu
        /// </summary>
        public void Close()
        {
            // Close Classification Window, passing control back to transaction detail windows
            // If a transaction has been split amongst different cost categories, first navigate to the "split" level before progressing further
            //var mHierarchyBillingTreeViewModel = ViewModelApplication.CurrentPopupViewModel;
            //

            ViewModelApplication.CurrentPopupViewModel = ((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel;
            var mKFinTranID = ((TransactionDetailTreeViewModel)ViewModelApplication.CurrentPopupViewModel).TransactionDetail[0].KFinTranID;

            var matches = ((TransactionTreeViewModel)((TransactionDetailTreeViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel).Trans_action.Where(x => x.KFinTranID == mKFinTranID).ToList();
            var Cnt = matches.Count;
            ViewModelApplication.ControlParameter1 = null;

            if (Cnt ==1)
            {
                ViewModelApplication.CurrentPopupViewModel = ((TransactionDetailTreeViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel;
                //ViewModelApplication.CurrentPopupViewModel = ((TransactionDetailTreeViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel;
                ViewModelApplication.CurrentPopupContent = PopupContent.Transaction;
            }
            else
            { 

                    ViewModelApplication.CurrentPopupContent = PopupContent.TransactionDetail;
            }
            ViewModelApplication.PopupVisible = true;

        }

        public async Task<bool> SetCostCategorySelectionAsync()
        {
            // Lock this command to ignore any other requests while processing

            return await RunCommandAsync(() => SetHierarchyCompleted, async () =>
            {
                // Update the Category Classification value on the server...

                ViewModelApplication.CurrentControlViewModel = ((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).Category;
                //ViewModelApplication.ControlParameter1 = ((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).Category;
                HierarchyParam = new ParameterHierarchyItemSelectApiModel
                {

                    Level = 100,
                    RootID =ViewModelApplication.FCostHierarchyID,
                };
                ViewModelApplication.CurrentPopupViewModel = new HierarchyTreeViewModel1(HierarchyParam);

                return true;
            });

        }
        public async Task<bool> SetPartyHierarchySelectionAsync()
        {
            // Lock this command to ignore any other requests while processing

            return await RunCommandAsync(() => SetHierarchyCompleted, async () =>
            {
                // Update the Party value on the server...

                ViewModelApplication.CurrentControlViewModel = ((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).Party;
                HierarchyParam = new ParameterHierarchyItemSelectApiModel
                {

                    Level = 0,
                    ClientID = ViewModelApplication.FClientID,
                    HierarchyTypeID = Party.HierarchyTypeID,
                };
                ViewModelApplication.CurrentPopupViewModel = new HierarchyTreeViewModel1(HierarchyParam);

                //ViewModelApplication.ControlParameter1 = ((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).Party;
                return true;
            });

        }
        




        /// <summary>
        /// Initialises the Cost Hierarchy Search
        /// </summary>
        /// <returns>Returns true if successful, false otherwise</returns>
        public async Task<bool> SelectCategoryAsync()
        {
            // Lock this command to ignore any other requests while processing


            return await RunCommandAsync(() => SelectCategoryCompleted, async () =>
            {

                ViewModelApplication.CurrentPopupViewModel = ViewModelApplication.CurrentPopupViewModel;
                Category.OriginalName = Category.EditedName;
                if (ViewModelApplication.ControlParameter1 != null)
                {
                    ViewModelApplication.CurrentPopupViewModel = ViewModelApplication.ControlParameter1;
                    ViewModelApplication.ControlParameter1 = null;
                    ViewModelApplication.CurrentPopupContent = PopupContent.Classify;
                    ViewModelApplication.PopupVisible = true;
                }
                //((ManageClassificationViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy = new CostHierarchyListViewModel(Root.EditedKid)
                //{
                //    MSelectedCostHierarchy = new CostHierarchyViewModel()
                //};
                //ViewModelApplication.CurrentControlViewModel = ((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy;
                return true;
            });
        }

        /// <summary>
        /// Initialises the Cost Hierarchy Search
        /// </summary>
        /// <returns>Returns true if successful, false otherwise</returns>
        public async Task<bool> SelectPartyAsync()
        {
            // Lock this command to ignore any other requests while processing


            return await RunCommandAsync(() => SelectPartyCompleted, async () =>
            {

                ViewModelApplication.CurrentPopupViewModel = ViewModelApplication.CurrentPopupViewModel;
                Party.OriginalName = Party.EditedName;
                if (ViewModelApplication.ControlParameter1 != null)
                {
                    ViewModelApplication.CurrentPopupViewModel = ViewModelApplication.ControlParameter1;
                    ViewModelApplication.ControlParameter1 = null;
                    ViewModelApplication.CurrentPopupContent = PopupContent.Classify;
                    ViewModelApplication.PopupVisible = true;
                }
                //((ManageClassificationViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy = new CostHierarchyListViewModel(Root.EditedKid)
                //{
                //    MSelectedCostHierarchy = new CostHierarchyViewModel()
                //};
                //ViewModelApplication.CurrentControlViewModel = ((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy;
                return true;
            });
        }
        /// <summary>
        /// Used tp insert a new node with the currently selected node as parent
        /// </summary>
        public void AlterTemplate()
        {
            //set flag to allow template update
            IsTemplate = true;
            AddClassification();
        }

            /// <summary>
            /// Used tp insert a new node with the currently selected node as parent
            /// </summary>
            public void AddClassification()
        {


        //if new document has been linked, copy to file server on web server
        //if (!(Selected.Document == null || !Selected.Document.IsNew))

        //{

            var docs = ((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).DocumentList.Where(x => (x.IsNew || x.IsRemove) && x.KDocID != "00000000-0000-0000-0000-000000000000").ToList();
            if (docs.Count>0)
            { 
                mRequest = new DocDataResultListApiModel();

                foreach  (var item in docs)
                { 

                    var mRqst = new DocDataResultApiModel
                    {
                        DocImage = item.DocImage,
                        DocName = item.DocName,
                        DocURL = item.DocURL,
                        KDocID = item.KDocID,
                        DocDescription = item.DocDescription,
                        FFintranID = item.FFintranID,
                        IsNew = item.IsNew,
                        IsRemove =item.IsRemove,
                    };
                    mRequest.Add(mRqst);
                }
                    var docsl= mRequest.Where(x => x.IsNew  && x.KDocID != "00000000-0000-0000-0000-000000000000").ToList();

                    if (docsl.Count >0)
                    {
                    var tmp = ((TransactionDetailTreeViewModel)((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel).PriorPopupViewModel;
                    var tmp1 = ((TransactionTreeViewModel)tmp).Trans_action;
                    var tmp2 = ((TransactionTreeViewModel)tmp).Trans_actionRec;
                    var rec = tmp1[tmp2];
                    rec.IsDocLinked = true;


                    Selected.IsDocLinked = true;
                    Selected1.IsDocLinked = true;
                };
                TaskManager.RunAndForget(DocumentStorageAsync);

             }
        //}
    

            var OrgActual = Selected.ActualAmount;


            Selected1.Posted_Date = Selected.Posted_Date;

            Selected1.Month = Selected.Month;
            Selected1.Description = Selected.Description;
            Selected1.TransAmount = Selected.TransAmount;
            Selected1.ActualAmount = Selected.ActualAmount;
            Selected1.ShortName = Selected.ShortName;
            Selected1.KCategoryID = Selected.KCategoryID;
            Selected1.KFinActualID = Selected.KFinActualID;
            Selected1.KFinTranID = Selected.KFinTranID;
            Selected1.KPartyID = Selected.KPartyID;
            Selected1.KPartyName = Selected.KPartyName;
            Selected1.FCatSrchID = Selected.FCatSrchID;
            Selected1.KHierarchyID = Selected.KHierarchyID;
            Selected1.Notes = Selected.Notes;

            decimal.TryParse(Allocation.EditedText??Allocation.OriginalText, NumberStyles.Currency, CultureInfo.CurrentCulture, out var IntAmnt);
            if (Category.EditedName == "Selected Category")
                { Category.EditedName = Category.OriginalName;
                    Category.EditedKid = Category.OriginalKid;
                }
            if (Party.EditedName == "Selected Party")
            {
                Party.EditedName = Party.OriginalName;
                Party.EditedKid = Party.OriginalKid;
            }
            var TstNotes = false;
            //if (Math.Abs(IntAmnt) == Math.Abs(Selected.ActualAmount)) { TstEqual = true; }
            if (Math.Abs(IntAmnt) > Math.Abs(Selected.ActualAmount)) { IntAmnt = Selected.ActualAmount; }
            if ((TransactionNotes.EditedText != null) && (Selected.Notes == null || TransactionNotes.EditedText != Selected.Notes))
            {
                TstNotes = true;
                Selected.Notes = Selected1.Notes = TransactionNotes.EditedText;
            }
            if (Category.EditedKid != Category.OriginalKid || IntAmnt != OrgActual || Party.EditedKid != Party.OriginalKid || TstNotes)
                //Don't do anything if cost category hasn't changed, the allocated amount has not changed, OR the linked party has not changed
            {
                var tmp0 = ((TransactionDetailTreeViewModel)((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel).PriorPopupViewModel;
                var tmp = ((TransactionTreeViewModel)tmp0).Trans_action;
                var tmp1 = ((TransactionDetailTreeViewModel)((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel).TransactionDetail;
                var tmp2 = ((TransactionTreeViewModel)((TransactionDetailTreeViewModel)((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel).PriorPopupViewModel).mChange;
                var tmp3 = ((TransactionTreeViewModel)tmp0).Trans_actionRec;
                var rec = tmp[tmp3];
                rec.Notes =TransactionNotes.EditedText;

                    //rec.IsTemplate = IsTemplate;
                //If full amount is not allocated to cost classificaton, create an additional (null) allocation for the remainder
                //if null allocation already exists, add this new portion


                //if classificaton being used already exists for this transaction,increase previous allocation
                var exists = tmp.Where(x => x.KCategoryID == Category.EditedKid && x.KFinTranID == Selected.KFinTranID).OrderByDescending(x => x.DateEffective).ToList();
                    var exxist = exists.FirstOrDefault();
                    var exists1 = tmp.Where(x => x.KCategoryID == "" && x.KFinTranID == Selected.KFinTranID).OrderByDescending(x => x.DateEffective).ToList();
                    var exxist1 = exists1.FirstOrDefault();
                    var matches = tmp.Where(x => x.KCategoryID == Category.OriginalKid && x.KFinTranID == Selected.KFinTranID).OrderByDescending(x => x.DateEffective).ToList();
                    var category = matches.FirstOrDefault();
                    var matches1 = tmp1.Where(x => x.KCategoryID == Category.OriginalKid && x.KFinTranID == Selected.KFinTranID).OrderByDescending(x => x.DateEffective).ToList();
                    var category1 = matches1.FirstOrDefault();               
                
                
                
                if (Category.EditedKid != Category.OriginalKid || Category.OriginalKid == "" ||IntAmnt != OrgActual)
                    //unprocessed amount cannot be adjusted without adding a classification first
                    //ToDo: message box in this regard
                {                 




                    if (Category.EditedKid == Category.OriginalKid && IntAmnt != OrgActual)
                    { 
                        //Update the classification being modified
                        Selected.ActualAmount = IntAmnt;

                        if (IntAmnt == 0)
                        //whole allocation removed from previous category
                        {
                            matches = tmp.Where(x => x.KCategoryID == "" && x.KFinTranID == Selected.KFinTranID).OrderByDescending(x => x.DateEffective).ToList();
                            category = matches.FirstOrDefault();
                            if (category == null)
                            {
                                //no existing null allocation for transaction
                                matches1 = tmp1.Where(x => x.KCategoryID == Category.OriginalKid && x.KFinTranID == Selected.KFinTranID).OrderByDescending(x => x.DateEffective).ToList();
                                category1 = matches1.FirstOrDefault();
                                matches = tmp.Where(x => x.KCategoryID == Category.OriginalKid && x.KFinTranID == Selected.KFinTranID).OrderByDescending(x => x.DateEffective).ToList();
                                category = matches.FirstOrDefault();
                                {

                                    category1.ShortName = "";
                                    category1.KCategoryID = "";


                                    
                                    category.ShortName = "";
                                    //category1.ActualAmount = 12000;
                                    category.KCategoryID = "";

                                    Selected1.ShortName = "";
                                    Selected1.KCategoryID = "";
                                }
                                var u = new TransactionResultApiModel
                                {
                                    Posted_Date = Selected1.Posted_Date,
                                    Month = Selected1.Month,
                                    Description = Selected1.Description,
                                    TransAmount = Selected1.TransAmount,
                                    ActualAmount = Selected1.ActualAmount,
                                    ShortName = Selected1.ShortName,
                                    KCategoryID = Selected1.KCategoryID,
                                    KFinActualID = Selected1.KFinActualID,
                                    KFinTranID = Selected1.KFinTranID,
                                    ChangeType = "c",
                                    DateEffective = DateTime.Now,
                                    KHierarchyID = Selected1.KHierarchyID,
                                    KClientID = ((HierarchyItemSelectionViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).Client).EditedKid,
                                    KPartyID = Selected1.KPartyID,
                                    IsTemplate = IsTemplate,
                                    FCatSrchID = Selected1.FCatSrchID,
                                    Notes = Selected1.Notes,

                                };
                                tmp2.Add(u);

                            }
                            else
                            //update existing null allocation and remove record that has been reduced to zero
                            {
                                matches = tmp.Where(x => x.KCategoryID == "" && x.KFinTranID == Selected.KFinTranID).OrderByDescending(x => x.DateEffective).ToList();
                                category = matches.FirstOrDefault();
                                matches1 = tmp1.Where(x => x.KCategoryID == "" && x.KFinTranID == Selected.KFinTranID).OrderByDescending(x => x.DateEffective).ToList();
                                category1 = matches1.FirstOrDefault();
                                {

                                    category1.ActualAmount = category1.ActualAmount + (OrgActual - IntAmnt);
                                    Selected1.ActualAmount = category1.ActualAmount;
                                    category.ActualAmount = category.ActualAmount + (OrgActual - IntAmnt);
                                }
                                var u = new TransactionResultApiModel
                                {
                                    Posted_Date = Selected1.Posted_Date,
                                    Month = Selected1.Month,
                                    Description = Selected1.Description,
                                    TransAmount = Selected1.TransAmount,
                                    ActualAmount = Selected1.ActualAmount,
                                    ShortName = Selected1.ShortName,
                                    KCategoryID = Selected1.KCategoryID,
                                    KFinActualID = Selected1.KFinActualID,
                                    KFinTranID = Selected1.KFinTranID,
                                    ChangeType = "c",
                                    DateEffective = DateTime.Now,
                                    KHierarchyID = Selected1.KHierarchyID,
                                    KClientID = ((HierarchyItemSelectionViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).Client).EditedKid,
                                    KPartyID = Selected1.KPartyID,
                                    IsTemplate = IsTemplate,
                                    Notes = Selected1.Notes,
                                    FCatSrchID = Selected1.FCatSrchID,

                                };
                                tmp2.Add(u);
                                matches1 = tmp1.Where(x => x.KCategoryID == Category.OriginalKid && x.KFinTranID == Selected.KFinTranID).OrderByDescending(x => x.DateEffective).ToList();
                                category1 = matches1.FirstOrDefault();
                                matches = tmp.Where(x => x.KCategoryID == Category.OriginalKid && x.KFinTranID == Selected.KFinTranID).OrderByDescending(x => x.DateEffective).ToList();
                                category = matches.FirstOrDefault();
                                u = new TransactionResultApiModel
                                {
                                    Posted_Date = Selected1.Posted_Date,
                                    Month = Selected1.Month,
                                    Description = Selected1.Description,
                                    TransAmount = Selected1.TransAmount,
                                    ActualAmount = Selected1.ActualAmount,
                                    ShortName = Selected1.ShortName,
                                    KCategoryID = Selected1.KCategoryID,
                                    KFinActualID = Category.OriginalKid,
                                    KFinTranID = Selected1.KFinTranID,
                                    ChangeType = "d",
                                    DateEffective = DateTime.Now,
                                    KHierarchyID = Selected1.KHierarchyID,
                                    KClientID = ((HierarchyItemSelectionViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).Client).EditedKid,
                                    KPartyID = Selected1.KPartyID,
                                    IsTemplate = IsTemplate,
                                    Notes = Selected1.Notes,
                                    FCatSrchID = Selected1.FCatSrchID,
                                };
                                tmp2.Add(u);

                                tmp1.Remove(category1);
                                tmp.Remove(category);



                            }

                        }
                        else
                        {
                            //Add record for change on API model


                            //update transaction view modelSelected.

                            matches1 = tmp1.Where(x => x.KCategoryID == Category.OriginalKid && x.KFinTranID == Selected.KFinTranID).OrderByDescending(x => x.DateEffective).ToList();
                            category1 = matches1.FirstOrDefault();
                            matches = tmp.Where(x => x.KCategoryID == Category.OriginalKid && x.KFinTranID == Selected.KFinTranID).OrderByDescending(x => x.DateEffective).ToList();
                            category = matches.FirstOrDefault();
                            {   
                                category1.ActualAmount = IntAmnt;
                                category1.ShortName = Selected.ShortName;
                                category1.KCategoryID = Selected.KCategoryID;


                                category.ActualAmount = IntAmnt;
                                category.ShortName = Selected.ShortName;
                                category.KCategoryID = Selected.KCategoryID;
                                Selected1.ActualAmount = IntAmnt;
                                Selected1.ShortName = Selected.ShortName;
                                Selected1.KCategoryID = Selected.KCategoryID;
                            }

                            var u = new TransactionResultApiModel
                            {
                                Posted_Date = Selected1.Posted_Date,
                                Month = Selected1.Month,
                                Description = Selected1.Description,
                                TransAmount = Selected1.TransAmount,
                                ActualAmount = Selected1.ActualAmount,
                                ShortName = Selected1.ShortName,
                                KCategoryID = Selected1.KCategoryID,
                                KFinActualID = Selected1.KFinActualID,
                                KFinTranID = Selected1.KFinTranID,
                                ChangeType = "c",
                                DateEffective = DateTime.Now,
                                KHierarchyID = Selected1.KHierarchyID,
                                KClientID = ((HierarchyItemSelectionViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).Client).EditedKid,
                                KPartyID = Selected1.KPartyID,
                                IsTemplate = IsTemplate,

                            };
                            tmp2.Add(u);
                            matches = tmp.Where(x => x.KCategoryID == "" && x.KFinTranID == Selected.KFinTranID).OrderByDescending(x => x.DateEffective).ToList();
                            category = matches.FirstOrDefault();
                            if (category == null)
                            {
                                //no existing null allocation for transaction
                                matches1 = tmp1.Where(x => x.KCategoryID == Category.OriginalKid && x.KFinTranID == Selected.KFinTranID).OrderByDescending(x => x.DateEffective).ToList();
                                category1 = matches1.FirstOrDefault();

                                // deal with unassigne amount
                                Selected1.ActualAmount = (OrgActual - IntAmnt);
                                Selected1.ShortName = "";
                                Selected1.KCategoryID = "";
                                Selected1.KFinActualID = Guid.NewGuid().ToString().ToUpper();
                                Selected1.Description = Selected1.Description;
                                Selected1.KFinTranID = Selected1.KFinTranID;
                                Selected1.KChangeID = Selected1.KChangeID;
                                Selected1.Posted_Date = Selected1.Posted_Date;
                                Selected1.Month = Selected1.Month;
                                Selected1.TransAmount = Selected1.TransAmount;

                                //tmp.Add(Selected1);
                                ((TransactionTreeViewModel)tmp0).AddItem(Selected1);
                                tmp1.Add(Selected1);


                                u = new TransactionResultApiModel
                                {
                                    Posted_Date = Selected1.Posted_Date,
                                    Month = Selected1.Month,
                                    Description = Selected1.Description,
                                    TransAmount = Selected1.TransAmount,
                                    ActualAmount = Selected1.ActualAmount,
                                    ShortName = Selected1.ShortName,
                                    KCategoryID = Selected1.KCategoryID,
                                    KFinActualID = Selected1.KFinActualID,
                                    KFinTranID = Selected1.KFinTranID,
                                    ChangeType = "a",
                                    DateEffective = DateTime.Now,
                                    KHierarchyID = Selected1.KHierarchyID,
                                    KClientID = ((HierarchyItemSelectionViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).Client).EditedKid,
                                    KPartyID = Selected1.KPartyID,
                                    IsTemplate = IsTemplate,
                                    Notes = Selected1.Notes,
                                    FCatSrchID = Selected1.FCatSrchID,
                                };
                                tmp2.Add(u);

                            }
                            else
                            //update existing null allocation and remove record that has been reduced to zero
                            {
                                matches = tmp.Where(x => x.KCategoryID == "" && x.KFinTranID == Selected.KFinTranID).OrderByDescending(x => x.DateEffective).ToList();
                                category = matches.FirstOrDefault();
                                matches1 = tmp1.Where(x => x.KCategoryID == "" && x.KFinTranID == Selected.KFinTranID).OrderByDescending(x => x.DateEffective).ToList();
                                category1 = matches1.FirstOrDefault();
                                {

                                    category1.ActualAmount = category1.ActualAmount + (OrgActual - IntAmnt);
                                    Selected1.ActualAmount = category1.ActualAmount;
                                    category.ActualAmount = category1.ActualAmount ;
                                }
                                u = new TransactionResultApiModel
                                {
                                    Posted_Date = Selected1.Posted_Date,
                                    Month = Selected1.Month,
                                    Description = Selected1.Description,
                                    TransAmount = Selected1.TransAmount,
                                    ActualAmount = Selected1.ActualAmount,
                                    ShortName = Selected1.ShortName,
                                    KCategoryID = Selected1.KCategoryID,
                                    KFinActualID = Selected1.KFinActualID,
                                    KFinTranID = Selected1.KFinTranID,
                                    ChangeType = "c",
                                    DateEffective = DateTime.Now,
                                    KHierarchyID = Selected1.KHierarchyID,
                                    KClientID = ((HierarchyItemSelectionViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).Client).EditedKid,
                                    KPartyID = Selected1.KPartyID,
                                    IsTemplate = IsTemplate,
                                    Notes = Selected1.Notes,
                                    FCatSrchID = Selected1.FCatSrchID,
                                };
                                tmp2.Add(u);



                            }
                        }


                    }
                    else
                    //category has changed - and this category may already be used for the transaction
                    //if total amount has been allocated, no new record required, but could require a record deletion if allocation has previously been made to this category
                    //if portion has been allocated, and the same category is not already linked to the transaction
                    {
                        if (IntAmnt == OrgActual)
                        {
                            //Update the classification being modified
                            //Selected.ActualAmount = IntAmnt;
                            //Selected.ShortName = Category.EditedName;
                            //Selected.KCategoryID = Category.EditedKid;



                            //Add record for change on API model
                            matches = tmp.Where(x => x.KCategoryID == Category.EditedKid && x.KFinTranID == Selected.KFinTranID).OrderByDescending(x => x.DateEffective).ToList();
                            
                            category = matches.FirstOrDefault();
                            if (category != null) 
                            //check whether this allocation category is already in use for the transaction
                            {

                            //update transaction view model
                            matches = tmp.Where(x => x.KCategoryID == Category.EditedKid && x.KFinTranID == Selected.KFinTranID).OrderByDescending(x => x.DateEffective).ToList();
                            category = matches.FirstOrDefault();
                            matches1 = tmp1.Where(x => x.KCategoryID == Category.EditedKid && x.KFinTranID == Selected.KFinTranID).OrderByDescending(x => x.DateEffective).ToList();
                            category1 = matches1.FirstOrDefault();


                                {
                                    //category.ActualAmount += IntAmnt;
                                    category1.ActualAmount += IntAmnt;
                                    category.ActualAmount = category1.ActualAmount;
                                    Selected1.ActualAmount = category1.ActualAmount;

                                }

                                var u = new TransactionResultApiModel
                                {
                                    Posted_Date = Selected1.Posted_Date,
                                    Month = Selected1.Month,
                                    Description = Selected1.Description,
                                    TransAmount = Selected1.TransAmount,
                                    ActualAmount = Selected1.ActualAmount,
                                    ShortName = Selected1.ShortName,
                                    KCategoryID = Selected1.KCategoryID,
                                    KFinActualID = Selected1.KFinActualID,
                                    KFinTranID = Selected1.KFinTranID,
                                    ChangeType = "c",
                                    DateEffective = DateTime.Now,
                                    KHierarchyID = Selected1.KHierarchyID,
                                    KClientID = ((HierarchyItemSelectionViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).Client).EditedKid,
                                    KPartyID = Selected1.KPartyID,
                                    IsTemplate = IsTemplate,
                                    Notes = Selected1.Notes,
                                    FCatSrchID = Selected1.FCatSrchID,

                                };
                                tmp2.Add(u);

                                //then delete the original assignment
                                matches1 = tmp1.Where(x => x.KCategoryID == Category.OriginalKid && x.KFinTranID == Selected.KFinTranID).OrderByDescending(x => x.DateEffective).ToList();
                                category1 = matches1.FirstOrDefault();

                                u = new TransactionResultApiModel
                                {
                                    Posted_Date = Selected1.Posted_Date,
                                    Month = Selected1.Month,
                                    Description = Selected1.Description,
                                    TransAmount = Selected1.TransAmount,
                                    ActualAmount = Selected1.ActualAmount,
                                    ShortName = Selected1.ShortName,
                                    KCategoryID = Selected1.KCategoryID,
                                    KFinActualID = Category.OriginalKid,
                                    KFinTranID = Selected1.KFinTranID,
                                    ChangeType = "d",
                                    DateEffective = DateTime.Now,
                                    KHierarchyID = Selected1.KHierarchyID,
                                    KClientID = ((HierarchyItemSelectionViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).Client).EditedKid,
                                    KPartyID = Selected1.KPartyID,
                                    IsTemplate = IsTemplate,
                                    Notes = Selected1.Notes,
                                    FCatSrchID = Selected1.FCatSrchID,

                                };
                                tmp2.Add(u);

                                tmp1.Remove(category1);
                                tmp.Remove(category);
                            }
                            else
                            // first time use of the allocation category for the transaction
                            {
                                //update transaction view model
                                matches = tmp.Where(x => x.KCategoryID == Category.OriginalKid && x.KFinTranID == Selected.KFinTranID).OrderByDescending(x => x.DateEffective).ToList();
                                category = matches.FirstOrDefault();
                                matches1 = tmp1.Where(x => x.KCategoryID == Category.OriginalKid && x.KFinTranID == Selected.KFinTranID).OrderByDescending(x => x.DateEffective).ToList();
                                category1 = matches1.FirstOrDefault();


                                {
                                    category1.ActualAmount = IntAmnt;
                                    category1.ShortName = Category.EditedName;
                                    category1.KCategoryID = Category.EditedKid;
                                    category1.KPartyID = Party.EditedKid;
                                    category1.KPartyName = Party.EditedName;
                                    category.ActualAmount = IntAmnt;
                                    category.ShortName = Category.EditedName;
                                    category.KCategoryID = Category.EditedKid;
                                    category.KPartyID = Party.EditedKid;
                                    category.KPartyName = Party.EditedName;

                                    Selected1.ActualAmount = IntAmnt;
                                    Selected1.ShortName = Category.EditedName;
                                    Selected1.KCategoryID = Category.EditedKid;
                                    Selected1.KPartyName = Party.EditedName;
                                    Selected1.KPartyID = Party.EditedKid;
                                }

                                //var PartyID = Selected1.KPartyID;

                                var u = new TransactionResultApiModel
                                {
                                    Posted_Date = Selected1.Posted_Date,
                                    Month = Selected1.Month,
                                    Description = Selected1.Description,
                                    TransAmount = Selected1.TransAmount,
                                    ActualAmount = Selected1.ActualAmount,
                                    ShortName = Selected1.ShortName,
                                    KPartyName = Selected1.KPartyName,
                                    KCategoryID = Selected1.KCategoryID,
                                    KFinActualID = Selected1.KFinActualID,
                                    KFinTranID = Selected1.KFinTranID,
                                    ChangeType = "c",
                                    DateEffective = DateTime.Now,
                                    KHierarchyID = Selected1.KHierarchyID,
                                    KClientID = ((HierarchyItemSelectionViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).Client).EditedKid,
                                    KPartyID = Selected1.KPartyID,
                                    IsTemplate = IsTemplate,
                                    Notes = Selected1.Notes,
                                    FCatSrchID = Selected1.FCatSrchID,

                                };
                                tmp2.Add(u);
                            }
                        }
                        else
                        //allocation less than the original amount remaining
                        {
                            if (Category.OriginalKid == "")
                            {
                                //if transaction had a 'null' assignment to begin with,reduce the allocation amount by the new amount now assigned

                                matches = tmp.Where(x => x.KCategoryID == Category.OriginalKid && x.KFinTranID == Selected.KFinTranID).OrderByDescending(x => x.DateEffective).ToList();
                                category = matches.FirstOrDefault();
                                matches1 = tmp1.Where(x => x.KCategoryID == Category.OriginalKid && x.KFinTranID == Selected.KFinTranID).OrderByDescending(x => x.DateEffective).ToList();
                                category1 = matches1.FirstOrDefault();
                                {

 
                                    category1.ActualAmount = OrgActual - IntAmnt;
                                    Selected1.ActualAmount = category1.ActualAmount;

                                    category1.KCategoryID = Category.OriginalKid;
                                    category1.ShortName = Category.OriginalName;
                                    category.ActualAmount = category1.ActualAmount;
                                    category.KCategoryID = Category.OriginalKid;
                                    category.ShortName = Category.OriginalName;
                                    category.KPartyID = Party.OriginalKid;
                                    category.KPartyName = Party.EditedName;


                                }

                                var u = new TransactionResultApiModel
                                {
                                    Posted_Date = Selected1.Posted_Date,
                                    Month = Selected1.Month,
                                    Description = Selected1.Description,
                                    TransAmount = Selected1.TransAmount,
                                    ActualAmount = Selected1.ActualAmount,
                                    ShortName = Selected1.ShortName,
                                    KCategoryID = Selected1.KCategoryID,
                                    KFinActualID = Selected1.KFinActualID,
                                    KFinTranID = Selected1.KFinTranID,
                                    ChangeType = "c",
                                    DateEffective = DateTime.Now,
                                    KHierarchyID = Selected1.KHierarchyID,
                                    KClientID = ((HierarchyItemSelectionViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).Client).EditedKid,
                                    KPartyID = Selected1.KPartyID,
                                    IsTemplate = IsTemplate,
                                    Notes = Selected1.Notes,
                                    FCatSrchID = Selected1.FCatSrchID,

                                };
                                tmp2.Add(u);
                                // now create a new assignment for the new cost category
                                Selected1.ActualAmount = IntAmnt;
                                Selected1.ShortName = Category.EditedName;
                                Selected1.KCategoryID = Category.EditedKid;
                                Selected1.KFinActualID = Guid.NewGuid().ToString().ToUpper();
                                Selected1.Description = Selected1.Description;
                                Selected1.KFinTranID = Selected1.KFinTranID;
                                Selected1.KChangeID = Selected1.KChangeID;
                                Selected1.Posted_Date = Selected1.Posted_Date;
                                Selected1.Month = Selected1.Month;
                                Selected1.TransAmount = Selected1.TransAmount;
                                Selected1.FCatSrchID = Selected1.FCatSrchID;

                                //tmp.Add(Selected1);
                                ((TransactionTreeViewModel)tmp0).AddItem(Selected1);
                                tmp1.Add(Selected1);



                                u = new TransactionResultApiModel
                                {
                                    Posted_Date = Selected1.Posted_Date,
                                    Month = Selected1.Month,
                                    Description = Selected1.Description,
                                    TransAmount = Selected1.TransAmount,
                                    ActualAmount = Selected1.ActualAmount,
                                    ShortName = Selected1.ShortName,
                                    KCategoryID = Selected1.KCategoryID,
                                    KFinActualID = Selected1.KFinActualID,
                                    KFinTranID = Selected1.KFinTranID,
                                    ChangeType = "a",
                                    DateEffective = DateTime.Now,
                                    KHierarchyID = Selected1.KHierarchyID,
                                    KClientID = ((HierarchyItemSelectionViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).Client).EditedKid,
                                    KPartyID = Selected1.KPartyID,
                                    IsTemplate = IsTemplate,
                                    Notes = Selected1.Notes,
                                    FCatSrchID = Selected1.FCatSrchID,

                                };
                                tmp2.Add(u);

                            }
                            else
                            {
                                //original assignment was to a category other than null, so a partial assignment must result in the balance being assigned to null
                                //OR if a null assignment already exists, the unallocated amount must be added...

                                matches = tmp.Where(x => x.KCategoryID == Category.OriginalKid && x.KFinTranID == Selected.KFinTranID).OrderByDescending(x => x.DateEffective).ToList();
                                category = matches.FirstOrDefault();
                                matches1 = tmp1.Where(x => x.KCategoryID == Category.OriginalKid && x.KFinTranID == Selected.KFinTranID).OrderByDescending(x => x.DateEffective).ToList();
                                category1 = matches1.FirstOrDefault();
                                {
                                    category1.ActualAmount = IntAmnt;
                                    category1.ShortName = Category.EditedName;
                                    category1.KCategoryID = Category.EditedKid;
                                    category.ActualAmount = IntAmnt;
                                    category.ShortName = Category.EditedName;
                                    category.KCategoryID = Category.EditedKid;
                                    Selected1.ActualAmount = IntAmnt;
                                    Selected1.ShortName = Category.EditedName;
                                    Selected1.KCategoryID = Category.EditedKid;

                                }

                                var u = new TransactionResultApiModel
                                {
                                    Posted_Date = Selected1.Posted_Date,
                                    Month = Selected1.Month,
                                    Description = Selected1.Description,
                                    TransAmount = Selected1.TransAmount,
                                    ActualAmount = Selected1.ActualAmount,
                                    ShortName = Selected1.ShortName,
                                    KCategoryID = Selected1.KCategoryID,
                                    KFinActualID = Selected1.KFinActualID,
                                    KFinTranID = Selected1.KFinTranID,
                                    ChangeType = "c",
                                    DateEffective = DateTime.Now,
                                    KHierarchyID = Selected1.KHierarchyID,
                                    KClientID = ((HierarchyItemSelectionViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).Client).EditedKid,
                                    KPartyID = Selected1.KPartyID,
                                    IsTemplate = IsTemplate,
                                    Notes = Selected1.Notes,
                                    FCatSrchID = Selected1.FCatSrchID,

                                };
                                tmp2.Add(u); 

                                //matches = tmp.Where(x => x.KCategoryID == "" && x.KFinTranID == Selected.KFinTranID).OrderByDescending(x => x.DateEffective).ToList();
                                //category = matches.FirstOrDefault();
                                matches1 = tmp1.Where(x => x.KCategoryID == "" && x.KFinTranID == Selected.KFinTranID).OrderByDescending(x => x.DateEffective).ToList();
                                category1 = matches1.FirstOrDefault();

                                if (category1 == null)
                                {
                                    //add a null allocation record
                                    Selected1.ActualAmount = OrgActual - IntAmnt;
                                    Selected1.ShortName = "";
                                    Selected1.KCategoryID = "";
                                    Selected1.KFinActualID = Guid.NewGuid().ToString().ToUpper();
                                    Selected1.Description = Selected.Description;
                                    Selected1.KFinTranID = Selected.KFinTranID;
                                    Selected1.KChangeID = Selected.KChangeID;
                                    Selected1.Posted_Date = Selected.Posted_Date;
                                    Selected1.Month = Selected.Month;
                                    Selected1.TransAmount = Selected.TransAmount;
                                    tmp.Add(Selected1);
                                    tmp1.Add(Selected1);
                                    u = new TransactionResultApiModel
                                    {
                                        Posted_Date = Selected1.Posted_Date,
                                        Month = Selected1.Month,
                                        Description = Selected1.Description,
                                        TransAmount = Selected1.TransAmount,
                                        ActualAmount = Selected1.ActualAmount,
                                        ShortName = Selected1.ShortName,
                                        KCategoryID = Selected1.KCategoryID,
                                        KFinActualID = Selected1.KFinActualID,
                                        KFinTranID = Selected1.KFinTranID,
                                        ChangeType = "a",
                                        DateEffective = DateTime.Now,
                                        KHierarchyID = Selected1.KHierarchyID,
                                        KPartyID = Selected1.KPartyID,
                                        IsTemplate = IsTemplate,
                                        Notes = Selected1.Notes,
                                        FCatSrchID = Selected1.FCatSrchID,

                                        //KClientID =((HierarchyItemSelectionViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).Root).EditedKid,
                                    };
                                    tmp2.Add(u);
                                }
                                else
                                //null allocation record already exists, adjust the current allocation value
                                {
                                    category1.ActualAmount += OrgActual - IntAmnt;
                                    category.ActualAmount = category1.ActualAmount;
                                    Selected1.ActualAmount = category1.ActualAmount;


                                    u = new TransactionResultApiModel
                                    {
                                        Posted_Date = Selected1.Posted_Date,
                                        Month = Selected1.Month,
                                        Description = Selected1.Description,
                                        TransAmount = Selected1.TransAmount,
                                        ActualAmount = Selected1.ActualAmount,
                                        ShortName = Selected1.ShortName,
                                        KCategoryID = Selected1.KCategoryID,
                                        KFinActualID = Selected1.KFinActualID,
                                        KFinTranID = Selected1.KFinTranID,
                                        ChangeType = "c",
                                        DateEffective = DateTime.Now,
                                        KHierarchyID = Selected1.KHierarchyID,
                                        KPartyID = Selected1.KPartyID,
                                        IsTemplate = IsTemplate,
                                        Notes = Selected1.Notes,
                                        KClientID = ((HierarchyItemSelectionViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).Client).EditedKid,
                                        FCatSrchID = Selected1.FCatSrchID,
                                    };
                                    tmp2.Add(u);
                                }



                            }

                        }
                    }
                    //var mViewModel = (HierarchyTreeViewModel)ViewModelApplication.CurrentControlViewModel;
                    ViewModelApplication.CurrentPopupViewModel = ViewModelApplication.CurrentPopupViewModel;
                    matches = tmp1.OrderByDescending(x => x.Posted_Date).ThenBy(x => x.KFinTranID).ThenBy(x => x.ShortName).ToList();
                    Remove(matches,tmp1 );
                    Clone(matches, tmp1);
                    matches = tmp.OrderByDescending(x => x.Posted_Date).ThenBy(x => x.KFinTranID).ThenBy(x => x.ShortName).ToList();
                    Remove(matches, tmp);
                    Clone(matches, tmp);
                }
                else
                    //update all derived transactions with the latest allocated Party
                {
                    matches = tmp.Where(x => x.KCategoryID == Category.OriginalKid && x.KFinTranID == Selected.KFinTranID).OrderByDescending(x => x.DateEffective).ToList();
                    category = matches.FirstOrDefault();
                    matches1 = tmp1.Where(x => x.KCategoryID == Category.OriginalKid && x.KFinTranID == Selected.KFinTranID).OrderByDescending(x => x.DateEffective).ToList();
                    category1 = matches1.FirstOrDefault();


                    {
                        category1.ActualAmount = IntAmnt;
                        category1.ShortName = Category.EditedName;
                        category1.KCategoryID = Category.EditedKid;
                        category1.KPartyID = Party.EditedKid;
                        category1.KPartyName = Party.EditedName;
                        category.ActualAmount = IntAmnt;
                        category.ShortName = Category.EditedName;
                        category.KCategoryID = Category.EditedKid;
                        category.KPartyID = Party.EditedKid;
                        category.KPartyName = Party.EditedName;

                        Selected1.ActualAmount = IntAmnt;
                        Selected1.ShortName = Category.EditedName;
                        Selected1.KCategoryID = Category.EditedKid;
                        Selected1.KPartyName = Party.EditedName;
                        Selected1.KPartyID = Party.EditedKid;
                    }
                    var u = new TransactionResultApiModel
                    {
                        Posted_Date = Selected1.Posted_Date,
                        Month = Selected1.Month,
                        Description = Selected1.Description,
                        TransAmount = Selected1.TransAmount,
                        ActualAmount = Selected1.ActualAmount,
                        ShortName = Selected1.ShortName,
                        KCategoryID = Selected1.KCategoryID,
                        KFinActualID = Selected1.KFinActualID,
                        KFinTranID = Selected1.KFinTranID,
                        ChangeType = "c",
                        DateEffective = DateTime.Now,
                        KHierarchyID = Selected1.KHierarchyID,
                        KClientID = ((HierarchyItemSelectionViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).Client).EditedKid,
                        KPartyID =  ((HierarchyItemSelectionViewModel)((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).Party).EditedKid,
                        //KPartyID = (Selected1.KPartyID??Selected.KPartyID),
                        KPartyName = ((HierarchyItemSelectionViewModel)((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).Party).EditedName,
                        FCatSrchID = Selected1.FCatSrchID,
                        Notes = Selected.Notes,
                      
                    };
                    tmp2.Add(u);
                    Selected1.KPartyID = ((HierarchyItemSelectionViewModel)((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).Party).EditedKid;
                    Selected1.KPartyName = ((HierarchyItemSelectionViewModel)((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).Party).EditedName;
                    Selected.KPartyID = ((HierarchyItemSelectionViewModel)((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).Party).EditedKid;
                    Selected.KPartyName = ((HierarchyItemSelectionViewModel)((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).Party).EditedName;


                }

            }



            Close();
        }

        //public void EditClassification()
        public void BrowseImage()
        {
            //ViewModelApplication.CurrentPopupViewModel = ViewModelApplication.CurrentPopupViewModel;
            //First check for any document holders not filled with a document

            var exists = DocumentList.Where(x => x.DocURL == "\\somepath\\filename.jpg").ToList();
            if (exists.Count > 0 && exists.FirstOrDefault().DocURL == "\\somepath\\filename.jpg")
            {  
                Document = exists.FirstOrDefault(); 
            }
            else
            //...create a new document holder
            {
                Document = new DocDataViewModel();
                DocumentList.Add(Document);
            }

            Document.KDocID = Guid.NewGuid().ToString();
            using (var openFileDialog = new OpenFileDialog())
            {
                 //openFileDialog.Filter = "PDF Files|*.pdf";
                 openFileDialog.Filter = "Images (*.jpg,*.png)|*.jpg;*.png|All Files(*.*)|*.*";
                 openFileDialog.Multiselect = false;
                //openFileDialog.InitialDirectory = @"C:\Users\triek\OneDrive\Documents\Timstuff\Rheebok\Images\BrBed";
                //openFileDialog.fil
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //var filePath = openFileDialog.FileName;
                    var filePath = openFileDialog.FileName;
                    Document.DocURL = filePath;

                    Document.DocName = GetFileFolderName(Document.DocURL);
                    Document.DocDescription = GetFileFolderName(Document.DocURL);
                    Selected.Document.DocName = Selected.Document.KDocID + GetFileExtension(Document.DocDescription);
                    var path = Path.GetTempPath();
                    var fileName = path + Document.DocName;
                   Document.DocURL = fileName;
                    using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = new BinaryReader(stream))
                        {
                            Document.DocImage = reader.ReadBytes((int)stream.Length);
                        }
                    }
                    Document.IsNew = true;
                    Document.FFintranID = Selected.KFinTranID;
                    //Document = Selected.Document;
                    //fileName = Environment.GetFolderPath(Environment.SpecialFolder.Resources);
                    //try
                    //{
                    //    System.IO.File.Copy(filePath, fileName);
                    //}
                    //catch (Exception)
                    //{
                    //    //this file already exists in the working directory, if physically different, please rename...
                    //    throw;
                    //}



                    //MyImage.
                    //openFileDialog.
                }
            }
            //    MyImage.Source = new BitmapImage(new Uri(lImagePath.Text));

            //    using (var fs = new FileStream(ImagePath.Text, FileMode.Open, FileAccess.Read))
            //    {
            //        _imageBytes = new byte[fs.Length];
            //        fs.Read(imgBytes, 0, System.Convert.ToInt32(fs.Length));
            //    }
            //}


        }
        /// <summary>
        /// Open the selected document in the default application on the client machine
        /// Document is temporarily converted from the bytestream into a physical file in the temp directory
        /// </summary>
        /// <param name="doccie"></param>
        public void OpenDocument(DocDataViewModel doccie)
        {
            //ViewModelApplication.CurrentPopupViewModel = ViewModelApplication.CurrentPopupViewModel;
            //Selected.Document.KDocID = Guid.NewGuid().ToString();
            using (var openFileDialog = new OpenFileDialog())
            {
                //Selected.Document.DocDescription = GetFileFolderName(Document.DocURL);
                //Selected.Document.DocName = Selected.Document.KDocID + GetFileExtension( Selected.Document.DocDescription);
                //Selected.Document.FFintranID = Selected.KFinTranID;
                //Document.IsNew = true;
                var path = Path.GetTempPath();
                var fileName = path + doccie.DocName;
                doccie.DocURL = fileName;
                using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(doccie.DocImage, 0,doccie.DocImage.Length);
                    //return true;
                }
                Process.Start(doccie.DocURL);

            }


                //    MyImage.Source = new BitmapImage(new Uri(lImagePath.Text));

                //    using (var fs = new FileStream(ImagePath.Text, FileMode.Open, FileAccess.Read))
                //    {
                //        _imageBytes = new byte[fs.Length];
                //        fs.Read(imgBytes, 0, System.Convert.ToInt32(fs.Length));
                //    }
                //}


            }

        /// Open the selected document in the default application on the client machine
        /// Document is temporarily converted from the bytestream into a physical file in the temp directory
        /// </summary>
        /// <param name="doccie"></param>
        public void RemoveDocument(DocDataViewModel doccie)
        {

            {
                //Selected.Document.DocDescription = GetFileFolderName(Document.DocURL);
                //Selected.Document.DocName = Selected.Document.KDocID + GetFileExtension( Selected.Document.DocDescription);
                //Selected.Document.FFintranID = Selected.KFinTranID;
                //Document.IsNew = true;
                doccie.IsRemove = true;

            }
        }

            public static void DatabaseFilePut(string varFilePath)
            {
                byte[] file;
                using (var stream = new FileStream(varFilePath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        file = reader.ReadBytes((int)stream.Length);
                    }
                }
                //using (var varConnection = Locale.sqlConnectOneTime(Locale.sqlDataConnectionDetails))
                //using (var sqlWrite = new SqlCommand("INSERT INTO Raporty (RaportPlik) Values(@File)", varConnection))
                //{
                //    sqlWrite.Parameters.Add("@File", SqlDbType.VarBinary, file.Length).Value = file;
                //    sqlWrite.ExecuteNonQuery();
                //}
            }


        #endregion

        #region Helpers

        /// <summary>
        /// Extract the file extenstion from a full path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFileExtension(string path)
        {
            // C:\Something\a folder
            // C:\Something\a file.png
            // a file file.png

            // If we have no opath, return empty
            if (string.IsNullOrEmpty(path))
                return string.Empty;



            // Find the last backslash in the path
            var lastIndex = path.LastIndexOf('.');

            // If we dont find a backslash, return the path itself
            if (lastIndex <= 0)
                return path;

            // Return the name after the last backslash
            return path.Substring(lastIndex);
        }


        /// <summary>
        /// Extract the file or folder name from a full path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFileFolderName(string path)
        {
            // C:\Something\a folder
            // C:\Something\a file.png
            // a file file.png

            // If we have no opath, return empty
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            // Make all slashes back slashes
            var normalizedPath = path.Replace('/', '\\');

            // Find the last backslash in the path
            var lastIndex = normalizedPath.LastIndexOf('\\');

            // If we dont find a backslash, return the path itself
            if (lastIndex <= 0)
                return path;

            // Return the name after the last backslash
            return path.Substring(lastIndex + 1);
        }

        #endregion
        /// <summary>
        /// This function removes items from the target list included in the source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public void Remove(List<TransactionViewModel> source, ObservableCollection<TransactionViewModel> target)
        {
            foreach (var item in source)
                target.Remove(item);
        }

        /// <summary>
        /// This method will make a clone of the source List of objects
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public void Clone(List<TransactionViewModel> source, ObservableCollection<TransactionViewModel> target)
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
                };
                target.Add(mTR);
            }

        }




    }


    
}
