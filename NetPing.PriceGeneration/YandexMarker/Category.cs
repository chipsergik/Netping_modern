using System;
using System.Xml.Serialization;

namespace NetPing.PriceGeneration.YandexMarker
{
    [Serializable]
    public class Category
    {
        [XmlAttribute("id")]
        public int Id { get; set; }

        [XmlIgnore]
        public int? ParentId { get; set; }

        [XmlAttribute("parentId")]
        public string ProxyParentId
        {
            get
            {
                return ParentId.HasValue ? ParentId.Value.ToString() : null;
            }
            set
            {
                int parentId;
                if (int.TryParse(value, out parentId))
                {
                    ParentId = parentId;
                }
                else
                {
                    ParentId = null;
                }
            }
        }

        [XmlText]
        public string Name { get; set; }
    }
}
