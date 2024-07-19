using Fasetto.Word.Core;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Fasetto.Word
{
    /// <summary>
    /// The Hierarchy element as a view model
    /// </summary>
    public class DocDataViewModel : BaseViewModel

    {
        #region Data

        private readonly DocDataModel mElement;

        #endregion // Data

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
        ///  Flag to indicate whether doc to be unlinked on server
        /// </summary>
        public bool IsRemove { get; set; }





        /// <summary>
        ///  String representation of GUID for selected transaction
        /// </summary>
        public string FFintranID{ get; set; }


        /// <summary>
        /// TO DO: Determine the color of the text to be displayed depending
        ///on the specific hierarchy type being displayed. Default will be UI 
        ///default color.
        /// </summary>
        public string TextColor => "FF8B0000";


        #endregion

        #region Constructors

        public DocDataViewModel()

        {

        }
        public DocDataViewModel(DocDataModel element)

                 //: this(element, null)
        {
        }




        #endregion // Constructors




 



    }
}
