using System;
using System.Collections.ObjectModel;

namespace Fasetto.Word
{
    public partial class ManageClassificationViewModel
    {
        #endregion

        #region Adding to a collection
        public static class ObservableCollectionExtensions
        {
            /// <summary>
            /// Sorted add for observable collection using custom comparer
            /// </summary>
            public static void AddSorted<T>(this ObservableCollection<T> collection, T item, IComparer<T> comparer)
            {
                var sortableList = new List<T>(collection);
                var index = sortableList.BinarySearch(item, comparer);
                if (index < 0)
                    index = ~index;
                collection.Insert(index, item);
            }
        }

        #endregion Adding to a collection



    }
}
