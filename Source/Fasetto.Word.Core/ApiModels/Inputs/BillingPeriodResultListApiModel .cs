using System.Collections.Generic;

namespace Fasetto.Word.Core
{
    /// <summary>
    /// Return of hierarchy items from database for a selected hierarchy
    /// </summary>
    public class CostHierarchyResultListApiModel : List<CostHierarchyResultApiModel>
    {
        public void Remove(CostHierarchyResultListApiModel source, CostHierarchyResultListApiModel target)
        {
            foreach (var item in source)
                target.Remove(item);
        }
        public void Clone(CostHierarchyResultListApiModel source, CostHierarchyResultListApiModel target)
        {
            foreach (var item in source)
                target.Add(item);
        }
    }

}
