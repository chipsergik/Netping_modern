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

        public const string DateFormat = "yyyy-MM-dd HH:mm";

        [XmlAttribute("date")]
        public string ProxyDateTime
        {
            get
            {

                return Date.ToString(DateFormat);
            }
            set
            {
                Date = DateTime.ParseExact(value, DateFormat, CultureInfo.InvariantCulture);
            }
        }

        [XmlElement("shop")]
        public Shop Shop { get; set; }
    }
}
