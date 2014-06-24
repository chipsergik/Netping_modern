using System.Collections.Generic;
using System.Collections.ObjectModel;
using NetPing.PriceGeneration.Word;

namespace NetPing.PriceGeneration
{
    public abstract class ReplacementsTree
    {
        private ICollection<IReplacement> _replacemenets;
        private Dictionary<string, ReplacementsTree> _subRanges = new Dictionary<string, ReplacementsTree>();

        public void Apply(WordRange range, object source)
        {
            foreach (IReplacement replacement in Replacements)
            {
                replacement.Apply(range, source);
            }
            foreach (KeyValuePair<string, ReplacementsTree> entry in _subRanges)
            {
                string tagName = entry.Key;
                WordRange nextEntry = GetNextEntry(range, tagName);
                int index = 0;
                ReplacementsTree replacementsTree = entry.Value;
                IList<object> items = replacementsTree.GetItems(source);
                while (nextEntry != null)
                {
                    if (items.Count == 0)
                    {
                        nextEntry.Replace(tagName, string.Empty);
                    }
                    else
                    {
                        if (index < items.Count)
                        {
                            if (index < items.Count - 1)
                            {
                                nextEntry.InsertAfter(nextEntry);
                            }
                            object item = items[index];
                            replacementsTree.Apply(nextEntry, item);

                            AddBookmark(item, nextEntry);
                            index++;

                            if (index == items.Count)
                            {
                                index = 0;
                            }
                        }

                        WordRange expandedRange = nextEntry.GetExpandedRange(0, 1);
                        string expandedText = expandedRange.Text;
                        if (!(expandedText.Length > nextEntry.Text.Length && expandedText[expandedText.Length - 1] == '\r'))
                        {
                            expandedRange = nextEntry;
                        }
                        string[] tags = tagName.Split('*');
                        for (int i = 0; i < tags.Length; i++)
                        {
                            DeleteTag(expandedRange, tags[i]);
                        }
                    }
                    nextEntry = GetNextEntry(range, tagName);
                }
            }
            
        }

        private static void AddBookmark(object item, WordRange nextEntry)
        {
            var bookmark = item as IBookmark;
            if (bookmark != null)
            {
                nextEntry.AddBookmark(bookmark.BookmarkName);
            }
        }

        protected virtual IList<object> GetItems(object source)
        {
            return new List<object>();
        }

        private void DeleteTag(WordRange range, string tag)
        {
            string[] stringsToDelete = {tag + "^13", tag};
            for (int i = 0; i < stringsToDelete.Length; i++)
            {
                if (range.Delete(stringsToDelete[i]));
            }
        }

        private WordRange GetNextEntry(WordRange range, string tagName)
        {
            return range.Find(tagName, false, true);
        }

        protected virtual ICollection<IReplacement> CreateReplacemenets()
        {
            var result = new Collection<IReplacement>();
            return result;
        }

        protected ICollection<IReplacement> Replacements
        {
            get { return _replacemenets ?? (_replacemenets = CreateReplacemenets()); }
        }

        public void Add(string pattern, ReplacementsTree subTree)
        {
            _subRanges.Add(pattern, subTree);
        }
    }
}
