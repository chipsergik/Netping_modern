using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetPing.Global.Config
{
    public interface IConfig
    {
        NetPing.Global.Config.SharePointSettings SPSettings { get; }
    }
}