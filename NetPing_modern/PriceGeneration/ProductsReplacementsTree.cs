using System.Collections.Generic;
using NetPing.PriceGeneration;
using NetPing.PriceGeneration.PriceList;

namespace NetPing_modern.PriceGeneration
{
    public class ProductsReplacementsTree : ReplacementsTree
    {
        protected override IList<object> GetItems(object source)
        {
            var items = base.GetItems(source);

            if (source is ISection)
            {
                var section = (ISection) source;
                foreach (IProduct product in section.Products)
                {
                    items.Add(product);
                }
            }
            return items;
        }

        protected override ICollection<IReplacement> CreateReplacemenets()
        {
            var rs = base.CreateReplacemenets();
            rs.Add(new ProductReplacement("%product.*%"));
            rs.Add(new ProductImageReplacement("%device.image%"));
            return rs;
        }
    }
}
