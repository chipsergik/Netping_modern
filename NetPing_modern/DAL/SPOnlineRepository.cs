﻿using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using HtmlAgilityPack;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using Microsoft.Web.Mvc;
using NetPing.Controllers;
using NetPing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NetPing.PriceGeneration;
using NetPing.PriceGeneration.YandexMarker;
using NetpingHelpers;
using NetPing.Tools;
using NetPing.Global.Config;
using NetPing_modern.DAL;
using NetPing_modern.PriceGeneration;
using NetPing_modern.Resources.Views.Catalog;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NetPing_modern.Services.Confluence;
using Newtonsoft.Json.Linq;
using Category = NetPing_modern.PriceGeneration.Category;
using Group = System.Text.RegularExpressions.Group;

namespace NetPing.DAL
{
    internal class SPOnlineRepository : IRepository
    {
        private readonly IConfluenceClient _confluenceClient;

        public SPOnlineRepository(IConfluenceClient confluenceClient)
        {
            _confluenceClient = confluenceClient;
        }

        #region Properties

        public IEnumerable<SPTerm> TermsLabels { get { return (IEnumerable<SPTerm>)(PullFromCache("TermsLabels")); } }
        private IEnumerable<SPTerm> TermsLabels_Read() { return GetTermsFromSP("Labels"); }


        public IEnumerable<SPTerm> TermsDeviceParameters { get { return (IEnumerable<SPTerm>)(PullFromCache("TermsDeviceParameters")); } }
        private IEnumerable<SPTerm> TermsDeviceParameters_Read() { return GetTermsFromSP("Device parameters"); }

        public IEnumerable<SPTerm> TermsFileTypes { get { return (IEnumerable<SPTerm>)(PullFromCache("TermsFileTypes")); } }
        private IEnumerable<SPTerm> TermsFileTypes_Read() { return GetTermsFromSP("Documents types"); }

        public IEnumerable<SPTerm> TermsDestinations { get { return (IEnumerable<SPTerm>)(PullFromCache("TermsDestinations")); } }
        private IEnumerable<SPTerm> TermsDestinations_Read() { return GetTermsFromSP("Destinations"); }

        public IEnumerable<SPTerm> Terms { get { return (IEnumerable<SPTerm>)(PullFromCache("Terms")); } }
        private IEnumerable<SPTerm> Terms_Read() { return GetTermsFromSP("Names"); }

        public IEnumerable<SPTerm> TermsSiteTexts { get { return (IEnumerable<SPTerm>)(PullFromCache("TermsSiteTexts")); } }
        private IEnumerable<SPTerm> TermsSiteTexts_Read() { return GetTermsFromSP("Site texts"); }


        public IEnumerable<SiteText> SiteTexts { get { return (IEnumerable<SiteText>)(PullFromCache("SiteTexts")); } }
        private IEnumerable<SiteText> SiteTexts_Read(IEnumerable<SPTerm> termsSiteTexts)
        {
            var result = new List<SiteText>();

            foreach (var item in (ListItemCollection)ReadSPList("Site_texts", NetPing_modern.Resources.Camls.Caml_SiteTexts))
            {
                result.Add(new SiteText
                {
                    Tag = (item["Tag"] as TaxonomyFieldValue).ToSPTerm(termsSiteTexts)
                   ,
                    Text = (Helpers.IsCultureEng) ? (item["Text_Eng"] as string).ReplaceInternalLinks() : (item["Text_RUS"] as string).ReplaceInternalLinks()
                });
            }
            if (result.Count == 0) throw new Exception("No one SiteText was readed!");
            return result;
        }


        public IEnumerable<DeviceParameter> DevicesParameters { get { return (IEnumerable<DeviceParameter>)(PullFromCache("DevicesParameters")); } }
        private IEnumerable<DeviceParameter> DevicesParameters_Read(IEnumerable<SPTerm> termsDeviceParameters, IEnumerable<SPTerm> terms)
        {
            var result = new List<DeviceParameter>();

            foreach (var item in (ListItemCollection)ReadSPList("Device_parameters", ""))
            {
                result.Add(new DeviceParameter
                {
                    Id = item.Id
                   ,
                    Name = (item["Parameter"] as TaxonomyFieldValue).ToSPTerm(termsDeviceParameters)
                   ,
                    Device = (item["Device"] as TaxonomyFieldValue).ToSPTerm(terms)
                   ,
                    Value = (Helpers.IsCultureEng) ? item["ENG_value"] as string : item["Title"] as string
                });
            }
            if (result.Count == 0) throw new Exception("No one deviceparameter was readed!");
            return result;
        }




