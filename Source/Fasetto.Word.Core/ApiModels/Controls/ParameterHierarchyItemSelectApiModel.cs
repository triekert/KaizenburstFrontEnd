using System;
using System.Collections.Generic;
using System.Linq;

namespace Fasetto.Word.Core.ApiModels.Controls
{
    /// <summary>
    /// Class representing each individual element of the hierarchy 
    /// </summary>
    /// 
    public class ParameterHierarchyItemSelectApiModel
    {
        // <summary>
        /// GUID of Client
        /// </summary>
        public string ClientID { get; set; }


        // <summary>
        /// GUID of Hierarchy Type
        /// </summary>
        public string HierarchyTypeID { get; set; }

        // <summary>
        /// GUID of Root item if only a subset is to be retrieved
        /// </summary>
        public string RootID { get; set; }


        // <summary>
        /// GUID of hierarchy
        /// </summary>
        public string FHierarchyID { get; set; }



        // <summary>
        /// Level limit for hierarchy's to be returned (1 = top level only...)
        /// </summary>
        public int Level { get; set; }
    }

}


