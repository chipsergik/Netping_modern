using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace NetPing.Global.Config
{
    public class Config : IConfig
    {
        public SharePointSettings SPSettings
        {
            get
            {
                return (SharePointSettings) ConfigurationManager.GetSection("SharePoint");
            }
        }
    }
}