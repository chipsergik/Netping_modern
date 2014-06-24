using System.IO;
using NetPing.PriceGeneration.PriceList;

namespace NetPing.PriceGeneration
{
    public interface IPriceListGenerator
    {
        void Generate(IPriceList priceList, FileInfo template, string outputFileName);
    }
}
