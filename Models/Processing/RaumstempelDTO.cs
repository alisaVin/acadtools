using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace ACADTools.Models.Processing
{
    public class RaumstempelDTO
    {
        public ObjectId Id { get; set; }
        public string BlockDefinitionName { get; set; }
        public Point3d InsertionPoint { get; set; }
        public bool IsInside { get; set; }
        //vllt. auch Attribute
    }
}
