using System.Collections.Generic;
using NetPing.Models;

namespace NetPing_modern.Models
{
    public class DeviceParameterEqualityComparer : IEqualityComparer<DeviceParameter>
    {
        public bool Equals(DeviceParameter x, DeviceParameter y)
        {
            return string.Equals(x.Name.OwnNameFromPath, y.Name.OwnNameFromPath);
        }

        public int GetHashCode(DeviceParameter obj)
        {
            return obj.Name.OwnNameFromPath.GetHashCode();
        }
    }
}