using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace NetPing.Global.Config
{
    public class SharePointSettings : ConfigurationSection
    {
        [ConfigurationProperty("SiteUrl", IsRequired = true)]
        public string SiteUrl
        {
            get
            {
                return this["SiteUrl"] as string;
            }
            set
            {
                this["SiteUrl"] = value;
            }
        }

        [ConfigurationProperty("Login", IsRequired = true)]
        public string Login
        {
            get
            {
                return this["Login"] as string;
            }
            set
            {
                this["Login"] = value;
            }
        }

        [ConfigurationProperty("Password", IsRequired = true)]
        public string Password
        {
            get
            {
                return this["Password"] as string;
            }
            set
            {
                this["Password"] = value;
            }
        }

        [ConfigurationProperty("CacheTimeout", IsRequired = true, DefaultValue = "43200000")]
        public int CacheTimeout
        {
            get
            {
                return int.Parse(this["CacheTimeout"] as string);
            }
            set
            {
                this["CacheTimeout"] = value;
            }
        }
    } 
}