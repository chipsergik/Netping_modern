using System.Configuration;
using NetPing_modern.Global.Config;

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

        public ConfluenceSettings ConfluenceSettings
        {
            get
            {
                return (ConfluenceSettings) ConfigurationManager.GetSection("confluence");
            }
        }
    }
}