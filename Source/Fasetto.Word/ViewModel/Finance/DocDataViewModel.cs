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

        /// <summary>
        /// String representation of GUID for linked document
        /// </summary>
        //public string KDocID => mElement.KDocID;

        /// <summary>
        ///  Name of Doc linked to Transaction
        /// </summary>
        //public string DocName => mElement.DocName;


        /// <summary>
        ///  Image of  Doc linked to Transaction
        /// </summary>
        //public byte[] DocImage => mElement.DocImage;


        /// <summary>
        ///  URL of  Doc linked to Transaction
        /// </summary>
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
        ///  Image of  Doc linked to Transaction
        /// </summary>
        public byte[] DocImage { get; set; }


        /// <summary>
        ///  URL of  Doc linked to Transaction
        /// </summary>
        public string DocURL { get; set; }



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
