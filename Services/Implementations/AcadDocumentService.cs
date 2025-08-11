using ACADTools.Services.Contracts;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using System.IO;

namespace ACADTools.Services.Implementations
{
    public class AcadDocumentService : IAcadDocumentService
    {
        private readonly Document _doc;
        private readonly Database _db;

        public AcadDocumentService(Document doc, Database db)
        {
            _doc = doc;
            _db = db;
        }
        public string GetFullPathOfCurrentDwg()
        {
            return HostApplicationServices.Current.FindFile(_doc.Name, _db, FindFileHint.Default);
        }

        public string GetNameOfCurrentDwg()
        {
            return Path.GetFileName(_doc.Name);
        }
    }
}
