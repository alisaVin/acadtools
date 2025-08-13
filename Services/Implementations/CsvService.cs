using ACADTools.Models;
using ACADTools.Services.Contracts;
using CsvHelper;
using CsvHelper.Configuration;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace ACADTools.Services.Implementations
{
    // Weiter machen
    public class CsvService : ICsvService
    {
        public void CreateAndSaveCsv(string saveFilePath, List<RaumpolygonEntity> polygons, List<RaumstempelEntity> raumstempeln, List<TextRaumstempelEntity> texts)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture) //kann man nach der "Ländereinstellungen" in UI definieren
            {
                Delimiter = ";",
                ShouldQuote = (field) =>
                {
                    return true;
                }
            };

            using (var writer = new StreamWriter(saveFilePath, false, System.Text.Encoding.Default))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteHeader<RaumpolygonEntity>();
                csv.WriteHeader<RaumstempelEntity>();
                csv.WriteHeader<TextElementEnumerator>();
                csv.NextRecord();

                foreach (var polygon in polygons)
                {
                    writer.WriteLine($"=\"{polygon.Id}\";{polygon.Handle};{polygon.Name};{polygon.Area};{polygon.Perimeter};"); // KONFIGURIEREN UND UMSCHREIBEN FÜR ALLE ENTITIES

                    //for (int i = 0; i < block.Attributes.Count; i++)
                    //{
                    //    var attribute = block.Attributes[i];
                    //    writer.WriteLine($";;=\"{attribute.CadIdAttribute}\";{attribute.AttributeName};{attribute.AttributeValue};");
                    //}
                }

                foreach (var text in texts)
                {
                    writer.WriteLine($"=\"{text.Id}\";{text.Handle};{text.Text};");
                }
            }
        }
    }
}
