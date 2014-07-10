using System.Collections.Generic;
using NetPing.PriceGeneration;

namespace NetPing_modern.PriceGeneration
{
    public class PriceListReplacementsTree : ReplacementsTree
    {
        public PriceListReplacementsTree()
        {
        }

        protected override ICollection<IReplacement> CreateReplacemenets()
        {
            var replacements = base.CreateReplacemenets();
            replacements.Add(new DateReplacement("%date%"));
            replacements.Add(new HeaderDateReplacement("%headerdate%"));
            return replacements;
        }
    }
}
