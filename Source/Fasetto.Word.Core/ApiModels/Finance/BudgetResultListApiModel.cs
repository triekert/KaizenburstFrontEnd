using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Fasetto.Word.Core
{ 
    /// <summary>
    /// Return of hierarchy items from database for a selected hierarchy
    /// </summary>
    public class BudgetResultListApiModel : List<BudgetResultApiModel>
    {
        public void Remove(BudgetResultListApiModel source,BudgetResultListApiModel target)
        {
            foreach (var item in source)
                target.Remove(item);
        }
        public void Clone(BudgetResultListApiModel source,BudgetResultListApiModel target)
        {
            foreach (var item in source)
                target.Add(item);
        }
    }

}
