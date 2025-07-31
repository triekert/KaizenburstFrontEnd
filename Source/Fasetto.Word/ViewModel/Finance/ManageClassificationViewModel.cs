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
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using static Fasetto.Word.Core.CoreDI;
using static Fasetto.Word.DI;

//using System.Windows.Forms;


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
        /// Field containing units added to the transaction allocation (a transaction may have one or more allocations linked)
        /// </summary>
        public TextEntryViewModel Units { get; set; }


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


        /// <summary>
        /// The Project to be linked to the allocation, if required
        /// </summary>
        public HierarchyItemSelectionViewModel Project { get; set; }


        /// <summary>
        /// The Asset to be linked to the allocation, if required
        /// </summary>
        public HierarchyItemSelectionViewModel Asset { get; set; }

        /// <summary>
        /// The Account linked to the transaction
        /// </summary>
        public HierarchyItemSelectionViewModel Account { get; set; }

        /// <summary>
        /// An internal party (Such as family member) to be linked for the allocation
        /// </summary>
        public HierarchyItemSelectionViewModel PartyInternal { get; set; }
        public string KCategoryID { get; set; }

        /// <summary>
        /// Parent ID  of hierarchy item
        /// </summary>
        public string ParentCategoryID { get; set; }

        /// <summary>
        /// Parent ShortName of hierarchy item
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
        /// True to show the hierarchy retrieval command is running
        /// </summary>
        public bool UpdateAllocationCompleted { get; set; }
        /// <summary>
        /// Indicates if the current control is pending an update (in progress)
        /// </summary>
        public bool Working { get; set; }



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



        /// <summary>
        /// A flag indicating if the account selection is complete
        /// </summary>
        public bool SelectAccountCompleted { get; set; }

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
        public ObservableCollection<TransactionViewModel> Mtmp { get; set; }
        public ObservableCollection<TransactionViewModel> Mtmp1{ get; set; }
        public TransactionResultListApiModel Mtmp2 { get; set; }
        public int Mtmp3 {get; set; }
        /// <summary>
        /// The selected allocation, being processed
        /// </summary>
        public TransactionViewModel Selected { get; }

        /// <summary>
        /// Copy of the original selected allocation
        /// </summary>
        public TransactionViewModel Selected1 { get; }


        /// <summary>
        /// Subset of selected transaction
        /// </summary>

        public System.Collections.Generic.List<TransactionViewModel> Mmatches { get; set; }

        /// <summary>
        /// Subset of selected transaction
        /// </summary>
        public System.Collections.Generic.List<TransactionViewModel> Mmatches1 { get; set;}

        /// <summary>
        /// Subset of selected transaction
        /// </summary>
        public System.Collections.Generic.List<TransactionViewModel> Mexists { get; set; }

        /// <summary>
        /// Subset of selected transaction
        /// </summary>
        public System.Collections.Generic.List<TransactionViewModel> Mexists1 { get; set; }


        /// <summary>
        /// Subset of selected transaction
        /// </summary>
        public System.Collections.Generic.List<TransactionViewModel> Mexists2 { get; set; }




        /// <summary>
        /// Subset of selected transaction
        /// </summary>
        public TransactionViewModel Mcategory { get; set; }

        /// <summary>
        /// Subset of selected transaction
        /// </summary>
        public TransactionViewModel Mcategory1 { get; set; }

        /// <summary>
        /// Subset of selected transaction
        /// </summary>
        public TransactionViewModel Mexxist { get; set; }

        /// <summary>
        /// Subset of selected transaction
        /// </summary>
        public TransactionViewModel Mexxist1 { get; set;  }
 
        /// <summary>
        /// Subset of selected transaction
        /// </summary>
        public TransactionViewModel Mexxist2 { get; set; }       
        
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
                //prevent editing of allocation amount if the category type is null
                PrepareAction = PrepareAllocationAsync,
                CommitAction = UpdateAllocationAsync,
            };


            // Create Node Name
            Units = new TextEntryViewModel
            {
                Label = "No Of Units",
                OriginalText = selected.Units.ToString( CultureInfo.CurrentCulture),
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
                ClientID = Selected.KClientID,
                HierarchyID = selected.KHierarchyID,
                //HierarchyTypeID = "64413ae7-822f-4866-9ebe-433083d699ac",
                RootID = selected.KHierarchyID,
                PrepareAction = SetCostCategorySelectionAsync,
                PriorPopupViewModel = ViewModelApplication.CurrentPopupViewModel,

                //HierarchyID = ((CostHierarchyViewModel)((CostHierarchyListViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy).MSelectedCostHierarchy).KCategoryID,

                CommitAction = SelectCategoryAsync,           
            };
            Party = new HierarchyItemSelectionViewModel
            {
                Label = "Select Transacting Party",
                //EditedName = mLoadingText,
                EditedName = "Selected Party",
                OriginalKid = selected.KPartyID,
                OriginalName = selected.KPartyName,
                EditedKid = null,
                ClientID = Selected.KClientID,
                HierarchyTypeID = "ADEEBB16-F553-48F8-955F-663227A4886C",
                PrepareAction = SetPartyHierarchySelectionAsync,
                //HierarchyTypeID = ((CostHierarchyListViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy).f,
                //HierarchyID = ((CostHierarchyViewModel)((CostHierarchyListViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy).MSelectedCostHierarchy).KCategoryID,
                PriorPopupViewModel = ViewModelApplication.CurrentPopupViewModel,
                CommitAction = SelectPartyAsync,
            };


            Account = new HierarchyItemSelectionViewModel
            {
                Label = "Select Account",
                //EditedName = mLoadingText,
                EditedName = "Selected Account",
                OriginalKid = selected.KAccountID,
                OriginalName = selected.KAccountName,
                EditedKid = null,
                ClientID = Selected.KClientID,
                HierarchyTypeID = "ADEEBB16-F553-48F8-955F-663227A4886C",
                PrepareAction = SetAccountHierarchySelectionAsync,
                //HierarchyTypeID = ((CostHierarchyListViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy).f,
                //HierarchyID = ((CostHierarchyViewModel)((CostHierarchyListViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy).MSelectedCostHierarchy).KCategoryID,
                PriorPopupViewModel = ViewModelApplication.CurrentPopupViewModel,
                CommitAction = SelectAccountAsync,
            };


            Project = new HierarchyItemSelectionViewModel
            {
                Label = "Select Project",
                //EditedName = mLoadingText,
                EditedName = "Selected Project",
                OriginalKid = selected.KProjectID,
                OriginalName = selected.KProjectName,
                EditedKid = null,
                ClientID = Selected.KClientID,
                HierarchyTypeID = "7D26F714-E4DB-40E1-9F03-B5822E96EF9F",
                PrepareAction = SetAccountHierarchySelectionAsync,
                //HierarchyTypeID = ((CostHierarchyListViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy).f,
                //HierarchyID = ((CostHierarchyViewModel)((CostHierarchyListViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy).MSelectedCostHierarchy).KCategoryID,
                PriorPopupViewModel = ViewModelApplication.CurrentPopupViewModel,
                CommitAction = SelectAccountAsync,
            };



            Asset = new HierarchyItemSelectionViewModel
            {
                Label = "Select Asset",
                //EditedName = mLoadingText,
                EditedName = "Selected Asset",
                OriginalKid = selected.KAssetID,
                OriginalName = selected.KAssetName,
                EditedKid = null,
                ClientID = Selected.KClientID,
                HierarchyTypeID = "ADEEBB16-F553-48F8-955F-663227A4886C",
                PrepareAction = SetAccountHierarchySelectionAsync,
                //HierarchyTypeID = ((CostHierarchyListViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy).f,
                //HierarchyID = ((CostHierarchyViewModel)((CostHierarchyListViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy).MSelectedCostHierarchy).KCategoryID,
                PriorPopupViewModel = ViewModelApplication.CurrentPopupViewModel,
                CommitAction = SelectAccountAsync,
            };


            PartyInternal = new HierarchyItemSelectionViewModel
            {
                Label = "Select Specific Person",
                //EditedName = mLoadingText,
                EditedName = "Selected Person",
                OriginalKid = selected.KPersonID,
                OriginalName = selected.KPersonName,
                EditedKid = null,
                ClientID = Selected.KClientID,
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
            AddClassificationCommand = new RelayCommand(async () => await AddClassificationAsync());
            AlterTemplateCommand = new RelayCommand(AlterTemplate);
            DeleteClassificationCommand = new RelayCommand(async () => await AddClassificationAsync());
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
                //var token = (await scopedClientDataStore.GetLoginCredentialsAsync())?.Token;
                var token = ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).Token;
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
                //var token = (await scopedClientDataStore.GetLoginCredentialsAsync())?.Token;
                var token = ((LoginCredentialsDataModel)ViewModelApplication.CurrentCredential).Token;
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

             Mmatches = ((TransactionTreeViewModel)((TransactionDetailTreeViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel).MPersist.Where(x => x.KFinTranID == mKFinTranID).ToList();
            var Cnt = Mmatches.Count;
            ViewModelApplication.ControlParameter1 = null;


            if (Cnt ==1)
            {
                ViewModelApplication.CurrentPopupViewModel = ((TransactionDetailTreeViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel;
                //ViewModelApplication.CurrentPopupViewModel = ((TransactionDetailTreeViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel;
                ViewModelApplication.CurrentPopupContent = PopupContent.Transaction;
            }
            else
            {
                ((TransactionDetailTreeViewModel)ViewModelApplication.CurrentPopupViewModel).RefreshTransactionList(mKFinTranID, ((TransactionTreeViewModel)((TransactionDetailTreeViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel).MPersist);


                    ViewModelApplication.CurrentPopupContent = PopupContent.TransactionDetail;
            }
            ViewModelApplication.PopupVisible = true;

        }


        /// <summary>
        /// Update the total budget for the selected category hierarchy
        /// </summary>
        /// <returns></returns>
        public async Task<bool> UpdateAllocationAsync()
        {
            // Lock this command to ignore any other requests while processing

            return await RunCommandAsync(() => UpdateAllocationCompleted, async () =>
            {
                // Update the Category Classification value on the server...
                var decAllocation = Selected.ActualAmount;
                decimal.TryParse(Allocation.EditedText, NumberStyles.Currency, CultureInfo.CurrentCulture, out decAllocation);

                //Allocation amount cannot have the sign of the value changed - it can be removed by setting equal to zero
                if (Math.Sign(decAllocation)!= Math.Sign(Selected.ActualAmount)&&(Math.Sign(Selected.ActualAmount) != 0))
                    {
                        System.Windows.MessageBox.Show($"The sign of the changed Allocation amount cannot differ from the sign of the previous allocation");
                        decAllocation = decAllocation * -1;
                    }
                Allocation.OriginalText = decAllocation.ToString("C", CultureInfo.CurrentCulture);
                Allocation.EditedText = decAllocation.ToString("C", CultureInfo.CurrentCulture);
                return true;
            });

        }

        public async Task<bool> PrepareAllocationAsync()
        {
            // Lock this command to ignore any other requests while processing

            return await RunCommandAsync(() => UpdateAllocationCompleted, async () =>
            {
                // if there is an attempt to alter the allocation amount on a null category assignment, ignore the request...
                //if ((Category.EditedKid ?? Category.OriginalKid) == null)
                //{
                //    Allocation.Editing = false;
                //    return true;
                //}
                //else
                //{
                //    //Allocation.Editing = false;

                //   }
                    return true;
            });

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
                if (ViewModelApplication.ControlPopupCostCategory == null||ViewModelApplication.ControlPopupCostCategory.GetType().Name != "HierarchyTreeViewModel1")
                { ViewModelApplication.CurrentPopupViewModel = new HierarchyTreeViewModel1(HierarchyParam); }
                else
                { ViewModelApplication.CurrentPopupViewModel = ViewModelApplication.ControlPopupCostCategory; }
                ((HierarchyTreeViewModel1)ViewModelApplication.CurrentPopupViewModel).SearchText = Category.OriginalKid;
                ((HierarchyTreeViewModel1)ViewModelApplication.CurrentPopupViewModel).PerformKIdSearch();
                ((HierarchyTreeViewModel1)ViewModelApplication.CurrentPopupViewModel).SearchText = "";
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
                if (ViewModelApplication.ControlPopupParty == null)
                { ViewModelApplication.CurrentPopupViewModel = new HierarchyTreeViewModel1(HierarchyParam); }
                else
                { ViewModelApplication.CurrentPopupViewModel = ViewModelApplication.ControlPopupParty; }
                ((HierarchyTreeViewModel1)ViewModelApplication.CurrentPopupViewModel).SearchText = Party.OriginalKid;
                ((HierarchyTreeViewModel1)ViewModelApplication.CurrentPopupViewModel).PerformKIdSearch();
                ((HierarchyTreeViewModel1)ViewModelApplication.CurrentPopupViewModel).SearchText = "";
                //ViewModelApplication.ControlParameter1 = ((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).Party;
                return true;
            });

        }
        public async Task<bool> SetAccountHierarchySelectionAsync()
        {
            // Lock this command to ignore any other requests while processing

            return await RunCommandAsync(() => SetHierarchyCompleted, async () =>
            {
                // Update the Party value on the server...

                ViewModelApplication.CurrentControlViewModel = ((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).Account;
                HierarchyParam = new ParameterHierarchyItemSelectApiModel
                {

                    Level = 0,
                    RootID = "FEF29B52-81CE-48A9-8E02-A7DAC07A9297"
                };
                ViewModelApplication.CurrentPopupViewModel = new HierarchyTreeViewModel1(HierarchyParam);
                ((HierarchyTreeViewModel1)ViewModelApplication.CurrentPopupViewModel).SearchText = Account.OriginalKid;

                //ViewModelApplication.ControlParameter1 = ((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).Party;
                return true;
            });

        }


        public async Task<bool> SetProjectHierarchySelectionAsync()
        {
            // Lock this command to ignore any other requests while processing

            return await RunCommandAsync(() => SetHierarchyCompleted, async () =>
            {
                // Update the Party value on the server...

                ViewModelApplication.CurrentControlViewModel = ((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).Project;
                HierarchyParam = new ParameterHierarchyItemSelectApiModel
                {

                    Level = 0,
                    RootID = "FEF29B52-81CE-48A9-8E02-A7DAC07A9297"
                };
                ViewModelApplication.CurrentPopupViewModel = new HierarchyTreeViewModel1(HierarchyParam);
                ((HierarchyTreeViewModel1)ViewModelApplication.CurrentPopupViewModel).SearchText = Account.OriginalKid;

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


                if (Category.EditedName != "Selected Category")
                Category.OriginalName = Category.EditedName;
                ViewModelApplication.ControlPopupCostCategory = ViewModelApplication.CurrentPopupViewModel;
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

                //ViewModelApplication.ControlPopupParty = ViewModelApplication.CurrentPopupViewModel;
                if (Category.EditedName != "Selected Party")
                    Party.OriginalName = Party.EditedName;
                   ViewModelApplication.ControlPopupParty = ViewModelApplication.CurrentPopupViewModel;
                if (ViewModelApplication.ControlParameter1 != null)
                {
                    ViewModelApplication.CurrentPopupViewModel = ViewModelApplication.ControlParameter1;
                    ViewModelApplication.ControlParameter1 = null;
                    ViewModelApplication.CurrentPopupContent = PopupContent.Classify;
                    ViewModelApplication.PopupVisible = true;
                }


                return true;
            });
        }


        public async Task<bool> SelectAccountAsync()
        {
            // Lock this command to ignore any other requests while processing

            return await RunCommandAsync(() => SelectAccountCompleted, async () =>
            {
            if (Category.EditedName != "Selected Account")
            //ViewModelApplication.CurrentPopupViewModel = ViewModelApplication.CurrentPopupViewModel;
            Account.OriginalName = Account.EditedName;
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
            //new RelayCommand(async () => await AddClassificationAsync());
            AddClassificationAsync();
        }





        /// <summary>
        /// Used to update the classification of a transaction component - Cost category, amount, account, etc
        /// </summary>
        public async Task<bool> AddClassificationAsync()
        {
            return await RunCommandAsync(() => Working, async () =>
            {


                var docs = ((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).DocumentList.Where(x => (x.IsNew || x.IsRemove) && x.KDocID != "00000000-0000-0000-0000-000000000000").ToList();
                var tmp0 = ((TransactionDetailTreeViewModel)((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel).PriorPopupViewModel;
                var tmp = ((ObservableCollection<TransactionViewModel>)((TransactionTreeViewModel)((TransactionDetailTreeViewModel)((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel).PriorPopupViewModel).MPersist);
                Mtmp = ((TransactionTreeViewModel)((TransactionDetailTreeViewModel)((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel).PriorPopupViewModel).MPersist;
                Mtmp1 = ((TransactionDetailTreeViewModel)((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel).TransactionDetail;
                Mtmp2 = ((TransactionTreeViewModel)((TransactionDetailTreeViewModel)((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel).PriorPopupViewModel).mChange;
                Mtmp3 = ((TransactionTreeViewModel)tmp0).Trans_actionRec;
                var rec = Mtmp[Mtmp3];
                rec.IsDocLinked = true;






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
                Selected1.KAccountID = Selected.KAccountID;
                Selected1.KAccountName = Selected.KAccountName;
                Selected1.Units = Selected.Units;
                Selected1.KClientID = Selected.KClientID;

                rec.Notes = Selected.Notes;
                if (TransactionNotes.EditedText != "" && rec.KCategoryID!= "") { rec.Notes = TransactionNotes.EditedText; }
                var IntAmnt = Selected.ActualAmount;
                if (!(Allocation.EditedText == null || Allocation.EditedText == ""))
                { decimal.TryParse(Allocation.EditedText, NumberStyles.Currency, CultureInfo.CurrentCulture, out IntAmnt); }
                if (Math.Sign(IntAmnt) != Math.Sign(OrgActual) &&( Math.Sign(OrgActual)!= 0))
                {
                    System.Windows.MessageBox.Show($"The sign of the Allocation amount cannot differ from the sign of the previous allocation");
                    return true;
                }

                var IntUnits = Selected.Units;

                if (!(Units.EditedText == null || Units.EditedText == ""))
                { int.TryParse(Units.EditedText, out IntUnits); }

                if (Selected.KFinTranID == "00000000-0000-0000-0000-000000000001")

                {
                    if (Account.EditedKid == null)
                    {
                        System.Windows.MessageBox.Show($"A transaction created manually must be linked to a valid Account!");
                        return true;

                    }
                    Mmatches = Mtmp.Where(x => x.KCategoryID == Category.OriginalKid && x.KFinTranID == Selected.KFinTranID).OrderByDescending(x => x.DateEffective).ToList();
                    Mcategory= Mmatches.FirstOrDefault();
                    Mmatches1 = Mtmp1.Where(x => x.KCategoryID == Category.OriginalKid && x.KFinTranID == Selected.KFinTranID).OrderByDescending(x => x.DateEffective).ToList();
                    Mcategory1 = Mmatches1.FirstOrDefault();
                    Selected.KFinTranID = Guid.NewGuid().ToString().ToUpper();
                    Selected.Posted_Date = DateTime.Parse(TransactionDate);
                    Selected.Description = TransactionDescription.EditedText;
                    Selected.TransAmount = IntAmnt;
                    Selected.ActualAmount = IntAmnt;
                    Selected.ShortName = Category.EditedName ?? Category.OriginalName;
                    Selected.KCategoryID = Category.EditedKid ?? Category.OriginalKid;
                    Selected.KFinActualID = Selected1.KFinActualID;
                    Selected.DateEffective = DateTime.Now;
                    Selected.KPartyID = Party.EditedKid ?? Party.OriginalKid;
                    Selected.KPartyName = Party.EditedName ?? Party.OriginalName;
                    Selected.IsTemplate = IsTemplate;
                    Selected.Notes = TransactionNotes.EditedText;
                    Selected.KAccountID = Account.EditedKid ?? Account.OriginalKid;
                    Selected.KAccountName = (Account.EditedKid == null) ? Account.OriginalName : (Account.EditedName ?? Account.OriginalName);
                    Selected.Units = IntUnits;
                    ((TransactionDetailTreeViewModel)((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel).TransactionDetail[0].KFinTranID = Selected.KFinTranID;

                    Mcategory.KFinTranID = Selected.KFinTranID;
                    Mcategory.Posted_Date = Selected.Posted_Date;
                    Mcategory.Description = Selected.Description;
                    Mcategory.TransAmount = Selected.TransAmount;
                    Mcategory.ActualAmount = Selected.ActualAmount;
                    Mcategory.ShortName = Selected.ShortName;
                    Mcategory.KCategoryID = Selected.KCategoryID;
                    Mcategory.KFinActualID = Selected.KFinActualID;
                    Mcategory.DateEffective = Selected.DateEffective;
                    Mcategory.KPartyID = Selected.KPartyID;
                    Mcategory.KPartyName = Selected.KPartyName;
                    Mcategory.IsTemplate = Selected.IsTemplate;
                    Mcategory.Notes = Selected.Notes;
                    Mcategory.KAccountID = Account.EditedKid ?? Account.OriginalKid;
                    Mcategory.KAccountName = (Account.EditedKid == null) ? Account.OriginalName : (Account.EditedName ?? Account.OriginalName);
                    Mcategory.Units = Selected.Units;

                    Mcategory1.KFinTranID = Selected.KFinTranID;
                    Mcategory1.Posted_Date = Selected.Posted_Date;
                    Mcategory1.Description = Selected.Description;
                    Mcategory1.TransAmount = Selected.TransAmount;
                    Mcategory1.ActualAmount = Selected.ActualAmount;
                    Mcategory1.ShortName = Selected.ShortName;
                    Mcategory1.KCategoryID = Selected.KCategoryID;
                    Mcategory1.KFinActualID = Selected.KFinActualID;
                    Mcategory1.DateEffective = Selected.DateEffective;
                    Mcategory1.KPartyID = Selected.KPartyID;
                    Mcategory1.KPartyName = Selected.KPartyName;
                    Mcategory1.IsTemplate = Selected.IsTemplate;
                    Mcategory1.Notes = Selected.Notes;
                    Mcategory1.KAccountID = Account.EditedKid ?? Account.OriginalKid;
                    Mcategory1.KAccountName = (Account.EditedKid == null) ? Account.OriginalName : (Account.EditedName ?? Account.OriginalName) ;
                    Mcategory1.Units = Selected.Units;



                    var u = new TransactionResultApiModel
                    {
                        Posted_Date = DateTime.Parse(TransactionDate),
                        Month = Selected.Month,
                        Description = TransactionDescription.EditedText,
                        TransAmount = IntAmnt,
                        ActualAmount = IntAmnt,
                        ShortName = Category.EditedName ?? Category.OriginalName,
                        KCategoryID = Category.EditedKid ?? Category.OriginalKid,
                        KFinActualID = Selected.KFinActualID,
                        KFinTranID = Selected.KFinTranID,
                        ChangeType = "a",
                        DateEffective = DateTime.Now,
                        KHierarchyID = Selected.KHierarchyID,
                        KClientID = Selected.KClientID,
                        KPartyID = Party.EditedKid ?? Party.OriginalKid,
                        KPartyName = (Party.EditedKid == null) ? Party.OriginalName : (Party.EditedName ?? Party.OriginalName),
                        IsTemplate = IsTemplate,
                        Notes = TransactionNotes.EditedText,
                        KAccountID = Account.EditedKid ?? Account.OriginalKid,
                        KAccountName = (Account.EditedKid == null) ? Account.OriginalName : (Account.EditedName ?? Account.OriginalName) ,
                        Units = IntUnits,
                    };
                    Mtmp2.Add(u);
                    if (docs.Count > 0)
                    {
                        mRequest = new DocDataResultListApiModel();

                        foreach (var item in docs)
                        {
                            var mRqst = new DocDataResultApiModel
                            {
                                DocImage = item.DocImage,
                                DocName = item.DocName,
                                DocURL = item.DocURL,
                                KDocID = item.KDocID,
                                DocDescription = item.DocDescription,
                                FFintranID = Selected.KFinTranID,
                                IsNew = item.IsNew,
                                IsRemove = item.IsRemove,
                            };
                            mRequest.Add(mRqst);
                        }
                        var docsl = mRequest.Where(x => (x.IsNew || x.IsRemove) && x.KDocID != "00000000-0000-0000-0000-000000000000").ToList();
                        if (docsl.Count > 0)
                        {
                            Selected.IsDocLinked = true;
                            Selected1.IsDocLinked = true;
                            TaskManager.RunAndForget(DocumentStorageAsync);
                        }
                    }
                    //return true;
                }
                else
                {
                    if (docs.Count > 0)
                    {
                        mRequest = new DocDataResultListApiModel();
                        foreach (var item in docs)
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
                                IsRemove = item.IsRemove,
                            };
                            mRequest.Add(mRqst);
                        }
                        var docsl = mRequest.Where(x => (x.IsNew || x.IsRemove) && x.KDocID != "00000000-0000-0000-0000-000000000000").ToList();

                        if (docsl.Count > 0)
                        {

                            Selected.IsDocLinked = true;
                            Selected1.IsDocLinked = true;
                            TaskManager.RunAndForget(DocumentStorageAsync);
                        }


                    }
                    ;
                    var TstNotes = false;
                    //if (Math.Abs(IntAmnt) == Math.Abs(Selected.ActualAmount)) { TstEqual = true; }

                    //TstAmnt  is true if absolute value of edited allocation exceeds that of the current allocation for the cost category
                    var TstAmnt = false;
                    if ((TransactionNotes.EditedText != null) && (Selected.Notes != "" || TransactionNotes.EditedText != Selected.Notes))
                    {
                        TstNotes = true;
                        Selected.Notes = TransactionNotes.EditedText;
                    }

                    if (Math.Sign(IntAmnt) != Math.Sign(Selected.TransAmount) && IntAmnt != 0 && Math.Sign(IntAmnt) != Math.Sign(Selected.ActualAmount))
                    {
                        System.Windows.MessageBox.Show($"The allocation amount {Selected.ActualAmount} must be of the same sign ", $"as the transaction total {Selected.TransAmount}");
                        return true;
                    }

                    if (Math.Abs(IntAmnt) > Math.Abs(Selected1.ActualAmount))

                    { TstAmnt = true; }
                    //{ IntAmnt = Selected.ActualAmount; }                

                    if (Math.Sign(IntAmnt) != Math.Sign(Selected.TransAmount) && IntAmnt != 0 && Math.Sign(IntAmnt) != Math.Sign(Selected.ActualAmount))
                    {
                        System.Windows.MessageBox.Show($"The allocation amount {Selected.ActualAmount} must be of the same sign ", $"as the transaction total {Selected.TransAmount}");
                        return true;
                    }

                    if (Math.Abs(IntAmnt) > Math.Abs(Selected1.ActualAmount))

                    { TstAmnt = true; }


                    //Check for any changes made to transaction assignment
                    if ((Category.EditedKid ?? Category.OriginalKid) != Category.OriginalKid || IntAmnt != OrgActual || (Party.EditedKid ?? Party.OriginalKid) != Party.OriginalKid 
                    || (Account.EditedKid ?? Account.OriginalKid) != Account.OriginalKid  || TstNotes || Selected.Description != (TransactionDescription.EditedText ?? TransactionDescription.OriginalText) 
                    || Selected.Posted_Date.Date != DateTime.Parse(TransactionDate).Date || IntUnits != Selected.Units )
                    {


                        //rec.IsTemplate = IsTemplate;
                        //If full amount is not allocated to cost classificaton, create an additional (null) allocation for the remainder
                        //if null allocation already exists, add this new portion


                        //if classificaton being used already exists for this transaction,increase previous allocation
                        Mexists = Mtmp.Where(x => x.KCategoryID == (Category.EditedKid ?? Category.OriginalKid) && x.KFinTranID == Selected.KFinTranID).OrderByDescending(x => x.DateEffective).ToList();
                        Mexxist = Mexists.FirstOrDefault();
                        var IsCategoryUsed = (Mexxist != null);
                        Mexists1 = Mtmp.Where(x => x.KCategoryID == "" && x.KFinTranID == Selected.KFinTranID).OrderByDescending(x => x.DateEffective).ToList();
                        Mexxist1 = Mexists1.FirstOrDefault();
                        Mexists2 = Mtmp1.Where(x => x.KCategoryID == "" && x.KFinTranID == Selected.KFinTranID).OrderByDescending(x => x.DateEffective).ToList();
                        Mexxist2 = Mexists2.FirstOrDefault();
                        var IsEqualiser = (Mexxist1 != null);

                        Mmatches = Mtmp.Where(x => x.KCategoryID == Category.OriginalKid && x.KFinTranID == Selected.KFinTranID).OrderByDescending(x => x.DateEffective).ToList();
                        Mcategory = Mmatches.FirstOrDefault();
                        Mmatches1 = Mtmp1.Where(x => x.KCategoryID == Category.OriginalKid && x.KFinTranID == Selected.KFinTranID).OrderByDescending(x => x.DateEffective).ToList();
                        Mcategory1 = Mmatches1.FirstOrDefault();
                        //Equaliser Allocation can either be of the same sign as the transaction (remainder still available for allocation), or of different sign (where an amount of the opposite sign is required
                        //to balance the 
                        var IsEqualiserSignSame = Mexxist1 != null ? (Math.Sign(Mexxist1.ActualAmount) == Math.Sign(Mexxist1.TransAmount) ): true;
                        var EqualiserValue = 0.00M;
                        if (Mexxist1 != null)
                        {
                            EqualiserValue = Mexxist1.ActualAmount;
                        }

//look for any changes made to the transaction assignment
                        if ((Category.EditedKid ?? Category.OriginalKid) != Category.OriginalKid || IntAmnt != OrgActual || (Party.EditedKid ?? Party.OriginalKid) != Party.OriginalKid || 
                        (Account.EditedKid ?? Account.OriginalKid) != Account.OriginalKid || TstNotes || Selected.Description != (TransactionDescription.EditedText ?? TransactionDescription.OriginalText) || 
                        Selected.Posted_Date.Date != DateTime.Parse(TransactionDate).Date || IntUnits != Selected.Units)
                        {
                            //if ((Category.EditedKid ?? Category.OriginalKid) != Category.OriginalKid && IntAmnt != OrgActual)
                            //{
                                //Same category assigned as previously, but amount assigned has changed
                                if ((Category.EditedKid ?? Category.OriginalKid) == Category.OriginalKid && IntAmnt != OrgActual)
                                    {
                                    //New value has a greater scalar value
                                    if (TstAmnt)
                                    {

                                        if (IsEqualiserSignSame)
                                        //no provision already made for a transaction adjustment?
                                        {
                                            if ((IntAmnt - OrgActual) - EqualiserValue >= 0)
                                            {
                                                //reduce the scalar value of the adjustment
                                                {
                                                    ChangeTranAdjust(IntAmnt, OrgActual, 0);
                                                    ExistCatUsage(IntAmnt, OrgActual, IntUnits);
                                                    //{
                                                    //    Mcategory1.ActualAmount = IntAmnt;
                                                    //    Mcategory1.KCategoryID = Selected.KCategoryID;
                                                    //    Mcategory.ActualAmount = IntAmnt;
                                                    //    Mcategory.ShortName = Selected.ShortName;
                                                    //    Mcategory.KCategoryID = Selected.KCategoryID;
                                                    //    Mcategory.Units = IntUnits;
                                                    //    Selected1.ActualAmount = IntAmnt;
                                                    //    Selected.ActualAmount = IntAmnt;
                                                    //    Selected1.ShortName = Selected.ShortName;
                                                    //    Selected1.KCategoryID = Selected.KCategoryID;
                                                    //    Selected.Units = IntUnits;
                                                    //    Selected1.KFinActualID = Mcategory.KFinActualID;
                                                    //    Selected.KFinActualID = Selected1.KFinActualID;
                                                    //}


                                                    //var u = new TransactionResultApiModel
                                                    //{
                                                    //    Posted_Date = DateTime.Parse(TransactionDate),
                                                    //    Month = Selected1.Month,
                                                    //    Description = TransactionDescription.EditedText ?? Selected.Description,
                                                    //    TransAmount = Selected1.TransAmount,
                                                    //    ActualAmount = Selected1.ActualAmount,
                                                    //    ShortName = (Category.EditedKid == null) ? Category.OriginalName : (Category.EditedName ?? Category.OriginalName),
                                                    //    KCategoryID = Category.EditedKid ?? Category.OriginalKid,
                                                    //    KFinActualID = Selected1.KFinActualID,
                                                    //    KFinTranID = Selected1.KFinTranID,
                                                    //    ChangeType = "c",
                                                    //    DateEffective = DateTime.Now,
                                                    //    KHierarchyID = Selected1.KHierarchyID,
                                                    //    KClientID = Selected.KClientID,
                                                    //    KPartyID = Party.EditedKid ?? Party.OriginalKid,
                                                    //    KPartyName = (Party.EditedKid == null) ? Party.OriginalName : (Party.EditedName ?? Party.OriginalName),
                                                    //    IsTemplate = IsTemplate,
                                                    //    FCatSrchID = Selected1.FCatSrchID,
                                                    //    Notes = TransactionNotes.EditedText,
                                                    //    KAccountID = Account.EditedKid ?? Account.OriginalKid,
                                                    //    KAccountName = (Account.EditedKid == null) ? Account.OriginalName : (Account.EditedName ?? Account.OriginalName),
                                                    //    Units = IntUnits,

                                                    //};
                                                    //Mtmp2.Add(u);


                                                    //{
                                                    //    Mexxist2.ActualAmount = Mexxist2.ActualAmount + (OrgActual - IntAmnt);
                                                    //    Mexxist1.ActualAmount = Mexxist1.ActualAmount + (OrgActual - IntAmnt);
                                                    //}
                                                    //if (Mexxist2.ActualAmount == 0)
                                                    ////Adjustment transaction no longer required
                                                    //{
                                                    //    u = new TransactionResultApiModel
                                                    //    {
                                                    //        Posted_Date = Selected.Posted_Date,
                                                    //        Month = Selected1.Month,
                                                    //        Description = TransactionDescription.EditedText ?? Selected.Description,
                                                    //        TransAmount = Selected1.TransAmount,
                                                    //        ActualAmount = Selected1.ActualAmount,
                                                    //        ShortName = (Category.EditedKid == null) ? Category.OriginalName : (Category.EditedName ?? Category.OriginalName),
                                                    //        KCategoryID = Category.EditedKid ?? Category.OriginalKid,
                                                    //        KFinActualID = Selected1.KFinActualID,
                                                    //        KFinTranID = Selected1.KFinTranID,
                                                    //        ChangeType = "d",
                                                    //        DateEffective = DateTime.Now,
                                                    //        KHierarchyID = Selected1.KHierarchyID,
                                                    //        KClientID = Selected.KClientID,
                                                    //        KPartyID = Party.EditedKid ?? Party.OriginalKid,
                                                    //        KPartyName = (Party.EditedKid == null) ? Party.OriginalName : (Party.EditedName ?? Party.OriginalName),
                                                    //        IsTemplate = IsTemplate,
                                                    //        FCatSrchID = Selected1.FCatSrchID,
                                                    //        Notes = TransactionNotes.EditedText,
                                                    //        KAccountID = Account.EditedKid ?? Account.OriginalKid,
                                                    //        KAccountName = (Account.EditedKid == null) ? Account.OriginalName : (Account.EditedName ?? Account.OriginalName),
                                                    //        Units = IntUnits,

                                                    //    };
                                                    //    Mtmp2.Add(u);
                                                    //    Mtmp1.Remove(Mexxist2);
                                                    //    Mtmp.Remove(Mexxist1);
                                                    //}
                                                    //else
                                                    //{
                                                    //    u = new TransactionResultApiModel
                                                    //    {
                                                    //        Posted_Date = DateTime.Parse(TransactionDate),
                                                    //        Month = Selected1.Month,
                                                    //        Description = TransactionDescription.EditedText ?? Selected.Description,
                                                    //        TransAmount = Selected1.TransAmount,
                                                    //        ActualAmount = Selected1.ActualAmount,
                                                    //        ShortName = (Category.EditedKid == null) ? Category.OriginalName : (Category.EditedName ?? Category.OriginalName),
                                                    //        KCategoryID = Category.EditedKid ?? Category.OriginalKid,
                                                    //        KFinActualID = Selected1.KFinActualID,
                                                    //        KFinTranID = Selected1.KFinTranID,
                                                    //        ChangeType = "c",
                                                    //        DateEffective = DateTime.Now,
                                                    //        KHierarchyID = Selected1.KHierarchyID,
                                                    //        KClientID = Selected.KClientID,
                                                    //        KPartyID = Party.EditedKid ?? Party.OriginalKid,
                                                    //        KPartyName = (Party.EditedKid == null) ? Party.OriginalName : (Party.EditedName ?? Party.OriginalName),
                                                    //        IsTemplate = IsTemplate,
                                                    //        FCatSrchID = Selected1.FCatSrchID,
                                                    //        Notes = TransactionNotes.EditedText,
                                                    //        KAccountID = Account.EditedKid ?? Account.OriginalKid,
                                                    //        KAccountName = (Account.EditedKid == null) ? Account.OriginalName : (Account.EditedName ?? Account.OriginalName),
                                                    //        Units = IntUnits,

                                                    //    };
                                                    //Mtmp2.Add(u);
                                                    //}

                                                }
                                            }
                                            else
                                            {
                                                //either limit current allocation to amount of unallocated balance or authorise transaction adjustment for excess
                                                var result = System.Windows.MessageBox.Show("Create adjustment entry (Y) to extend Transaction balance, limit to Transaction balance(N)", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
                                                switch (result)
                                                {
                                                    case MessageBoxResult.Yes:

                                                        {

                                                        if (Mexxist2 == null)
                                                        //Create new null category - existing null category being reassigned
                                                        {
                                                            AddTranAdjust(IntAmnt, OrgActual, 0);
                                                        }
                                                        else
                                                        {
                                                            ChangeTranAdjust(IntAmnt, OrgActual, 0);
                                                        }
                                                        ExistCatUsage(IntAmnt, OrgActual, IntUnits);
                                                        //{
                                                        //        Mcategory1.ActualAmount = IntAmnt;
                                                        //        Mcategory1.KCategoryID = Selected.KCategoryID;
                                                        //       Mcategory.ActualAmount = IntAmnt;
                                                        //       Mcategory.ShortName = Selected.ShortName;
                                                        //       Mcategory.KCategoryID = Selected.KCategoryID;
                                                        //       Mcategory.Units = IntUnits;
                                                        //        Selected1.ActualAmount = IntAmnt;
                                                        //        Selected.ActualAmount = IntAmnt;
                                                        //        Selected1.ShortName = Selected.ShortName;
                                                        //        Selected1.KCategoryID = Selected.KCategoryID;
                                                        //        Selected.Units = IntUnits;
                                                        //        Selected1.KFinActualID = Mcategory.KFinActualID;
                                                        //        Selected.KFinActualID = Selected1.KFinActualID;
                                                        //    }


                                                        //    var u = new TransactionResultApiModel
                                                        //    {
                                                        //        Posted_Date = DateTime.Parse(TransactionDate),
                                                        //        Month = Selected1.Month,
                                                        //        Description = TransactionDescription.EditedText ?? Selected.Description,
                                                        //        TransAmount = Selected1.TransAmount,
                                                        //        ActualAmount = Selected1.ActualAmount,
                                                        //        ShortName = (Category.EditedKid == null) ? Category.OriginalName : (Category.EditedName ?? Category.OriginalName),
                                                        //        KCategoryID = Category.EditedKid ?? Category.OriginalKid,
                                                        //        KFinActualID = Selected1.KFinActualID,
                                                        //        KFinTranID = Selected1.KFinTranID,
                                                        //        ChangeType = "c",
                                                        //        DateEffective = DateTime.Now,
                                                        //        KHierarchyID = Selected1.KHierarchyID,
                                                        //        KClientID = Selected.KClientID,
                                                        //        KPartyID = Party.EditedKid ?? Party.OriginalKid,
                                                        //        KPartyName = (Party.EditedKid == null) ? Party.OriginalName : (Party.EditedName ?? Party.OriginalName),
                                                        //        IsTemplate = IsTemplate,
                                                        //        FCatSrchID = Selected1.FCatSrchID,
                                                        //        Notes = TransactionNotes.EditedText,
                                                        //        KAccountID = Account.EditedKid ?? Account.OriginalKid,
                                                        //        KAccountName = (Account.EditedKid == null) ? Account.OriginalName : (Account.EditedName ?? Account.OriginalName),
                                                        //        Units = IntUnits,

                                                        //    };
                                                        //    Mtmp2.Add(u);

                                                        //    if (Mexxist2 != null)
                                                        //    {

                                                        //        {
                                                        //            Mexxist2.ActualAmount = Mexxist2.ActualAmount + (OrgActual - IntAmnt);
                                                        //            Mexxist1.ActualAmount = Mexxist1.ActualAmount + (OrgActual - IntAmnt);
                                                        //        }
                                                        //        u = new TransactionResultApiModel
                                                        //        {
                                                        //            Posted_Date = DateTime.Parse(TransactionDate),
                                                        //            Month = Selected1.Month,
                                                        //            Description = TransactionDescription.EditedText ?? Selected.Description,
                                                        //            TransAmount = Selected1.TransAmount,
                                                        //            ActualAmount = Selected1.ActualAmount,
                                                        //            ShortName = (Category.EditedKid == null) ? Category.OriginalName : (Category.EditedName ?? Category.OriginalName),
                                                        //            KCategoryID = Category.EditedKid ?? Category.OriginalKid,
                                                        //            KFinActualID = Selected1.KFinActualID,
                                                        //            KFinTranID = Selected1.KFinTranID,
                                                        //            ChangeType = "c",
                                                        //            DateEffective = DateTime.Now,
                                                        //            KHierarchyID = Selected1.KHierarchyID,
                                                        //            KClientID = Selected.KClientID,
                                                        //            KPartyID = Party.EditedKid ?? Party.OriginalKid,
                                                        //            KPartyName = (Party.EditedKid == null) ? Party.OriginalName : (Party.EditedName ?? Party.OriginalName),
                                                        //            IsTemplate = IsTemplate,
                                                        //            FCatSrchID = Selected1.FCatSrchID,
                                                        //            Notes = TransactionNotes.EditedText,
                                                        //            KAccountID = Account.EditedKid ?? Account.OriginalKid,
                                                        //            KAccountName = (Account.EditedKid == null) ? Account.OriginalName : (Account.EditedName ?? Account.OriginalName),
                                                        //            Units = IntUnits,

                                                        //        };
                                                        //        Mtmp2.Add(u);
                                                            //}
                                                            //else

                                                            //{
                                                            //    Selected1.ActualAmount = (OrgActual - IntAmnt);
                                                            //    Selected1.ShortName = "";
                                                            //    Selected1.KCategoryID = "";
                                                            //    Selected1.KFinActualID = Guid.NewGuid().ToString().ToUpper();
                                                            //    Selected1.Description = Selected.Description;
                                                            //    Selected1.KFinTranID = Selected.KFinTranID;
                                                            //    Selected1.KChangeID = Selected.KChangeID;
                                                            //    Selected1.Posted_Date = Selected.Posted_Date;
                                                            //    Selected1.Month = Selected.Month;
                                                            //    Selected1.TransAmount = Selected.TransAmount;
                                                            //    Selected1.Units = 0;
                                                            //    Selected1.KClientID = Selected.KClientID;
                                                            //    Selected1.KPartyID = Selected.KPartyID;
                                                            //    Selected1.KPartyName = (Party.EditedKid == null) ? Party.OriginalName : (Party.EditedName ?? Party.OriginalName);
                                                            //    Selected1.KAccountID = Selected.KAccountID;
                                                            //    Selected1.KAccountName = (Account.EditedKid == null) ? Account.OriginalName : (Account.EditedName ?? Account.OriginalName);

                                                            //    //Mtmp.Add(Selected1);
                                                            //    Mtmp.Add(Selected1);
                                                            //    Mtmp1.Add(Selected1);
                                                            //    u = new TransactionResultApiModel
                                                            //    {
                                                            //        Posted_Date = DateTime.Parse(TransactionDate),
                                                            //        Month = Selected1.Month,
                                                            //        Description = Selected1.Description,
                                                            //        TransAmount = Selected1.TransAmount,
                                                            //        ActualAmount = Selected1.ActualAmount,
                                                            //        ShortName = "",
                                                            //        KCategoryID = "",
                                                            //        KFinActualID = Selected1.KFinActualID,
                                                            //        KFinTranID = Selected1.KFinTranID,
                                                            //        ChangeType = "a",
                                                            //        DateEffective = DateTime.Now,
                                                            //        KHierarchyID = Selected1.KHierarchyID,
                                                            //        KClientID = Selected1.KClientID,
                                                            //        KPartyID = Selected1.KPartyID,
                                                            //        KPartyName = Selected1.KPartyName,
                                                            //        IsTemplate = IsTemplate,
                                                            //        KAccountID = Selected1.KAccountID,
                                                            //        KAccountName = (Account.EditedKid == null) ? Account.OriginalName : (Account.EditedName ?? Account.OriginalName),
                                                            //        Units = 0,

                                                            //    };
                                                            //    Mtmp2.Add(u);
                                                            //}
                                                        }

                                                        break;
                                                    case MessageBoxResult.No:
                                                        {
                                                            {
                                                                Mcategory1.ActualAmount = IntAmnt;
                                                                Mcategory1.KCategoryID = Selected.KCategoryID;
                                                               Mcategory.ActualAmount = IntAmnt;
                                                               Mcategory.ShortName = Selected.ShortName;
                                                               Mcategory.KCategoryID = Selected.KCategoryID;
                                                               Mcategory.Units = IntUnits;
                                                                Selected1.ActualAmount = IntAmnt;
                                                                Selected.ActualAmount = IntAmnt;
                                                                Selected1.ShortName = Selected.ShortName;
                                                                Selected1.KCategoryID = Selected.KCategoryID;
                                                                Selected.Units = IntUnits;
                                                                Selected1.KFinActualID =Mcategory.KFinActualID;
                                                                Selected.KFinActualID = Selected1.KFinActualID;
                                                            }


                                                            var u = new TransactionResultApiModel
                                                            {
                                                                Posted_Date = DateTime.Parse(TransactionDate),
                                                                Month = Selected1.Month,
                                                                Description = TransactionDescription.EditedText ?? Selected.Description,
                                                                TransAmount = Selected1.TransAmount,
                                                                ActualAmount = Selected1.ActualAmount,
                                                                ShortName = (Category.EditedKid == null) ? Category.OriginalName : (Category.EditedName ?? Category.OriginalName),
                                                                KCategoryID = Category.EditedKid ?? Category.OriginalKid,
                                                                KFinActualID = Selected1.KFinActualID,
                                                                KFinTranID = Selected1.KFinTranID,
                                                                ChangeType = "c",
                                                                DateEffective = DateTime.Now,
                                                                KHierarchyID = Selected1.KHierarchyID,
                                                                KClientID = Selected.KClientID,
                                                                KPartyID = Party.EditedKid ?? Party.OriginalKid,
                                                                KPartyName = (Party.EditedKid == null) ? Party.OriginalName : (Party.EditedName ?? Party.OriginalName),
                                                                IsTemplate = IsTemplate,
                                                                FCatSrchID = Selected1.FCatSrchID,
                                                                Notes = TransactionNotes.EditedText,
                                                                KAccountID = Account.EditedKid ?? Account.OriginalKid,
                                                                KAccountName = (Account.EditedKid == null) ? Account.OriginalName : (Account.EditedName ?? Account.OriginalName),
                                                                Units = IntUnits,

                                                            };
                                                            Mtmp2.Add(u);


                                                            {
                                                                Mexxist2.ActualAmount = Mexxist2.ActualAmount + (OrgActual - IntAmnt);
                                                                Mexxist1.ActualAmount = Mexxist1.ActualAmount + (OrgActual - IntAmnt);
                                                            }
                                                            u = new TransactionResultApiModel
                                                            {
                                                                Posted_Date = DateTime.Parse(TransactionDate),
                                                                Month = Selected1.Month,
                                                                Description = TransactionDescription.EditedText ?? Selected.Description,
                                                                TransAmount = Selected1.TransAmount,
                                                                ActualAmount = Selected1.ActualAmount,
                                                                ShortName = (Category.EditedKid == null) ? Category.OriginalName : (Category.EditedName ?? Category.OriginalName),
                                                                KCategoryID = Category.EditedKid ?? Category.OriginalKid,
                                                                KFinActualID = Selected1.KFinActualID,
                                                                KFinTranID = Selected1.KFinTranID,
                                                                ChangeType = "c",
                                                                DateEffective = DateTime.Now,
                                                                KHierarchyID = Selected1.KHierarchyID,
                                                                KClientID = Selected.KClientID,
                                                                KPartyID = Party.EditedKid ?? Party.OriginalKid,
                                                                KPartyName = (Party.EditedKid == null) ? Party.OriginalName : (Party.EditedName ?? Party.OriginalName),
                                                                IsTemplate = IsTemplate,
                                                                FCatSrchID = Selected1.FCatSrchID,
                                                                Notes = TransactionNotes.EditedText,
                                                                KAccountID = Account.EditedKid ?? Account.OriginalKid,
                                                                KAccountName = (Account.EditedKid == null) ? Account.OriginalName : (Account.EditedName ?? Account.OriginalName),
                                                                Units = IntUnits,

                                                            };
                                                            Mtmp2.Add(u);
                                                        }
                                                        break;
                                                    default:
                                                        break;
                                                }


                                            }
                                        }
                                        else
                                        // a transaction adjustment has already been created
                                        {
                                            var result = System.Windows.MessageBox.Show("Increase adjustment entry (Y) to extend Transaction balance, or ignore allocation change(N)", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
                                            switch (result)
                                            {
                                                case MessageBoxResult.Yes:

                                                    {
                                                    if (Mexxist2 == null)
                                                    //Create new null category - existing null category being reassigned
                                                    {
                                                        AddTranAdjust(IntAmnt, OrgActual, 0);
                                                    }
                                                    else
                                                    {
                                                        ChangeTranAdjust(IntAmnt, OrgActual, 0);
                                                    }
                                                    {
                                                            Mcategory1.ActualAmount = IntAmnt;
                                                            Mcategory1.KCategoryID = Selected.KCategoryID;
                                                            Mcategory.ActualAmount = IntAmnt;
                                                            Mcategory.ShortName = Selected.ShortName;
                                                            Mcategory.KCategoryID = Selected.KCategoryID;
                                                            Mcategory.Units = IntUnits;
                                                            Selected1.ActualAmount = IntAmnt;
                                                            Selected.ActualAmount = IntAmnt;
                                                            Selected1.ShortName = Selected.ShortName;
                                                            Selected1.KCategoryID = Selected.KCategoryID;
                                                            Selected.Units = IntUnits;
                                                            Selected1.KFinActualID = Mcategory.KFinActualID;
                                                            Selected.KFinActualID = Selected1.KFinActualID;
                                                        }


                                                        var u = new TransactionResultApiModel
                                                        {
                                                            Posted_Date = DateTime.Parse(TransactionDate),
                                                            Month = Selected1.Month,
                                                            Description = TransactionDescription.EditedText ?? Selected.Description,
                                                            TransAmount = Selected1.TransAmount,
                                                            ActualAmount = Selected1.ActualAmount,
                                                            ShortName = (Category.EditedKid == null) ? Category.OriginalName : (Category.EditedName ?? Category.OriginalName),
                                                            KCategoryID = Category.EditedKid ?? Category.OriginalKid,
                                                            KFinActualID = Selected1.KFinActualID,
                                                            KFinTranID = Selected1.KFinTranID,
                                                            ChangeType = "c",
                                                            DateEffective = DateTime.Now,
                                                            KHierarchyID = Selected1.KHierarchyID,
                                                            KClientID = Selected.KClientID,
                                                            KPartyID = Party.EditedKid ?? Party.OriginalKid,
                                                            KPartyName = (Party.EditedKid == null) ? Party.OriginalName : (Party.EditedName ?? Party.OriginalName),
                                                            IsTemplate = IsTemplate,
                                                            FCatSrchID = Selected1.FCatSrchID,
                                                            Notes = TransactionNotes.EditedText,
                                                            KAccountID = Account.EditedKid ?? Account.OriginalKid,
                                                            KAccountName = (Account.EditedKid == null) ? Account.OriginalName : (Account.EditedName ?? Account.OriginalName),
                                                            Units = IntUnits,

                                                        };
                                                        Mtmp2.Add(u);


                                                        {
                                                            Mexxist2.ActualAmount = Mexxist2.ActualAmount + (OrgActual - IntAmnt);
                                                            Mexxist1.ActualAmount = Mexxist1.ActualAmount + (OrgActual - IntAmnt);
                                                        }
                                                        u = new TransactionResultApiModel
                                                        {
                                                            Posted_Date = DateTime.Parse(TransactionDate),
                                                            Month = Selected1.Month,
                                                            Description = TransactionDescription.EditedText ?? Selected.Description,
                                                            TransAmount = Selected1.TransAmount,
                                                            ActualAmount = Mexxist2.ActualAmount,
                                                            ShortName = "",
                                                            KCategoryID = "",
                                                            KFinActualID = Selected1.KFinActualID,
                                                            KFinTranID = Selected1.KFinTranID,
                                                            ChangeType = "c",
                                                            DateEffective = DateTime.Now,
                                                            KHierarchyID = Selected1.KHierarchyID,
                                                            KClientID = Selected.KClientID,
                                                            KPartyID = Party.EditedKid ?? Party.OriginalKid,
                                                            KPartyName = (Party.EditedKid == null) ? Party.OriginalName : (Party.EditedName ?? Party.OriginalName),
                                                            IsTemplate = IsTemplate,
                                                            FCatSrchID = Selected1.FCatSrchID,
                                                            Notes = "",
                                                            KAccountID = Account.EditedKid ?? Account.OriginalKid,
                                                            KAccountName = (Account.EditedKid == null) ? Account.OriginalName : (Account.EditedName ?? Account.OriginalName),
                                                            Units = 0,

                                                        };
                                                        Mtmp2.Add(u);
                                                    }

                                                    break;
                                                case MessageBoxResult.No:
                                                    Allocation.OriginalText = OrgActual.ToString("C", CultureInfo.CurrentCulture);
                                                    Allocation.EditedText = OrgActual.ToString("C", CultureInfo.CurrentCulture);
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                    }
                                    //New value has a lesser scalar value
                                    else
                                    {
                                        //scalar value of assignment reduced and no existing credit adjustment
                                        if (IsEqualiserSignSame)
                                        {
                                        if (Mexxist2 == null)
                                            //Create new null category - existing null category being reassigned
                                            {
                                                AddTranAdjust(IntAmnt, OrgActual, 0);
                                            }
                                            else
                                            {
                                                ChangeTranAdjust(IntAmnt, OrgActual, 0);
                                            }


                                            PriorCatUsage(IntAmnt, OrgActual, IntUnits);

                                            ////increase the scalar value of the adjustment
                                            //{
                                            //    {
                                            //        Mcategory1.ActualAmount = IntAmnt;
                                            //        Mcategory1.KCategoryID = Selected.KCategoryID;
                                            //        Mcategory.ActualAmount = IntAmnt;
                                            //        Mcategory.ShortName = Selected.ShortName;
                                            //        Mcategory.KCategoryID = Selected.KCategoryID;
                                            //        Mcategory.Units = IntUnits;
                                            //        Selected1.ActualAmount = IntAmnt;
                                            //        Selected.ActualAmount = IntAmnt;
                                            //        Selected1.ShortName = Selected.ShortName;
                                            //        Selected1.KCategoryID = Selected.KCategoryID;
                                            //        Selected.Units = IntUnits;
                                            //        Selected1.KFinActualID = Mcategory.KFinActualID;
                                            //        Selected.KFinActualID = Selected1.KFinActualID;
                                            //    }


                                            //    var u = new TransactionResultApiModel
                                            //    {
                                            //        Posted_Date = DateTime.Parse(TransactionDate),
                                            //        Month = Selected1.Month,
                                            //        Description = TransactionDescription.EditedText ?? Selected.Description,
                                            //        TransAmount = Selected1.TransAmount,
                                            //        ActualAmount = Selected1.ActualAmount,
                                            //        ShortName = (Category.EditedKid == null) ? Category.OriginalName : (Category.EditedName ?? Category.OriginalName),
                                            //        KCategoryID = Category.EditedKid ?? Category.OriginalKid,
                                            //        KFinActualID = Selected1.KFinActualID,
                                            //        KFinTranID = Selected1.KFinTranID,
                                            //        ChangeType = "c",
                                            //        DateEffective = DateTime.Now,
                                            //        KHierarchyID = Selected1.KHierarchyID,
                                            //        KClientID = Selected.KClientID,
                                            //        KPartyID = Party.EditedKid ?? Party.OriginalKid,
                                            //        KPartyName = (Party.EditedKid == null) ? Party.OriginalName : (Party.EditedName ?? Party.OriginalName),
                                            //        IsTemplate = IsTemplate,
                                            //        FCatSrchID = Selected1.FCatSrchID,
                                            //        Notes = TransactionNotes.EditedText,
                                            //        KAccountID = Account.EditedKid ?? Account.OriginalKid,
                                            //        KAccountName = (Account.EditedKid == null) ? Account.OriginalName : (Account.EditedName ?? Account.OriginalName),
                                            //        Units = IntUnits,

                                            //    };
                                            //    Mtmp2.Add(u);

                                            //    if (Mexxist1 != null)
                                            //    //no adjustment currently exists for transaction
                                            //    {
                                            //        Mexxist1.ActualAmount += (OrgActual - IntAmnt);
                                            //        Mexxist2.ActualAmount += (OrgActual - IntAmnt);
                                            //        {
                                            //            u = new TransactionResultApiModel
                                            //            {
                                            //                Posted_Date = Mexxist1.Posted_Date,
                                            //                Month = Mexxist1.Month,
                                            //                Description = Mexxist1.Description,
                                            //                TransAmount = Mexxist1.TransAmount,
                                            //                ActualAmount = Mexxist1.ActualAmount,
                                            //                ShortName = Mexxist1.ShortName,
                                            //                KCategoryID = Mexxist1.KCategoryID,
                                            //                KFinActualID = Mexxist1.KFinActualID,
                                            //                KFinTranID = Mexxist1.KFinTranID,
                                            //                ChangeType = "c",
                                            //                DateEffective = DateTime.Now,
                                            //                KHierarchyID = Mexxist1.KHierarchyID,
                                            //                KClientID = Mexxist1.KClientID,
                                            //                KPartyID = Mexxist1.KPartyID,
                                            //                KPartyName = Mexxist1.KPartyName,
                                            //                IsTemplate = Mexxist1.IsTemplate,
                                            //                FCatSrchID = Mexxist1.FCatSrchID,
                                            //                Notes = Mexxist1.Notes,
                                            //                KAccountID = Mexxist1.KAccountID,
                                            //                KAccountName = (Account.EditedKid == null) ? Account.OriginalName : (Account.EditedName ?? Account.OriginalName) ,
                                            //                Units = Mexxist1.Units,

                                            //            };
                                            //            Mtmp2.Add(u);
                                            //        }

                                            //    }
                                            //    else
                                            //    //no adjustment currently exists at transaction level - add 
                                            //    {
                                            //        Selected1.ActualAmount = (OrgActual - IntAmnt);
                                            //        Selected1.ShortName = "";
                                            //        Selected1.KCategoryID = "";
                                            //        Selected1.KFinActualID = Guid.NewGuid().ToString().ToUpper();
                                            //        Selected1.Description = Selected.Description;
                                            //        Selected1.KFinTranID = Selected.KFinTranID;
                                            //        Selected1.KChangeID = Selected.KChangeID;
                                            //        Selected1.Posted_Date = Selected.Posted_Date;
                                            //        Selected1.Month = Selected.Month;
                                            //        Selected1.TransAmount = Selected.TransAmount;
                                            //        Selected1.Units = 0;
                                            //        Selected1.KClientID = Selected.KClientID;
                                            //        Selected1.KPartyID = Selected.KPartyID;
                                            //        Selected1.KPartyName = (Party.EditedKid == null) ? Party.OriginalName : (Party.EditedName ?? Party.OriginalName);
                                            //        Selected1.KAccountID = Selected.KAccountID;
                                            //        Selected1.KAccountName = (Account.EditedKid == null) ? Account.OriginalName : (Account.EditedName ?? Account.OriginalName);

                                            //        //Mtmp.Add(Selected1);
                                            //        Mtmp.Add(Selected1);
                                            //        Mtmp1.Add(Selected1);
                                            //        u = new TransactionResultApiModel
                                            //        {
                                            //            Posted_Date = DateTime.Parse(TransactionDate),
                                            //            Month = Selected1.Month,
                                            //            Description = Selected1.Description,
                                            //            TransAmount = Selected1.TransAmount,
                                            //            ActualAmount = Selected1.ActualAmount,
                                            //            ShortName = "",
                                            //            KCategoryID = "",
                                            //            KFinActualID = Selected1.KFinActualID,
                                            //            KFinTranID = Selected1.KFinTranID,
                                            //            ChangeType = "a",
                                            //            DateEffective = DateTime.Now,
                                            //            KHierarchyID = Selected1.KHierarchyID,
                                            //            KClientID = Selected1.KClientID,
                                            //            KPartyID = Selected1.KPartyID,
                                            //            KPartyName = Selected1.KPartyName,
                                            //            IsTemplate = IsTemplate,
                                            //            KAccountID = Selected1.KAccountID,
                                            //            KAccountName = (Account.EditedKid == null) ? Account.OriginalName : (Account.EditedName ?? Account.OriginalName),
                                            //            Units = 0,

                                            //        };
                                            //    }

                                            //    Mtmp2.Add(u);

                                            //}
                                        }

                                        else
                                        {

                                        //if scalar value of allocation is less, and opposite sign transaction adjustment had already been created,change value of adjustment
                                        ChangeTranAdjust(IntAmnt, OrgActual, 0);
                                        ExistCatUsage(IntAmnt, OrgActual, IntUnits);
                                            //{
                                            //    {
                                            //        Mcategory1.ActualAmount = IntAmnt;
                                            //        Mcategory1.KCategoryID = Selected.KCategoryID;
                                            //        Mcategory.ActualAmount = IntAmnt;
                                            //        Mcategory.ShortName = Selected.ShortName;
                                            //        Mcategory.KCategoryID = Selected.KCategoryID;
                                            //        Mcategory.Units = IntUnits;
                                            //        Selected1.ActualAmount = IntAmnt;
                                            //        Selected.ActualAmount = IntAmnt;
                                            //        Selected1.ShortName = Selected.ShortName;
                                            //        Selected1.KCategoryID = Selected.KCategoryID;
                                            //        Selected.Units = IntUnits;
                                            //        Selected1.KFinActualID = Mcategory.KFinActualID;
                                            //        Selected.KFinActualID = Selected1.KFinActualID;
                                            //    }


                                            //    var u = new TransactionResultApiModel
                                            //    {
                                            //        Posted_Date = DateTime.Parse(TransactionDate),
                                            //        Month = Selected1.Month,
                                            //        Description = TransactionDescription.EditedText ?? Selected.Description,
                                            //        TransAmount = Selected1.TransAmount,
                                            //        ActualAmount = Selected1.ActualAmount,
                                            //        ShortName = (Category.EditedKid == null) ? Category.OriginalName : (Category.EditedName ?? Category.OriginalName),
                                            //        KCategoryID = Category.EditedKid ?? Category.OriginalKid,
                                            //        KFinActualID = Selected1.KFinActualID,
                                            //        KFinTranID = Selected1.KFinTranID,
                                            //        ChangeType = "c",
                                            //        DateEffective = DateTime.Now,
                                            //        KHierarchyID = Selected1.KHierarchyID,
                                            //        KClientID = Selected.KClientID,
                                            //        KPartyID = Party.EditedKid ?? Party.OriginalKid,
                                            //        KPartyName = (Party.EditedKid == null) ? Party.OriginalName : (Party.EditedName ?? Party.OriginalName),
                                            //        IsTemplate = IsTemplate,
                                            //        FCatSrchID = Selected1.FCatSrchID,
                                            //        Notes = TransactionNotes.EditedText,
                                            //        KAccountID = Account.EditedKid ?? Account.OriginalKid,
                                            //        KAccountName = (Account.EditedKid == null) ? Account.OriginalName : (Account.EditedName ?? Account.OriginalName),
                                            //        Units = IntUnits,

                                            //    };
                                            //    Mtmp2.Add(u);


                                            //    {
                                            //        Mexxist2.ActualAmount = Mexxist2.ActualAmount + (OrgActual - IntAmnt);
                                            //        Mexxist1.ActualAmount = Mexxist1.ActualAmount + (OrgActual - IntAmnt);
                                            //    }
                                            //    if (Mexxist1.ActualAmount == 0)
                                            //    {

                                            //        u = new TransactionResultApiModel
                                            //        {
                                            //            TransAmount = Mexxist1.TransAmount,
                                            //            ActualAmount = Mexxist1.ActualAmount,
                                            //            ShortName = Mexxist1.ShortName,
                                            //            KCategoryID = Mexxist1.KCategoryID,
                                            //            KFinActualID = Mexxist1.KFinActualID,
                                            //            KFinTranID = Mexxist1.KFinTranID,
                                            //            ChangeType = "d",
                                            //            DateEffective = DateTime.Now,
                                            //            KHierarchyID = Mexxist1.KHierarchyID,
                                            //            KClientID = Mexxist1.KClientID,
                                            //            KPartyID = Mexxist1.KPartyID,
                                            //            KPartyName = Mexxist1.KPartyName,
                                            //            IsTemplate = Mexxist1.IsTemplate,
                                            //            FCatSrchID = Mexxist1.FCatSrchID,
                                            //            Notes = Mexxist1.Notes,
                                            //            KAccountID = Mexxist1.KAccountID,
                                            //            KAccountName = (Account.EditedKid == null) ? Account.OriginalName : (Account.EditedName ?? Account.OriginalName),
                                            //            Units = Mexxist1.Units,
                                            //            Posted_Date = Mexxist1.Posted_Date,
                                            //            Description = Mexxist1.Description,

                                            //        };
                                            //        Mtmp2.Add(u);
                                            //        Mtmp1.Remove(Mexxist2);
                                            //        Mtmp.Remove(Mexxist1);
                                            //    }
                                            //    else
                                            //    {
                                            //        u = new TransactionResultApiModel
                                            //        {
                                            //            TransAmount = Mexxist1.TransAmount,
                                            //            ActualAmount = Mexxist1.ActualAmount,
                                            //            ShortName = Mexxist1.ShortName,
                                            //            KCategoryID = Mexxist1.KCategoryID,
                                            //            KFinActualID = Mexxist1.KFinActualID,
                                            //            KFinTranID = Mexxist1.KFinTranID,
                                            //            ChangeType = "c",
                                            //            DateEffective = DateTime.Now,
                                            //            KHierarchyID = Mexxist1.KHierarchyID,
                                            //            KClientID = Mexxist1.KClientID,
                                            //            KPartyID = Mexxist1.KPartyID,
                                            //            KPartyName = Mexxist1.KPartyName,
                                            //            IsTemplate = Mexxist1.IsTemplate,
                                            //            FCatSrchID = Mexxist1.FCatSrchID,
                                            //            Notes = Mexxist1.Notes,
                                            //            KAccountID = Mexxist1.KAccountID,
                                            //            KAccountName = (Account.EditedKid == null) ? Account.OriginalName : (Account.EditedName ?? Account.OriginalName),
                                            //            Units = Mexxist1.Units,
                                            //            Description = TransactionDescription.EditedText ?? Selected.Description,
                                            //            Posted_Date = Mexxist1.Posted_Date,

                                            //        };
                                            //        Mtmp2.Add(u);
                                            //    }

                                            //}

                                        }
                                    }
                                    }
                                else
                                //Assignment category has changed and assignment total unchanged - Move entire original assignment to new category
                                {
                                //return true;
                                    if ((Category.EditedKid ?? Category.OriginalKid) != Category.OriginalKid)
                                    //exclude cases where category is unchanged
                                    {
                                        if (IntAmnt == OrgActual)
                                        //Just change category, no further manipulation required
                                        {
                                        PriorCatUsage(IntAmnt, OrgActual, IntUnits);
                                        //remove current assignment record as information has been merged with original
                                        //var u = new TransactionResultApiModel
                                        //{
                                        //    Posted_Date = Mcategory1.Posted_Date,
                                        //    Month =Mcategory1.Month,
                                        //    Description = Mcategory1.Description,
                                        //    TransAmount =Mcategory1.TransAmount,
                                        //    ActualAmount =Mcategory1.ActualAmount,
                                        //    ShortName = Mcategory1.ShortName,
                                        //    KCategoryID = Mcategory1.KCategoryID,
                                        //    KFinActualID =Mcategory1.KFinActualID,
                                        //    KFinTranID =Mcategory1.KFinTranID,
                                        //    ChangeType = "d",
                                        //    DateEffective = DateTime.Now,
                                        //    KHierarchyID =Mcategory1.KHierarchyID,
                                        //    KClientID = Mcategory1.KClientID,
                                        //    KPartyID = Mcategory1.KClientID,
                                        //    KPartyName = Mcategory1.KPartyName,
                                        //    IsTemplate = IsTemplate,
                                        //    FCatSrchID =Mcategory1.FCatSrchID,
                                        //    Notes = TransactionNotes.EditedText,
                                        //    KAccountID = Mcategory1.KAccountID,
                                        //    KAccountName = Mcategory1.KAccountName,
                                        //    Units = IntUnits,

                                        //};
                                        //Mtmp2.Add(u);
                                        //Mtmp1.Remove(Mcategory1);
                                        //Mtmp.Remove(Mcategory);

                                        //{
                                        //    {
                                        //        Mcategory1.ActualAmount = IntAmnt;
                                        //        Mcategory1.KCategoryID = (Category.EditedKid == null) ? Category.OriginalKid : (Category.EditedKid ?? Category.OriginalKid);
                                        //        Mcategory1.ShortName = (Category.EditedKid == null) ? Category.OriginalName : (Category.EditedName ?? Category.OriginalName);
                                        //        Mcategory1.Units = IntUnits;
                                        //        Mcategory1.KPartyID = (Party.EditedKid == null) ? Party.OriginalKid  : (Party.EditedKid ?? Party.OriginalKid);
                                        //        Mcategory1.KPartyName = (Party.EditedKid == null) ? Party.OriginalName : (Party.EditedName ?? Party.OriginalName);   
                                        //        Mcategory1.KAccountID = (Account.EditedKid == null) ? Account.OriginalKid : (Account.EditedKid ?? Account.OriginalKid);
                                        //        Mcategory1.KAccountName = (Account.EditedKid == null) ? Account.OriginalName : (Account.EditedName ?? Account.OriginalName);                                                  

                                        //        Mcategory.ActualAmount = IntAmnt;
                                        //        Mcategory.KCategoryID = (Category.EditedKid == null) ? Category.OriginalKid : (Category.EditedKid ?? Category.OriginalKid);
                                        //        Mcategory.ShortName = (Category.EditedKid == null) ? Category.OriginalName : (Category.EditedName ?? Category.OriginalName);
                                        //        Mcategory.Units = IntUnits;
                                        //        Mcategory.KPartyID =(Party.EditedKid == null) ? Party.OriginalKid : (Party.EditedKid ?? Party.OriginalKid);
                                        //        Mcategory.KPartyName = (Party.EditedKid == null) ? Party.OriginalName : (Party.EditedName ?? Party.OriginalName);
                                        //        Mcategory.KAccountID = (Account.EditedKid == null) ? Account.OriginalKid : (Account.EditedKid ?? Account.OriginalKid);
                                        //        Mcategory.KAccountName = (Account.EditedKid == null) ? Account.OriginalName : (Account.EditedName ?? Account.OriginalName);

                                        //        Selected1.ActualAmount = IntAmnt;
                                        //        Selected.ActualAmount = IntAmnt;
                                        //        Selected.ShortName = Mcategory.ShortName;
                                        //        Selected.KCategoryID = Mcategory.KCategoryID;
                                        //        Selected.Units = IntUnits;
                                        //    }


                                        //    var u = new TransactionResultApiModel
                                        //    {
                                        //        Posted_Date = DateTime.Parse(TransactionDate),
                                        //        Month = Selected1.Month,
                                        //        Description = TransactionDescription.EditedText ?? Selected.Description,
                                        //        TransAmount = Selected.TransAmount,
                                        //        ActualAmount = Selected.ActualAmount,
                                        //        ShortName = Mcategory.ShortName,
                                        //        KCategoryID = Mcategory1.KCategoryID,
                                        //        KFinActualID = Selected.KFinActualID,
                                        //        KFinTranID = Selected.KFinTranID,
                                        //        ChangeType = "c",
                                        //        DateEffective = DateTime.Now,
                                        //        KHierarchyID = Selected.KHierarchyID,
                                        //        KClientID = Selected.KClientID,
                                        //        KPartyID = Mcategory.KPartyID,
                                        //        KPartyName = Mcategory.KPartyName,
                                        //        IsTemplate = IsTemplate,
                                        //        FCatSrchID = Selected.FCatSrchID,
                                        //        Notes = TransactionNotes.EditedText,
                                        //        KAccountID = Mcategory.KAccountID,
                                        //        KAccountName = Mcategory.KAccountName,
                                        //        Units = IntUnits,

                                        //    };
                                        //        Mtmp2.Add(u);
                                        //}
                                    }
                                    else
                                    {
                                        {
                                            //New value has a greater scalar value than assigned to the previous category
                                            if (TstAmnt)
                                            {

                                                if (IsEqualiserSignSame)
                                                //no provision already made for a transaction adjustment?
                                                {


                                                    //if (Category.OriginalKid == "")
                                                    //{ 
                                                    
                                                    //};
                                                    if ((IntAmnt - OrgActual) - EqualiserValue >= 0 && Category.OriginalKid != ""||(IntAmnt - OrgActual) >= 0 && Category.OriginalKid != "")
                                                    {
                                                        //reduce the scalar value of the adjustment
                                                        {
                                                            {
                                                                Mcategory1.ActualAmount = IntAmnt;
                                                                Mcategory1.KCategoryID = (Category.EditedKid == null) ? Category.OriginalKid : (Category.EditedKid ?? Category.OriginalKid);
                                                                Mcategory1.ShortName = (Category.EditedKid == null) ? Category.OriginalName : (Category.EditedName ?? Category.OriginalName);
                                                                Mcategory1.Units = IntUnits;
                                                                Mcategory1.KPartyID = (Party.EditedKid == null) ? Party.OriginalKid : (Party.EditedKid ?? Party.OriginalKid);
                                                                Mcategory1.KPartyName = (Party.EditedKid == null) ? Party.OriginalName : (Party.EditedName ?? Party.OriginalName);
                                                                Mcategory1.KAccountID = (Account.EditedKid == null) ? Account.OriginalKid : (Account.EditedKid ?? Account.OriginalKid);
                                                                Mcategory1.KAccountName = (Account.EditedKid == null) ? Account.OriginalName : (Account.EditedName ?? Account.OriginalName);

                                                                Mcategory.ActualAmount = IntAmnt;
                                                                Mcategory.KCategoryID = (Category.EditedKid == null) ? Category.OriginalKid : (Category.EditedKid ?? Category.OriginalKid);
                                                                Mcategory.ShortName = (Category.EditedKid == null) ? Category.OriginalName : (Category.EditedName ?? Category.OriginalName);
                                                                Mcategory.Units = IntUnits;
                                                                Mcategory.KPartyID = (Party.EditedKid == null) ? Party.OriginalKid : (Party.EditedKid ?? Party.OriginalKid);
                                                                Mcategory.KPartyName = (Party.EditedKid == null) ? Party.OriginalName : (Party.EditedName ?? Party.OriginalName);
                                                                Mcategory.KAccountID = (Account.EditedKid == null) ? Account.OriginalKid : (Account.EditedKid ?? Account.OriginalKid);
                                                                Mcategory.KAccountName = (Account.EditedKid == null) ? Account.OriginalName : (Account.EditedName ?? Account.OriginalName);

                                                                Selected1.ActualAmount = IntAmnt;
                                                                Selected.ActualAmount = IntAmnt;
                                                                Selected.ShortName = Mcategory.ShortName;
                                                                Selected.KCategoryID = Mcategory.KCategoryID;
                                                                Selected.Units = IntUnits;
                                                            }


                                                            var u = new TransactionResultApiModel
                                                            {
                                                                Posted_Date = DateTime.Parse(TransactionDate),
                                                                Month = Selected1.Month,
                                                                Description = TransactionDescription.EditedText ?? Selected.Description,
                                                                TransAmount = Selected.TransAmount,
                                                                ActualAmount = Selected.ActualAmount,
                                                                ShortName = Mcategory.ShortName,
                                                                KCategoryID = Mcategory1.KCategoryID,
                                                                KFinActualID = Selected.KFinActualID,
                                                                KFinTranID = Selected.KFinTranID,
                                                                ChangeType = "c",
                                                                DateEffective = DateTime.Now,
                                                                KHierarchyID = Selected.KHierarchyID,
                                                                KClientID = Selected.KClientID,
                                                                KPartyID = Mcategory.KPartyID,
                                                                KPartyName = Mcategory.KPartyName,
                                                                IsTemplate = IsTemplate,
                                                                FCatSrchID = Selected.FCatSrchID,
                                                                Notes = TransactionNotes.EditedText,
                                                                KAccountID = Mcategory.KAccountID,
                                                                KAccountName = Mcategory.KAccountName,
                                                                Units = IntUnits,

                                                            };
                                                            Mtmp2.Add(u);


                                                            {
                                                                Mexxist2.ActualAmount = Mexxist2.ActualAmount + (OrgActual - IntAmnt);
                                                                Mexxist1.ActualAmount = Mexxist1.ActualAmount + (OrgActual - IntAmnt);
                                                            }
                                                            if (Mexxist2.ActualAmount == 0)
                                                            //Adjustment transaction no longer required
                                                            {
                                                                u = new TransactionResultApiModel
                                                                {
                                                                    Posted_Date = Selected1.Posted_Date,
                                                                    Month = Selected1.Month,
                                                                    Description = TransactionDescription.EditedText ?? Selected.Description,
                                                                    TransAmount = Selected1.TransAmount,
                                                                    ActualAmount = Selected1.ActualAmount,
                                                                    ShortName = (Category.EditedKid == null) ? Category.OriginalName : (Category.EditedName ?? Category.OriginalName),
                                                                    KCategoryID = Category.EditedKid ?? Category.OriginalKid,
                                                                    KFinActualID = Selected1.KFinActualID,
                                                                    KFinTranID = Selected1.KFinTranID,
                                                                    ChangeType = "d",
                                                                    DateEffective = DateTime.Now,
                                                                    KHierarchyID = Selected1.KHierarchyID,
                                                                    KClientID = Selected.KClientID,
                                                                    KPartyID = Party.EditedKid ?? Party.OriginalKid,
                                                                    KPartyName =  (Party.EditedKid == null) ? Party.OriginalName : (Party.EditedName ?? Party.OriginalName),
                                                                    IsTemplate = IsTemplate,
                                                                    FCatSrchID = Selected1.FCatSrchID,
                                                                    Notes = TransactionNotes.EditedText,
                                                                    KAccountID = Account.EditedKid ?? Account.OriginalKid,
                                                                    KAccountName =  (Account.EditedKid == null) ? Account.OriginalName : (Account.EditedName ?? Account.OriginalName),
                                                                    Units = IntUnits,

                                                                };
                                                                Mtmp2.Add(u);
                                                                Mtmp1.Remove(Mexxist2);
                                                                Mtmp.Remove(Mexxist1);
                                                            }
                                                            else
                                                            {
                                                                u = new TransactionResultApiModel
                                                                {
                                                                    Posted_Date = DateTime.Parse(TransactionDate),
                                                                    Month = Selected1.Month,
                                                                    Description = TransactionDescription.EditedText ?? Selected.Description,
                                                                    TransAmount = Selected1.TransAmount,
                                                                    ActualAmount = Selected1.ActualAmount,
                                                                    ShortName = (Category.EditedKid == null) ? Category.OriginalName : (Category.EditedName ?? Category.OriginalName),
                                                                    KCategoryID = Category.EditedKid ?? Category.OriginalKid,
                                                                    KFinActualID = Selected1.KFinActualID,
                                                                    KFinTranID = Selected1.KFinTranID,
                                                                    ChangeType = "c",
                                                                    DateEffective = DateTime.Now,
                                                                    KHierarchyID = Selected1.KHierarchyID,
                                                                    KClientID = Selected.KClientID,
                                                                    KPartyID = Party.EditedKid ?? Party.OriginalKid,
                                                                    KPartyName = (Party.EditedKid == null) ? Party.OriginalName : (Party.EditedName ?? Party.OriginalName),
                                                                    IsTemplate = IsTemplate,
                                                                    FCatSrchID = Selected1.FCatSrchID,
                                                                    Notes = TransactionNotes.EditedText,
                                                                    KAccountID = Account.EditedKid ?? Account.OriginalKid,
                                                                    KAccountName = (Account.EditedKid == null) ? Account.OriginalName : (Account.EditedName ?? Account.OriginalName),
                                                                    Units = IntUnits,

                                                                };
                                                                Mtmp2.Add(u);
                                                            }

                                                        }
                                                    }
                                                    else
                                                    {
                                                        //either limit current allocation to amount of unallocated balance or authorise transaction adjustment for excess
                                                        var result = System.Windows.MessageBox.Show("Create adjustment entry (Y) to extend Transaction balance, limit to Transaction balance(N)", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
                                                        switch (result)
                                                        {
                                                            case MessageBoxResult.Yes:

                                                                {


                                                                    if (Mexxist2 != null )
                                                                    {
                                                                        {
                                                                            if (Category.OriginalKid == "")
                                                                            //Create new null category - existing null category being reassigned
                                                                            {
                                                                                AddTranAdjust(IntAmnt, OrgActual, 0);
                                                                            }
                                                                            else
                                                                            {
                                                                                ChangeTranAdjust(IntAmnt, OrgActual, 0);
                                                                            }
                                                                        }
                                                                        PriorCatUsage(IntAmnt, OrgActual, IntUnits);

                                                                    }
                                                                    else

                                                                    {
                                                                    }


                                                                }
                                                                break;
                                                            case MessageBoxResult.No:
                                                                {
                                                                    {
                                                                        Mcategory1.ActualAmount = IntAmnt;
                                                                        Mcategory1.KCategoryID = Selected.KCategoryID;
                                                                        Mcategory.ActualAmount = IntAmnt;
                                                                        Mcategory.ShortName = Selected.ShortName;
                                                                        Mcategory.KCategoryID = Selected.KCategoryID;
                                                                        Mcategory.Units = IntUnits;
                                                                        Selected1.ActualAmount = IntAmnt;
                                                                        Selected.ActualAmount = IntAmnt;
                                                                        Selected1.ShortName = Selected.ShortName;
                                                                        Selected1.KCategoryID = Selected.KCategoryID;
                                                                        Selected.Units = IntUnits;
                                                                        Selected1.KFinActualID = Mcategory.KFinActualID;
                                                                        Selected.KFinActualID = Selected1.KFinActualID;
                                                                    }


                                                                    var u = new TransactionResultApiModel
                                                                    {
                                                                        Posted_Date = DateTime.Parse(TransactionDate),
                                                                        Month = Selected1.Month,
                                                                        Description = TransactionDescription.EditedText ?? Selected.Description,
                                                                        TransAmount = Selected1.TransAmount,
                                                                        ActualAmount = Selected1.ActualAmount,
                                                                        ShortName = (Category.EditedKid == null) ? Category.OriginalName : (Category.EditedName ?? Category.OriginalName),
                                                                        KCategoryID = Category.EditedKid ?? Category.OriginalKid,
                                                                        KFinActualID = Selected1.KFinActualID,
                                                                        KFinTranID = Selected1.KFinTranID,
                                                                        ChangeType = "c",
                                                                        DateEffective = DateTime.Now,
                                                                        KHierarchyID = Selected1.KHierarchyID,
                                                                        KClientID = Selected.KClientID,
                                                                        KPartyID = Party.EditedKid ?? Party.OriginalKid,
                                                                        KPartyName = (Party.EditedKid == null) ? Party.OriginalName : (Party.EditedName ?? Party.OriginalName),
                                                                        IsTemplate = IsTemplate,
                                                                        FCatSrchID = Selected1.FCatSrchID,
                                                                        Notes = TransactionNotes.EditedText,
                                                                        KAccountID = Account.EditedKid ?? Account.OriginalKid,
                                                                        KAccountName = (Account.EditedKid == null) ? Account.OriginalName : (Account.EditedName ?? Account.OriginalName),
                                                                        Units = IntUnits,

                                                                    };
                                                                    Mtmp2.Add(u);


                                                                    {
                                                                        Mexxist2.ActualAmount = Mexxist2.ActualAmount + (OrgActual - IntAmnt);
                                                                        Mexxist1.ActualAmount = Mexxist1.ActualAmount + (OrgActual - IntAmnt);
                                                                    }
                                                                    u = new TransactionResultApiModel
                                                                    {
                                                                        Posted_Date = DateTime.Parse(TransactionDate),
                                                                        Month = Selected1.Month,
                                                                        Description = TransactionDescription.EditedText ?? Selected.Description,
                                                                        TransAmount = Selected1.TransAmount,
                                                                        ActualAmount = Selected1.ActualAmount,
                                                                        ShortName = (Category.EditedKid == null) ? Category.OriginalName : (Category.EditedName ?? Category.OriginalName),
                                                                        KCategoryID = Category.EditedKid ?? Category.OriginalKid,
                                                                        KFinActualID = Selected1.KFinActualID,
                                                                        KFinTranID = Selected1.KFinTranID,
                                                                        ChangeType = "c",
                                                                        DateEffective = DateTime.Now,
                                                                        KHierarchyID = Selected1.KHierarchyID,
                                                                        KClientID = Selected.KClientID,
                                                                        KPartyID = Party.EditedKid ?? Party.OriginalKid,
                                                                        KPartyName = (Party.EditedKid == null) ? Party.OriginalName : (Party.EditedName ?? Party.OriginalName),
                                                                        IsTemplate = IsTemplate,
                                                                        FCatSrchID = Selected1.FCatSrchID,
                                                                        Notes = TransactionNotes.EditedText,
                                                                        KAccountID = Account.EditedKid ?? Account.OriginalKid,
                                                                        KAccountName = (Account.EditedKid == null) ? Account.OriginalName : (Account.EditedName ?? Account.OriginalName),
                                                                        Units = IntUnits,

                                                                    };
                                                                    Mtmp2.Add(u);
                                                                }
                                                                break;
                                                            default:
                                                                break;
                                                        }


                                                    }
                                                }
                                                else
                                                // a transaction adjustment has already been created
                                                {
                                                    var result = System.Windows.MessageBox.Show("Increase adjustment entry (Y) to extend Transaction balance, or ignore allocation change(N)", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
                                                    switch (result)
                                                    {
                                                        case MessageBoxResult.Yes:

                                                            {
                                                                {
                                                                    Mcategory1.ActualAmount = IntAmnt;
                                                                    Mcategory1.KCategoryID = Selected.KCategoryID;
                                                                    Mcategory.ActualAmount = IntAmnt;
                                                                    Mcategory.ShortName = Selected.ShortName;
                                                                    Mcategory.KCategoryID = Selected.KCategoryID;
                                                                    Mcategory.Units = IntUnits;

                                                                    Selected1.ActualAmount = IntAmnt;
                                                                    Selected.ActualAmount = IntAmnt;
                                                                    Selected1.ShortName = Selected.ShortName;
                                                                    Selected1.KCategoryID = Selected.KCategoryID;
                                                                    Selected.Units = IntUnits;
                                                                    Selected1.KFinActualID = Mcategory.KFinActualID;
                                                                    Selected.KFinActualID = Selected1.KFinActualID;
                                                                }


                                                                var u = new TransactionResultApiModel
                                                                {
                                                                    Posted_Date = DateTime.Parse(TransactionDate),
                                                                    Month = Selected1.Month,
                                                                    Description = TransactionDescription.EditedText ?? Selected.Description,
                                                                    TransAmount = Selected1.TransAmount,
                                                                    ActualAmount = Selected1.ActualAmount,
                                                                    ShortName = (Category.EditedKid == null) ? Category.OriginalName : (Category.EditedName ?? Category.OriginalName),
                                                                    KCategoryID = Category.EditedKid ?? Category.OriginalKid,
                                                                    KFinActualID = Selected1.KFinActualID,
                                                                    KFinTranID = Selected1.KFinTranID,
                                                                    ChangeType = "c",
                                                                    DateEffective = DateTime.Now,
                                                                    KHierarchyID = Selected1.KHierarchyID,
                                                                    KClientID = Selected.KClientID,
                                                                    KPartyID = Party.EditedKid ?? Party.OriginalKid,
                                                                    KPartyName = (Party.EditedKid == null) ? Party.OriginalName : (Party.EditedName ?? Party.OriginalName),
                                                                    IsTemplate = IsTemplate,
                                                                    FCatSrchID = Selected1.FCatSrchID,
                                                                    Notes = TransactionNotes.EditedText,
                                                                    KAccountID = Account.EditedKid ?? Account.OriginalKid,
                                                                    KAccountName = (Account.EditedKid == null) ? Account.OriginalName : (Account.EditedName ?? Account.OriginalName),
                                                                    Units = IntUnits,

                                                                };
                                                                Mtmp2.Add(u);


                                                                {
                                                                    Mexxist2.ActualAmount = Mexxist2.ActualAmount + (OrgActual - IntAmnt);
                                                                    Mexxist1.ActualAmount = Mexxist1.ActualAmount + (OrgActual - IntAmnt);
                                                                }
                                                                u = new TransactionResultApiModel
                                                                {
                                                                    Posted_Date = DateTime.Parse(TransactionDate),
                                                                    Month = Selected1.Month,
                                                                    Description = TransactionDescription.EditedText ?? Selected.Description,
                                                                    TransAmount = Selected1.TransAmount,
                                                                    ActualAmount = Mexxist2.ActualAmount,
                                                                    ShortName = "",
                                                                    KCategoryID = "",
                                                                    KFinActualID = Selected1.KFinActualID,
                                                                    KFinTranID = Selected1.KFinTranID,
                                                                    ChangeType = "c",
                                                                    DateEffective = DateTime.Now,
                                                                    KHierarchyID = Selected1.KHierarchyID,
                                                                    KClientID = Selected.KClientID,
                                                                    KPartyID = Party.EditedKid ?? Party.OriginalKid,
                                                                    KPartyName = (Party.EditedKid == null) ? Party.OriginalName : (Party.EditedName ?? Party.OriginalName),
                                                                    IsTemplate = IsTemplate,
                                                                    FCatSrchID = Selected1.FCatSrchID,
                                                                    Notes = "",
                                                                    KAccountID = Account.EditedKid ?? Account.OriginalKid,
                                                                    KAccountName = (Account.EditedKid == null) ? Account.OriginalName : (Account.EditedName ?? Account.OriginalName),
                                                                    Units = 0,

                                                                };
                                                                Mtmp2.Add(u);
                                                            }

                                                            break;
                                                        case MessageBoxResult.No:
                                                            Allocation.OriginalText = OrgActual.ToString("C", CultureInfo.CurrentCulture);
                                                            Allocation.EditedText = OrgActual.ToString("C", CultureInfo.CurrentCulture);
                                                            break;
                                                        default:
                                                            break;
                                                    }
                                                }
                                            }

                                            //New value has a lesser scalar value
                                            else
                                            {
                                                //scalar value of assignment reduced and no existing credit adjustment
                                                if (IsEqualiserSignSame)
                                                {
                                                     if (Category.OriginalKid =="")
                                                    //allocation being made from the balance against the transaction
                                                    {
                                                        AddTranAdjust(IntAmnt, OrgActual, 0);
                                                    }
                                                    else
                                                    { 
                                                        if (Mexxist1 == null)
                                                        {
                                                            AddTranAdjust(IntAmnt, OrgActual, 0);
                                                        }
                                                        else
                                                        {
                                                            ChangeTranAdjust(IntAmnt, OrgActual, 0);
                                                        }
                                                    }
                                                    PriorCatUsage(IntAmnt, OrgActual, IntUnits);
                                                }
                                                if (Mexxist1 != null)
                                                //An adjustment (same sign) currently exists fo
                                                //r the transaction
                                                {
                                                    ////first add 

                                                    //Selected1.ActualAmount -= IntAmnt;
                                                    //Selected1.ShortName = (Category.EditedKid == null) ? Category.OriginalName : (Category.EditedName ?? Category.OriginalName);
                                                    //Selected1.KCategoryID = Category.EditedKid ?? Category.OriginalKid;
                                                    //Selected1.KFinActualID = Selected.KFinActualID;
                                                    //Selected1.Description = Selected.Description;
                                                    //Selected1.KFinTranID = Selected.KFinTranID;
                                                    //Selected1.KChangeID = Selected.KChangeID;
                                                    //Selected1.Posted_Date = Selected.Posted_Date;
                                                    //Selected1.Month = Selected.Month;
                                                    //Selected1.TransAmount = Selected.TransAmount;
                                                    //Selected1.Units = IntUnits;
                                                    //Selected1.KClientID = Selected.KClientID;
                                                    //Selected1.KPartyID = Party.EditedKid ?? Party.OriginalKid;
                                                    //Selected1.KPartyName = (Party.EditedKid == null) ? Party.OriginalName : (Party.EditedName ?? Party.OriginalName);
                                                    //Selected1.KAccountID = Selected.KAccountID;
                                                    //Selected1.KAccountName = (Account.EditedKid == null) ? Account.OriginalName : (Account.EditedName ?? Account.OriginalName);
                                                    //Mcategory = Mcategory;


                                                    ////Mtmp.Add(Selected1);
                                                    //Mtmp.Add(Selected1);
                                                    //Mtmp1.Add(Selected1);
                                                    //var u = new TransactionResultApiModel
                                                    //{
                                                    //    Posted_Date = DateTime.Parse(TransactionDate),
                                                    //    Month = Selected1.Month,
                                                    //    Description = Selected1.Description,
                                                    //    TransAmount = Selected1.TransAmount,
                                                    //    ActualAmount = Selected1.ActualAmount,
                                                    //    ShortName = Selected1.ShortName,
                                                    //    KCategoryID = Selected1.KCategoryID,
                                                    //    KFinActualID = Selected1.KFinActualID,
                                                    //    KFinTranID = Selected1.KFinTranID,
                                                    //    ChangeType = "c",
                                                    //    DateEffective = DateTime.Now,
                                                    //    KHierarchyID = Selected1.KHierarchyID,
                                                    //    KClientID = Selected1.KClientID,
                                                    //    KPartyID = Selected1.KPartyID,
                                                    //    KPartyName = Selected1.KPartyName,
                                                    //    IsTemplate = IsTemplate,
                                                    //    KAccountID = Selected1.KAccountID,
                                                    //    KAccountName = Selected1.KAccountName,
                                                    //    Units = IntUnits,
                                                    //};
                                                    //Mtmp2.Add(u);


                                                    ////increase the scalar value of the adjustment

                                                    //Mexxist2.ActualAmount = (OrgActual - IntAmnt);
                                                    //Mexxist1.ActualAmount = Mexxist2.ActualAmount;
                                                    //{
                                                    //    u = new TransactionResultApiModel
                                                    //    {
                                                    //        Posted_Date = Mexxist1.Posted_Date,
                                                    //        Month = Mexxist1.Month,
                                                    //        Description = Mexxist1.Description,
                                                    //        TransAmount = Mexxist1.TransAmount,
                                                    //        ActualAmount = Mexxist1.ActualAmount,
                                                    //        ShortName = Mexxist1.ShortName,
                                                    //        KCategoryID = Mexxist1.KCategoryID,
                                                    //        KFinActualID = Mexxist1.KFinActualID,
                                                    //        KFinTranID = Mexxist1.KFinTranID,
                                                    //        ChangeType = "c",
                                                    //        DateEffective = DateTime.Now,
                                                    //        KHierarchyID = Mexxist1.KHierarchyID,
                                                    //        KClientID = Mexxist1.KClientID,
                                                    //        KPartyID = Mexxist1.KPartyID,
                                                    //        KPartyName = Selected1.KPartyName,
                                                    //        //
                                                    //        IsTemplate = Mexxist1.IsTemplate,
                                                    //        FCatSrchID = Mexxist1.FCatSrchID,
                                                    //        Notes = Mexxist1.Notes,
                                                    //        KAccountID = Mexxist1.KAccountID,
                                                    //        KAccountName = Mexxist1.KAccountName,
                                                    //        Units = Mexxist1.Units,

                                                    //    };
                                                    //    Mtmp2.Add(u);
                                                    //    Mexxist2.KPartyName = Selected1.KPartyName;
                                                    //    Mexxist1.KPartyName = Mexxist2.KPartyName;
                                                    //    //Mexists1 = exists1;
                                                    //    //exists2 = exists2;

                                                    //}



                                                }
                                                //else 
                                                ////Change category allocation AND add adjustment at transaction level to accept the amount 'returned' to the transaction
                                                //{

                                                //    //                            {

                                                //    var used = Mtmp.Where(x => x.KCategoryID == Category.EditedKid && x.KFinTranID == Selected.KFinTranID).OrderByDescending(x => x.DateEffective).ToList() ;
                                                //    var usedRec = used.FirstOrDefault();
                                                //    var used1 = Mtmp1.Where(x => x.KCategoryID == Category.EditedKid && x.KFinTranID == Selected.KFinTranID).OrderByDescending(x => x.DateEffective).ToList();
                                                //    var usedRec1 = used1.FirstOrDefault();

                                                //    if (usedRec == null)
                                                //    //Check whether this Category has already being used for this transaction, if not do a straight allocation AND create an adjustment against the transaction
                                                //    //for the remainder
                                                //    {
                                                //        Selected1.ActualAmount = OrgActual-IntAmnt;
                                                //        Selected1.ShortName = "";
                                                //        Selected1.KCategoryID = "";
                                                //        Selected1.KFinActualID = Guid.NewGuid().ToString().ToUpper();
                                                //        Selected1.Description = Selected1.Description;
                                                //        Selected1.KFinTranID = Selected1.KFinTranID;
                                                //        Selected1.KChangeID = Selected1.KChangeID;
                                                //        Selected1.Posted_Date = Selected1.Posted_Date;
                                                //        Selected1.Month = Selected1.Month;
                                                //        Selected1.TransAmount = Selected1.TransAmount;
                                                //        Selected1.FCatSrchID = Selected1.FCatSrchID;
                                                //        Selected1.Units = 0;
                                                //        Selected1.KAccountID = Account.EditedKid ?? Account.OriginalKid;
                                                //        Selected1.KAccountName = (Account.EditedKid == null) ? Account.OriginalName : (Account.EditedName ?? Account.OriginalName);
                                                //        Selected1.KPartyID = Party.EditedKid ?? Party.OriginalKid;
                                                //        Selected1.KPartyName = (Party.EditedKid == null) ? Party.OriginalName : (Party.EditedName ?? Party.OriginalName);
                                                //        //Mtmp.Add(Selected1);
                                                //        //control update to observable collection
                                                //        Mtmp.Add(Selected1);
                                                //        Mtmp1.Add(Selected1);

                                                //        var u = new TransactionResultApiModel
                                                //        {
                                                //            Posted_Date = Selected1.Posted_Date,
                                                //            Month = Selected1.Month,
                                                //            Description = Selected1.Description,
                                                //            TransAmount = Selected1.TransAmount,
                                                //            ActualAmount = Selected1.ActualAmount,
                                                //            ShortName = Selected1.ShortName,
                                                //            KCategoryID = Selected1.KCategoryID,
                                                //            KFinActualID = Selected1.KFinActualID,
                                                //            KFinTranID = Selected1.KFinTranID,
                                                //            ChangeType = "a",
                                                //            DateEffective = DateTime.Now,
                                                //            KHierarchyID = Selected1.KHierarchyID,
                                                //            KClientID = Selected1.KClientID,
                                                //            KPartyID = Selected1.KPartyID,
                                                //            KPartyName = Selected1.KPartyName,
                                                //            IsTemplate = IsTemplate,
                                                //            FCatSrchID = Selected1.FCatSrchID,
                                                //            Notes ="",
                                                //            KAccountID = Selected1.KAccountID,
                                                //            KAccountName = Selected1.KAccountName,
                                                //            Units = Selected1.Units,
                                                //        };
                                                //        Mtmp2.Add(u);
                                                //        //category1.ActualAmount = IntAmnt;
                                                //        //category1.ShortName = Selected.ShortName;
                                                //        //category1.KCategoryID = Selected.KCategoryID;


                                                //        Mcategory.ActualAmount = IntAmnt;
                                                //        Mcategory.ShortName = (Category.EditedKid == null) ? Category.OriginalName : (Category.EditedName ?? Category.OriginalName);
                                                //        Mcategory.KCategoryID = Category.EditedKid ?? Category.OriginalKid;
                                                //        Mcategory.Units = IntUnits;
                                                //        Mcategory.KPartyID = Party.EditedKid ?? Party.OriginalKid;
                                                //        Mcategory.KPartyName = (Party.EditedKid == null) ? Party.OriginalName : (Party.EditedName ?? Party.OriginalName);
                                                //        Mcategory.KAccountID = Account.EditedKid ?? Account.OriginalKid;
                                                //        Mcategory.KAccountName = (Account.EditedKid == null) ? Account.OriginalName : (Account.EditedName ?? Account.OriginalName);
                                                //        Mcategory.Notes = Selected.Notes;
                                                //        Mcategory1.ActualAmount = IntAmnt;
                                                //        Mcategory1.ShortName = (Category.EditedKid == null) ? Category.OriginalName : (Category.EditedName ?? Category.OriginalName);
                                                //        Mcategory1.KCategoryID = Category.EditedKid ?? Category.OriginalKid;
                                                //        Mcategory1.Units = IntUnits;
                                                //        Mcategory1.KPartyID = Party.EditedKid ?? Party.OriginalKid;
                                                //        Mcategory1.KPartyName = Party.EditedName ?? Party.OriginalName;
                                                //        Mcategory1.KAccountID = Account.EditedKid ?? Account.OriginalKid;
                                                //        Mcategory1.KAccountName = (Account.EditedKid == null) ? Account.OriginalName : (Account.EditedName ?? Account.OriginalName);
                                                //        Mcategory1.Notes = Selected.Notes;


                                                //        //Selected1.ActualAmount = IntAmnt;
                                                //        //Selected1.ShortName = Selected.ShortName;
                                                //        //Selected1.KCategoryID = Selected.KCategoryID;
                                                //        //Selected.Units = IntUnits;

                                                //        //                            }

                                                //        u = new TransactionResultApiModel
                                                //        {
                                                //            Posted_Date   =   Mcategory.Posted_Date  ,
                                                //            Month         =   Mcategory.Month        ,      
                                                //            Description   =   Mcategory.Description  ,
                                                //            TransAmount   =   Mcategory.TransAmount  ,
                                                //            ActualAmount  =   Mcategory.ActualAmount ,
                                                //            ShortName     =   Mcategory.ShortName    ,  
                                                //            KCategoryID   =   Mcategory.KCategoryID  ,
                                                //            KFinActualID  =   Mcategory.KFinActualID ,
                                                //            KFinTranID    =   Mcategory.KFinTranID   , 
                                                //            DateEffective = DateTime.Now,
                                                //            KHierarchyID  =   Mcategory.KHierarchyID ,
                                                //            KClientID     =   Mcategory.KClientID    ,  
                                                //            KPartyID      =   Mcategory.KPartyID     ,   
                                                //            KPartyName    =   Mcategory.KPartyName   , 
                                                //            IsTemplate    =   Mcategory.IsTemplate   , 
                                                //            FCatSrchID    =   Mcategory.FCatSrchID   , 
                                                //            Notes         =   Mcategory.Notes        ,         
                                                //            KAccountID    =   Mcategory.KAccountID   , 
                                                //            KAccountName  =   Mcategory.KAccountName ,
                                                //            Units         =   Mcategory.Units        ,      
                                                //            ChangeType = "c"                         ,
                                                //        };                                          
                                                //        Mtmp2.Add(u);
                                                //    }
                                                //    else
                                                //    {

                                                //        Selected1.ActualAmount = Mcategory.ActualAmount + IntAmnt;
                                                //        Selected1.ShortName = Mcategory.ShortName;
                                                //        Selected1.KCategoryID = Mcategory.KCategoryID;
                                                //        Selected1.KFinActualID = Mcategory.KFinActualID;
                                                //        Selected1.Description = Mcategory.Description;
                                                //        Selected1.KFinTranID = Mcategory.KFinTranID;
                                                //        Selected1.KChangeID = Mcategory.KChangeID;
                                                //        Selected1.Posted_Date = Mcategory.Posted_Date;
                                                //        Selected1.Month = Mcategory.Month;
                                                //        Selected1.TransAmount = Mcategory.TransAmount;
                                                //        Selected1.FCatSrchID = Mcategory.FCatSrchID;
                                                //        Selected1.Units = Mcategory.Units + IntUnits;
                                                //        Mcategory.ActualAmount = Selected1.ActualAmount;
                                                //        Mcategory.Units = Selected1.Units;

                                                //        var u = new TransactionResultApiModel
                                                //        {
                                                //            Posted_Date = DateTime.Parse(TransactionDate),
                                                //            Month = Selected1.Month,
                                                //            Description = Selected1.Description,
                                                //            TransAmount = Selected1.TransAmount,
                                                //            ActualAmount = Selected1.ActualAmount,
                                                //            ShortName = Selected1.ShortName,
                                                //            KCategoryID = Selected1.KCategoryID,
                                                //            KFinActualID = Selected1.KFinActualID,
                                                //            KFinTranID = Selected1.KFinTranID,
                                                //            ChangeType = "c",
                                                //            DateEffective = DateTime.Now,
                                                //            KHierarchyID = Selected1.KHierarchyID,
                                                //            KClientID = Selected.KClientID,
                                                //            KPartyID = Party.OriginalKid,
                                                //            IsTemplate = IsTemplate,
                                                //            FCatSrchID = Selected1.FCatSrchID,
                                                //            Notes = Selected1.Notes,
                                                //            KAccountID = Account.OriginalKid,
                                                //            KAccountName = (Account.OriginalName),
                                                //            Units = Selected1.Units,
                                                //        };
                                                //        Mtmp2.Add(u);
                                                //        Selected.ShortName = u.ShortName;
                                                //        Selected.KCategoryID = u.KCategoryID;
                                                //        Selected.ActualAmount = u.ActualAmount;
                                                //        Selected.KPartyName = u.KPartyName;
                                                //        Selected.KPartyID = u.KPartyID;
                                                //        Selected.Units = IntUnits;
                                                //        Mcategory.ShortName = u.ShortName;
                                                //        Mcategory.KCategoryID = u.KCategoryID;
                                                //        Mcategory.ActualAmount = u.ActualAmount;
                                                //        Mcategory.KPartyName = u.KPartyName;
                                                //        Mcategory.KPartyID = u.KPartyID;
                                                //        Mcategory.Units = IntUnits;
                                                //        Mcategory1.ShortName = u.ShortName;
                                                //        Mcategory1.KCategoryID = u.KCategoryID;
                                                //        Mcategory1.ActualAmount = u.ActualAmount;
                                                //        Mcategory1.KPartyName = u.KPartyName;
                                                //        Mcategory1.KPartyID = u.KPartyID;
                                                //        Mcategory1.Units = IntUnits;
                                                //    }
                                                //}
                                                //}

                                                //else
                                                //{

                                                //    //if scalar value of allocation is less, and opposite sign transaction adjustment had already been created,change value of adjustment

                                                //    {
                                                //        {
                                                //            Mcategory1.ActualAmount = IntAmnt;
                                                //            Mcategory1.KCategoryID = Selected.KCategoryID;
                                                //            Mcategory.ActualAmount = IntAmnt;
                                                //            Mcategory.ShortName = Selected.ShortName;
                                                //            Mcategory.KCategoryID = Selected.KCategoryID;
                                                //            Mcategory.Units = IntUnits;
                                                //            Selected1.ActualAmount = IntAmnt;
                                                //            Selected.ActualAmount = IntAmnt;
                                                //            Selected1.ShortName = Selected.ShortName;
                                                //            Selected1.KCategoryID = Selected.KCategoryID;
                                                //            Selected.Units = IntUnits;
                                                //            Selected1.KFinActualID = Mcategory.KFinActualID;
                                                //            Selected.KFinActualID = Selected1.KFinActualID;
                                                //        }


                                                //        var u = new TransactionResultApiModel
                                                //        {
                                                //            Posted_Date = DateTime.Parse(TransactionDate),
                                                //            Month = Selected1.Month,
                                                //            Description = TransactionDescription.EditedText ?? Selected.Description,
                                                //            TransAmount = Selected1.TransAmount,
                                                //            ActualAmount = Selected1.ActualAmount,
                                                //            ShortName = (Category.EditedKid == null) ? Category.OriginalName : (Category.EditedName ?? Category.OriginalName),
                                                //            KCategoryID = Category.EditedKid ?? Category.OriginalKid,
                                                //            KFinActualID = Selected1.KFinActualID,
                                                //            KFinTranID = Selected1.KFinTranID,
                                                //            ChangeType = "c",
                                                //            DateEffective = DateTime.Now,
                                                //            KHierarchyID = Selected1.KHierarchyID,
                                                //            KClientID = Selected.KClientID,
                                                //            KPartyID = Party.EditedKid ?? Party.OriginalKid,
                                                //            KPartyName = Party.EditedName ?? Party.OriginalName,
                                                //            IsTemplate = IsTemplate,
                                                //            FCatSrchID = Selected1.FCatSrchID,
                                                //            Notes = TransactionNotes.EditedText,
                                                //            KAccountID = Account.EditedKid ?? Account.OriginalKid,
                                                //            KAccountName = (Account.EditedKid == null) ? Account.OriginalName : (Account.EditedName ?? Account.OriginalName),
                                                //            Units = IntUnits,

                                                //        };
                                                //        Mtmp2.Add(u);


                                                //        {
                                                //            Mexxist2.ActualAmount = Mexxist2.ActualAmount + (IntAmnt - OrgActual);
                                                //            Mexxist1.ActualAmount = Mexxist1.ActualAmount + (IntAmnt - OrgActual);
                                                //        }
                                                //        if (Mexxist1.ActualAmount == 0)
                                                //        {
                                                //            u = new TransactionResultApiModel
                                                //            {
                                                //                TransAmount = Mexxist1.TransAmount,
                                                //                ActualAmount = Mexxist1.ActualAmount,
                                                //                ShortName = Mexxist1.ShortName,
                                                //                KCategoryID = Mexxist1.KCategoryID,
                                                //                KFinActualID = Mexxist1.KFinActualID,
                                                //                KFinTranID = Mexxist1.KFinTranID,
                                                //                ChangeType = "d",
                                                //                DateEffective = DateTime.Now,
                                                //                KHierarchyID = Mexxist1.KHierarchyID,
                                                //                KClientID = Mexxist1.KClientID,
                                                //                KPartyID = Mexxist1.KPartyID,
                                                //                KPartyName = Mexxist1.KPartyName,
                                                //                IsTemplate = Mexxist1.IsTemplate,
                                                //                FCatSrchID = Mexxist1.FCatSrchID,
                                                //                Notes = Mexxist1.Notes,
                                                //                KAccountID = Mexxist1.KAccountID,
                                                //                KAccountName = Mexxist1.KAccountName,
                                                //                Units = Mexxist1.Units,
                                                //                Posted_Date = Mexxist1.Posted_Date,
                                                //                Description = Mexxist1.Description,
                                                //            };
                                                //            Mtmp2.Add(u);
                                                //            Mtmp1.Remove(Mexxist2);
                                                //            Mtmp.Remove(Mexxist1);
                                                //        }
                                                //        else
                                                //        {
                                                //            u = new TransactionResultApiModel
                                                //            {
                                                //                TransAmount = Mexxist1.TransAmount,
                                                //                ActualAmount = Mexxist1.ActualAmount,
                                                //                ShortName = Mexxist1.ShortName,
                                                //                KCategoryID = Mexxist1.KCategoryID,
                                                //                KFinActualID = Mexxist1.KFinActualID,
                                                //                KFinTranID = Mexxist1.KFinTranID,
                                                //                ChangeType = "c",
                                                //                DateEffective = Mexxist1.DateEffective,
                                                //                KHierarchyID = Mexxist1.KHierarchyID,
                                                //                KClientID = Mexxist1.KClientID,
                                                //                KPartyID = Mexxist1.KPartyID,
                                                //                KPartyName = Mexxist1.KPartyName,
                                                //                IsTemplate = Mexxist1.IsTemplate,
                                                //                FCatSrchID = Mexxist1.FCatSrchID,
                                                //                Notes = Mexxist1.Notes,
                                                //                KAccountID = Mexxist1.KAccountID,
                                                //                KAccountName = Mexxist1.KAccountName,
                                                //                Units = Mexxist1.Units,

                                                //            };
                                                //            Mtmp2.Add(u);
                                                //        }

                                                //    }

                                            }
                                        }

                                    }
                                    }
                                else
                                {
                                        //Allocation amount unchanged, but other details may have been updated

                                        Mcategory.ActualAmount = IntAmnt;
                                        Mcategory.ShortName = (Category.EditedKid == null) ? Category.OriginalName : (Category.EditedName ?? Category.OriginalName);
                                        Mcategory.KCategoryID = Category.EditedKid ?? Category.OriginalKid;
                                        Mcategory.Units = IntUnits;
                                        Mcategory.KPartyID = Party.EditedKid ?? Party.OriginalKid;
                                        Mcategory.KPartyName = (Party.EditedKid == null) ? Party.OriginalName : (Party.EditedName ?? Party.OriginalName);
                                        Mcategory.KAccountID = Account.EditedKid ?? Account.OriginalKid;
                                        Mcategory.KAccountName = (Account.EditedKid == null) ? Account.OriginalName : (Account.EditedName ?? Account.OriginalName);
                                        Mcategory.Notes = Selected.Notes;
                                        Mcategory1.ActualAmount = IntAmnt;
                                        Mcategory1.ShortName = (Category.EditedKid == null) ? Category.OriginalName : (Category.EditedName ?? Category.OriginalName);
                                        Mcategory1.KCategoryID = Category.EditedKid ?? Category.OriginalKid;
                                        Mcategory1.Units = IntUnits;
                                        Mcategory1.KPartyID = Party.EditedKid ?? Party.OriginalKid;
                                        Mcategory1.KPartyName = (Party.EditedKid == null) ? Party.OriginalName : (Party.EditedName ?? Party.OriginalName);
                                        Mcategory1.KAccountID = Account.EditedKid ?? Account.OriginalKid;
                                        Mcategory1.KAccountName = (Account.EditedKid == null) ? Account.OriginalName : (Account.EditedName ?? Account.OriginalName);
                                        Mcategory1.Notes = Selected.Notes;


                                        //Selected1.ActualAmount = IntAmnt;
                                        //Selected1.ShortName = Selected.ShortName;
                                        //Selected1.KCategoryID = Selected.KCategoryID;
                                        //Selected.Units = IntUnits;

                                        //                            }

                                        var u = new TransactionResultApiModel
                                        {
                                            Posted_Date = Mcategory.Posted_Date,
                                            Month = Mcategory.Month,
                                            Description = Mcategory.Description,
                                            TransAmount = Mcategory.TransAmount,
                                            ActualAmount = Mcategory.ActualAmount,
                                            ShortName = Mcategory.ShortName,
                                            KCategoryID = Mcategory.KCategoryID,
                                            KFinActualID = Mcategory.KFinActualID,
                                            KFinTranID = Mcategory.KFinTranID,
                                            DateEffective = DateTime.Now,
                                            KHierarchyID = Mcategory.KHierarchyID,
                                            KClientID = Mcategory.KClientID,
                                            KPartyID = Mcategory.KPartyID,
                                            KPartyName = Mcategory.KPartyName,
                                            IsTemplate = Mcategory.IsTemplate,
                                            FCatSrchID = Mcategory.FCatSrchID,
                                            Notes = Mcategory.Notes,
                                            KAccountID = Mcategory.KAccountID,
                                            KAccountName = Mcategory.KAccountName,
                                            Units = Mcategory.Units,
                                            ChangeType = "c",
                                        };
                                        Mtmp2.Add(u);


                                }

                                }





                        }
                        //return true;


                    }
                    else
                    {
                        if ((Category.EditedKid ?? Category.OriginalKid) == "" && IntAmnt != OrgActual)
                        { 
                            System.Windows.MessageBox.Show($"An unassigned amount (No category selected..) cannot be adjusted directly");
                            return true;
                        }

                            System.Windows.MessageBox.Show($"No changes detected in transaction allocation adjustment");
                            return true;


                    }

                }



            //Refresh UI for Transaction List
            ((TransactionTreeViewModel)tmp0).RefreshTransactionList();
                //Refresh UI for Transaction Detail List
                ((TransactionDetailTreeViewModel)((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel).RefreshTransactionList(Selected.KFinTranID, Mtmp);
                await ((TransactionTreeViewModel)tmp0).PersistTransClassAsync();
                Close();
                return true;
            });

            }


        /// <summary>
        /// Modify null category adjustment to transaction based on current sub transaction assignment, delete if adjustment is 0
        /// </summary>
        /// <param name="IntAmnt"></param>
        /// <param name="OrgActual"></param>
        /// <param name="IntUnits"></param>
        public void ChangeTranAdjust(decimal IntAmnt, decimal OrgActual, int IntUnits)
        {
            {
                Mexxist2.ActualAmount = Mexxist2.ActualAmount + (OrgActual - IntAmnt);
                Mexxist1.ActualAmount = Mexxist1.ActualAmount + (OrgActual - IntAmnt);
            }
            if (Mexxist2.ActualAmount == 0)
            //Adjustment transaction no longer required
            {
                var u = new TransactionResultApiModel
                {
                    Posted_Date = Selected.Posted_Date,
                    Month = Mexxist1.Month,
                    Description = TransactionDescription.EditedText ?? Selected.Description,
                    TransAmount = Mexxist1.TransAmount,
                    ActualAmount = Mexxist1.ActualAmount,
                    ShortName = (Category.EditedKid == null) ? Category.OriginalName : (Category.EditedName ?? Category.OriginalName),
                    KCategoryID = Category.EditedKid ?? Category.OriginalKid,
                    KFinActualID = Mexxist1.KFinActualID,
                    KFinTranID = Mexxist1.KFinTranID,
                    ChangeType = "d",
                    DateEffective = DateTime.Now,
                    KHierarchyID = Mexxist1.KHierarchyID,
                    KClientID = Selected.KClientID,
                    KPartyID = Party.EditedKid ?? Party.OriginalKid,
                    KPartyName = (Party.EditedKid == null) ? Party.OriginalName : (Party.EditedName ?? Party.OriginalName),
                    IsTemplate = IsTemplate,
                    FCatSrchID = Mexxist1.FCatSrchID,
                    Notes = TransactionNotes.EditedText,
                    KAccountID = Account.EditedKid ?? Account.OriginalKid,
                    KAccountName = (Account.EditedKid == null) ? Account.OriginalName : (Account.EditedName ?? Account.OriginalName),
                    Units = IntUnits,

                };
                Mtmp2.Add(u);
                Mtmp1.Remove(Mexxist2);
                Mtmp.Remove(Mexxist1);
            }
            else
            {
                var u = new TransactionResultApiModel
                {
                    Posted_Date = DateTime.Parse(TransactionDate),
                    Month = Mexxist1.Month,
                    Description = TransactionDescription.EditedText ?? Selected.Description,
                    TransAmount =  Mexxist1.TransAmount,
                    ActualAmount = Mexxist1.ActualAmount,
                    ShortName = Mexxist1.ShortName,
                    KCategoryID = Mexxist1.KCategoryID,
                    KFinActualID =  Mexxist1.KFinActualID,
                    KFinTranID =  Mexxist1.KFinTranID,
                    ChangeType = "c",
                    DateEffective = DateTime.Now,
                    KHierarchyID =  Mexxist1.KHierarchyID,
                    KClientID = Selected.KClientID,
                    KPartyID = Party.EditedKid ?? Party.OriginalKid,
                    KPartyName = (Party.EditedKid == null) ? Party.OriginalName : (Party.EditedName ?? Party.OriginalName),
                    IsTemplate = IsTemplate,
                    FCatSrchID =  Mexxist1.FCatSrchID,
                    Notes = TransactionNotes.EditedText,
                    KAccountID = Account.EditedKid ?? Account.OriginalKid,
                    KAccountName = (Account.EditedKid == null) ? Account.OriginalName : (Account.EditedName ?? Account.OriginalName),
                    Units = IntUnits,

                };
                Mtmp2.Add(u);
            }
        }



        /// <summary>
        /// Add new null category adjustment to transaction if no null adjustment currently exists 
        /// </summary>
        /// <param name="IntAmnt"></param>
        /// <param name="OrgActual"></param>
        /// <param name="IntUnits"></param>
        public void AddTranAdjust(decimal IntAmnt, decimal OrgActual, int IntUnits)
        {
            Selected1.ActualAmount = (OrgActual - IntAmnt);
            Selected1.ShortName = "";
            Selected1.KCategoryID = "";
            Selected1.KFinActualID = Guid.NewGuid().ToString().ToUpper();
            Selected1.Description = Selected.Description;
            Selected1.KFinTranID = Selected.KFinTranID;
            Selected1.KChangeID = Selected.KChangeID;
            Selected1.Posted_Date = Selected.Posted_Date;
            Selected1.Month = Selected.Month;
            Selected1.TransAmount = Selected.TransAmount;
            Selected1.Units = 0;
            Selected1.KClientID = Selected.KClientID;
            Selected1.KPartyID = Selected.KPartyID;
            Selected1.KPartyName = (Party.EditedKid == null) ? Party.OriginalName : (Party.EditedName ?? Party.OriginalName);
            Selected1.KAccountID = Selected.KAccountID;
            Selected1.KAccountName = (Account.EditedKid == null) ? Account.OriginalName : (Account.EditedName ?? Account.OriginalName);
            Selected1.DateEffective = DateTime.Now;


            var u = new TransactionResultApiModel
            {
                Posted_Date = DateTime.Parse(TransactionDate),
                Month = Selected1.Month,
                Description = Selected1.Description,
                TransAmount = Selected1.TransAmount,
                ActualAmount = Selected1.ActualAmount,
                ShortName = "",
                KCategoryID = "",
                KFinActualID = Selected1.KFinActualID,
                KFinTranID = Selected1.KFinTranID,
                ChangeType = "a",
                DateEffective = DateTime.Now,
                KHierarchyID = Selected1.KHierarchyID,
                KClientID = Selected1.KClientID,
                KPartyID = Selected1.KPartyID,
                KPartyName = (Party.EditedKid == null) ? Party.OriginalName : (Party.EditedName ?? Party.OriginalName),
                IsTemplate = IsTemplate,
                KAccountID = Selected1.KAccountID,
                KAccountName = Selected1.KAccountName,
                Units = 0,
            };
            Mtmp2.Add(u);
            var v = new TransactionViewModel
            {
                Posted_Date = DateTime.Parse(TransactionDate),
                Month = Selected1.Month,
                Description = Selected1.Description,
                TransAmount = Selected1.TransAmount,
                ActualAmount = Selected1.ActualAmount,
                ShortName = "",
                KCategoryID = "",
                KFinActualID = Selected1.KFinActualID,
                KFinTranID = Selected1.KFinTranID,
                DateEffective = DateTime.Now,
                KHierarchyID = Selected1.KHierarchyID,
                KClientID = Selected1.KClientID,
                KPartyID = Selected1.KPartyID,
                KPartyName = (Party.EditedKid == null) ? Party.OriginalName : (Party.EditedName ?? Party.OriginalName),
                IsTemplate = IsTemplate,
                KAccountID = Selected1.KAccountID,
                KAccountName = Selected1.KAccountName,
                Units = 0,
            };
            Mtmp.Add(v);
            Mtmp1.Add(v);
        }

        /// <summary>
        /// Check for prior usage of Category/Project/Asset  
        /// </summary>
        /// <param name="IntAmnt"></param>
        /// <param name="OrgActual"></param>
        /// <param name="IntUnits"></param>
        public void PriorCatUsage(decimal IntAmnt, decimal OrgActual, int IntUnits)
        {
            var used = Mtmp.Where(x => x.KCategoryID == Category.EditedKid && x.KFinTranID == Selected.KFinTranID).OrderByDescending(x => x.DateEffective).ToList();
            var usedRec = used.FirstOrDefault();
            var used1 = Mtmp1.Where(x => x.KCategoryID == Category.EditedKid && x.KFinTranID == Selected.KFinTranID).OrderByDescending(x => x.DateEffective).ToList();
            var usedRec1 = used1.FirstOrDefault();

            if (usedRec == null)
            //Check whether this Category has already being used for this transaction, if not change the category allocation on the currently selected record to the new allocaiton
            //for the remainder
            {
               


                Mcategory.ActualAmount = IntAmnt;
                Mcategory.ShortName = (Category.EditedKid == null) ? Category.OriginalName : (Category.EditedName ?? Category.OriginalName);
                Mcategory.KCategoryID = Category.EditedKid ?? Category.OriginalKid;
                Mcategory.Units = IntUnits;
                Mcategory.KPartyID = Party.EditedKid ?? Party.OriginalKid;
                Mcategory.KPartyName = (Party.EditedKid == null) ? Party.OriginalName : (Party.EditedName ?? Party.OriginalName);
                Mcategory.KAccountID = Account.EditedKid ?? Account.OriginalKid;
                Mcategory.KAccountName = (Account.EditedKid == null) ? Account.OriginalName : (Account.EditedName ?? Account.OriginalName);
                Mcategory.Notes = Selected.Notes;
                Mcategory1.ActualAmount = Mcategory.ActualAmount ;
                Mcategory1.ShortName =    Mcategory.ShortName    ;
                Mcategory1.KCategoryID =  Mcategory.KCategoryID  ;
                Mcategory1.Units =        Mcategory.Units        ;
                Mcategory1.KPartyID =     Mcategory.KPartyID     ;
                Mcategory1.KPartyName =   Mcategory.KPartyName   ;
                Mcategory1.KAccountID =   Mcategory.KAccountID   ;
                Mcategory1.KAccountName = Mcategory.KAccountName ;
                Mcategory1.Notes = Mcategory.Notes;




                //                            }

               var u = new TransactionResultApiModel
                {
                    Posted_Date = Mcategory.Posted_Date,
                    Month = Mcategory.Month,
                    Description = Mcategory.Description,
                    TransAmount = Mcategory.TransAmount,
                    ActualAmount = Mcategory.ActualAmount,
                    ShortName = Mcategory.ShortName,
                    KCategoryID = Mcategory.KCategoryID,
                    KFinActualID = Mcategory.KFinActualID,
                    KFinTranID = Mcategory.KFinTranID,
                    DateEffective = DateTime.Now,
                    KHierarchyID = Mcategory.KHierarchyID,
                    KClientID = Mcategory.KClientID,
                    KPartyID = Mcategory.KPartyID,
                    KPartyName = Mcategory.KPartyName,
                    IsTemplate = Mcategory.IsTemplate,
                    FCatSrchID = Mcategory.FCatSrchID,
                    Notes = Mcategory.Notes,
                    KAccountID = Mcategory.KAccountID,
                    KAccountName = Mcategory.KAccountName,
                    Units = Mcategory.Units,
                    ChangeType = "c",
                };
                Mtmp2.Add(u);
            }
            else
            {
                Mtmp = Mtmp;
                Mtmp1 = Mtmp1;
                Selected1.KCategoryID = "testing 1";
                Selected1.ActualAmount = usedRec.ActualAmount + IntAmnt;
                Selected1.ShortName = usedRec.ShortName;
                Selected1.KCategoryID = usedRec.KCategoryID;
                Selected1.KFinActualID = usedRec.KFinActualID;
                Selected1.Description = usedRec.Description;
                Selected1.KFinTranID = usedRec.KFinTranID;
                Selected1.KChangeID = usedRec.KChangeID;
                Selected1.Posted_Date = usedRec.Posted_Date;
                Selected1.Month = usedRec.Month;
                Selected1.TransAmount = usedRec.TransAmount;
                Selected1.FCatSrchID = usedRec.FCatSrchID;
                Selected1.Units = usedRec.Units + IntUnits;
                usedRec.ActualAmount = Selected1.ActualAmount;
                usedRec.Units = Selected1.Units;
                usedRec.Notes = usedRec.Notes + " / " + Selected.Notes;
                usedRec1.ActualAmount =  usedRec.ActualAmount;
                usedRec1.Units = usedRec.Units;
                usedRec1.Notes = usedRec.Notes;

                var u = new TransactionResultApiModel
                {
                    Posted_Date = DateTime.Parse(TransactionDate),
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
                    KClientID = Selected.KClientID,
                    KPartyID = Party.EditedKid ?? Party.OriginalKid,
                    KPartyName = (Party.EditedKid == null) ? Party.OriginalName : (Party.EditedName ?? Party.OriginalName),
                    IsTemplate = IsTemplate,
                    FCatSrchID = Selected1.FCatSrchID,
                    Notes = Selected1.Notes,
                    KAccountID = Account.EditedKid ?? Account.OriginalKid,
                    KAccountName = (Account.EditedKid == null) ? Account.OriginalName : (Account.EditedName ?? Account.OriginalName),
                    Units = Selected1.Units,
                };
                Mtmp2.Add(u);
                //is this needed?
                //Selected.ShortName = u.ShortName;
                //Selected.KCategoryID = u.KCategoryID;
                //Selected.ActualAmount = u.ActualAmount;
                //Selected.KPartyName = u.KPartyName;
                //Selected.KPartyID = u.KPartyID;
                //Selected.Units = IntUnits;
                //Mcategory.ShortName = u.ShortName;
                //Mcategory.KCategoryID = u.KCategoryID;
                //Mcategory.ActualAmount = u.ActualAmount;
                //Mcategory.KPartyName = u.KPartyName;
                //Mcategory.KPartyID = u.KPartyID;
                //Mcategory.Units = IntUnits;
                //Mcategory1.ShortName = u.ShortName;
                //Mcategory1.KCategoryID = u.KCategoryID;
                //Mcategory1.ActualAmount = u.ActualAmount;
                //Mcategory1.KPartyName = u.KPartyName;
                //Mcategory1.KPartyID = u.KPartyID;
                //Mcategory1.Units = IntUnits;

                if (Category.OriginalKid == "")
                //delete original NULL allocation as this will not be used for new category allocation
                {
                    u = new TransactionResultApiModel
                    {
                        Posted_Date = Selected.Posted_Date,
                        Month = Mexxist1.Month,
                        Description = TransactionDescription.EditedText ?? Selected.Description,
                        TransAmount = Mexxist1.TransAmount,
                        ActualAmount = Mexxist1.ActualAmount,
                        ShortName = (Category.EditedKid == null) ? Category.OriginalName : (Category.EditedName ?? Category.OriginalName),
                        KCategoryID = Category.EditedKid ?? Category.OriginalKid,
                        KFinActualID = Mexxist1.KFinActualID,
                        KFinTranID = Mexxist1.KFinTranID,
                        ChangeType = "d",
                        DateEffective = DateTime.Now,
                        KHierarchyID = Mexxist1.KHierarchyID,
                        KClientID = Selected.KClientID,
                        KPartyID = Party.EditedKid ?? Party.OriginalKid,
                        KPartyName = (Party.EditedKid == null) ? Party.OriginalName : (Party.EditedName ?? Party.OriginalName),
                        IsTemplate = IsTemplate,
                        FCatSrchID = Mexxist1.FCatSrchID,
                        Notes = TransactionNotes.EditedText,
                        KAccountID = Account.EditedKid ?? Account.OriginalKid,
                        KAccountName = (Account.EditedKid == null) ? Account.OriginalName : (Account.EditedName ?? Account.OriginalName),
                        Units = IntUnits,

                    };
                    Mtmp2.Add(u);
                    Mtmp1.Remove(Mexxist2);
                    Mtmp.Remove(Mexxist1);
                }
            }
        }

        /// <summary>
        /// Check for prior usage of Category/Project/Asset  
        /// </summary>
        /// <param name="IntAmnt"></param>
        /// <param name="OrgActual"></param>
        /// <param name="IntUnits"></param>
        public void ExistCatUsage(decimal IntAmnt, decimal OrgActual, int IntUnits)
        {
            {
                Mcategory.ActualAmount = IntAmnt;
                Mcategory.ShortName = (Category.EditedKid == null) ? Category.OriginalName : (Category.EditedName ?? Category.OriginalName);
                Mcategory.KCategoryID = Category.EditedKid ?? Category.OriginalKid;
                Mcategory.Units = IntUnits;
                Mcategory.KPartyID = Party.EditedKid ?? Party.OriginalKid;
                Mcategory.KPartyName = (Party.EditedKid == null) ? Party.OriginalName : (Party.EditedName ?? Party.OriginalName);
                Mcategory.KAccountID = Account.EditedKid ?? Account.OriginalKid;
                Mcategory.KAccountName = (Account.EditedKid == null) ? Account.OriginalName : (Account.EditedName ?? Account.OriginalName);
                Mcategory.Notes = Mcategory.Notes + " " + TransactionNotes.EditedText;
                Mcategory1.ActualAmount = Mcategory.ActualAmount;
                Mcategory1.ShortName = Mcategory.ShortName;
                Mcategory1.KCategoryID = Mcategory.KCategoryID;
                Mcategory1.Units = Mcategory.Units;
                Mcategory1.KPartyID = Mcategory.KPartyID;
                Mcategory1.KPartyName = Mcategory.KPartyName;
                Mcategory1.KAccountID = Mcategory.KAccountID;
                Mcategory1.KAccountName = Mcategory.KAccountName;
                Mcategory1.Notes = Mcategory.Notes;





                //                            }

                var u = new TransactionResultApiModel
                {
                    Posted_Date = Mcategory.Posted_Date,
                    Month = Mcategory.Month,
                    Description = Mcategory.Description,
                    TransAmount = Mcategory.TransAmount,
                    ActualAmount = Mcategory.ActualAmount,
                    ShortName = Mcategory.ShortName,
                    KCategoryID = Mcategory.KCategoryID,
                    KFinActualID = Mcategory.KFinActualID,
                    KFinTranID = Mcategory.KFinTranID,
                    DateEffective = DateTime.Now,
                    KHierarchyID = Mcategory.KHierarchyID,
                    KClientID = Mcategory.KClientID,
                    KPartyID = Mcategory.KPartyID,
                    KPartyName = Mcategory.KPartyName,
                    IsTemplate = Mcategory.IsTemplate,
                    FCatSrchID = Mcategory.FCatSrchID,
                    Notes = Mcategory.Notes,
                    KAccountID = Mcategory.KAccountID,
                    KAccountName = Mcategory.KAccountName,
                    Units = Mcategory.Units,
                    ChangeType = "c",
                };
                Mtmp2.Add(u);
            }
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
                Process.Start(new ProcessStartInfo(doccie.DocURL) { UseShellExecute = true });


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
