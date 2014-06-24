using NetPing.PriceGeneration;
using NetPing.PriceGeneration.PriceList;
using NetPing.PriceGeneration.Word;
using File = System.IO.File;

namespace NetPing_modern.PriceGeneration
{
    public class ProductImageReplacement : BaseReplacemenet
    {
        public ProductImageReplacement(string tagName) : base(tagName)
        {
        }

        protected override void ApplyEntry(WordRange range, object source)
        {
            var product = source as IProduct;

            if (product != null)
            {
                if (!string.IsNullOrEmpty(product.ImageFileName))
                {
                    if (File.Exists(product.ImageFileName))
                    {
                        range.InsertPicture(product.ImageFileName);
                    }
                }
            }
        }
    }
}