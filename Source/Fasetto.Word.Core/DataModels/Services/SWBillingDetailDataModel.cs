using System;
using System.Collections.Generic;
using System.Linq;

namespace Fasetto.Word.Core
{
    /// <summary>
    /// Class representing each individual element of the hierarchy 
    /// </summary>
    public class SWBillingDetailDataModel
    {
        //name of Category element
        public string ShortName { get; set; }
        //description of Category element
        public string Description { get; set; }
        ////string representation of card where the expense category is determined by the linked card
        //public string Card { get; set; }
        ////integer indicating the number of months between expected occurrences of expense category
        //public int Frequency { get; set; }
        //string representation of GUID for a Category element
        public string KCategoryID { get; set; }
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
        /// Manual adjustment of consumption,first month
        /// </summary>
        public decimal Adjustment { get; set; }


        /// <summary>
        /// Manual adjustment of consumption,first month
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

        public List<SWBillingDetailDataModel> Children { get; set; }





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


    }
}
