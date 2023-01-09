using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Fasetto.Word.Core.ApiModels.Services
{
    /// <summary>
    /// Return of hierarchy items from database for a selected hierarchy
    /// </summary>
    public class BulkReconResultListApiModel : List<BulkReconResultApiModel>
    {
        public void Remove(BulkReconResultListApiModel source, BulkReconResultListApiModel target)
        {
            foreach (var item in source)
                target.Remove(item);
        }
        public void Clone(BulkReconResultListApiModel source, BulkReconResultListApiModel target)
        {
            foreach (var item in source)
                target.Add(item);
        }
    }

}
