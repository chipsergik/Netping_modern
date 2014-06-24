using System;
using System.Reflection;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Word;
using Shape = Microsoft.Office.Interop.Word.Shape;

namespace NetPing.PriceGeneration.Word
{
    public class WordRange
    {
        private readonly Range _range;

        internal WordRange(Range range)
        {
            _range = range;
        }

        public WordRange Find(string text)
        {
            return Find(text, false, true);
        }

        public WordRange Find(string text, bool caseSensitive, bool wholeWord)
        {
            return Find(text, caseSensitive, wholeWord, true);
        }

        public WordRange Find(string text, bool caseSensitive, bool wholeWord, bool matchWildCard)
        {
            Range result = _range.Duplicate;
            Find findObject = result.Find;
            if (Find(new Object[] {text, caseSensitive, wholeWord, matchWildCard}, findObject))
            {
                return new WordRange(result);
            }
            return null;
        }

        private static bool Find(object[] parameters, Find findObject)
        {
            return
                (bool)
                    findObject.GetType()
                        .InvokeMember("Execute", BindingFlags.InvokeMethod, null, findObject, parameters);
        }

        public bool Replace(string findText, string replaceText)
        {
            return Replace(findText, replaceText, false, true);
        }

        public bool Replace(string findText, string replaceText, bool caseSensitive, bool matchWholeWord)
        {
            Range resultRange = _range.Duplicate;
            Find findObject = resultRange.Find;
            var parameters = new object[11];
            parameters[0] = findText;           // find text
            parameters[1] = caseSensitive;      // match case
            parameters[2] = matchWholeWord;     // match whole word
            parameters[3] = true;               // match wild card
            parameters[4] = false;              // match sound like
            parameters[5] = false;              // match all word forms
            parameters[6] = true;               // forward
            parameters[7] = false;              // wrap
            parameters[8] = false;              // format
            parameters[9] = replaceText;        // replace text
            parameters[10] = WdReplace.wdReplaceOne;
            return Find(parameters, findObject);
        }

        public void InsertAfter(string text)
        {
            _range.InsertAfter(text);
        }

        public void InsertAfter(WordRange wordRange)
        {
            Range newRange = GetNextRange();
            newRange.FormattedText = wordRange._range;
        }

        public void InsertPicture(string fileName)
        {
            var inlineShapes = _range.InlineShapes;
            InlineShape shape = (InlineShape) inlineShapes.GetType()
                .InvokeMember("AddPicture", BindingFlags.InvokeMethod, null, inlineShapes,
                    new object[] {fileName});
            shape.LockAspectRatio = MsoTriState.msoCTrue;
            shape.Width = 140;
            var s = shape.ConvertToShape();
            s.WrapFormat.Type = WdWrapType.wdWrapSquare;
            s.WrapFormat.DistanceLeft = 0;
            s.WrapFormat.DistanceTop = 0;
            s.Left = 0;
            s.Top = 0;
        }

        public bool Delete(string findText)
        {
            Range resultRange = _range.Duplicate;
            Find findObject = resultRange.Find;
            if (Find(new object[] {findText, false, true, true}, findObject))
            {
                resultRange.Text = "";
                return true;
            }
            return false;
        }

        public WordRange GetExpandedRange(int expandedLeft, int expandedRight)
        {
            return new WordRange(GetRange(_range.Start - expandedLeft, _range.End + expandedRight));
        }

        private Range GetNextRange()
        {
            return GetRange(_range.End, _range.End);
        }

        private Range GetRange(int start, int end)
        {
            return
                (Range)
                    _range.Document.GetType()
                        .InvokeMember("Range", BindingFlags.InvokeMethod, null, _range.Document,
                            new object[] {start, end});
        }

        public string Text
        {
            get
            {
                return _range.Text;
            }
        }
    }
}
