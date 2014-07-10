using System;
using System.Globalization;
using NetPing.PriceGeneration;
using NetPing.PriceGeneration.Word;

namespace NetPing_modern.PriceGeneration
{
    public class HeaderDateReplacement : BaseReplacemenet
    {
        public HeaderDateReplacement(string tagName) : base(tagName)
        {
        }

        protected override void ApplyEntry(WordRange range, object source)
        {
            InsertLines(range, new string[] { DateTime.Now.ToString("dd MMMM yyyy г", CultureInfo.GetCultureInfo("RU-ru")) });
        }
    }
}