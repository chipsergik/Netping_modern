using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NetPing.PriceGeneration.PriceList;

namespace NetPing_modern.PriceGeneration
{
    public class PriceList : IPriceList, IDisposable
    {
        public PriceList()
        {
            Categories = new Collection<ICategory>();
        }

        public ICollection<ICategory> Categories { get; private set; }
        public void Dispose()
        {
            foreach (Category category in Categories)
            {
                category.Dispose();
            }
        }
    }
}
