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

                if (HttpRuntime.Cache.Get("SiteTexts") != null)
                    return HttpRuntime.Cache.Get("SiteTexts") as IEnumerable<SiteText>;

                var list = context.Web.Lists.GetByTitle("Site_texts");
                CamlQuery camlquery = new CamlQuery();
                var items = list.GetItems(camlquery);

                context.Load(list);
                context.Load(items);
                context.ExecuteQuery();

                var result = new List<SiteText>();
                foreach (var item in items)
                {
                    result.Add(new SiteText
                    {
                        Tag = (item["Tag"] as TaxonomyFieldValue).ToSPTerm(TermsSiteTexts)
                       ,
                        Text = (item["Text_RUS"] as string).ReplaceInternalLinks()
                    });
                }

                HttpRuntime.Cache.Insert("SiteTexts", result, new TimerCacheDependency());

                return result;
            }
        }


        public IEnumerable<DeviceParameter> DevicesParameters
        {
            get
            {

                if (HttpRuntime.Cache.Get("DevicesParameters") != null)
                    return HttpRuntime.Cache.Get("DevicesParameters") as IEnumerable<DeviceParameter>;

                var list = context.Web.Lists.GetByTitle("Device_parameters");
                CamlQuery camlquery = new CamlQuery();
                var items = list.GetItems(camlquery);

                context.Load(list);
                context.Load(items);
                context.ExecuteQuery();

                var result = new List<DeviceParameter>();
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

                HttpRuntime.Cache.Insert("DevicesParameters", result, new TimerCacheDependency());

                return result;
            }
        }

        public IEnumerable<Device> Devices
        {
            get
            {
                if (HttpRuntime.Cache.Get("Devices") != null)
                    return HttpRuntime.Cache.Get("Devices") as IEnumerable<Device>;

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

                var devices = new List<Device>();
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

                return devices;
            }
        }

        public IEnumerable<SFile> SFiles
        {
            get
            {

                if (HttpRuntime.Cache.Get("SFiles") != null)
                    return HttpRuntime.Cache.Get("SFiles") as IEnumerable<SFile>;

                var list = context.Web.Lists.GetByTitle("Pub files");
                CamlQuery camlquery = new CamlQuery();
                camlquery.ViewXml = NetPing_modern.Properties.Resources.caml_Pub_files;
                camlquery.ViewXml = Regex.Replace(camlquery.ViewXml, @"\s{2,}", string.Empty);
                var items = list.GetItems(camlquery);

                context.Load(list);
                context.Load(items);
                context.ExecuteQuery();

                var result = new List<SFile>();
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

                HttpRuntime.Cache.Insert("Sfiles", result, new TimerCacheDependency());

                return result;
            }
        }

        public IEnumerable<Post> Posts 
        {
            get 
            {

                if (HttpRuntime.Cache.Get("Posts") != null)
                    return HttpRuntime.Cache.Get("Posts") as IEnumerable<Post>;

                var list = context.Web.Lists.GetByTitle("Posts");
                CamlQuery camlquery = new CamlQuery();
                camlquery.ViewXml = NetPing_modern.Properties.Resources.caml_Posts;
                camlquery.ViewXml = Regex.Replace(camlquery.ViewXml, @"\s{2,}", string.Empty);
                var items   = list.GetItems(camlquery);

                context.Load(list);
                context.Load(items);
                context.ExecuteQuery();

                var result = new List<Post>();
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

                HttpRuntime.Cache.Insert("Posts", result, new TimerCacheDependency());

                return result;      
            }
        }
        #endregion

        private IEnumerable<SPTerm> GetTermsFromSP(string setname, int lcid)
        {
            if (HttpRuntime.Cache.Get("Terms" + setname) != null)
                return HttpRuntime.Cache.Get("Terms" + setname) as IEnumerable<SPTerm>;

            var session = TaxonomySession.GetTaxonomySession(context);
            var termSets = session.GetTermSetsByName(setname, 1033);
            context.Load(session);
            context.Load(termSets);
            context.ExecuteQuery();

            var allTerms = termSets.First().GetAllTerms();
            context.Load(allTerms);
            context.ExecuteQuery();

            List<SPTerm> terms = new List<SPTerm>();
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
            HttpRuntime.Cache.Insert("Terms" + setname, terms, new TimerCacheDependency());
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