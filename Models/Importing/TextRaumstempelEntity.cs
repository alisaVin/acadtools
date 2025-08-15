using Autodesk.AutoCAD.Geometry;

namespace ACADTools.Models.Importing
{
    public class TextRaumstempelEntity
    {
        public string Id { get; set; }
        public string Handle { get; set; }
        public string Text { get; set; }
        public Point3d InsertPoint { get; set; }
    }
}
