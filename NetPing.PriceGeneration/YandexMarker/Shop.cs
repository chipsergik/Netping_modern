using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace NetPing.PriceGeneration.YandexMarker
{
    [Serializable]
    public class Shop
    {
        public Shop()
        {
            Categories = new Collection<Category>();
            Currencies = new Collection<Currency>();
            Offers = new Collection<Offer>();
        }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("company")]
        public string Company { get; set; }

        [XmlElement("url")]
        public string Url { get; set; }

        [XmlArray("currencies")]
        [XmlArrayItem("currency")]
        public Collection<Currency> Currencies { get; set; }

        [XmlArray("categories")]
        [XmlArrayItem("category")]
        public Collection<Category> Categories { get; set; }

        [XmlElement("local_delivery_cost")]
        public int LocalDeliveryCost { get; set; }

        [XmlArray("offers")]
        [XmlArrayItem("offer")]
        public Collection<Offer> Offers { get; set; }
    }
}
