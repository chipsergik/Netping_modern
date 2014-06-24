using System.Collections.Generic;

namespace NetPing.PriceGeneration.PriceList
{
    public interface ICategory
    {
        string CategoryName { get; set; }

        ICollection<ISection> Sections{ get; } 
    }
}
