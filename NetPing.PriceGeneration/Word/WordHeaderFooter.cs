using Microsoft.Office.Interop.Word;

namespace NetPing.PriceGeneration.Word
{
    public class WordHeaderFooter
    {
        private readonly HeaderFooter _headerFooter;

        internal WordHeaderFooter(HeaderFooter headerFooter)
        {
            this._headerFooter = headerFooter;
        }

        public WordRange Range
        {
            get
            {
                return new WordRange(_headerFooter.Range);
            }
        }
    }
}
