using Microsoft.Office.Interop.Word;

namespace NetPing.PriceGeneration.Word
{
    public class WordSection
    {
        private readonly Section _section;

        public WordSection(Section section)
        {
            _section = section;
        }

        public WordHeadersFootersCollection Headers
        {
            get
            {
                return new WordHeadersFootersCollection(_section.Headers);
            }
        }

        public WordHeadersFootersCollection Footers
        {
            get
            {
                return new WordHeadersFootersCollection(_section.Footers);
            }
        }
    }
}
