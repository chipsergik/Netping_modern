using NetPing.PriceGeneration.PriceList;

namespace NetPing_modern.PriceGeneration
{
    public class Product : IProduct
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Cost { get; set; }

        public string ImageFileName { get; set; }
        public string Url { get; set; }
    }
}
