using System.Web.Mvc;
using NetPing_modern.Tools;

namespace NetPing
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new LastModifiedCacheAttribute());
        }
    }
}