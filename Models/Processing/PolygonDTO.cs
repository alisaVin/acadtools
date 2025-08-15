using Autodesk.AutoCAD.DatabaseServices;

namespace ACADTools.Models.Processing
{
    public class PolygonDTO
    {
        public ObjectId Id { get; set; }
        public double Area { get; set; }
        public double Perimeter { get; set; }
    }
}
