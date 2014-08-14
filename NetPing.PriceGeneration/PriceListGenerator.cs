using System.IO;
using Microsoft.Office.Interop.Word;
using NetPing.PriceGeneration.PriceList;
using NetPing.PriceGeneration.Word;

namespace NetPing.PriceGeneration
{
    public class PriceListGenerator
    {
        private readonly ReplacementsTree _replacements;

        public PriceListGenerator(ReplacementsTree replacements)
        {
            _replacements = replacements;
        }

        public void Generate(IPriceList priceList, FileInfo template, string outputFileName)
        {
            lock (WordApplication.SyncRoot)
            {
                WordApplication application = WordApplication.Instance;

                try
                {
                    WordDocument document = application.OpenDocument(template.FullName);
                    _replacements.Apply(document.Content, priceList);
                    foreach (WordSection section in document.Sections)
                    {
                        foreach (var header in section.Headers)
                        {
                            _replacements.Apply(header.Range, priceList);
                        }
                        foreach (var footer in section.Footers)
                        {
                            _replacements.Apply(footer.Range, priceList);
                        }
                    
                    }
                    document.Content.UpdateFields();
                    document.SaveAs(outputFileName, WdSaveFormat.wdFormatPDF);
                    document.Close(false);
                }
                finally
                {
                    application.Quit(false);
                }
            }
        }
    }
}
