﻿using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NetPing.PriceGeneration.YandexMarker;

namespace NetPing.PriceGeneration
{
    public static class YmlGenerator
    {
        public static void Generate(YmlCatalog catalog, string outputFileName)
        {
            var serializer = new XmlSerializer(typeof (YmlCatalog), "");
            using (TextWriter textWriter = new StreamWriter(outputFileName))
            {
                using (var writer = new XmlTextWriter(textWriter))
                {
                    var ns = new XmlSerializerNamespaces();
                    ns.Add("","");
                    writer.Formatting = Formatting.Indented;
                    writer.WriteStartDocument();
                    writer.WriteDocType("yml_catalog", null, "shops.dtd", null);
                    serializer.Serialize(writer, catalog, ns);
                }
            }
        }
    }
}