        public IEnumerable<Device> Devices
        {
            get
            {
                var terms = new List<SPTerm>(Terms);
                var devices = (IEnumerable<Device>)(PullFromCache("Devices"));
                var index = BuildIndex(devices, terms);
                return devices.OrderBy(d => index[d]);
            }
        }

        private Dictionary<Device, int> BuildIndex(IEnumerable<Device> list, List<SPTerm> terms)
        {
            var result = new Dictionary<Device, int>();
            foreach (var device in list)
            {
                var i = FindIndex(terms, s => s.Id == device.Name.Id);
                result.Add(device, i > -1 ? i : int.MaxValue);
            }
            return result;
        }

        private static int FindIndex<T>(IEnumerable<T> items, Func<T, bool> predicate)
        {
            int index = 0;
            foreach (var item in items)
            {
                if (predicate(item))
                    return index;
                index++;
            }
            return -1;
        }

        private readonly Regex _contentIdRegex = new Regex(@"pageId=(?<id>\d+)");
        private readonly Regex _spaceTitleRegex = new Regex(@"\/display\/(?<spaceKey>[\w \.\-\+%]+)\/(?<title>[\w \.\-\+%]+)?");

        private void LoadFromConfluence(Device device, Expression<Func<Device, string>> expression, string url)
        {
            var meta = ModelMetadata.FromLambdaExpression(expression, new ViewDataDictionary<Device>());
            var propertyInfo = typeof(Device).GetProperty(meta.PropertyName);

            var mc = _contentIdRegex.Matches(url);
            if (mc.Count > 0)
            {
                Match m = mc[0];
                if (m.Success)
                {
                    Group group = m.Groups["id"];
                    int id = int.Parse(group.Value);

                    var contentTask = _confluenceClient.GetContenAsync(id);
                    string content = contentTask.Result;
                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        string propertyValue = ReplaceConfluenceImages(StylishHeaders3(CleanSpanStyles(CleanFonts((content)))));
                        propertyInfo.SetValue(device, propertyValue);
                    }
                }
            }
            else
            {
                mc = _spaceTitleRegex.Matches(url);
                if (mc.Count > 0)
                {
                    Match m = mc[0];
                    if (m.Success)
                    {
                        Group spaceKeyGroup = m.Groups["spaceKey"];
                        string spaceKey = spaceKeyGroup.Value;

                        Group titleGroup = m.Groups["title"];
                        string title = titleGroup.Value;

                        var contentTask = _confluenceClient.GetContentBySpaceAndTitle(spaceKey, title);
                        int contentId = contentTask.Result;
                        if (contentId > 0)
                        {
                            var contentTask2 = _confluenceClient.GetContenAsync(contentId);
                            string content = contentTask2.Result;
                            if (!string.IsNullOrWhiteSpace(content))
                            {
                                string propertyValue = ReplaceConfluenceImages(StylishHeaders3(CleanSpanStyles(CleanFonts((content)))));
                                propertyInfo.SetValue(device, propertyValue);
                            }
                        }
                    }
                }
            }

            if (propertyInfo.GetValue(device) == null)
            {
                propertyInfo.SetValue(device, string.Empty);
            }
        }

