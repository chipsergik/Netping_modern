using System;
using System.Reflection;
using Microsoft.Office.Interop.Word;

namespace NetPing.PriceGeneration.Word
{
    internal class WordApplication
    {
        private Application _application;
        private static readonly WordApplication instance;
        private static object _root = new object();

        static WordApplication()
        {
            instance = new WordApplication();
        }

        private WordApplication()
        {
        }

        ~WordApplication()
        {
            if (_application != null)
            {
                Quit(false);
            }
        }

        public static WordApplication Instance
        {
            get
            {
                if (instance._application == null)
                    instance._application = new Application();

                return instance;
            }
        }

        public static object SyncRoot
        {
            get
            {
                return _root;
            }
        }

        public void Quit(bool saveOpenedDocuments)
        {
            if (_application == null) 
                return;

            var saveOption = saveOpenedDocuments
                ? WdSaveOptions.wdSaveChanges
                : WdSaveOptions.wdDoNotSaveChanges;

            _application.GetType()
                .InvokeMember("Quit", BindingFlags.InvokeMethod, null, _application, new object[] {saveOption});
            _application = null;
        }

        public WordDocument OpenDocument(string fileName)
        {
            if (_application == null)
                return null;

            Type documentsType = _application.Documents.GetType();
            var document =
                (_Document)
                    documentsType.InvokeMember("Open", BindingFlags.InvokeMethod, null, _application.Documents,
                        new object[]
                        {
                            fileName
                        });

            return new WordDocument(document);
        }
    }
}
