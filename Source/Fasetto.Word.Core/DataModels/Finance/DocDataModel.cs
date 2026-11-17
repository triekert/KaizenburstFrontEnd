using System;

namespace Fasetto.Word.Core
{
    /// <summary>
    /// Class representing each individual element of the hierarchy data model
    /// </summary>
    public class DocDataModel
    {
        public string KDocID { get; set; }
        public string DocName { get; set; }
        public string DocDescription { get; set; }
        public string DocCreator { get; set; }
        public DateTime DateCreated { get; set; }
        public byte[] DocImage { get; set; }
        public string DocURL { get; set; }
        public bool IsNew { get; set; }
        public string FFintranID { get; set; }
    }
}
