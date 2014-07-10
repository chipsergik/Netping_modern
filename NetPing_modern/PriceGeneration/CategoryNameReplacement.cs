using NetPing.PriceGeneration;
using NetPing.PriceGeneration.PriceList;
using NetPing.PriceGeneration.Word;

namespace NetPing_modern.PriceGeneration
{
    public class CategoryNameReplacement : BaseReplacemenet
    {
        public CategoryNameReplacement(string tagName) : base(tagName)
        {
        }

        protected override void ApplyEntry(WordRange range, object source)
        {
            var category = (ICategory) source;
            InsertLines(range, new string[] {category.CategoryName});
        }
    }
}
