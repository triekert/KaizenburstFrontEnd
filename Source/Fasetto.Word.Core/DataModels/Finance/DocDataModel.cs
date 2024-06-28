namespace Fasetto.Word.Core
{
    /// <summary>
    /// Class representing each individual element of the hierarchy 
    /// </summary>
    public class DocDataModel
    {
        #region Data
        private readonly DocDataModel mElement;

        #endregion  Data

        #region Public Properties


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
        ///  String representation of GUID for selected transaction
        /// </summary>
        public string FFintranID { get; set; }
        #endregion Public Properties

    }        

}
