using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NetPing.PriceGeneration.PriceList;

namespace NetPing_modern.PriceGeneration
{
    public class Category : ICategory, IDisposable
    {
        public Category(string categoryName)
        {
            Sections = new Collection<ISection>();
            CategoryName = categoryName.Replace("<br/>", " ");
        }

        public string CategoryName { get; set; }
        public ICollection<ISection> Sections { get; private set; }
        public void Dispose()
        {
            foreach (Section section in Sections)
            {
                section.Dispose();
            }
        }
    }
}
