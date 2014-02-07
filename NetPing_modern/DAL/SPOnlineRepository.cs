using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using NetPing.Models;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NetPing.Helpers;
using NetPing.Tools;
using NetPing.Global.Config;
using System.Web.Mvc;
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
          
        public IEnumerable<SPTerm> TermsLabels { get  { return GetTermsFromSP("Labels", 1049); }      }

        public IEnumerable<SPTerm> TermsDeviceParameters { get  { return GetTermsFromSP("Device parameters", 1049); }      }

        public IEnumerable<SPTerm> TermsFileTypes { get  { return GetTermsFromSP("Documents types", 1049); }      }

        public IEnumerable<SPTerm> TermsDestinations { get  { return GetTermsFromSP("Destinations", 1049); }      }

        public IEnumerable<SPTerm> Terms{ get  { return GetTermsFromSP("Names", 1049); }      }

        public IEnumerable<SPTerm> TermsSiteTexts { get { return GetTermsFromSP("Site texts", 1049); } }


        public IEnumerable<SiteText> SiteTexts
        {
            get
            {
                var result = (List<SiteText>)(PullFromCache("SiteTexts"));
                if (result != null) return result;
                result = new List<SiteText>();

                var list = context.Web.Lists.GetByTitle("Site_texts");
                CamlQuery camlquery = new CamlQuery();
                var items = list.GetItems(camlquery);
                context.Load(list);
                context.Load(items);
                context.ExecuteQuery();

                foreach (var item in items)
                {
                    result.Add(new SiteText
                    {
                        Tag = (item["Tag"] as TaxonomyFieldValue).ToSPTerm(TermsSiteTexts)
                       ,
                        Text = (item["Text_RUS"] as string).ReplaceInternalLinks()
                    });
                }

                PushToCache("SiteTexts",result);

                return result;
            }
        }


        public IEnumerable<DeviceParameter> DevicesParameters
        {
            get
            {
                var result = (List<DeviceParameter>)(PullFromCache("DevicesParameters"));
                if (result != null) return result;
                result = new List<DeviceParameter>();


                var list = context.Web.Lists.GetByTitle("Device_parameters");
                CamlQuery camlquery = new CamlQuery();
                var items = list.GetItems(camlquery);

                context.Load(list);
                context.Load(items);
                context.ExecuteQuery();

                foreach (var item in items)
                {
                    result.Add(new DeviceParameter
                    {
                        Id = item.Id
                       ,
                        Name = (item["Parameter"] as TaxonomyFieldValue).ToSPTerm(TermsDeviceParameters)
                       ,
                        Device = (item["Device"] as TaxonomyFieldValue).ToSPTerm(Terms)
                       ,
                        Value = item["Title"] as string
                    });
                }

                PushToCache("DevicesParameters", result);

                return result;
            }
        }




        public IEnumerable<Device> Devices
        {
            get
            {
                var devices = (List<Device>)(PullFromCache("Devices"));
                if (devices!=null) return devices;
                devices = new List<Device>();

                var list    = context.Web.Lists.GetByTitle("Device keys");
                CamlQuery camlquery = new CamlQuery();
                camlquery.ViewXml = NetPing_modern.Properties.Resources.caml_Device_keys;
                camlquery.ViewXml = Regex.Replace(camlquery.ViewXml, @"\s{2,}", string.Empty);
                var items = list.GetItems(camlquery);

                context.Load(list);
                context.Load(items);
                context.ExecuteQuery();

                var allPosts = Posts;
                var allFiles = SFiles;

               // var devices = new List<Device>();
                foreach (var item in items)
                {
                    var device = new Device 
                             {
                                Id              = item.Id
                                , Key           = item["Title"] as string
                                , Name          = (item["Name"] as TaxonomyFieldValue).ToSPTerm(Terms)
                                , Destination = (item["Destination"] as TaxonomyFieldValueCollection).ToSPTermList(TermsDestinations)
                                , Connected_devices = (item["Connected_devices"] as TaxonomyFieldValueCollection).ToSPTermList(Terms)
                                , Russian_price = item["Russian_price"] as double?
                                , Label = (item["Russian_label"] as TaxonomyFieldValue).ToSPTerm(TermsLabels)
                                , Created = (DateTime)item["Created"]
                             };

                    devices.Add(device);
                }

                HttpRuntime.Cache.Insert("Devices", devices, new TimerCacheDependency());

                
                SPTerm dest_russia = TermsDestinations.FirstOrDefault(dest => dest.IsEqualStrId( NetPing_modern.Properties.Resources.Guid_Destination_Russia));
                foreach (var dev in devices)
                {
                    // Collect Posts for corresponded to device
                    if (dev.Name.Level == 3)  // it is not group
                    {                         // Collect all posts where dev.Name.Path contains Device name of any device from post
                            dev.Posts=Posts.Where(pst => 
                                         pst.Devices.FirstOrDefault( d => dev.Name.Path.Contains(d.OwnNameFromPath))!=null
                                         &&
                                         pst.Devices.ListNamesToListDesitnations().Contains(dest_russia)
                                ).ToList();
                    }
                    else                      // it is group
                    {                         // Collect all posts where any Device from post path contains dev.Name
                        dev.Posts = Posts.Where(pst =>
                                     pst.Devices.FirstOrDefault(d => d.Path.Contains(dev.Name.OwnNameFromPath))!=null
                                     &&
                                     pst.Devices.ListNamesToListDesitnations().Contains(dest_russia)
                            ).ToList();
                    }

                    //Set Short description
                    var post    = dev.Posts.FirstOrDefault(pst => pst.Cathegory=="Catalog, short description");
                    if (post == null) dev.Short_description="#error";
                        else dev.Short_description=post.Body;

                    //Set Long description
                    post = dev.Posts.FirstOrDefault(pst => pst.Cathegory == "Catalog, long description");
                    if (post == null) dev.Long_description = "#error";
                    else dev.Long_description = post.Body;
                
                    //Collect SFiles according device
                    // get all files where dev.Name.Path contains Device name of any device from SFile
                    dev.SFiles = allFiles.Where(fl =>
                                     fl.Devices.FirstOrDefault(d => dev.Name.Path.Contains(d.OwnNameFromPath)) != null
                                            ).ToList();

                    // collect device parameters 
                    dev.DeviceParameters = DevicesParameters.Where(par => par.Device==dev.Name).ToList();
                }

                PushToCache("Devices", devices);

                return devices;
            }
        }

        public IEnumerable<PubFiles> PubFiles
        {
            get
            {
                var result = (List<PubFiles>)(PullFromCache("PubFiles"));
                if (result != null) return result;
                result = new List<PubFiles>();

                var list = context.Web.Lists.GetByTitle("Photos_to_pub");
                CamlQuery camlquery = new CamlQuery();
                camlquery.ViewXml = NetPing_modern.Properties.Resources.caml_Photos_to_pub;
                camlquery.ViewXml = Regex.Replace(camlquery.ViewXml, @"\s{2,}", string.Empty);
                var items = list.GetItems(camlquery);
                context.Load(list);
                context.Load(items);
                context.ExecuteQuery();

                foreach (var item in items)
                {
                    result.Add(new PubFiles
                    {
                        Name = item["FileLeafRef"] as string
                       ,
                        File_type = (item["File_type"] as TaxonomyFieldValue).ToSPTerm(TermsFileTypes)
                       ,
                        Url = "https://netpingeastcoltd-public.sharepoint.com/Pub/Photos/"+(item["FileLeafRef"] as string)
                    });
                }

                PushToCache("PubFiles", result);

                return result;
            }
        }

        public IEnumerable<SFile> SFiles
        {
            get
            {
                var result = (List<SFile>)(PullFromCache("SFiles"));
                if (result != null) return result;
                result = new List<SFile>();
  
                var list = context.Web.Lists.GetByTitle("Pub files");
                CamlQuery camlquery = new CamlQuery();
                camlquery.ViewXml = NetPing_modern.Properties.Resources.caml_Pub_files;
                camlquery.ViewXml = Regex.Replace(camlquery.ViewXml, @"\s{2,}", string.Empty);
                var items = list.GetItems(camlquery);
                context.Load(list);
                context.Load(items);
                context.ExecuteQuery();

                foreach (var item in items)
                {

                    result.Add(new SFile
                    {
                        Id = item.Id
                       ,
                        Name = item["FileLeafRef"] as string
                       ,
                        Title = item["Title"] as string
                       ,
                        Devices = (item["Devices"] as TaxonomyFieldValueCollection).ToSPTermList(Terms)
                       ,
                        File_type  = (item["File_x0020_type0"] as TaxonomyFieldValue).ToSPTerm(TermsFileTypes)
                       ,
                        Created = (DateTime)item["Created"]
                       ,
                        Url = (item["Public_url"] as FieldUrlValue).ToFileUrlStr(item["FileLeafRef"] as string)
                    });
                }

                PushToCache("Sfiles", result);

                return result;
            }
        }

        public IEnumerable<Post> Posts 
        {
            get 
            {
                var result = (List<Post>)(PullFromCache("Posts"));
                if (result != null) return result;
                result = new List<Post>();

                var list = context.Web.Lists.GetByTitle("Posts");
                CamlQuery camlquery = new CamlQuery();
                camlquery.ViewXml = NetPing_modern.Properties.Resources.caml_Posts;
                camlquery.ViewXml = Regex.Replace(camlquery.ViewXml, @"\s{2,}", string.Empty);
                var items   = list.GetItems(camlquery);
                context.Load(list);
                context.Load(items);
                context.ExecuteQuery();

                foreach (var item in items)
                {
                    result.Add(new Post
                             {
                                 Id = item.Id
                                ,
                                 Title = item["Title"] as string
                                ,
                                 Devices = (item["Devices"] as TaxonomyFieldValueCollection).ToSPTermList(Terms)
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

                PushToCache("Posts", result);

                return result;      
            }
        }
        #endregion


        private object PullFromCache(string cache_name)
        {
            object obj = HttpRuntime.Cache.Get(cache_name);
            if (obj != null) return obj;
#if DEBUG
            // Check file cache
            Stream streamRead = null;
            string file_name = HttpContext.Current.Server.MapPath("~/Content/Data/" + cache_name + ".dat");
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
                return null;
            }
            HttpRuntime.Cache.Insert(cache_name, obj, new TimerCacheDependency());
            
#endif
            return obj;
        }

        private void PushToCache(string cache_name, object obj)
        {
            HttpRuntime.Cache.Insert(cache_name, obj, new TimerCacheDependency());

            string file_name = HttpContext.Current.Server.MapPath("~/Content/Data/" + cache_name + ".dat");
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

        private IEnumerable<SPTerm> GetTermsFromSP(string setname, int lcid)
        {
            var terms = (List<SPTerm>)(PullFromCache("Terms" + setname));
            if (terms != null) return terms;
            terms = new List<SPTerm>();

            var session = TaxonomySession.GetTaxonomySession(context);
            var termSets = session.GetTermSetsByName(setname, 1033);
            context.Load(session);
            context.Load(termSets);
            context.ExecuteQuery();

            var allTerms = termSets.First().GetAllTerms();
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

                    if (lang_label.FirstOrDefault() != null) name = lang_label.FirstOrDefault().Value;
                }
                terms.Add(new SPTerm
                {
                    Id = term.Id
                   ,
                    Name = name
                   ,
                    Path = term.PathOfTerm
                });

            }
            
            PushToCache("Terms" + setname, terms);

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