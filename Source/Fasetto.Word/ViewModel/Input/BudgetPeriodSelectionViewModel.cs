using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using static Fasetto.Word.DI;

namespace Fasetto.Word
{
    /// <summary>
    /// The view model for managing budget period and budget month selections.
    /// </summary>
    public class BudgetPeriodSelectionViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// The label to identify what this control is for
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// List of available budget periods
        /// </summary>
        public ObservableCollection<BudgetPeriodDataModel> BudgetPeriodList { get; set; }

        /// <summary>
        /// Currently selected budget period
        /// </summary>
        private BudgetPeriodDataModel mSelectedBudget;
        public BudgetPeriodDataModel MSelectedBudget
        {
            get => mSelectedBudget;
            set
            {
                if (mSelectedBudget == value)
                    return;

                mSelectedBudget = value;
                OnBudgetSelected(value);
            }
        }

        /// <summary>
        /// List of valid budget months generated for the selected budget
        /// </summary>
        public ObservableCollection<BudgetMonthDataModel> BudgetMonthList { get; set; }

        /// <summary>
        /// Currently selected budget month
        /// </summary>
        public BudgetMonthDataModel MSelectedBudgetMonth { get; set; }

        /// <summary>
        /// Indicates if the control is in edit mode
        /// </summary>
        public bool Editing { get; set; }

        /// <summary>
        /// Indicates if the control is pending an update / working
        /// </summary>
        public bool Working { get; set; }

        #endregion

        #region Public Commands

        public ICommand EditCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand SaveCommand { get; set; }

        #endregion

        #region Constructor

        public BudgetPeriodSelectionViewModel()
        {
            EditCommand = new RelayCommand(async () => await EditAsync());
            CancelCommand = new RelayCommand(async () => await CancelAsync());
            SaveCommand = new RelayCommand(async () => await SaveAsync());

            BudgetPeriodList = new ObservableCollection<BudgetPeriodDataModel>();
            BudgetMonthList = new ObservableCollection<BudgetMonthDataModel>();
        }

        #endregion

        #region Command Methods

        public async Task EditAsync()
        {
            await RunCommandAsync(() => Working, async () =>
            {
                Editing = true;
                await Task.Delay(50);
            });
        }

        public async Task SaveAsync()
        {
            await RunCommandAsync(() => Working, async () =>
            {
                Editing = false;
                await Task.Delay(50);
            });
        }

        public async Task CancelAsync()
        {
            await Task.CompletedTask;
            Editing = false;
        }

        #endregion

        #region Selection Logic

        /// <summary>
        /// Handles business logic when a budget is selected, updating page view models and rebuilding months.
        /// </summary>
        private void OnBudgetSelected(BudgetPeriodDataModel selectedBudget)
        {
            if (selectedBudget == null)
                return;

            var currentPage = ViewModelApplication.CurrentPageViewModel;
            if (currentPage == null)
                return;

            if (currentPage is BudgetSelectionPageViewModel budgetSelectionVM)
            {
                budgetSelectionVM.SelectedBudget = selectedBudget;
                budgetSelectionVM.AddMonthRange();
                if (budgetSelectionVM.BudgetMonthList is BudgetMonthListViewModel monthListVM1)
                {
                    monthListVM1.BuildMonthList();
                    BudgetMonthList = monthListVM1.BudgetMonthList;
                }
            }
            else if (currentPage is ExpenditureVSBudgetPageViewModel expenditureVM)
            {
                expenditureVM.SelectedBudget = selectedBudget;
                expenditureVM.AddMonthRange();
                if (expenditureVM.BudgetMonthList is BudgetMonthListViewModel monthListVM2)
                {
                    monthListVM2.BuildMonthList();
                    BudgetMonthList = monthListVM2.BudgetMonthList;
                }
            }
        }

        #endregion
    }
}