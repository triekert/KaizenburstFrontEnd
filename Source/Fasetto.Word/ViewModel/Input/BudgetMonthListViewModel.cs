
using Dna;
using Fasetto.Word.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using static Fasetto.Word.Core.CoreDI;
using System.Windows.Forms;
using static Fasetto.Word.DI;
using System.Windows;


namespace Fasetto.Word
{
    /// <summary>
    /// This is the view-model of the UI.  It provides a data source
    /// for the TreeView (the FirstGeneration property), a bindable
    /// SearchText property, and the SearchCommand to perform a search.
    /// </summary>
    public class BudgetMonthListViewModel : BaseViewModel

    {

        #region Public Properties

        /// <summary>
        /// A set of Budget Months for the selected budget period
        /// </summary>
        public ObservableCollection<BudgetMonthDataModel> BudgetMonthList{ get; set; }

        /// <summary>
        /// The selected Budget Month view model
        /// </summary>
        public BudgetMonthDataModel MSelectedBudgetMonth{ get; set; }

        //public ObservableCollection<HierarchyViewModel> FirstGeneration1 { get; set; }

        #endregion
        #region Properties
        #region Public Properties



        /// <summary>
        /// A flag indicating if the login command is running
        /// </summary>
        public bool BulkReconBuildIsRunning { get; set; }

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





        #endregion //Properties

        #region Data

        public BudgetMonthDataModel mCHVM;


        /// <summary>
        /// Indicates if the current text is in edit mode
        /// </summary>
        public bool Editing { get; set; }


        private string mSearchText = "", mSearchKCategoryID = string.Empty, mParentCategoryID = string.Empty;

        #endregion // Data
        #region Public Commands
        /// <summary>
        /// The command to close the settings menu
        /// </summary>
        public ICommand CloseCommand { get; set; }

        /// <summary>
        /// Puts the control into edit mode
        /// </summary>
        public ICommand EditCommand { get; set; }
        #endregion//Public Commands
        #region Constructor
        /// <summary>
        /// The HierarchyTreeViewModel is a visual interface for interacting with hierarchical
        /// Structures persisted on the database linked to the application
        /// Generic hierarchy structures with parent-child relationships may be used to represent
        /// appropriate data sets
        /// </summary>
        /// <param name="hierarchyTable"></param>
        /// The hierarchyTable passed through as a paremeter identifies the specific hierarchy set to be retrieved
        /// from persistent s
        public BudgetMonthListViewModel()
        {
            #region Build HierarchyViewCollection
            BudgetMonthList = new ObservableCollection<BudgetMonthDataModel> {

             new BudgetMonthDataModel
            {

                BudgetMonth = 99999,
                
            } };



            //mTableName = hierarchyTable;
            #endregion
            //retrieve hierarchy from persistent storage on server
            //To Do: Add mTableName as parameter when calling HierarchyAsync to populate hierarchy
            if (ViewModelApplication.CurrentPageViewModel != null)
            { 
                    if ( ViewModelApplication.CurrentPageViewModel.GetType().Name == "BudgetSelectionPageViewModel")
                { 
                    if ((BudgetPeriodDataModel)((BudgetPeriodListViewModel)((BudgetSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).Budget1).MSelectedBudgetPeriod == null)
                    {
                        System.Windows.MessageBox.Show(
                                  "No Budget has been selected.",
                                  "Select Budget First",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Information
                                  );
                        return;

                    }
                }
            }
            //UpdateTreeViewElements();
            CloseCommand = new RelayCommand(Close);
            EditCommand = new RelayCommand(Edit);
            //mSearchCommand = new SearchCategoryTreeCommand(this);
        }


        /// <summary>
        /// Puts the control into edit mode
        /// </summary>
        public void Edit()
        {
            // Set the edited text to the current value

            //Go into edit mode
            ViewModelApplication.CurrentControlViewModel = ((BudgetSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy;
            //((TransactionSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).Populate();
            //MSelectedCostHierarchy = MSelectedCostHierarchy;
            //((SWBillingPageViewModel)ViewModelApplication.CurrentPageViewModel).CostHierarchy.MSelectedCostHierarchy = MSelectedCostHierarchy;
            //((SWBillingPageViewModel)ViewModelApplication.CurrentPageViewModel).SelectedCostHierarchy = MSelectedCostHierarchy;
            Editing = true;
            //ViewModelApplication.CurrentControlViewModel
        }


        #endregion // Constructor


        /// <summary>
        /// Return Hierarchy of interest from Object persistence infrastructure
        /// User credentials are used to determine access authorisation
        /// </summary>
        /// <returns></returns>
        public void BuildMonthList()
        {
            //foreach (var item in ((BudgetSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).BudgetMonth)

            BudgetMonthList = new ObservableCollection<BudgetMonthDataModel>();
            if (ViewModelApplication.CurrentPageViewModel.GetType().Name == "BudgetSelectionPageViewModel")
            {
                for (var i = 0; i < ((BudgetSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).BudgetMonth.Count; i++)
                {
                    var mTVM = new BudgetMonthDataModel
                    {
                        BudgetMonth = ((BudgetSelectionPageViewModel)ViewModelApplication.CurrentPageViewModel).BudgetMonth[i],
                    };
                    BudgetMonthList.Add(mTVM);
                }
            }
            else
            {
                for (var i = 0; i < ((ExpenditureVSBudgetPageViewModel)ViewModelApplication.CurrentPageViewModel).BudgetMonth.Count; i++)
                {
                    var mTVM = new BudgetMonthDataModel
                    {
                        BudgetMonth = ((ExpenditureVSBudgetPageViewModel)ViewModelApplication.CurrentPageViewModel).BudgetMonth[i],
                    };
                    BudgetMonthList.Add(mTVM);
                }
            }
        }


 



        public void Close()
        {
            // Close settings menu


            ViewModelApplication.PopupVisible = false;



        }





    }

}