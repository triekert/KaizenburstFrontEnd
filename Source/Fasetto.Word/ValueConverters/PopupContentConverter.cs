using Fasetto.Word.Core;
using System;
using System.Globalization;

namespace Fasetto.Word
{
    /// <summary>
    /// A converter that takes a <see cref="PopupContent"/> and converts it to the 
    /// correct UI element
    /// </summary>
    public class PopupContentConverter : BaseValueConverter<PopupContentConverter>
    {
        #region Protected Members


 

        #endregion

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Get the side menu type
            var popupType = (PopupContent)value;

            // Switch based on type
            switch (popupType)
            {

                case PopupContent.AddElement:
                    var mHierarchyElementControl = new HierarchyElementControl();
                    return mHierarchyElementControl;

                case PopupContent.Settings:
                    var mSettings= new SettingsControl();
                    return mSettings;

                case PopupContent.Finance:
                    var mFinanceControl = new FinanceSelectionControl();
                    return mFinanceControl;

                case PopupContent.BulkReconDetail:
                    var mBulkReconDetailControl = new BulkReconDetailControl();
                    return mBulkReconDetailControl;

                case PopupContent.BulkRecon:
                    var mBulkReconControl = new BulkReconControl();
                    return mBulkReconControl;

                case PopupContent.BulkMeterSelectionControl:
                    var mBulkMeterSelectionControl = new BulkMeterSelectionControl();
                    return mBulkMeterSelectionControl;

                case PopupContent.HierarchySelection:                    
                    var mHierarchySelection = new HierarchySelectionControl();
                    return mHierarchySelection;

                case PopupContent.SWBilling:
                    var mSWBillingControl = new SWBillingControl();
                    return mSWBillingControl;

                case PopupContent.SWAdjust:
                    var mSWAdjustControl = new SWAdjustControl();
                    return mSWAdjustControl;

                case PopupContent.Transaction:
                    var mTransactionControl = new TransactionControl();
                    return mTransactionControl;

                case PopupContent.TransactionDetail:
                    var mTransactionDetailControl = new TransactionDetailControl();
                    return mTransactionDetailControl;

                case PopupContent.Classify:
                    var mManageClassificationControl = new ManageClassificationControl();
                    return mManageClassificationControl;

                case PopupContent.SWBillingDetail:
                    var mSWBillingDetailControl = new SWBillingDetailControl();
                    return mSWBillingDetailControl;

                case PopupContent.BudgetReview:
                    var mBudgetReviewControl = new BudgetReviewControl();
                    return mBudgetReviewControl;

                case PopupContent.BudgetAdjust:
                    var mBudgetAdjustControl = new BudgetAdjustControl();
                    return mBudgetAdjustControl;

                case PopupContent.ExpenditureReview:
                    var mExpenditureReviewControl = new ExpenditureReviewControl();
                    return mExpenditureReviewControl;

                case PopupContent.ExpenditureAdjust:
                    var mExpenditureAdjustControl = new ExpenditureAdjustControl();
                    return mExpenditureAdjustControl;

                case PopupContent.StockHoldingAdjust:
                    var mStockHoldingAdjustControl = new StockHoldingAdjustControl();
                    return mStockHoldingAdjustControl;

                case PopupContent.InvestmentReview:
                    var mInvestmentReviewControl = new StockHoldingAdjustControl();
                    return mInvestmentReviewControl;

                case PopupContent.Hierarchy:
                    var mHierarchyControl = new HierarchyControl();
                    return mHierarchyControl;

                // Unknown
                default:
                    return "No UI yet, sorry :)";
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
