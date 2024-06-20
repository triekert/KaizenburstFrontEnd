namespace Fasetto.Word.Core
{
    /// <summary>
    /// Class representing each individual element of the hierarchy 
    /// </summary>
    public class DocDataModel
    {

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



        /// <summary>
        ///  URL of  Doc linked to Transaction
        /// </summary>
        public bool IsNew { get; set; }

    }
}
