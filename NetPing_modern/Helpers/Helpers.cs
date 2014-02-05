using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using NetPing.Models;
using NetPing.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;

namespace NetPing.Helpers
{
    public static class Helpers
    {

        public static string ToFileUrlStr(this FieldUrlValue url, string file_name)
        {
            if (url == null) return "https://netpingeastcoltd-public.sharepoint.com/Pub/" + file_name;
              else return url.Url;
        }

        public static List<SPTerm> ListNamesToListDesitnations(this List<SPTerm> names)
        {
            List<SPTerm> result = new List<SPTerm>();
            var repository = new SPOnlineRepository();
            foreach (var name in names)
            {
                var device= repository.Devices.FirstOrDefault(dev => dev.Name == name);
                if (device != null)  result=result.Union(device.Destination).ToList();
            }
            return result;
        }

        public static string ToShortTextHTML(this string str,string url)
        {
            str=Regex.Replace(str, @"<(.|n)*?>", string.Empty).Replace("&nbsp", " ").Replace("&#160;"," "); // To clear text from HTML
    	    
            string[] words = str.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);		
		    StringBuilder sb = new StringBuilder();
		    for (int i = 0, count = 0; i < words.Length && count < 100; i++)
		    {
	    	    sb.AppendFormat("{0} ", words[i]);
			    count += words[i].Length + 1; // плюс пробел
		    }
            str = sb.ToString() + "<a href=\"" + url + "\"> подробнее...</a>";
            return str;
        }


        // Get device names terms list, return destinations list for this devices
        public static List<SPTerm> DevicesToDestinations(this List<SPTerm> values)
        {
            List<SPTerm> result=new List<SPTerm>();
            if (values == null || values.Count < 1)
                return result;
            
            var repository = new SPOnlineRepository();
            foreach (SPTerm value in values)
            {
                List<SPTerm> dev_dests = repository.Devices.ToList().Find(i => i.Name.Name == value.Name).Destination;
                result=result.Union(dev_dests).ToList();
            }

            return result;
        }

        // Replace links inside Sharepoint HTML body
        public static string ReplaceInternalLinks(this string value)
        {
            value=value.Replace("src=\"/web/feeds/Lists/Photos/", "src=\"https://netpingeastcoltd-public.sharepoint.com/Pub/Photos/Old_feeds/");
            value=value.Replace("src=\"/web/Catalog/Lists/Photos/", "src=\"https://netpingeastcoltd-public.sharepoint.com/Pub/Photos/Old_catalog/");
            value = value.Replace("src=\"/web/Lists/Photos/", "src=\"https://netpingeastcoltd-public.sharepoint.com/Pub/Photos/");
            return value;
        }

        public static List<string> ToStringList(this TaxonomyFieldValueCollection values)
        {
            var result = new List<string>();
            if (values == null || values.Count < 1)
                return result;

            foreach (var value in values)
            {
                result.Add(value.Label);
            }

            return result;
        }

        public static List<SPTerm> ToSPTermList(this TaxonomyFieldValueCollection values, IEnumerable<SPTerm> terms)
        {
            var result = new List<SPTerm>();
            if (values == null || values.Count < 1)
                return result;

            foreach (var value in values)
            {
                result.Add(value.ToSPTerm(terms));
            }

            return result; 
        }

        public static SPTerm ToSPTerm(this TaxonomyFieldValue value, IEnumerable<SPTerm> terms)
        {
            if (value == null)
                return new SPTerm();

            var guid = new Guid(value.TermGuid);

            return terms.FirstOrDefault(t => t.Id == guid);
        }

        public static string ToNormalString(this TaxonomyFieldValue value)
        {
            if (value == null)
                return string.Empty;

            return value.Label;
        }

        public static List<string> ToStringList(this FieldLookupValue[] values)
        {
            var result = new List<string>();
            if (values == null || values.Count() < 1)
                return result;

            foreach (var value in values)
            {
                result.Add(value.LookupValue);
            }

            return result;
        }

        public static SecureString ToSecureString(this string value)
        {
            var result = new SecureString();
            foreach (char c in value) result.AppendChar(c);
            
            return result;
        }
    }
}