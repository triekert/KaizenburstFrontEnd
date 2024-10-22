using System;
using System.Windows.Controls;
using System.Windows.Input;
using static Fasetto.Word.DI;



namespace Fasetto.Word
{
    /// <summary>
    /// Interaction logic for FinancePage.xaml
    /// </summary>
    public partial class ExpenditureVSBudgetPage : BasePage<ExpenditureVSBudgetPageViewModel>
    {
        #region Constructor

        /// <summary>s
        /// Default constructor
        /// </summary>
        public ExpenditureVSBudgetPage() : base()
        {
            //InitializeComponent();

        }

        /// <summary>
        /// Constructor with specific view model
        /// </summary>
        /// <param name="specificViewModel">The specific view model to use for this page</param>
        public ExpenditureVSBudgetPage(ExpenditureVSBudgetPageViewModel specificViewModel) : base(specificViewModel)
        {

            //ViewModelApplication.CurrentPageViewModel = ViewModelApplication.CurrentPageViewModel;
            InitializeComponent();
        }

        #endregion

    }
}
