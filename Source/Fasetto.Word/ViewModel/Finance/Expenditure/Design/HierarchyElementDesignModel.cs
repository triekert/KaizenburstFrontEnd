using System.Collections.Generic;

namespace Fasetto.Word
{
    /// <summary>
    /// The design-time data for a <see cref="SettingsDesignModel"/>
    /// </summary>
    public class HierarchyElementDesignModel : HierarchyElementViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model
        /// </summary>
        public static HierarchyElementDesignModel Instance => new HierarchyElementDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public HierarchyElementDesignModel()
        {
            ShortName = new TextEntryViewModel { Label = "Node Name", OriginalText = "New Node" };
            Description = new TextEntryViewModel { Label = "Node Description", OriginalText = "New Node Description" };
            KCategoryID = "132AB-AF1245-941QW" ;
            ParentCategoryID = "132AB-AF1245-941QW" ;
            ParentShortName = "Parent Node";
        }

        #endregion
    }
}
