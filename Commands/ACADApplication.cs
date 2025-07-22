using ACADTools.Models;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using System.Collections.Generic;
using System.Linq;
using app = Autodesk.AutoCAD.ApplicationServices.Application;


namespace ACADTools.Commands
{
    public class ACADApplication
    {
        Document doc;
        Database db;
        Editor ed;

        public ACADApplication()
        {
            doc = app.DocumentManager.MdiActiveDocument;
            db = doc.Database;
            ed = doc.Editor;
        }

        public List<string> GetLayerNamesFromCAD()
        {
            using (var trans = db.TransactionManager.StartOpenCloseTransaction())
            {
                return ((LayerTable)trans.GetObject(db.LayerTableId, OpenMode.ForRead))
                            .Cast<ObjectId>()
                            .Select(id => ((LayerTableRecord)trans.GetObject(id, OpenMode.ForRead)).Name)
                            .ToList();
            }
        }

        public List<BlockDefinitionEntity> GetBlocksFromLayer(string selectedLayer)
        {
            if (string.IsNullOrEmpty(selectedLayer)) return null;

            List<BlockDefinitionEntity> blDefEntities = new List<BlockDefinitionEntity>();

            using (var trans = db.TransactionManager.StartOpenCloseTransaction())
            {
                var bt = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                var ms = trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead) as BlockTableRecord;

                foreach (ObjectId id in ms)
                {
                    if (!id.IsValid || id.IsNull || id.IsErased || id.IsEffectivelyErased)
                    {
                        continue;
                    }

                    var entity = trans.GetObject(id, OpenMode.ForRead) as Entity;
                    if (entity is BlockReference br)
                    {
                        var btr = trans.GetObject(br.BlockTableRecord, OpenMode.ForRead) as BlockTableRecord;
                        BlockDefinitionEntity bEntity = new BlockDefinitionEntity
                        {
                            Id = br.Id.ToString(),
                            Handle = br.Handle.ToString(),
                            Name = br.Name,
                        };
                        blDefEntities.Add(bEntity);
                    }
                }
            }
            return blDefEntities;
        }
    }
}

//return ms.Cast<ObjectId>()
//                         .Where(id =>
//                         {
//                             var entity = trans.GetObject(id, OpenMode.ForRead);
//                             return entity is BlockReference br && br.Layer == selectedLayer;
//                         })
//                         .Select(id =>
//                         {
//                             var br = trans.GetObject(id, OpenMode.ForRead);
//                             return new BlockDefinitionEntity
//                             {
//                                 ObjectId = id.ToString(),
//                                 Handle = br.Handle.ToString(),
//                                 Name = selectedLayer
//                             };
//                         })
//                         .ToList();
