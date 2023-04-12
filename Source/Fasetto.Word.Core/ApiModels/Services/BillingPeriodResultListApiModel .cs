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
    public class BillingPeriodResultListApiModel : List<BillingPeriodResultApiModel>
    {
        public void Remove(BillingPeriodResultListApiModel source, BillingPeriodResultListApiModel target)
        {
            foreach (var item in source)
                target.Remove(item);
        }
        public void Clone(BillingPeriodResultListApiModel source, BillingPeriodResultListApiModel target)
        {
            foreach (var item in source)
                target.Add(item);
        }
    }

}
