using System;
using System.Windows.Input;
using static Fasetto.Word.DI;

namespace Fasetto.Word
{
    /// <summary>
    /// The Hierarchy element as a view model
    /// </summary>
    public class TransactionDetailViewModel : BaseViewModel

    {
        //#region Data

        //public readonly BulkReconViewModel mParent;
        //private readonly TransactionDetailDataModel mElement;
        //public ObservableCollection<BulkReconViewModel> mChildren;
        //public bool mIsExpanded;
        //public bool mIsSelected;
        //public bool mIsAllowDrop;
        //#endregion // Data


        #region Public Properties

        /// <summary>
        /// timestamp of Transaction
        /// </summary>

        public DateTime Posted_Date { get; set; }

        /// <summary>
        /// integer conversion for Year_month of transaction
        /// </summary>
        public int Month { get; set; }


        /// <summary>
        ///Description linked to transaction
        /// </summary>

        public string Description { get; set; }


        /// <summary>
        /// Transaction value - source transaction
        /// </summary>

        public decimal TransAmount { get; set; }

        /// <summary>
        /// Transaction value allocated to specific cost item
        /// </summary>


        public decimal ActualAmount { get; set; }

        /// <summary>
        /// Name of cost classification
        /// </summary>

        public string ShortName { get; set; }

        /// <summary>
        /// String representation of GUID for linked category
        /// </summary>


        public string KCategoryID { get; set; }

        /// <summary>
        /// String representation of GUID for transaction allocation


        public string KFinActualID { get; set; }

        /// <summary>
        /// String representation of GUID for originating transaction
        /// </summary>


        public string KFinTranID { get; set; }


        /// <summary>
        /// Calendar date from which Element is seen as active
        /// </summary>
        public DateTime DateEffective { get; set; }

        /// <summary>
        /// Calendar date from which Element is deactivated
        /// </summary>
        public DateTime DateDiscontinued { get; set; }

        /// <summary>
        /// Attach the current activity to a Change object
        /// </summary>
        public string KChangeID { get; set; }

        /// <summary>
        /// Title of Control
        /// </summary>
        public string Title { get; set; } = "Transaction Allocation";


        /// <summary>
        /// TO DO: Determine the color of the text to be displayed depending
        ///on the specific hierarchy type being displayed. Default will be UI 
        ///default color.
        /// </summary>
        public string TextColor => "FF8B0000";



        /// <summary>
        /// Indicates if this item can be expanded
        /// </summary>

        //public bool CanExpand => Children?.Count(f => f != null) > 0;



        #endregion

        #region Public Commands

        /// <summary>
        /// The command to close the BulkDetailsDataGrid and return to calling grid
        /// </summary>
        public ICommand CloseCommand { get; set; }

        #endregion
        #region Constructors

        public TransactionDetailViewModel()

        {
            CloseCommand = new RelayCommand(Close);
        }

        public void Close()
        {
            // Close settings menu
            ViewModelApplication.CurrentPopupViewModel = null;
            ViewModelApplication.PopupVisible = false;

        }


        #endregion // Constructors


    }
}
