using System;
using System.Collections.Generic;
using System.Linq;

namespace Fasetto.Word.Core
{
    /// <summary>
    /// Class representing each individual element of the hierarchy 
    /// </summary>
    public class HierarchyBillingSummaryDataModel
    {
        //name of Category element
        public string ShortName { get; set; }
        //description of Category element
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
        ///Sequence no for sorting property meters
        /// </summary>
        public int Sequence { get; set; }





    }

}
