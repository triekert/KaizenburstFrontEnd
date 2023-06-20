using System;
using System.Collections.Generic;
using System.Linq;

namespace Fasetto.Word.Core
{
    /// <summary>
    /// Class representing each individual element of the hierarchy 
    /// </summary>
    public class BulkReconDataModel
    {

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
        public int Missing{ get; set; }


        /// <summary>
        ///Total number of consumer meters linked to bulk meter
        /// </summary>
        public int ChildMeters{ get; set; }


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



        /// <summary>
        ///30 day moving average of mismatch per hour
        /// </summary>
        public float MonthTotMvgAvg { get; set; }



        /// <summary>
        ///30 day moving average of mismatch for timeslot
        /// </summary>
        public float MonthSlotMvgAvg { get; set; }



        /// <summary>
        ///name of Category element
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        ///description of Category element
        /// </summary>
        /// 
        public string Description { get; set; }

        /// <summary>
        //string representation of card where the expense category is determined by the linked card
        /// </summary>
        /// 
        public string Card { get; set; }

        /// <summary>
        //integer indicating the number of months between expected occurrences of expense category
        /// </summary>
        public int Frequency { get; set; }

        /// <summary>
        //string representation of GUID for a Category element
        /// </summary>
        public string KCategoryID { get; set; }

        /// <summary>
        //string representation of GUID for the Parent category of a Category element
        //the parent of all root elements will be NULL... any hierarchy will have at least one root element
        /// </summary>
        public string ParentCategoryID { get; set; }

        /// <summary>
        //string representation of GUID for a specific Client
        /// </summary>
        public string FClientID { get; set; }

        /// <summary>
        /// Parent ShortName of hiearchy item
        /// </summary>
        public string ParentShortName { get; set; }

        /// <summary>
        //the link to tthe ICON used to depict this category
        /// </summary>
        public string FIconID { get; set; }

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
        /// If a menu item, link tree item to menu Page
        /// </summary>
        public string Page { get; set; }


        /// <summary>
        /// If a menu item, link tree item to menu Page
        /// </summary>
        public string Root { get; set; }


        /// <summary>
        /// Property to indicate whether this element is a Menu Item or not..
        /// </summary>
        public bool IsMenuItem { get; set; }

        /// <summary>
        /// Property to indicate whether element is being evaluated by a change request
        /// and whether it should be excluded from current operations
        /// </summary>
        public bool IsUnderReview { get; set; }

        /// <summary>
        /// Property to indicate whether this element has been newly added change request
        /// and whether it should be excluded from current operations
        /// </summary>
        public bool IsNewElement { get; set; }


        /// <summary>
        /// Property to indicate whether this element is to be removed from the persistence layer
        /// </summary>
        public bool IsDeleteElement { get; set; }




        /// <summary>
        //sub categories that are also categories in themself
        /// </summary>
        public List<BulkReconDataModel> Children { get; set; }



    }
}
