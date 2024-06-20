using System.Collections.Generic;

namespace Fasetto.Word
{
    /// <summary>
    /// A view model for the overview chat list
    /// </summary>
    public class DocDataListViewModel : BaseViewModel
    {
        /// <summary>
        /// The chat list items for the list
        /// </summary>
        public List<DocDataViewModel> Items { get; set; }
    }
}
