using NetPing.PriceGeneration;
using NetPing.PriceGeneration.Word;

namespace NetPing_modern.PriceGeneration
{
    public class SectionNameReplacement : BaseReplacemenet
    {
        public SectionNameReplacement(string tagName) : base(tagName)
        {
        }

        protected override void ApplyEntry(WordRange range, object source)
        {
            var section = (Section) source;
            InsertLines(range, new string[] {section.SectionName});
        }
    }
}
