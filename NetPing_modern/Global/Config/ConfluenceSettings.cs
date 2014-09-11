using System.Configuration;

namespace NetPing_modern.Global.Config
{
    public class ConfluenceSettings : ConfigurationSection
    {
        [ConfigurationProperty("Url", IsRequired = true)]
        public string Url
        {
            get
            {
                return this["Url"] as string;
            }
            set
            {
                this["Url"] = value;
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
    }
}