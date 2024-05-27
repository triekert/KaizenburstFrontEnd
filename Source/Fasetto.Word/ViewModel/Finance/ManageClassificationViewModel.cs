using Dna;
using Fasetto.Word.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using static Fasetto.Word.DI;

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
        /// API parameter model
        /// </summary>
        /// 
        public ParameterBillingAdjustmentApiModel MAPI { get; set; }

        /// <summary>
        /// True to show the attachment menu, false to hide it
        /// </summary>
        public bool SetHierarchyCompleted { get; set; }


        /// <summary>
        /// The action to run when saving the text.
        /// Returns true if the commit was successful, or false otherwise.
        /// </summary>
        public Func<Task<bool>> CommitAction { get; set; }



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
        public TransactionDetailViewModel New1 { get; set; }

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
        public ICommand EditClassificationCommand { get; set; }

         /// <summary>
        /// The command to remove a classification from the transaction, if only one classification remains the previous allocation reverts to that classification
        /// otherwise the amount is allocated to 'unprocessed'
        /// </summary>
        public ICommand DeleteClassificationCommand { get; set; }





        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public ManageClassificationViewModel(ObservableCollection<TransactionViewModel> source, TransactionViewModel selected)
        {
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
                ClientID = ((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).Root.EditedKid,
                HierarchyID = ((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy.EditedKid,
                //HierarchyTypeID = "64413ae7-822f-4866-9ebe-433083d699ac",
                RootID = ((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy.EditedKid,
                PrepareAction = SetHierarchySelectionAsync,
                PriorPopupViewModel = ViewModelApplication.CurrentPopupViewModel,

                //HierarchyID = ((CostHierarchyViewModel)((CostHierarchyListViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy).MSelectedCostHierarchy).KCategoryID,

                CommitAction = AddClassificationAsync
            };
            Party = new HierarchyItemSelectionViewModel
            {
                Label = "Select Linked Party",
                //EditedName = mLoadingText,
                EditedName = "Selected Party",
                OriginalKid = selected.KPartyID,
                OriginalName = selected.KPartyName,
                EditedKid = null,
                ClientID = ((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).Root.EditedKid,
                HierarchyTypeID = "ADEEBB16-F553-48F8-955F-663227A4886C",
                PrepareAction = SetPartyHierarchySelectionAsync,
                //HierarchyTypeID = ((CostHierarchyListViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy).f,
                //HierarchyID = ((CostHierarchyViewModel)((CostHierarchyListViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy).MSelectedCostHierarchy).KCategoryID,
                PriorPopupViewModel = ViewModelApplication.CurrentPopupViewModel,
                CommitAction = AddClassificationAsync
            };



            TransactionDate = (selected.Posted_Date).ToString();

            TransactionDetail = selected.Description;

            HeadingText = "Manage classification of selected Transaction";

            // Create commands
            CloseCommand = new RelayCommand(Close);
            AddClassificationCommand = new RelayCommand(AddClassification);
            EditClassificationCommand = new RelayCommand(AddClassification);
            DeleteClassificationCommand = new RelayCommand(AddClassification);


            // TODO: Get from localization
            AddClassificationButtonText = "Manage Transaction Classification:";
            Source = source;
            Selected= selected;
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
            // Close settings menu
            //var mHierarchyBillingTreeViewModel = ((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel;
            //var mHierarchyBillingTreeViewModel = ViewModelApplication.CurrentPopupViewModel;
            //

            ViewModelApplication.CurrentPopupViewModel = ((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel;
            var mKFinTranID = ((TransactionDetailTreeViewModel)ViewModelApplication.CurrentPopupViewModel).TransactionDetail[0].KFinTranID;
            var matches = ((TransactionTreeViewModel)((TransactionDetailTreeViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel).Trans_action.Where(x => x.KFinTranID == mKFinTranID).ToList();
            var Cnt = matches.Count;
                    ViewModelApplication.CurrentPopupContent = PopupContent.TransactionDetail;
                    ViewModelApplication.ControlParameter1 = null;
            if (Cnt ==1)
            {             

                    ViewModelApplication.CurrentPopupViewModel = ((TransactionDetailTreeViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel;
                ViewModelApplication.CurrentPopupContent = PopupContent.Transaction;
            }


            ViewModelApplication.PopupVisible = true;


        }

        public async Task<bool> SetHierarchySelectionAsync()
        {
            // Lock this command to ignore any other requests while processing

            return await RunCommandAsync(() => SetHierarchyCompleted, async () =>
            {
                // Update the Category Classification value on the server...

                ViewModelApplication.CurrentControlViewModel = ((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).Category;
                //ViewModelApplication.ControlParameter1 = ((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).Category;
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
                //ViewModelApplication.ControlParameter1 = ((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).Party;
                return true;
            });

        }
        

        /// <summary>
        /// Initialises the Cost Hierarchy Search
        /// </summary>
        /// <returns>Returns true if successful, false otherwise</returns>
        public async Task<bool> AddClassificationAsync()
        {
            // Lock this command to ignore any other requests while processing


            return await RunCommandAsync(() => CostClassificationIsSaving, async () =>
            {

                ViewModelApplication.CurrentPopupViewModel = ViewModelApplication.CurrentPopupViewModel;
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
        public void AddClassification()
        {
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
            //var TstEqual = false;
            //if (Math.Abs(IntAmnt) == Math.Abs(Selected.ActualAmount)) { TstEqual = true; }
            if (Math.Abs(IntAmnt) > Math.Abs(Selected.ActualAmount)) { IntAmnt = Selected.ActualAmount; }
            if (Category.EditedKid != Category.OriginalKid || IntAmnt != OrgActual||Party.EditedKid!=Party.OriginalKid) 
                //Don't do anything if cost category hasn't changed,AND the allocated amount has not changed
            {  
                    var tmp = ((TransactionTreeViewModel)((TransactionDetailTreeViewModel)((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel).PriorPopupViewModel).Trans_action;
                    var tmp1 = ((TransactionDetailTreeViewModel)((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel).TransactionDetail;
                    var tmp2 = ((TransactionTreeViewModel)((TransactionDetailTreeViewModel)((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).PriorPopupViewModel).PriorPopupViewModel).mChange;
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
                                    KHierarchyID = ((HierarchyItemSelectionViewModel)((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).Category).EditedKid,
                                    KClientID = ((HierarchyItemSelectionViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).Root).EditedKid,
                                    KPartyID = Selected1.KPartyID,
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
                                    KHierarchyID = ((HierarchyItemSelectionViewModel)((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).Category).EditedKid,
                                    KClientID = ((HierarchyItemSelectionViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).Root).EditedKid,
                                    KPartyID = Selected1.KPartyID,

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
                                    KHierarchyID = ((HierarchyItemSelectionViewModel)((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).Category).EditedKid,
                                    KClientID = ((HierarchyItemSelectionViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).Root).EditedKid,
                                    KPartyID = Selected1.KPartyID,
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
                                KHierarchyID = ((HierarchyItemSelectionViewModel)((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).Category).EditedKid,
                                KClientID = ((HierarchyItemSelectionViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).Root).EditedKid,
                                KPartyID = Selected1.KPartyID,

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
                                    KHierarchyID = ((HierarchyItemSelectionViewModel)((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).Category).EditedKid,
                                    KClientID = ((HierarchyItemSelectionViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).Root).EditedKid,
                                    KPartyID = Selected1.KPartyID,

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
                                    KHierarchyID = ((HierarchyItemSelectionViewModel)((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).Category).EditedKid,
                                    KClientID = ((HierarchyItemSelectionViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).Root).EditedKid,
                                    KPartyID = Selected1.KPartyID,
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
                                    KHierarchyID = ((HierarchyItemSelectionViewModel)((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).Category).EditedKid,
                                    KClientID = ((HierarchyItemSelectionViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).Root).EditedKid,
                                    KPartyID = Selected1.KPartyID,

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
                                    KHierarchyID = ((HierarchyItemSelectionViewModel)((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).Category).EditedKid,
                                    KClientID = ((HierarchyItemSelectionViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).Root).EditedKid,
                                    KPartyID = Selected1.KPartyID,

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
                                    KHierarchyID = ((HierarchyItemSelectionViewModel)((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).Category).EditedKid,
                                    KClientID = ((HierarchyItemSelectionViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).Root).EditedKid,
                                    KPartyID = Selected1.KPartyID,

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
                                    KHierarchyID = ((HierarchyItemSelectionViewModel)((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).Category).EditedKid,
                                    KClientID = ((HierarchyItemSelectionViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).Root).EditedKid,
                                    KPartyID = Selected1.KPartyID,

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
                                    KHierarchyID = ((HierarchyItemSelectionViewModel)((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).Category).EditedKid,
                                    KClientID = ((HierarchyItemSelectionViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).Root).EditedKid,
                                    KPartyID = Selected1.KPartyID,

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
                                    KHierarchyID = ((HierarchyItemSelectionViewModel)((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).Category).EditedKid,
                                    KClientID = ((HierarchyItemSelectionViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).Root).EditedKid,
                                    KPartyID = Selected1.KPartyID,

                                };
                                tmp2.Add(u); 

                                //matches = tmp.Where(x => x.KCategoryID == "" && x.KFinTranID == Selected.KFinTranID).OrderByDescending(x => x.DateEffective).ToList();
                                //category = matches.FirstOrDefault();
                                matches1 = tmp1.Where(x => x.KCategoryID == "" && x.KFinTranID == Selected.KFinTranID).OrderByDescending(x => x.DateEffective).ToList();
                                category1 = matches1.FirstOrDefault();

                                if (category == null)
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
                                        KHierarchyID = ((HierarchyItemSelectionViewModel)((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).Category).EditedKid,
                                        KPartyID = Selected1.KPartyID,

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
                                        KHierarchyID = ((HierarchyItemSelectionViewModel)((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).Category).EditedKid,
                                        KPartyID = Selected1.KPartyID,
                                        KClientID = ((HierarchyItemSelectionViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).Root).EditedKid,
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
                        KHierarchyID = ((HierarchyItemSelectionViewModel)((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).Category).EditedKid,
                        KClientID = ((HierarchyItemSelectionViewModel)((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).Root).EditedKid,
                        KPartyID =  ((HierarchyItemSelectionViewModel)((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).Party).EditedKid,
                        //KPartyID = (Selected1.KPartyID??Selected.KPartyID),
                        KPartyName = ((HierarchyItemSelectionViewModel)((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).Party).EditedName,

                    };
                    tmp2.Add(u);
                    Selected1.KPartyID = ((HierarchyItemSelectionViewModel)((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).Party).EditedKid;
                    Selected1.KPartyName = ((HierarchyItemSelectionViewModel)((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).Party).EditedName;
                    Selected.KPartyID = ((HierarchyItemSelectionViewModel)((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).Party).EditedKid;
                    Selected.KPartyName = ((HierarchyItemSelectionViewModel)((ManageClassificationViewModel)ViewModelApplication.CurrentPopupViewModel).Party).EditedName;


                }

            }


            //Check for allocation of party or change in allocated party

            Close();
        }
        public async Task BillingPeriodAdjustAsync()
        {
            await RunCommandAsync(() => IsRunning, async () =>
            {

                // Store single transcient instance of client data store
                var scopedClientDataStore = ClientDataStore;

                // Update values from local cache
                // Get the user token
                var token = (await scopedClientDataStore.GetLoginCredentialsAsync())?.Token;
                // Call the server and attempt to register with the provided credentials
                // If we don't have a token (then not logged in...)
                if (string.IsNullOrEmpty(token))
                    // Then do nothing more
                    return;

                var result = await WebRequests.PostAsync<ApiResponse>(
                // Set URL
                    RouteHelpers.GetAbsoluteRoute(ApiRoutes.BillingPeriodAdjustment),
                    MAPI ,
                    bearerToken: token);




                // If the response has an error...
                if (await result.HandleErrorIfFailedAsync("Capture of Adjustment failed"))
                    // We are done
                    return;

                // OK successfully registered (and logged in)... now get aprpropriate tree view data
                //for now; keep a snapshot of persisted data
                //mOriginal = result.ServerResponse.Response;
                //;

                //try
                //{
                //    //var hierarchyResultApiModels = mOriginal.ToList();
                //    //make a clone of the persisted data for manipulation on front end
                //    mPersist = new HierarchyResultListApiModel();
                //    mPersist.Clone(mOriginal, mPersist);
                //}
                //catch (Exception e)
                //{
                //    throw e;
                //}



                ViewModelApplication.CurrentControlViewModel = ViewModelApplication.CurrentControlViewModel;


            });
        }

#endregion

        public void Remove(List<TransactionViewModel> source, ObservableCollection<TransactionViewModel> target)
        {
            foreach (var item in source)
                target.Remove(item);
        }
        public void Clone(List<TransactionViewModel> source,ObservableCollection<TransactionViewModel>  target)
        {
            foreach (var item in source)
                target.Add(item);
        }





    }


    
}
