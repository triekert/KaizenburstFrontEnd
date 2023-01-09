using System;
using System.Collections.Generic;

namespace Fasetto.Word.Core
{
    /// <summary>
    /// Persistance of expense hierarchy item on database
    /// </summary>
    public class BulkReconApiModel
    {
        #region Public Properties


        /// <summary>
        /// string representation of BulkMeter Name
        /// </summary>
        public string BulkMeter { get; set; }


        /// <summary>
        ///timestamp of TimeSlot
        /// </summary>
        public DateTime TimeSlotStart { get; set; }


        /// <summary>
        ///Number of meters for which no readings currently received for  TImeSlot
        /// </summary>
        public int Missing { get; set; }


        /// <summary>
        ///Total number of consumer meters linked to bulk meter
        /// </summary>
        public int ChildMeters { get; set; }


        /// <summary>
        ///Consumption recorded/calculated for bulk meter for Timeslot
        /// </summary>
        public float VolumeIn { get; set; }


        /// <summary>
        ///Aggregate consumption recorded/calculated on all consumer meters for Timeslot
        /// </summary>
        public float VolumeOut { get; set; }


        /// <summary>
        ///Aggregate consumption difference between input and output  for Timeslot
        /// </summary>
        public float VolumeDelta { get; set; }


        /// <summary>
        ///Moving average for calculated Delta  for Timeslot
        /// </summary>
        public float MovingAvgDelta { get; set; }


        /// <summary>
        ///VolumeDelta expressed as a percentage of VolumeIn  for Timeslot
        /// </summary>
        public float PercDelta { get; set; }


        #endregion
    }
}
