using System;
using System.Collections.Generic;
using System.Text;

namespace Fasetto.Word.Core
{


    public class WaterReading
    {
        public string SiteName { get; set; }
        public List<Reading> Readings { get; set; }
    }

    public class Reading
    {
        public decimal Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DeviceId { get; set; }
        public bool Active { get; set; }
        public string MeterType { get; set; }
        public string sitename { get; set; }
        public string Battery { get; set; }
        public string RSSI { get; set; }
        public string LQI { get; set; }
        public string timestamp { get; set; }
        public decimal Value { get; set; }
        public string fastLeak { get; set; }
        public string comment { get; set; }
    }


}

