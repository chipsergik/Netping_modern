using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetPing.Models
{
    // Store information about File
    [Serializable]
    public class DevicePhoto
    {
        public SPTerm Dev_name { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }  // Public url of file
        public bool IsBig { get; set; }
        public bool IsCover { get; set; }
    }
}