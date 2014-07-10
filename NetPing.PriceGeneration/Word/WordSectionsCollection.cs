using System.Collections;
using System.Collections.Generic;
using Microsoft.Office.Interop.Word;

namespace NetPing.PriceGeneration.Word
{
    public class WordSectionsCollection : IEnumerable<WordSection>
    {
        private readonly IList<WordSection> _sections = new List<WordSection>();

        public WordSectionsCollection(Sections sections)
        {
            foreach (Section section in sections)
            {
                _sections.Add(new WordSection(section));
            }
        }

        public IEnumerator<WordSection> GetEnumerator()
        {
            return _sections.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
