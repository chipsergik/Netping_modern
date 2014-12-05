using NetPing_modern.Global.Config;

namespace NetPing.Global.Config
{
    public interface IConfig
    {
        SharePointSettings SPSettings { get; }
        ConfluenceSettings ConfluenceSettings { get; }
    }
}