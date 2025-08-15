using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace ACADTools.Models.Processing
{
    public class TextRaumstempelDTO
    {
        public ObjectId Id { get; set; }
        public string Text { get; set; }
        public Point3d InsertPoint { get; set; }
    }
}
