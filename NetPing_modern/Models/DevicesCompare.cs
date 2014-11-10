using System.Collections.Generic;
using NetPing.Models;

namespace NetPing_modern.ViewModels
{
    public class DevicesCompare
    {
        public List<Device> Devices { get; set; }
        public List<DeviceParameter> Parameters { get; set; } 
    }
}