using NetPing.PriceGeneration.Word;

namespace NetPing.PriceGeneration
{
    public abstract class BaseReplacemenet : IReplacement
    {
        protected const string TagOpeninngSymbol = "%";
        protected const string TagClosingSymbol = "%";

        protected BaseReplacemenet(string tagName)
        {
            this.TagName = tagName;
        }

        protected abstract void ApplyEntry(WordRange range, object source);

        public void Apply(WordRange range, object source)
        {
            WordRange nextEntry = GetNextEntry(range);
            while (nextEntry != null)
            {
                ApplyEntry(nextEntry, source);
                ClearoutPatternEntries(nextEntry);
                nextEntry = GetNextEntry(range);
            }
        }

        private WordRange GetNextEntry(WordRange range)
        {
            return range.Find(TagName, false, true);
        }

        private void ClearoutPatternEntries(WordRange range)
        {
            range.Replace(TagName, string.Empty);
        }

        public string TagName { get; private set; }

        protected void InsertLines(WordRange range, string[] lines)
        {
            if (lines == null || lines.Length == 0)
                return;

            int linesCount = lines.Length;
            for (int i = 0; i < linesCount - 1; i++)
            {
                range.InsertAfter(lines[i] + "\n");
            }

            range.InsertAfter(lines[linesCount - 1]);
        }
    }
}
