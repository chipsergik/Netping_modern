using System.Collections.Generic;

namespace NetPing.PriceGeneration.PriceList
{
    public interface ISection
    {
        ICollection<IProduct> Products { get; } 

        string SectionName { get; }
    }
}
