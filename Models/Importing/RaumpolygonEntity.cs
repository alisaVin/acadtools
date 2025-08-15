using CsvHelper.Configuration.Attributes;

namespace ACADTools.Models.Importing
{
    public class RaumpolygonEntity
    {
        [Name("ID_Raumpolygon")]
        public string Id { get; set; }
        [Name("Handle_Raumpolygon")]
        public string Handle { get; set; }
        [Name("Name_Raumpolygon")]
        public string Name { get; set; }
        [Name("Flaeche_Raumpolygon")]
        public double Area { get; set; }
        [Name("Umfang_Raumpolygon")]
        public double Perimeter { get; set; }
    }
}
