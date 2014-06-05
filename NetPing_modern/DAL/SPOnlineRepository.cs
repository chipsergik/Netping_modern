using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using NetPing.Models;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NetpingHelpers;
using NetPing.Tools;
using NetPing.Global.Config;
using System.Web.Mvc;
using NetPing_modern.DAL;
using Ninject;
using System.Diagnostics;

using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace NetPing.DAL
{
    public class SPOnlineRepository : IRepository
    {
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


        public IEnumerable<SiteText> SiteTexts    {  get  {return (IEnumerable<SiteText>)(PullFromCache("SiteTexts"));} }
        private IEnumerable<SiteText> SiteTexts_Read(IEnumerable<SPTerm> termsSiteTexts) 
        {
                var result = new List<SiteText>();

                foreach (var item in (ListItemCollection)ReadSPList("Site_texts",NetPing_modern.Resources.Camls.Caml_SiteTexts))
                {
                    result.Add(new SiteText
                    {
                        Tag = (item["Tag"] as TaxonomyFieldValue).ToSPTerm(termsSiteTexts)
                       ,
                        Text =(Helpers.IsCultureEng) ? (item["Text_Eng"] as string).ReplaceInternalLinks() : (item["Text_RUS"] as string).ReplaceInternalLinks()
                    });
                }
                if (result.Count == 0) throw new Exception("No one SiteText was readed!");
                return result;
        }


        public IEnumerable<DeviceParameter> DevicesParameters { get { return (IEnumerable<DeviceParameter>)(PullFromCache("DevicesParameters")); } }
        private IEnumerable<DeviceParameter> DevicesParameters_Read(IEnumerable<SPTerm> termsDeviceParameters, IEnumerable<SPTerm> terms) 
        {
                var result = new List<DeviceParameter>();

                foreach (var item in (ListItemCollection)ReadSPList("Device_parameters",""))
                {
                    result.Add(new DeviceParameter
                    {
                        Id = item.Id
                       ,
                        Name = (item["Parameter"] as TaxonomyFieldValue).ToSPTerm(termsDeviceParameters)
                       ,
                        Device = (item["Device"] as TaxonomyFieldValue).ToSPTerm(terms)
                       ,
                        Value = (Helpers.IsCultureEng) ?  item["ENG_value"] as string : item["Title"] as string 
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

        private IEnumerable<Device> Devices_Read(IEnumerable<Post> allPosts, IEnumerable<SFile> allFiles, IEnumerable<DevicePhoto> allDevicePhotos, IEnumerable<DeviceParameter> allDevicesParameters, IEnumerable<SPTerm> terms, IEnumerable<SPTerm> termsDestinations, IEnumerable<SPTerm> termsLabels)
        {
                var devices = new List<Device>();

                var list    = context.Web.Lists.GetByTitle("Device keys");

                foreach (var item in (ListItemCollection)ReadSPList("Device keys",NetPing_modern.Resources.Camls.Caml_Device_keys))
                {
                    var device = new Device 
                             {
                                Id              = item.Id
                                , Key           = item["Title"] as string
                                , Name          = (item["Name"] as TaxonomyFieldValue).ToSPTerm(terms)
                                , Destination = (item["Destination"] as TaxonomyFieldValueCollection).ToSPTermList(termsDestinations)
                                , Connected_devices = (item["Connected_devices"] as TaxonomyFieldValueCollection).ToSPTermList(terms)
                                , Price = (Helpers.IsCultureEng) ?  item["Global_price"] as double? : item["Russian_price"] as double?
                                , Label = (item["Russian_label"] as TaxonomyFieldValue).ToSPTerm(termsLabels)
                                , Created = (DateTime)item["Created"]
                                , GroupUrl=item["Group_url"] as string
                             };

                    devices.Add(device);
                }
                
                SPTerm dest_russia = termsDestinations.FirstOrDefault(dest => dest.IsEqualStrId( NetPing_modern.Properties.Resources.Guid_Destination_Russia));
                foreach (var dev in devices)
                {
                   
                        Debug.WriteLine(dev.Name.Name);
                        // Collect Posts for corresponded to device
                        if (dev.Name.Level == 3)  // it is not group
                        {                         // Collect all posts where dev.Name.Path contains Device name of any device from post
                            dev.Posts = allPosts.Where(pst =>
                                         pst.Devices.FirstOrDefault(d => d != null && dev.Name.Path.Split(';').FirstOrDefault(n => n==d.OwnNameFromPath)!=null) !=null
                                         &&
                                         pst.Devices.ListNamesToListDesitnations(devices).Contains(dest_russia)
                                ).ToList();
                        }
                        else                      // it is group
                        {                         // Collect all posts where any Device from post path contains dev.Name
                            dev.Posts = allPosts.Where(pst =>
                                         pst.Devices.FirstOrDefault(d => d!=null && d.Path.Contains(dev.Name.OwnNameFromPath)) != null
                                         &&
                                         pst.Devices.ListNamesToListDesitnations(devices).Contains(dest_russia)
                                ).ToList();
                        }

                        //Set Short description
                        var post = dev.Posts.FirstOrDefault(pst => pst.Cathegory == "Catalog, short description");
                        if (post == null) dev.Short_description = "#error";
                        else dev.Short_description = post.Body;

                        //Set Long description
                        post = dev.Posts.FirstOrDefault(pst => pst.Cathegory == "Catalog, long description");
                        if (post == null) dev.Long_description = "#error";
                        else dev.Long_description = post.Body;

                        //Collect SFiles according device
                        // get all files where dev.Name.Path contains Device name of any device from SFile
                        dev.SFiles = allFiles.Where(fl =>
                                         fl.Devices.FirstOrDefault(d =>d!=null && dev.Name.Path.Contains(d.OwnNameFromPath)) != null
                                                ).ToList();

                        // collect device parameters 
                        dev.DeviceParameters = allDevicesParameters.Where(par => par.Device == dev.Name).ToList();

                        // Get device photos
                        dev.DevicePhotos = allDevicePhotos.Where(p => p.Dev_name == dev.Name).ToList();

                    }
                    
                if (devices.Count == 0) throw new Exception("No one devices was readed!");
                return devices;
        }

        public IEnumerable<DevicePhoto> DevicePhotos {  get  {return (IEnumerable<DevicePhoto>)(PullFromCache("DevicePhotos"));} }
        private IEnumerable<DevicePhoto> DevicePhotos_Read(IEnumerable<SPTerm> terms)
        {
                var result = new List<DevicePhoto>();

                foreach (var item in (ListItemCollection)ReadSPList("Device_photos",NetPing_modern.Resources.Camls.Caml_DevicePhotos))
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

        public IEnumerable<PubFiles> PubFiles {  get  {return (IEnumerable<PubFiles>)(PullFromCache("PubFiles"));} }
        private IEnumerable<PubFiles> PubFiles_Read(IEnumerable<SPTerm> termsFileTypes)
        {
                var result = new List<PubFiles>();

                foreach (var item in (ListItemCollection)ReadSPList("Photos_to_pub",NetPing_modern.Resources.Camls.Caml_Photos_to_pub))
                {
                    result.Add(new PubFiles
                    {
                        Name = item["FileLeafRef"] as string
                       ,
                        File_type = (item["File_type"] as TaxonomyFieldValue).ToSPTerm(termsFileTypes)
                       ,
                        Url = "https://netpingeastcoltd-public.sharepoint.com/Pub/Photos/"+(item["FileLeafRef"] as string)
                       ,
                        Url_link = (item["Url"] as FieldUrlValue).Url
                    });
                }
                //if (result.Count == 0) throw new Exception("No one PubFiles was readed!");
                return result;
        }

        public IEnumerable<SFile> SFiles  {  get  {return (IEnumerable<SFile>)(PullFromCache("SFiles"));} }
        private IEnumerable<SFile> SFiles_Read(IEnumerable<SPTerm> termsFileTypes, IEnumerable<SPTerm> terms)
        {
                var result = new List<SFile>();
  
                foreach (var item in (ListItemCollection)ReadSPList("Pub files",NetPing_modern.Resources.Camls.Caml_Pub_files))
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
                        File_type  = (item["File_x0020_type0"] as TaxonomyFieldValue).ToSPTerm(termsFileTypes)
                       ,
                        Created = (DateTime)item["Created"]
                       ,
                        Url = (item["Public_url"] as FieldUrlValue).ToFileUrlStr(item["FileLeafRef"] as string)
                    });
                }
                if (result.Count == 0) throw new Exception("No one SFile was readed!");
                return result;
        }

        public IEnumerable<Post> Posts {  get  {return (IEnumerable<Post>)(PullFromCache("Posts"));} }
        private IEnumerable<Post> Posts_Read(IEnumerable<SPTerm> terms)
        {
                var result = new List<Post>();

                foreach (var item in (ListItemCollection)ReadSPList("Posts",NetPing_modern.Resources.Camls.Caml_Posts))
                {
                    result.Add(new Post
                             {
                                 Id = item.Id
                                ,
                                 Title = item["Title"] as string
                                ,
                                 Devices = (item["Devices"] as TaxonomyFieldValueCollection).ToSPTermList(terms)
                                ,
                                 Body = (item["Body"] as string).ReplaceInternalLinks()
                                ,
                                 Cathegory = (item["PostCategory"] as FieldLookupValue[])[0].LookupValue.ToString()
                                ,
                                 IsActive = Convert.ToBoolean(item["Active"])
                                ,
                                 Created = (DateTime)item["Created"]
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
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

            return "OK!";
            
        }

        

        private object PullFromCache(string cache_name)
        {
            object obj = HttpRuntime.Cache.Get(cache_name);
            if (obj != null) return obj;

            // Check file cache
            Stream streamRead = null;
            string file_name = HttpContext.Current.Server.MapPath("~/Content/Data/" + cache_name +"_" + System.Globalization.CultureInfo.CurrentCulture.IetfLanguageTag + ".dat");
            try
            {
                streamRead = System.IO.File.OpenRead(file_name);
                BinaryFormatter binaryRead = new BinaryFormatter();
                obj = (object)binaryRead.Deserialize(streamRead);
                streamRead.Close();
            }
            catch (Exception ex)
            {
                if (streamRead!=null) streamRead.Close();
                UpdateAll();
                return HttpRuntime.Cache.Get(cache_name);
            }
            HttpRuntime.Cache.Insert(cache_name, obj, new TimerCacheDependency());
            
            return obj;
        }

        private void PushToCache(string cache_name, object obj)
        {
            HttpRuntime.Cache.Insert(cache_name, obj, new TimerCacheDependency());

            string file_name = HttpContext.Current.Server.MapPath("~/Content/Data/" + cache_name+"_" +System.Globalization.CultureInfo.CurrentCulture.IetfLanguageTag + ".dat");
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
                if (streamWrite!=null) streamWrite.Close();
                //toDo log exception to log file
            }
        }

        private IEnumerable<SPTerm> GetTermsFromSP(string setname)
        {
            var lcid = System.Globalization.CultureInfo.CurrentCulture.LCID;

            var terms = new List<SPTerm>();

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

                    if (lang_label.Count!=0) name = lang_label[0].Value;
                }

                terms.Add(new SPTerm
                {
                    Id = term.Id
                   ,
                    Name = name
                   ,
                    Path = term.PathOfTerm
                });

                if (term.CustomSortOrder != null)
                    terms.Sort(new SPTermComparerByCustomSortOrder(term.CustomSortOrder));

            }
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
                    _context.Dispose();
                    _context = null;
                }
            }
            this.disposed = true;
        }
        #endregion
    }
}