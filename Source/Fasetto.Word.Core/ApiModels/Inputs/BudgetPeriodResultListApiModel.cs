using System.Collections.Generic;

namespace Fasetto.Word.Core
{
    /// <summary>
    /// Return of hierarchy items from database for a selected hierarchy
    /// </summary>
    public class BudgetPeriodResultListApiModel : List<BudgetPeriodResultApiModel>
    {
        public void Remove(BudgetPeriodResultListApiModel source, BudgetPeriodResultListApiModel target)
        {
            foreach (var item in source)
                target.Remove(item);
        }
        public void Clone(BudgetPeriodResultListApiModel source, BudgetPeriodResultListApiModel target)
        {
            foreach (var item in source)
                target.Add(item);
        }
    }

}
