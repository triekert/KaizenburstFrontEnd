using Fasetto.Word.Core;
using System.Diagnostics;

namespace Fasetto.Word
{
    /// <summary>
    /// Converts the <see cref="ApplicationPage"/> to an actual view/page
    /// </summary>
    public static class ApplicationPageHelpers
    {
        /// <summary>
        /// Takes a <see cref="ApplicationPage"/> and a view model, if any, and creates the desired page
        /// </summary>
        /// <param name="page"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public static BasePage ToBasePage(this ApplicationPage page, object viewModel = null)
        {
            // Find the appropriate page
            switch (page)
            {
                case ApplicationPage.Login:
                    return new LoginPage(viewModel as LoginViewModel);

                case ApplicationPage.Register:
                    return new RegisterPage(viewModel as RegisterViewModel);

                case ApplicationPage.Chat:
                    return new ChatPage(viewModel as ChatMessageListViewModel);

                //case ApplicationPage.Finance:
                //    return new StructurePage(viewModel as StructurePageViewModel);

                case ApplicationPage.Structure:
                    return new StructurePage(viewModel as StructurePageViewModel);

                case ApplicationPage.Hierarchy:
                    return new HierarchyPage(viewModel as HierarchyPageViewModel);

                case ApplicationPage.Actuals:
                    return new HierarchyPage(viewModel as HierarchyPageViewModel);

                case ApplicationPage.LoadMeters:
                    return new LoadReadingsPage(viewModel as LoadReadingsPageViewModel);

                case ApplicationPage.BulkRecon:
                    return new BulkReconPage(viewModel as BulkReconPageViewModel);

                case ApplicationPage.MeterSelection:
                    return new MeterSelectionPage(viewModel as MeterSelectionPageViewModel);

                case ApplicationPage.SWBilling:
                    return new SWBillingPage(viewModel as SWBillingPageViewModel);

                case ApplicationPage.Transactions:
                    return new TransactionSelectionPage(viewModel as TransactionSelectionPageViewModel);

                case ApplicationPage.TransactionAnalysis:
                    return new TransactionSelectionAnalysisPage(viewModel as TransactionSelectionAnalysisPageViewModel);

                case ApplicationPage.BudgetReview:
                    return new BudgetSelectionPage(viewModel as BudgetSelectionPageViewModel);

                case ApplicationPage.ExpenditureVSBudget:
                    return new ExpenditureVSBudgetPage(viewModel as ExpenditureVSBudgetPageViewModel);

                case ApplicationPage.InvestmentManagement:
                    return new InvestmentSelectionPage(viewModel as InvestmentSelectionPageViewModel);

                default:
                    Debugger.Break();
                    return null;
            }
        }

        /// <summary>
        /// Converts a <see cref="BasePage"/> to the specific <see cref="ApplicationPage"/> that is for that type of page
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public static ApplicationPage ToApplicationPage(this BasePage page)
        {
            // Find application page that matches the base page
            if (page is ChatPage)
                return ApplicationPage.Chat;

            if (page is LoginPage)
                return ApplicationPage.Login;

            if (page is RegisterPage)
                return ApplicationPage.Register;

            //if (page is StructurePage)
            //    return ApplicationPage.Finance;

            if (page is StructurePage)
                return ApplicationPage.Structure;

            if (page is HierarchyPage)
                return ApplicationPage.Hierarchy;

            if (page is LoadReadingsPage)
                return ApplicationPage.LoadMeters;

            if (page is MeterSelectionPage)
                return ApplicationPage.MeterSelection;

            if (page is BulkReconPage)
                return ApplicationPage.BulkRecon;

            if (page is SWBillingPage)
                return ApplicationPage.SWBilling;

            if (page is TransactionSelectionPage)
                 return ApplicationPage.Transactions;

            if (page is BudgetSelectionPage)
                return ApplicationPage.BudgetReview;

            if (page is ExpenditureVSBudgetPage)
                return ApplicationPage.ExpenditureVSBudget;

            if (page is InvestmentSelectionPage)
                return ApplicationPage.InvestmentManagement;

            // Alert developer of issue
            Debugger.Break();
            return default;
        }

    }
}
