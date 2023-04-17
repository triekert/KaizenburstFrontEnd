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
    public class HierarchyBillingResultListApiModel : List<HierarchyBillingResultApiModel>
    {
        public void Remove(HierarchyBillingResultListApiModel source, HierarchyBillingResultListApiModel target)
        {
            foreach (var item in source)
                target.Remove(item);
        }
        public void Clone(HierarchyBillingResultListApiModel source, HierarchyBillingResultListApiModel target)
        {
            foreach(var item in source)
                target.Add(item);
        }
    }

}
