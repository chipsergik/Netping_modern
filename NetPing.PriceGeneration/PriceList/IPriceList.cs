using System.Collections.Generic;

namespace NetPing.PriceGeneration.PriceList
{
    public interface IPriceList
    {
        ICollection<ICategory> Categories { get; }
    }
}
