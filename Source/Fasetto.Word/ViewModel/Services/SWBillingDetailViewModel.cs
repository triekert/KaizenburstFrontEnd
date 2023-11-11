using Fasetto.Word.Core;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Fasetto.Word
{
    /// <summary>
    /// The Hierarchy element as a view model
    /// </summary>
    public class SWBillingDetailViewModel : BaseViewModel

    {
        //#region Data

        public readonly SWBillingDetailViewModel mParent;
        private readonly SWBillingDetailDataModel mElement;
        public ObservableCollection<SWBillingDetailViewModel> mChildren;
        public bool mIsExpanded;
        public bool mIsSelected;
        public bool mIsAllowDrop;
        //#endregion // Data

        #region Public Properties

        public string ShortName { get; set; }
        //description of Category element
        public string Description { get; set; }

        //string representation of GUID for a Category element
        public string KCategoryID { get; set; }

        //TO DO: If more convenient to include the BIlling Period GUID in the API result call....
        ////string representation of GUID for the selected billing  period
        //public string KBillingPeriodID { get; set; }


        //sstring representation of GUID for the Parent category of a Category element
        //the parent of all root elements will be NULL... any hierarchy will have at least one root element

        public string ParentCategoryID { get; set; }

        /// <summary>
        /// Calendar date from which Element is seen as active
        /// </summary>
        public DateTime DateEffective { get; set; }

        /// <summary>
        /// Calendar date from which Element is deactivated
        /// </summary>
        public DateTime DateDiscontinued { get; set; }

        /// <summary>
        /// Total Water Consumption for period
        /// </summary>

        public decimal TotalConsumption { get; set; }

        /// <summary>
        /// Water Bill
        /// </summary>
        public decimal WaterCost { get; set; }

        /// <summary>
        /// Sewerage Bill
        /// </summary>
        public decimal SewerCost { get; set; }

        /// <summary>
        /// Toal Cost
        /// </summary>
        public decimal TotalCost { get; set; }

        /// <summary>
        /// Period start
        /// </summary>
        public DateTime TimeStart { get; set; }

        /// <summary>
        /// Reading at start
        /// </summary>
        public decimal Startreading { get; set; }


        /// <summary>
        /// Period End
        /// </summary>
        public DateTime TimeEnd { get; set; }


        /// <summary>
        /// Reading at end of Period
        /// </summary>
        public decimal Endreading { get; set; }

        /// <summary>
        /// Start of period relevant to first Calendar month
        /// </summary>
        public DateTime DatePeriodStart { get; set; }

        /// <summary>
        ///END of period relevant to first Calendar month
        /// </summary>
        public DateTime DatePeriodEnd { get; set; }

        /// <summary>
        /// Total Water Consumption for period In first Calendar month
        /// </summary>
        public decimal Volume { get; set; }

        /// <summary>
        /// Calculated/Predicted Water Consumption for whole first Calendar month
        /// </summary>
        public decimal VolumePredicted { get; set; }

        /// <summary>
        /// Threshold level of water consumption linked to base cost in next variable
        /// </summary>
        public decimal ThresholdW { get; set; }

        /// <summary>
        /// Base cost for sewerage corresponding to threshold
        /// </summary>
        public decimal Basew { get; set; }

        /// <summary>
        /// Tariff per each unit exceeding base
        /// </summary>
        public decimal Tariffw { get; set; }

        /// <summary>
        /// Pro rata cost of water for billing period portion in month
        /// </summary>
        public decimal CostWater { get; set; }

        /// <summary>
        /// Threshold level of water(sewer) consumption linked to base cost in next variable
        /// </summary>
        public decimal ThresholdS { get; set; }

        /// <summary>
        /// Base cost for sewerage corresponding to threshold
        /// </summary>
        public decimal Bases { get; set; }

        /// <summary>
        /// Tariff per each unit exceeding base
        /// </summary>
        public decimal Tariffs { get; set; }

        /// <summary>
        /// Pro rata cost of Sewerage for billing period portion in month
        /// </summary>
        public decimal CostSewer { get; set; }

        /// <summary>
        /// Start of period relevant to first Calendar month
        /// </summary>
        public DateTime DatePeriodStartN { get; set; }

        /// <summary>
        ///END of period relevant to first Calendar month
        /// </summary>
        public DateTime DatePeriodEndN { get; set; }

        /// <summary>
        /// Total Water Consumption for period In first Calendar month
        /// </summary>
        public decimal VolumeN { get; set; }

        /// <summary>
        /// Calculated/Predicted Water Consumption for whole first Calendar month
        /// </summary>
        public decimal VolumePredictedN { get; set; }

        /// <summary>
        /// Threshold level of water consumption linked to base cost in next variable
        /// </summary>
        public decimal ThresholdWN { get; set; }

        /// <summary>
        /// Base cost for sewerage corresponding to threshold
        /// </summary>
        public decimal BasewN { get; set; }

        /// <summary>
        /// Tariff per each unit exceeding base
        /// </summary>
        public decimal TariffwN { get; set; }

        /// <summary>
        /// Pro rata cost of water for billing period portion in month
        /// </summary>
        public decimal CostWaterN { get; set; }

        /// <summary>
        /// Threshold level of water(sewer) consumption linked to base cost in next variable
        /// </summary>
        public decimal ThresholdSN { get; set; }

        /// <summary>
        /// Base cost for sewerage corresponding to threshold
        /// </summary>
        public decimal BasesN { get; set; }

        /// <summary>
        /// Tariff per each unit exceeding base
        /// </summary>
        public decimal TariffsN { get; set; }

        /// <summary>
        /// Pro rata cost of Sewerage for billing period portion in month
        /// </summary>
        public decimal CostSewerN { get; set; }

        /// <summary>
        ///Adjustment related to the portion of consumption in the first month
        /// </summary>
        public decimal Adjustment { get; set; }


        /// <summary>
        ///Adjustment related to the portion of consumption in the Next month
        /// </summary>
        public decimal AdjustmentN { get; set; }

        /// <summary>
        /// Aggregate adjustment to water bill from prior periods
        /// </summary>
        public decimal CostWaterAdjust { get; set; }


        /// <summary>
        /// Aggregate adjusdtment to sewerage bill from prior periods
        /// </summary>
        public decimal CostSewerAdjust { get; set; }



        /// <summary>
        /// Aggregate adjustment to total bill from prior periods
        /// </summary>
        public decimal CostTotalAdjust { get; set; }






        /// <summary>
        /// Last Reading before Period Start
        /// </summary>
        public decimal MeterReadingSP { get; set; }

        /// <summary>
        ///Timestamp of last Reading before Period Start
        /// </summary>
        public DateTime DateSP { get; set; }

        /// <summary>
        /// Reading at end of Period
        /// </summary>
        public decimal MeterReadingSN { get; set; }

        /// <summary>
        ///END of period relevant to first Calendar month
        /// </summary>
        public DateTime DateSN { get; set; }

        /// <summary>
        /// Last Reading before Period End
        /// </summary>
        public decimal MeterReadingFP { get; set; }
        /// <summary>
        ///Timestamp of  Last Reading before Period End
        /// </summary>
        public DateTime DateFP { get; set; }


        /// <summary>
        /// First Reading after Period End
        /// </summary>
        public decimal MeterReadingFN { get; set; }
        /// <summary>
        ///Timestamp of First Reading after Period End
        /// </summary>
        public DateTime DateFN { get; set; }



        /// <summary>
        ///Billing period Start timestamp
        /// </summary>
        public DateTime BillingStart { get; set; }

        /// <summary>
        ////Billing period End timestamp
        /// </summary>
        public DateTime BillingEnd { get; set; }


        /// <summary>
        /// Start Reading for billing period
        /// </summary>
        public decimal ReadingStart { get; set; }

        /// <summary>
        ///End Reading for billing period
        /// </summary>
        public decimal ReadingEnd { get; set; }

        /// <summary>
        /// A list of all children containd inside this item
        /// </summary>
        public ObservableCollection<SWBillingDetailViewModel> Children => mChildren;


        /// <summary>
        /// Title of Control
        /// </summary>
        public string Title { get; set; } = "S&W Billing Detail";


        /// <summary>
        /// TO DO: Determine the color of the text to be displayed depending
        ///on the specific hierarchy type being displayed. Default will be UI 
        ///default color.
        /// </summary>
        public string TextColor => "FF8B0000";


        /// <summary>
        /// Indicates if this item can be expanded
        /// </summary>

        public bool CanExpand => Children?.Count(f => f != null) > 0;



        #endregion
        #region Data

        public HierarchyListDataModel mHDML;
        #endregion
        #region Public Commands

        /// <summary>
        /// The command to expand this item
        /// </summary>
        public ICommand ExpandCommand { get; set; }

        #endregion
        #region Constructors

        public SWBillingDetailViewModel()

        {

        }
        public SWBillingDetailViewModel(SWBillingDetailDataModel element)

                 : this(element, null)
        {
        }

        private SWBillingDetailViewModel(SWBillingDetailDataModel element, SWBillingDetailViewModel parent)
        {
            mElement = element;
            mParent = parent;
            var exception = default(Exception);
            try
            { 
        
            mChildren = new ObservableCollection<SWBillingDetailViewModel>(
                    (from child in mElement.Children orderby(mElement.ShortName)
                     select new SWBillingDetailViewModel(child, this))
                     .ToList());
            }
            catch (Exception ex)
            {
                exception = ex;
            }

        }



        #endregion // Constructors


        #region Presentation Members

        #region IsExpanded

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get => mIsExpanded;
            set
            {
                if (value != mIsExpanded)
                {
                    mIsExpanded = value;
                    //OnPropertyChanged("IsExpanded");
                }

                // Expand all the way up to the root.
                if (mIsExpanded && mParent != null)
                    mParent.IsExpanded = true;
                var mDescription = mElement.Description;
            }
        }

        #endregion // IsExpanded

        #region IsSelected

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is selected in the UI.
        /// </summary>
        public bool IsSelected
        {
            get => mIsSelected;
            set
            {
                if (value != mIsSelected)
                {
                    mIsSelected = value;
                    //var kCategoryID = KCategoryID;

                    var name1 = ShortName;
                    //OnPropertyChanged("IsSelected");
                }
            }
        }

        #endregion // IsSelected
        #region IsAllowDrop

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is selected.
        /// </summary>
        public bool IsAllowDrop
        {
            get => mIsAllowDrop;
            set =>
                //if (value != _isAllowDrop)
                //{
                //    _isAllowDrop = value;
                //    //int ndx =base.GetEnumerator();
                //    string name1 = this.ShortName;
                //    this.OnPropertyChanged("IsAllowDrop");
                //}
                mIsAllowDrop = true;
        }

        #endregion // IsAllowDreop
        #region TreeView_MouseDown

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is selected.
        /// </summary>
        public void TreeView_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)

        {
            _ = MessageBox.Show("You clicked me at tree view item level ");
        }



        #endregion // IsSelected

        #region NameContainsText

        /// <summary>
        /// Check that the ShortName field contains data to enable the search
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool NameContainsText(string text)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(ShortName))
                return false;

            return ShortName.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) > -1;
        }

        #endregion // NameContainsText

        #region KCategoryIdContainsText

        /// <summary>
        /// Check that the KCategoryId field contains data to enable the search
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool KCategoryIdContainsText(string text)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(KCategoryID))
                return false;

            return KCategoryID.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) > -1;
        }

        #endregion // NameContainsText



        /// <summary>
        /// Check that the KCategoryId field contains data to enable the search
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>


        //#region Parent

        ////public HierarchyViewModel Parent => mParent;

        //#endregion // Parent

        #endregion // Presentation Members        
        #region Helper Method

        /// <summary>
        /// Removes all children from the list, adding a dummy item to show the expand icon if required
        /// </summary>
        //public void ClearChildren()
        //{
        //    // Clear Items
        //    Children = new ObservableCollection<HierarchyViewModel>();

        //    //check for children
        //    var children = from element in mHDML
        //                   where element.ParentCategoryID == KCategoryID
        //                   select (element.ShortName, element.Description, element.KCategoryID, element.ParentCategoryID);
        //    // Show the expand arrow if we are not a file
        //    if (children.Count() > 0)
        //       Children.Add(null);
        //}

        #endregion

        /// <summary>
        /// Epands this directory and finds all the children
        /// </summary>
        private void Expand()
        {



            //// Find all children
            //var children = from element in mHDML
            //               where element.ParentCategoryID == KCategoryID
            //               select (element.ShortName, element.Description, element.KCategoryID, element.ParentCategoryID);
            //// Hierarchy cannot be expanded
            //if (children.Count() == 0)
            //    return;
            //Children = new ObservableCollection<HierarchyViewModel>(
            //    children.Select(child => new HierarchyViewModel(child.ShortName, child.Description, child.KCategoryID, mHDML)));
        }



        //    #region INotifyPropertyChanged Members

        //    public event PropertyChangedEventHandler PropertyChanged;

        //    protected new virtual void OnPropertyChanged(string propertyName)
        //    {
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //    }

        //    #endregion // INotifyPropertyChanged Members
        //
    }
}
