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
    public class LastModifiedCacheAttribute : ActionFilterAttribute
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

                var readPath = server.MapPath("/Content/Data/netping_read.xml");
                if (!File.Exists(readPath))
                    File.Copy(server.MapPath("/Content/Data/netping.xml"), readPath);

                Stream streamRead = null;
                try
                {
                    streamRead = File.OpenRead(readPath);
                    XmlReader reader = XmlReader.Create(streamRead, settings);

                    reader.ReadToFollowing("yml_catalog");
                    reader.MoveToFirstAttribute();
                    string date = reader.Value;
                    DateTime lastModifiedDate;
                    if (DateTime.TryParseExact(date, YmlCatalog.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None,
                        out lastModifiedDate))
                    {
                        streamRead.Close();
                        File.Delete(readPath);
                        return lastModifiedDate;
                    }
                }
                catch (Exception ex)
                {
                    if (streamRead != null) streamRead.Close();
                }
                if (streamRead != null) streamRead.Close();

                //using (XmlReader reader = XmlReader.Create(File.OpenRead(readPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), settings))
                //{
                //    reader.ReadToFollowing("yml_catalog");
                //    reader.MoveToFirstAttribute();
                //    string date = reader.Value;
                //    DateTime lastModifiedDate;
                //    if (DateTime.TryParseExact(date, YmlCatalog.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None,
                //        out lastModifiedDate))
                //    {
                //        File.Delete(readPath);
                //        return lastModifiedDate;
                //    }
                //}
            }
            return null;
        }
    }
}