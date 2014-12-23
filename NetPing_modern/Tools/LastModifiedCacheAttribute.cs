using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using NetPing.PriceGeneration.YandexMarker;

namespace NetPing_modern.Tools
{
    public class LastModifiedCacheAttribute: ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            DateTime? priceGenerationDate = GetPriceListGenerationDate();
            if (priceGenerationDate.HasValue)
            {
                filterContext.RequestContext.HttpContext.Response.Cache.SetLastModified(priceGenerationDate.Value);
            }
            
            base.OnActionExecuted(filterContext);
        }

        private DateTime? GetPriceListGenerationDate()
        {
            if (HttpContext.Current != null)
            {
                HttpServerUtility server = HttpContext.Current.Server;
                FileInfo fi = new FileInfo(server.MapPath("/Content/Data/netping.xml"));
                
                if (!fi.Exists)
                {
                    return null;
                }

                XmlReaderSettings settings = new XmlReaderSettings();
                settings.DtdProcessing = DtdProcessing.Ignore;

                using (XmlReader reader = XmlReader.Create(fi.OpenRead(), settings))
                {
                    reader.ReadToFollowing("yml_catalog");
                    reader.MoveToFirstAttribute();
                    string date = reader.Value;
                    DateTime lastModifiedDate;
                    if (DateTime.TryParseExact(date, YmlCatalog.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None,
                        out lastModifiedDate))
                    {
                        return lastModifiedDate;
                    }
                }
            }
            return null;
        }
    }
}