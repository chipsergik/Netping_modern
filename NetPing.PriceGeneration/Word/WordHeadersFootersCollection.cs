using System.Collections;
using System.Collections.Generic;
using Microsoft.Office.Interop.Word;

namespace NetPing.PriceGeneration.Word
{
    public class WordHeadersFootersCollection : IEnumerable<WordHeaderFooter>
    {
        private readonly List<WordHeaderFooter> _headersFooters = new List<WordHeaderFooter>();

        internal WordHeadersFootersCollection(HeadersFooters headersFooters)
        {
            foreach (HeaderFooter headerFooter in headersFooters)
            {
                this._headersFooters.Add(new WordHeaderFooter(headerFooter));
            }
        }

        public IEnumerator<WordHeaderFooter> GetEnumerator()
        {
            return _headersFooters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
