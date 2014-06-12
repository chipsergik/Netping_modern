using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NetpingHelpers;

namespace NetPing.Models
{
    /// <summary>
    /// Store information of device
    /// </summary>
    [Serializable]
    public class Device
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public SPTerm Name { get; set; }
        public List<SPTerm> Destination { get; set; }
        public double? Price { get; set; }
        public string GetCurrency
        { get
            {
                if (Helpers.IsCultureEng) return "usd";
                return "руб.";
            }
        }
        public SPTerm Label { get; set; }
        public DateTime Created { get; set; }
        public List<SPTerm> Connected_devices { get; set; }
        public string GroupUrl;

        public List<Post> Posts { get; set; }
        public List<SFile> SFiles { get; set; }
        public List<DeviceParameter> DeviceParameters { get; set; }

        public List<DevicePhoto> DevicePhotos { get; set; }

        public Device()
        {
            SFiles = new List<SFile>();
            Posts = new List<Post>();
        }

        public string Short_description { get; set; }

        public string Long_description { get; set; }

        public DevicePhoto GetCoverPhoto(bool isBig)
        {
            DevicePhoto dp=DevicePhotos.FirstOrDefault(p => p.IsBig == isBig && p.IsCover);
            if (dp != null) return dp;
            return new DevicePhoto();
        }

        public string GetURLDevicePage()
        {
            return "/product_item.aspx?id=" + Key;
        }
        public bool IsGroup()
        {
            if (Key.Contains("#")) return true;
            return false;
        }
        public bool IsInGroup(Device group)
        {
            var path = Name.Path.Split(';');
            if (group == null) return false;
            if (path.FirstOrDefault(p => p == group.Name.OwnNameFromPath) == null) return false;
            return true;
        }
    }
}