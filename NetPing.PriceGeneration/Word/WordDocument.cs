using System;
using System.Reflection;
using Microsoft.Office.Interop.Word;

namespace NetPing.PriceGeneration.Word
{
    public class WordDocument
    {
        private _Document _document;

        internal WordDocument(_Document document)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            _document = document;
        }

        public void Close(bool saveOnClose)
        {
            var saveOptions = saveOnClose ? WdSaveOptions.wdSaveChanges : WdSaveOptions.wdDoNotSaveChanges;
            _document.GetType()
                .InvokeMember("Close", BindingFlags.InvokeMethod, null, _document, new object[] {saveOptions});
        }

        public void SaveAs(string fileName, WdSaveFormat format)
        {
            _document.GetType()
                .InvokeMember("SaveAs", BindingFlags.InvokeMethod, null, _document, new object[] {fileName, format});
        }

        public WordRange Content
        {
            get
            {
                return new WordRange(_document.Content);
            }
        }

        public WordSectionsCollection Sections
        {
            get
            {
                return new WordSectionsCollection(_document.Sections);
            }
        }
    }
}
