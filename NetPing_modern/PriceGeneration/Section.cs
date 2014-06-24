using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Web;
using HtmlAgilityPack;
using NetPing.DAL;
using NetPing.Models;
using NetPing.PriceGeneration.PriceList;

namespace NetPing_modern.PriceGeneration
{
    public class Section : ISection, IDisposable
    {
        private readonly string _sectionId;
        private readonly string _categoryId;
        private readonly SPOnlineRepository _repository = new SPOnlineRepository();

        public Section(string sectionName, string categoryId, string sectionId)
        {
            _sectionId = sectionId;
            _categoryId = categoryId;
            SectionName = sectionName;
        }

        private ICollection<IProduct> _products; 

        public ICollection<IProduct> Products
        {
            get
            {
                if (_products == null)
                {
                    var devs = _repository.GetDevices(_categoryId, _sectionId);
                    _products = new Collection<IProduct>();
                    foreach (var device in devs)
                    {
                        var product = new Product();
                        var price = device.Price;
                        if (price != null && price > 0)
                        {
                            product.Cost = string.Format(new CultureInfo(1035), "{0:N0} {1}", price, device.GetCurrency);
                        }

                        product.Title = device.Name.Name;

                        HtmlDocument html = new HtmlDocument();
                        html.LoadHtml(device.Short_description);
                        var ulNodes = html.DocumentNode.SelectNodes("//ul");
                        if (ulNodes != null)
                        {
                            foreach (var ulNode in ulNodes)
                            {
                                ulNode.Remove();
                            }
                        }
                        product.Description = html.DocumentNode.InnerText;

                        product.ImageFileName = GetImageFileName(device);

                        _products.Add(product);
                    }
                }

                return _products;
            }
        }

        private string GetImageFileName(Device device)
        {
            var dir = SectionContentDir;
            if (!dir.Exists)
            {
                dir.Create();
            }
            string uriString = device.GetCoverPhoto(true).Url;
            if (string.IsNullOrEmpty(uriString))
            {
                return null;
            }

            Uri address = new Uri(uriString);
            string fileName = dir.FullName + "\\" + address.Segments[address.Segments.Length - 1];
            

            using (WebClient client = new WebClient())
            {
                
                client.DownloadFile(address, fileName);
            }
            return fileName;
        }

        private DirectoryInfo SectionContentDir
        {
            get { return new DirectoryInfo(HttpContext.Current.Server.MapPath("/Content/Price/" + TempDirName)); }
        }

        private string _tempDirName = null;
        private string TempDirName
        {
            get
            {
                if (_tempDirName == null)
                {
                    _tempDirName = Guid.NewGuid().ToString();
                }
                return _tempDirName;
            }
        }

        public string SectionName { get; private set; }
        public void Dispose()
        {
            if (SectionContentDir.Exists)
            {
                try
                {
                    SectionContentDir.Delete(true);
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