        private IEnumerable<Device> Devices_Read(IEnumerable<Post> allPosts, IEnumerable<SFile> allFiles, IEnumerable<DevicePhoto> allDevicePhotos, IEnumerable<DeviceParameter> allDevicesParameters, IEnumerable<SPTerm> terms, IEnumerable<SPTerm> termsDestinations, IEnumerable<SPTerm> termsLabels)
        {
            var devices = new List<Device>();

            var list = context.Web.Lists.GetByTitle("Device keys");

            CamlQuery query = new CamlQuery();
            var items = list.GetItems(query);

            foreach (var item in (ListItemCollection)ReadSPList("Device keys", NetPing_modern.Resources.Camls.Caml_Device_keys))
            {
                var device = new Device
                         {
                             Id = item.Id
                            ,
                             Key = item["Title"] as string
                            ,
                             Name = (item["Name"] as TaxonomyFieldValue).ToSPTerm(terms)
                            ,
                             Destination = (item["Destination"] as TaxonomyFieldValueCollection).ToSPTermList(termsDestinations)
                            ,
                             Connected_devices = (item["Connected_devices"] as TaxonomyFieldValueCollection).ToSPTermList(terms)
                            ,
                             Price = (Helpers.IsCultureEng) ? item["Global_price"] as double? : item["Russian_price"] as double?
                            ,
                             Label = (item["Russian_label"] as TaxonomyFieldValue).ToSPTerm(termsLabels)
                            ,
                             Created = (DateTime)item["Created"]
                            ,
                             GroupUrl = item["Group_url"] as string
                         };

                

                var urlField = item[Helpers.IsCultureEng ? "Eng_short_descr" : "Rus_short_descr"] as FieldUrlValue;
                if (urlField != null)
                {
                    LoadFromConfluence(device, d => d.Short_description, urlField.Url);
                }

                urlField = item[Helpers.IsCultureEng ? "Eng_long_descr" : "Rus_long_descr"] as FieldUrlValue;
                if (urlField != null)
                {
                    LoadFromConfluence(device, d => d.Long_description, urlField.Url);
                }

                devices.Add(device);
            }

//            SPTerm dest_russia = termsDestinations.FirstOrDefault(dest => dest.IsEqualStrId(NetPing_modern.Properties.Resources.Guid_Destination_Russia));
            foreach (var dev in devices)
            {

//                Debug.WriteLine(dev.Name.Name);
                // Collect Posts and Sfiles corresponded to device

                dev.Posts = allPosts.Where(pst => dev.Name.IsIncludeAnyFromOthers(pst.Devices) || dev.Name.IsUnderAnyOthers(pst.Devices)).ToList();
                dev.SFiles = allFiles.Where(fl => dev.Name.IsIncludeAnyFromOthers(fl.Devices) || dev.Name.IsUnderAnyOthers(fl.Devices)).ToList(); 

  /*              
                if (dev.Name.IsGroup())  
                {  // collect all posts and files corresponding to group                      

 
                }
                else                     
                {    // Collect all posts and files corresponding to device                     

                    dev.Posts=allPosts.Where(pst => dev.Name.IsUnderAnyOthers(pst.Devices)).ToList();
                    dev.SFiles = allFiles.Where(fl => dev.Name.IsUnderAnyOthers(fl.Devices)).ToList(); 
                    
                }
                */
                // collect device parameters 
                dev.DeviceParameters = allDevicesParameters.Where(par => par.Device == dev.Name).ToList();

                // Get device photos
                dev.DevicePhotos = allDevicePhotos.Where(p => p.Dev_name == dev.Name).ToList();

            }

            if (devices.Count == 0) throw new Exception("No one devices was readed!");
            return devices;
        }

        private String CleanSpanStyles(String str)
        {
            for (int tagstart = str.IndexOf("<span"); tagstart != -1; tagstart = str.IndexOf("<span", tagstart + 1))
            {
                int tagend = str.IndexOf('>', tagstart);
                var tag = str.Substring(tagstart, tagend - tagstart + 1);
                if (tag.Contains("style"))
                {
                    str = str.Remove(
                        tag.IndexOf("style") + tagstart,
                        tag.LastIndexOf('"') - tag.IndexOf("style") + 1);
                }
            }
            return str;
        }

        private String CleanFonts(String str)
        {
            for (int tagstart = str.IndexOf("<font"); tagstart != -1; tagstart = str.IndexOf("<font", tagstart + 1))
            {
                int tagend = str.IndexOf('>', tagstart);
                var tag = str.Substring(tagstart, tagend - tagstart + 1);
                if (tag.Contains("color"))
                {
                    str = str.Remove(
                        tag.IndexOf("color") + tagstart,
                        tag.LastIndexOf('"') - tag.IndexOf("color") + 1);
                }
            }
            return str;
        }

