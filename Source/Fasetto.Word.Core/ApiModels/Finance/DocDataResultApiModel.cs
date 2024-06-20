
using System;
using System.Collections.Generic;

namespace Fasetto.Word.Core
{
    /// <summary>
    /// Return of selected  document item from web server repository table on database 
    /// </summary>
    public class DocDataResultApiModel
    {
        #region Public Properties

        /// <summary>
        /// String representation of GUID for linked document
        /// </summary>
        public string KDocID { get; set; }

        /// <summary>
        ///  Name of Doc linked to Transaction
        /// </summary>
        public string DocName { get; set; }


        /// <summary>
        ///  Image of  Doc linked to Transaction
        /// </summary>
        public byte[] DocImage { get; set; }


        /// <summary>
        ///  URL of  Doc linked to Transaction
        /// </summary>
        public string DocURL { get; set; }

        #endregion
    }
}
