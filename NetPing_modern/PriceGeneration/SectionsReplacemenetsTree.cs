using System.Collections.Generic;
using NetPing.PriceGeneration;
using NetPing.PriceGeneration.PriceList;

namespace NetPing_modern.PriceGeneration
{
    public class SectionsReplacemenetsTree : ReplacementsTree
    {
        protected override IList<object> GetItems(object source)
        {
            var items = base.GetItems(source);

            if (source is ICategory)
            {
                var category = (ICategory) source;
                foreach (ISection section in category.Sections)
                {
                    items.Add(section);
                }
            }
            return items;
        }

        protected override ICollection<IReplacement> CreateReplacemenets()
        {
            var rs = base.CreateReplacemenets();
            rs.Add(new SectionNameReplacement("%section.name%"));
            return rs;
        }
    }
}
