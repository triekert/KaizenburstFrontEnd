
using System;
using System.Collections.Generic;

namespace Fasetto.Word.Core
{
    /// <summary>
    /// Return of selected  document item from web server repository table on database 
    /// </summary>
    /// 
            #region Public Properties


    public class DocDataResultApiModel
    {


        //public string DocURL => mElement.DocURL;
        /// <summary>
        /// String representation of GUID for linked document
        /// </summary>
        public string KDocID { get; set; }

        /// <summary>
        ///  Name of Doc linked to Transaction
        /// </summary>
        public string DocName { get; set; }


        /// <summary>
        ///  Readable name of Doc linked to Transaction
        /// </summary>
        public string DocDescription { get; set; }


        /// <summary>
        ///  Image of  Doc linked to Transaction
        /// </summary>
        public byte[] DocImage { get; set; }


        /// <summary>
        ///  URL of  Doc linked to Transaction
        /// </summary>
        public string DocURL { get; set; }



        /// <summary>
        ///  Flag to indicate whether doc already registered on server
        /// </summary>
        public bool IsNew { get; set; }




        /// <summary>
        ///  Flag to indicate whether doc to be unlinked on server
        /// </summary>
        public bool IsRemove { get; set; }




        /// <summary>
        ///  String representation of GUID for selected transaction
        /// </summary>
        public string FFintranID { get; set; }

#endregion Public Properties

    }
}