        private String StylishHeaders3(String str)
        {
            //str = str.Replace("<h3", "<h3 class=\"shutter collapsed\"><span class=dashed>");
            //str = str.Replace("</h3>", "</span></h3>");
            return str;
        }

        private readonly Regex ConfluenceImageTagRegex = new Regex(@"\<img [^\>]+\>", RegexOptions.IgnoreCase);

        private string ReplaceConfluenceImages(string str)
        {
            return ConfluenceImageTagRegex.Replace(str, new MatchEvaluator(ConfluenceImage));
        }

        private readonly Regex ConfluenceImgSrcRegex = new Regex(@"\ssrc=""/wiki(?<src>[^\""]+)""");
        private readonly Regex ConfluenceDataBaseUrlRegex = new Regex(@"\sdata-base-url=""(?<src>[^\""]+)""");

        string ConfluenceImage(Match match)
        {
            var s = match.ToString();
            if (s.Contains("confluence-embedded-image"))
            {
                var src = ConfluenceImgSrcRegex.Match(s);
                var baseUrl = ConfluenceDataBaseUrlRegex.Match(s);
                if (src.Success && baseUrl.Success)
                {
                    return ConfluenceImgSrcRegex.Replace(s, string.Format(" src=\"{0}\"", baseUrl.Groups["src"].Value + src.Groups["src"].Value));
                }
            }
            return s;
        }

        public IEnumerable<DevicePhoto> DevicePhotos { get { return (IEnumerable<DevicePhoto>)(PullFromCache("DevicePhotos")); } }
        private IEnumerable<DevicePhoto> DevicePhotos_Read(IEnumerable<SPTerm> terms)
        {
            var result = new List<DevicePhoto>();

            foreach (var item in (ListItemCollection)ReadSPList("Device_photos", NetPing_modern.Resources.Camls.Caml_DevicePhotos))
            {
                result.Add(new DevicePhoto
                {
                    Name = item["FileLeafRef"] as string
                   ,
                    Dev_name = ((item["Device"] == null) ? null : item["Device"] as TaxonomyFieldValue).ToSPTerm(terms)
                   ,
                    Url = "https://netpingeastcoltd-public.sharepoint.com/Pub/Photos/Devices/" + (item["FileLeafRef"] as string)
                   ,
                    IsBig = (item["FileLeafRef"] as string).Contains("big") ? true : false
                   ,
                    IsCover = Convert.ToBoolean(item["Cover"])
                });
            }
            if (result.Count == 0) throw new Exception("No one DevicePhoto was readed!");
            return result;
        }

        public IEnumerable<PubFiles> PubFiles { get { return (IEnumerable<PubFiles>)(PullFromCache("PubFiles")); } }
        private IEnumerable<PubFiles> PubFiles_Read(IEnumerable<SPTerm> termsFileTypes)
        {
            var result = new List<PubFiles>();

            foreach (var item in (ListItemCollection)ReadSPList("Photos_to_pub", NetPing_modern.Resources.Camls.Caml_Photos_to_pub))
            {
                result.Add(new PubFiles
                {
                    Name = item["FileLeafRef"] as string
                   ,
                    File_type = (item["File_type"] as TaxonomyFieldValue).ToSPTerm(termsFileTypes)
                   ,
                    Url = "https://netpingeastcoltd-public.sharepoint.com/Pub/Photos/" + (item["FileLeafRef"] as string)
                   ,
                    Url_link = (item["Url"] as FieldUrlValue).Url
                });
            }
            //if (result.Count == 0) throw new Exception("No one PubFiles was readed!");
            return result;
        }

        public IEnumerable<SFile> SFiles { get { return (IEnumerable<SFile>)(PullFromCache("SFiles")); } }
        private IEnumerable<SFile> SFiles_Read(IEnumerable<SPTerm> termsFileTypes, IEnumerable<SPTerm> terms)
        {
            var result = new List<SFile>();

            foreach (var item in (ListItemCollection)ReadSPList("Pub files", NetPing_modern.Resources.Camls.Caml_Pub_files))
            {

                result.Add(new SFile
                {
                    Id = item.Id
                   ,
                    Name = item["FileLeafRef"] as string
                   ,
                    Title = item["Title"] as string
                   ,
                    Devices = (item["Devices"] as TaxonomyFieldValueCollection).ToSPTermList(terms)
                   ,
                    File_type = (item["File_x0020_type0"] as TaxonomyFieldValue).ToSPTerm(termsFileTypes)
                   ,
                    Created = (DateTime)item["Created"]
                   ,
                    Url = (item["Public_url"] as FieldUrlValue).ToFileUrlStr(item["FileLeafRef"] as string)
                });
            }
            if (result.Count == 0) throw new Exception("No one SFile was readed!");
            return result;
        }

        public IEnumerable<Post> Posts { get { return (IEnumerable<Post>)(PullFromCache("Posts")); } }
        private IEnumerable<Post> Posts_Read(IEnumerable<SPTerm> terms)
        {
            var result = new List<Post>();

            foreach (var item in (ListItemCollection)ReadSPList("Blog_posts", NetPing_modern.Resources.Camls.Caml_Posts))
            {
                var link = item["Body_link"] as FieldUrlValue;
                int? contentId = null;
                string url = null;
                if (link != null)
                {
                    url = link.Url;
                    contentId = _confluenceClient.GetContentIdFromUrl(url);
                }
                string content = string.Empty;
                string title = string.Empty;
                if (contentId.HasValue)
                {
                    Task<string> contentTask = _confluenceClient.GetContenAsync(contentId.Value);
                    content = contentTask.Result;
                    contentTask = _confluenceClient.GetContentTitleAsync(contentId.Value);
                    title = contentTask.Result;
                }

                result.Add(new Post
                         {
                             Id = int.Parse(item["Old_id"].ToString())
                            ,
                             Title = title
                            ,
                             Devices = (item["Devices"] as TaxonomyFieldValueCollection).ToSPTermList(terms)
                            ,
                             Body = content.ReplaceInternalLinks()
                            ,
                             Cathegory = item["Category"] as string
                            ,
                             Created = (DateTime)item["Pub_date"]
                             ,
                             Url_name = "/Blog/"+(item["Body_link"] as FieldUrlValue).Description,
                             IsTop = (bool) item["TOP"] 
                         });
            }
            if (result.Count == 0) throw new Exception("No one post was readed!");
            return result;
        }
        #endregion


        public string UpdateAll()
        {
            try
            {
                var termsLabels = TermsLabels_Read(); Debug.WriteLine("TermsLabels_Read OK");
                var termsDeviceParameters = TermsDeviceParameters_Read(); Debug.WriteLine("TermsDeviceParameters_Read OK");
                var termsFileTypes = TermsFileTypes_Read(); Debug.WriteLine("TermsFileTypes_Read OK");
                var termsDestinations = TermsDestinations_Read(); Debug.WriteLine("TermsDestinations_Read OK");
                var terms = Terms_Read(); Debug.WriteLine("Terms_Read OK");
                var termsSiteTexts = TermsSiteTexts_Read();
                var siteTexts = SiteTexts_Read(termsSiteTexts);
                var devicesParameters = DevicesParameters_Read(termsDeviceParameters, terms);
                var devicePhotos = DevicePhotos_Read(terms); Debug.WriteLine("DevicePhotos_Read OK");
                var pubFiles = PubFiles_Read(termsFileTypes);
                var sFiles = SFiles_Read(termsFileTypes, terms); Debug.WriteLine("SFiles_Read OK");
                var posts = Posts_Read(terms); Debug.WriteLine("Posts_Read OK");
                var devices = Devices_Read(posts, sFiles, devicePhotos, devicesParameters, terms, termsDestinations, termsLabels); Debug.WriteLine("Devices_Read OK");

                PushToCache("SiteTexts", siteTexts);
                PushToCache("TermsLabels", termsLabels);
                PushToCache("TermsDeviceParameters", termsDeviceParameters);
                PushToCache("TermsFileTypes", termsFileTypes);
                PushToCache("TermsDestinations", termsDestinations);
                PushToCache("Terms", terms);
                PushToCache("TermsSiteTexts", termsSiteTexts);
                PushToCache("DevicesParameters", devicesParameters);
                PushToCache("DevicePhotos", devicePhotos);
                PushToCache("PubFiles", pubFiles);
                PushToCache("SFiles", sFiles);
                PushToCache("Posts", posts);
                PushToCache("Devices", devices);

                return "";

                if (Helpers.IsCultureRus)
                {
                    GeneratePriceList();
                    GenerateYml();
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

            return "OK!";

        }

        private void GenerateYml()
        {
            var catalog = new YmlCatalog
                          {
                              Date = DateTime.Now
                          };
            var shop = new Shop();
            catalog.Shop = shop;


            const string netpingRu = "Netping.ru";
            shop.Name = netpingRu;
            shop.Company = netpingRu;
            shop.Url = "http://netping.ru";
            shop.Currencies.Add(new Currency
                                    {
                                        Id = "RUR",
                                        Rate = 1,
                                        Plus = 0
                                    });

            var tree = new DevicesTree(Devices);
            foreach (DeviceTreeNode categoryNode in tree.Nodes)
            {
                shop.Categories.Add(new PriceGeneration.YandexMarker.Category
                                    {
                                        Id = categoryNode.Id,
                                        Name = categoryNode.Name,
                                        ParentId = categoryNode.Parent == null ? (int?) null : categoryNode.Parent.Id
                                    });

                foreach (DeviceTreeNode childCategoryNode in categoryNode.Nodes)
                {
                    shop.Categories.Add(new PriceGeneration.YandexMarker.Category
                    {
                        Id = childCategoryNode.Id,
                        Name = childCategoryNode.Name,
                        ParentId = childCategoryNode.Parent == null ? (int?)null : childCategoryNode.Parent.Id
                    });

                    foreach (DeviceTreeNode offerNode in childCategoryNode.Nodes)
                    {
                        string shortDescription = offerNode.Device.Short_description;
                        string descr = string.Empty;
                        if (!string.IsNullOrWhiteSpace(shortDescription))
                        {
                            var htmlDoc = new HtmlDocument();

                            htmlDoc.LoadHtml(shortDescription);
                            var ulNodes = htmlDoc.DocumentNode.SelectNodes("//ul");
                            if (ulNodes != null)
                            {
                                foreach (var ulNode in ulNodes)
                                {
                                    ulNode.Remove();
                                }
                            }
                            descr = htmlDoc.DocumentNode.InnerText.Replace("&#160;", " ");
                        }
                        
                        shop.Offers.Add(new Offer
                                        {
                                            Id = offerNode.Id,
                                            Url = GetDeviceUrl(offerNode.Device),
                                            Price = (int) (offerNode.Device.Price.HasValue ? offerNode.Device.Price.Value : 0),
                                            CategoryId = childCategoryNode.Id,
                                            Picture = offerNode.Device.GetCoverPhoto(true).Url,
                                            TypePrefix = childCategoryNode.Name,
                                            VendorCode = offerNode.Name,
                                            Model = offerNode.Name,
                                            Description = descr
                                        });
                    }
                }
            }
            shop.LocalDeliveryCost = 350;

            YmlGenerator.Generate(catalog, HttpContext.Current.Server.MapPath("Content/Data/netping.xml"));
        }

        public IEnumerable<Device> GetDevices(string id, string groupId)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException("id");

            if (string.IsNullOrEmpty(groupId))
                throw new ArgumentNullException("groupId");

            var group = Devices.FirstOrDefault(d => d.Key == groupId + "#");
            var devices = Devices.Where(d => d.Name.IsUnderOther(group.Name) && !d.Name.IsGroup() );
            return devices;
        }
        
        private void GeneratePriceList()
        {
            using (var priceList = new PriceList())
            {
                var monitoring = new Category(Index.Sec_monitoring);

                monitoring.Sections.Add(new Section(this, Index.Sec_sub_devices, CategoryId.MonitoringId, CategoryId.MonitoringSection.DevicesId));
                monitoring.Sections.Add(new Section(this, Index.Sec_sub_sensors, CategoryId.MonitoringId, CategoryId.MonitoringSection.SensorsId));
                monitoring.Sections.Add(new Section(this, Index.Sec_sub_access, CategoryId.MonitoringId, CategoryId.MonitoringSection.AccessoriesId));
                monitoring.Sections.Add(new Section(this, Index.Sec_sub_solutions, CategoryId.MonitoringId, CategoryId.MonitoringSection.SolutionsId));
                priceList.Categories.Add(monitoring);

                var power = new Category(Index.Sec_power);
                power.Sections.Add(new Section(this, Index.Sec_sub_devices, CategoryId.PowerId, CategoryId.PowerSection.DevicesId));
                power.Sections.Add(new Section(this, Index.Sec_sub_sensors, CategoryId.PowerId, CategoryId.PowerSection.SensorsId));
                power.Sections.Add(new Section(this, Index.Sec_sub_access, CategoryId.PowerId, CategoryId.PowerSection.AccessoriesId));
                power.Sections.Add(new Section(this, Index.Sec_sub_solutions, CategoryId.PowerId, CategoryId.PowerSection.SolutionsId));
                priceList.Categories.Add(power);


                var switches = new Category(Index.Sec_switch);
                switches.Sections.Add(new Section(this, Index.Sec_sub_devices, CategoryId.SwitchesId, CategoryId.SwitchesSection.DevicesId));
                switches.Sections.Add(new Section(this, Index.Sec_sub_access, CategoryId.SwitchesId, CategoryId.SwitchesSection.AccessoriesId));
                switches.Sections.Add(new Section(this, Index.Sec_sub_solutions, CategoryId.SwitchesId, CategoryId.SwitchesSection.SolutionsId));
                priceList.Categories.Add(switches);

                var priceRepl = new PriceListReplacementsTree();
                var categoriesRepl = new CategoriesReplacementsTree();
                var sectionsRepl = new SectionsReplacemenetsTree();
                var productsRepl = new ProductsReplacementsTree();

                sectionsRepl.Add("%StartProducts%*%EndProducts%", productsRepl);
                categoriesRepl.Add("%StartSections%*%EndSections%", sectionsRepl);
                priceRepl.Add("%StartCategories%*%EndCategories%", categoriesRepl);

                var generator = new PriceListGenerator(priceRepl);
                generator.Generate(priceList, new FileInfo(HttpContext.Current.Server.MapPath("Content/Price/price_template.docx")),
                    HttpContext.Current.Server.MapPath("Content/Data/Price.pdf"));
            }
        }

        internal static string GetDeviceUrl(Device device)
        {
            var url =
                LinkBuilder.BuildUrlFromExpression<Product_itemController>(
                    new RequestContext(new HttpContextWrapper(HttpContext.Current), new RouteData()),
                    RouteTable.Routes, c => c.Index(device.Key));
            Uri uri = HttpContext.Current.Request.Url;
            url = string.Format("{0}://{1}{2}{3}", uri.Scheme, uri.Authority, HttpRuntime.AppDomainAppVirtualPath, url);
            return url;
        }


        private object PullFromCache(string cache_name)
        {
            object obj = HttpRuntime.Cache.Get(cache_name);
            if (obj != null) return obj;

            // Check file cache
            Stream streamRead = null;
            string file_name = HttpContext.Current.Server.MapPath("~/Content/Data/" + cache_name + "_" + System.Globalization.CultureInfo.CurrentCulture.IetfLanguageTag + ".dat");
            try
            {
                streamRead = System.IO.File.OpenRead(file_name);
                BinaryFormatter binaryRead = new BinaryFormatter();
                obj = (object)binaryRead.Deserialize(streamRead);
                streamRead.Close();
            }
            catch (Exception ex)
            {
                if (streamRead != null) streamRead.Close();
                UpdateAll();
                return HttpRuntime.Cache.Get(cache_name);
            }
            HttpRuntime.Cache.Insert(cache_name, obj, new TimerCacheDependency());

            return obj;
        }

        private void PushToCache(string cache_name, object obj)
        {
            HttpRuntime.Cache.Insert(cache_name, obj, new TimerCacheDependency());

            string file_name = HttpContext.Current.Server.MapPath("~/Content/Data/" + cache_name + "_" + System.Globalization.CultureInfo.CurrentCulture.IetfLanguageTag + ".dat");
            Stream streamWrite = null;
            try
            {
                streamWrite = System.IO.File.Create(file_name);
                BinaryFormatter binaryWrite = new BinaryFormatter();
                binaryWrite.Serialize(streamWrite, obj);
                streamWrite.Close();
            }
            catch (Exception ex)
            {
                if (streamWrite != null) streamWrite.Close();
                //toDo log exception to log file
            }
        }

        private IEnumerable<SPTerm> GetTermsFromSP(string setname)
        {
            var lcid = System.Globalization.CultureInfo.CurrentCulture.LCID;

            var terms = new List<SPTerm>();
            var sortOrders = new SortOrdersCollection<Guid>();

            var session = TaxonomySession.GetTaxonomySession(context);
            var termSets = session.GetTermSetsByName(setname, 1033);
            context.Load(session);
            context.Load(termSets);
            context.ExecuteQuery();

            var allTerms = termSets[0].GetAllTerms();
            context.Load(allTerms);
            context.ExecuteQuery();

            foreach (var term in allTerms)
            {
                string name = term.Name;
                if (lcid != 1033)   // If lcid label not avaliable or lcid==1033 keep default label
                {
                    var lang_label = term.GetAllLabels(lcid);
                    context.Load(lang_label);
                    context.ExecuteQuery();

                    if (lang_label.Count != 0) name = lang_label[0].Value;
                }

                terms.Add(new SPTerm
                        {
                            Id = term.Id
                           ,
                            Name = name
                           ,
                            Path = term.PathOfTerm
                        });
                if (!string.IsNullOrEmpty(term.CustomSortOrder))
                {
                    sortOrders.AddSortOrder(term.CustomSortOrder);
                }

            }
                var customSortOrder = sortOrders.GetSortOrders();

                terms.Sort(new SPTermComparerByCustomSortOrder(customSortOrder));
            
            if (terms.Count==0) throw new Exception("No terms was readed!");

            return terms;
        }

        public TreeNode<Device> DevicesTree(Device root, IEnumerable<Device> devices)
        {
            var tree = new TreeNode<Device>(root);
            BuildTree(tree, devices);

            return tree;
        }

        private void BuildTree(TreeNode<Device> dev, IEnumerable<Device> list)
        {
            var childrens = list.Where(d => d.Name.Level == dev.Value.Name.Level + 1 && d.Name.Path.Contains(dev.Value.Name.Path));
            foreach (var child in childrens)
            {
                BuildTree(dev.AddChild(child), list);
            }
        }

        #region SharePoint Context


        private ListItemCollection ReadSPList(string list_name, string caml_query)
        {
            var list = context.Web.Lists.GetByTitle(list_name);
            CamlQuery camlquery = new CamlQuery();
            camlquery.ViewXml = caml_query;
            var items = list.GetItems(camlquery);
            context.Load(list);
            context.Load(items);
            context.ExecuteQuery();
            return items;
        }

        private static IConfig _config
        {
            get
            {
                return new NetPing.Global.Config.Config();

                //return DependencyResolver.Current.GetService<NetPing_modern.Global.Config.IConfig>();
            }
        }

        private static ClientContext _context;
        protected static ClientContext context
        {
            get
            {

                if (_context != null)
                    return _context;

                _context = new ClientContext(_config.SPSettings.SiteUrl);

                _context.Credentials = new SharePointOnlineCredentials(_config.SPSettings.Login, _config.SPSettings.Password.ToSecureString());
                _context.ExecuteQuery();

                return _context;
            }
        }
        #endregion

        #region Disposing
        public void Dispose()
        {
            Dispose(true);
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (_context != null)
                    {
                        _context.Dispose();
                        _context = null;
                    }
                }
            }
            this.disposed = true;
        }
        #endregion
    }
}