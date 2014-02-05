using NetPing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetPing.DAL
{
    interface IRepository: IDisposable
    {
        IEnumerable<SPTerm> Terms { get; }
        IEnumerable<SPTerm> TermsDestinations { get; }
        IEnumerable<SPTerm> TermsDeviceParameters { get; }
        IEnumerable<SPTerm> TermsLabels { get; }
        IEnumerable<Device> Devices { get; }
        IEnumerable<Post> Posts { get; }
        IEnumerable<SFile> SFiles { get; }
        IEnumerable<DeviceParameter> DevicesParameters { get; }

        TreeNode<Device> DevicesTree(Device root, IEnumerable<Device> devices);
    }
}
