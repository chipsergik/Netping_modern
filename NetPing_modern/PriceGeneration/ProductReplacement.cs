using System;
using NetPing.PriceGeneration;
using NetPing.PriceGeneration.PriceList;
using NetPing.PriceGeneration.Word;

namespace NetPing_modern.PriceGeneration
{
    public class ProductReplacement : BaseReplacemenet
    {
        public ProductReplacement(string tagName) : base(tagName)
        {
        }

        protected override void ApplyEntry(WordRange range, object source)
        {
            var product = source as IProduct;

            if (product == null)
                return;

            string text = range.Text;
            string argument = GetArgumentName(text);

            if (string.IsNullOrEmpty(argument))
                return;

            string line = ComposeLine(product, argument);

            if (line == null)
                return;

            if (string.Equals(argument, "title", StringComparison.InvariantCultureIgnoreCase))
            {
                range.AddHyperLink(product.Url, line);
            }
            else
            {
                InsertLines(range, new string[] { line });
            }
        }

        private string ComposeLine(IProduct product, string argument)
        {
            var propertyInfo = product.GetType().GetProperty(argument);
            if (propertyInfo == null)
                return null;

            return propertyInfo.GetValue(product, null).ToString();
        }

        private string GetArgumentName(string text)
        {
            var terms = text.Split(new char[] {'.'}, StringSplitOptions.RemoveEmptyEntries);
            if (terms.Length < 2)
                return string.Empty;

            string argumentName = terms[1];
            int closingIndex = argumentName.LastIndexOf(TagClosingSymbol);

            argumentName = argumentName.Substring(0, closingIndex);

            return argumentName;
        }
    }
}
