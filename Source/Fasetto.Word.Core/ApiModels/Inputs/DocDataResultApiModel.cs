using Fasetto.Word.Core;
using System.Collections.Generic;
using Fasetto.Word;
namespace Fasetto.Word.Core
{
    /// <summary>
    /// Return of selected document item from web server repository table on database 
    /// </summary>
    public class DocDataResultApiModel : BaseModelResult<DocDataModel>
    {
        #region Constructors

        public DocDataResultApiModel() : base() { }

        public DocDataResultApiModel(DocDataModel element) : base(element) { }

        #endregion

        #region API-Specific Properties (Not part of core DocDataModel)

        /// <summary>
        /// Flag to indicate whether doc to be unlinked on server
        /// </summary>
        public bool IsRemove { get; set; }

        #endregion
    }

    /// <summary>
    /// Return of hierarchy items from database for a selected hierarchy
    /// </summary>
    public class DocDataResultListApiModel : List<DocDataResultApiModel>
    {
        public void Remove(DocDataResultListApiModel source, DocDataResultListApiModel target)
        {
            foreach (var item in source)
                target.Remove(item);
        }

        public void Clone(DocDataResultListApiModel source, DocDataResultListApiModel target)
        {
            foreach (var item in source)
                target.Add(item);
        }
    }
}