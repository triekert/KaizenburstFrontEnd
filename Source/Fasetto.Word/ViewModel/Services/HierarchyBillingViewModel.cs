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
    public class HierarchyBillingViewModel : BaseViewModel

    {
        //#region Data

        public readonly HierarchyBillingViewModel mParent;
        private readonly HierarchyBillingDataModel mElement;
        public ObservableCollection<HierarchyBillingViewModel> mChildren;
        public bool mIsExpanded;
        public bool mIsSelected;
        public bool mIsAllowDrop;
        //#endregion // Data

        #region Public Properties



        /// <summary>
        /// The name of this hierarchy item
        /// </summary>
        public string ShortName => mElement.ShortName;

        /// <summary>
        /// Description of hiearchy item
        /// </summary>
        public string Description =>mElement.Description;
        /// <summary>
        /// The Identifier of this hierarchy item
        /// </summary>
        public string KCategoryID => mElement.KCategoryID;

  
        /// <summary>
        /// Parent ID  of hiearchy item
        /// </summary>
        public string ParentCategoryID =>mElement.ParentCategoryID;


        /// <summary>
        /// Calendar date from which Element is seen as active
        /// </summary>
        public DateTime DateEffective => mElement.DateEffective;

        /// <summary>
        /// Calendar date from which Element is deactivated
        /// </summary>
        public DateTime DateDiscontinued => mElement.DateDiscontinued;

        /// <summary>
        /// Total Water Consumption for period
        /// </summary>

        public decimal TotalConsumption => mElement.TotalConsumption;
        /// <summary>
        /// Water Bill
        /// </summary>
        public decimal WaterCost => mElement.WaterCost;

        /// <summary>
        /// Sewerage Bill
        /// </summary>
        public decimal SewerCost => mElement.SewerCost;

        /// <summary>
        /// Toal Cost
        /// </summary>
        public decimal TotalCost => mElement.TotalCost;
        /// <summary>
        /// Period start
        /// </summary>
        public DateTime TimeStart => mElement.TimeStart;

        /// <summary>
        /// Reading at start
        /// </summary>
        public decimal Startreading => mElement.Startreading;


        /// <summary>
        /// Period End
        /// </summary>
        public DateTime TimeEnd => mElement.TimeEnd;


        /// <summary>
        /// Reading at end of Period
        /// </summary>
        public decimal Endreading => mElement.Endreading;

        /// <summary>
        /// Start of period relevant to first Calendar month
        /// </summary>
        public DateTime DatePeriodStart => mElement.DatePeriodStart;

        /// <summary>
        ///END of period relevant to first Calendar month
        /// </summary>
        public DateTime DatePeriodEnd => mElement.DatePeriodEnd;

        /// <summary>
        /// Total Water Consumption for period In first Calendar month
        /// </summary>
        public decimal Volume => mElement.Endreading;

        /// <summary>
        /// Calculated/Predicted Water Consumption for whole first Calendar month
        /// </summary>
        public decimal VolumePredicted => mElement.VolumePredicted;

        /// <summary>
        /// Threshold level of water consumption linked to base cost in next variable
        /// </summary>
        public decimal ThresholdW => mElement.ThresholdW;

        /// <summary>
        /// Base cost for sewerage corresponding to threshold
        /// </summary>
        public decimal Basew => mElement.Basew;

        /// <summary>
        /// Tariff per each unit exceeding base
        /// </summary>
        public decimal Tariffw => mElement.Tariffw;

        /// <summary>
        /// Pro rata cost of water for billing period portion in month
        /// </summary>
        public decimal CostWater => mElement.CostWater;

        /// <summary>
        /// Threshold level of water(sewer) consumption linked to base cost in next variable
        /// </summary>
        public decimal ThresholdS => mElement.ThresholdS;

        /// <summary>
        /// Base cost for sewerage corresponding to threshold
        /// </summary>
        public decimal Bases => mElement.Bases;

        /// <summary>
        /// Tariff per each unit exceeding base
        /// </summary>
        public decimal Tariffs => mElement.Tariffs;

        /// <summary>
        /// Pro rata cost of Sewerage for billing period portion in month
        /// </summary>
        public decimal CostSewer => mElement.CostSewer;

        /// <summary>
        /// Start of period relevant to first Calendar month
        /// </summary>
        public DateTime DatePeriodStartN => mElement.DatePeriodStartN;

        /// <summary>
        ///END of period relevant to first Calendar month
        /// </summary>
        public DateTime DatePeriodEndN => mElement.DatePeriodEndN;

        /// <summary>
        /// Total Water Consumption for period In first Calendar month
        /// </summary>
        public decimal VolumeN => mElement.VolumeN;

        /// <summary>
        /// Calculated/Predicted Water Consumption for whole first Calendar month
        /// </summary>
        public decimal VolumePredictedN => mElement.VolumePredictedN;

        /// <summary>
        /// Threshold level of water consumption linked to base cost in next variable
        /// </summary>
        public decimal ThresholdWN => mElement.ThresholdWN;

        /// <summary>
        /// Base cost for sewerage corresponding to threshold
        /// </summary>
        public decimal BasewN => mElement.BasewN;

        /// <summary>
        /// Tariff per each unit exceeding base
        /// </summary>
        public decimal TariffwN => mElement.TariffwN;

        /// <summary>
        /// Pro rata cost of water for billing period portion in month
        /// </summary>
        public decimal CostWaterN => mElement.CostWaterN;

        /// <summary>
        /// Threshold level of water(sewer) consumption linked to base cost in next variable
        /// </summary>
        public decimal ThresholdSN => mElement.ThresholdSN;

        /// <summary>
        /// Base cost for sewerage corresponding to threshold
        /// </summary>
        public decimal BasesN => mElement.BasesN;

        /// <summary>
        /// Tariff per each unit exceeding base
        /// </summary>
        public decimal TariffsN => mElement.TariffsN;

        /// <summary>
        /// Pro rata cost of Sewerage for billing period portion in month
        /// </summary>
        public decimal CostSewerN => mElement.CostSewerN;


        /// <summary>
        /// A list of all children containd inside this item
        /// </summary>
        public ObservableCollection<HierarchyBillingViewModel> Children => mChildren;


        /// <summary>
        /// Title of Control
        /// </summary>
        public string Title { get; set; } = "Water and Sewerage Billing";


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

        public HierarchyBillingViewModel()

        {

        }
        public HierarchyBillingViewModel(HierarchyBillingDataModel element)

                 : this(element, null)
        {
        }

        private HierarchyBillingViewModel(HierarchyBillingDataModel element, HierarchyBillingViewModel parent)
        {
            mElement = element;
            mParent = parent;
            var exception = default(Exception);
            try
            { 
        
            mChildren = new ObservableCollection<HierarchyBillingViewModel>(
                    (from child in mElement.Children orderby(mElement.ShortName)
                     select new HierarchyBillingViewModel(child, this))
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
