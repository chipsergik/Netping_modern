using System;
using System.Globalization;
using System.Xml.Serialization;

namespace NetPing.PriceGeneration.YandexMarker
{
    [XmlRoot("yml_catalog")]
    public class YmlCatalog
    {
        [XmlIgnore]
        public DateTime Date { get; set; }

        private string dateFormat = "yyyy-MM-dd HH:mm";

        [XmlAttribute("date")]
        public string ProxyDateTime
        {
            get
            {

                return Date.ToString(dateFormat);
            }
            set
            {
                Date = DateTime.ParseExact(value, dateFormat, CultureInfo.InvariantCulture);
            }
        }

        [XmlElement("shop")]
        public Shop Shop { get; set; }
    }
}
