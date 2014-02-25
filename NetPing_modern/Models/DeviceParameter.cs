using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetPing.Models
{
    [Serializable]
    public class DeviceParameter
    {
        public int Id { get; set; }
        public SPTerm Name { get; set; }
        public SPTerm Device { get; set; }
        public string Value { get; set; }

    }
}