using Fasetto.Word.Core;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using static Fasetto.Word.DI;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace Fasetto.Word
{
    /// <summary>
    /// The Hierarchy element as a view model
    /// </summary>
    public class BulkReconDetailViewModel : BaseViewModel

    {
        //#region Data

        //public readonly BulkReconViewModel mParent;
        private readonly BulkReconDataModel mElement;
        //public ObservableCollection<BulkReconViewModel> mChildren;
        //public bool mIsExpanded;
        //public bool mIsSelected;
        //public bool mIsAllowDrop;
        //#endregion // Data

        #region Public Properties

        /// <summary>
        /// GUID of  Property 
        /// </summary>
        //public string Meter { get; set; }

        /// <summary>
        ///name of property/Bulkmeter
        /// </summary>
        public string ShortName { get; set; }


        /// <summary>
        ///timestamp  beginning
        /// </summary>
        public DateTime TimeStart { get; set; }


        /// <summary>
        ///Consumption for meter over defined period
        /// </summary>
        public float Volume { get; set; }

        /// <summary>
        ///Meter reading (calculated) for start of measurement period
        /// </summary>
        public float MeterReadingCalc{ get; set; }


        /// <summary>
        ///timestamp  of last meter reading prior to start of period
        /// </summary>
        public DateTime ReadingTimePrior { get; set; }

        /// <summary>
        /// Last Actual Meter reading obtained prior to start of measurement period
        /// </summary>
        public float ReadingPrior { get; set; }


        /// <summary>
        ///timestamp  of first meter reading after start of period
        /// </summary>
        public DateTime ReadingTimeNext { get; set; }

        /// <summary>
        /// First Actual Meter reading obtained after start of measurement period
        /// </summary>
        public float ReadingNext { get; set; }

        /// <summary>
        ///timestamp  end
        /// </summary>
        public DateTime TimeEnd { get; set; }

        /// <summary>
        ///Meter reading (calculated) for end of measurement period
        /// </summary>
        public float MeterReadingCalcE { get; set; }


        /// <summary>
        ///timestamp  of last meter reading prior to End of period
        /// </summary>
        public DateTime ReadingTimePriorE { get; set; }

        /// <summary>
        /// Last Actual Meter reading obtained prior to end of measurement period
        /// </summary>
        public float ReadingPriorE { get; set; }


        /// <summary>
        ///timestamp  of first meter reading after end of period
        /// </summary>
        public DateTime ReadingTimeNextE { get; set; }

        /// <summary>
        /// First Actual Meter reading obtained after start of measurement period
        /// </summary>
        public float ReadingNextE { get; set; }



    
        /// <summary>
        ///description of Category element
        /// </summary>
        /// 
        public string Description { get; set; }

        /// <summary>
        /// Title of Control
        /// </summary>
        public string Title { get; set; } = "Bulk Meter Reconciliation Detail ";



        /// <summary>
        /// GUID of BulkMeter linked Property
        /// </summary>
        public string BulkMeter { get; set; }



        /// <summary>
        /// Name of BulkMeter
        /// </summary>
        public string BulkMeterName { get; set; }

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

        public BulkReconDetailViewModel()

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
