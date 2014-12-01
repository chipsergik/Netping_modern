using System.Collections.Generic;
using NetPing.Models;

namespace NetPing_modern.Models
{
    public class ProductsModel
    {
        public ProductsModel()
        {
            Sections = new List<SectionModel>();
        }

        public IEnumerable<Device> Devices { get; set; }

        public List<SectionModel> Sections { get; set; } 

        public SectionModel ActiveSection { get; set; }
    }
}