using System.Collections.Generic;
using NetPing.PriceGeneration;
using NetPing.PriceGeneration.PriceList;

namespace NetPing_modern.PriceGeneration
{
    public class CategoriesReplacementsTree : ReplacementsTree
    {
        protected override ICollection<IReplacement> CreateReplacemenets()
        {
            var rs =  base.CreateReplacemenets();
            rs.Add(new CategoryNameReplacement("%category.name%"));
            return rs;
        }

        protected override IList<object> GetItems(object source)
        {
            var items = base.GetItems(source);

            if (source is IPriceList)
            {
                var price = (IPriceList) source;
                foreach (ICategory category in price.Categories)
                {
                    items.Add(category);
                }
            }
            return items;
        }
    }
}
