using System.Collections.Generic;

namespace Fasetto.Word.Core
{
    /// <summary>
    /// Return of hierarchy items from database for a selected hierarchy
    /// </summary>
    public class BulkReconDetailResultListApiModel : List<BulkReconDetailResultApiModel>
    {
        public void Remove(BulkReconDetailResultListApiModel source, BulkReconDetailResultListApiModel target)
        {
            foreach (var item in source)
                target.Remove(item);
        }
        public void Clone(BulkReconDetailResultListApiModel source, BulkReconDetailResultListApiModel target)
        {
            foreach (var item in source)
                target.Add(item);
        }
    }

}
