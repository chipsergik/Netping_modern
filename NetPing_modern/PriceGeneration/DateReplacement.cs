using System;
using NetPing.PriceGeneration;
using NetPing.PriceGeneration.Word;

namespace NetPing_modern.PriceGeneration
{
    public class DateReplacement : BaseReplacemenet
    {
        public DateReplacement(string tagName) : base(tagName)
        {
        }

        protected override void ApplyEntry(WordRange range, object source)
        {
            InsertLines(range, new string[] {DateTime.Now.ToString("dd.MM.yyyy")});
        }
    }
}