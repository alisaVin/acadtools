using ACADTools.Models;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using System;
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

        //for room polygones
        public List<RaumpolygonEntity> GetPolygonesFromLayer(string selectedLayer)
        {
            if (string.IsNullOrEmpty(selectedLayer)) return null;

            List<RaumpolygonEntity> polygones = new List<RaumpolygonEntity>();
            ObjectIdCollection dwgPolygones = SelectAllPolygones(selectedLayer);
            DBObject polygoneObj = null;

            using (var trans = db.TransactionManager.StartOpenCloseTransaction())
            {
                //var bt = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                //var ms = trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead) as BlockTableRecord;

                foreach (ObjectId dwgPolygone in dwgPolygones)
                {
                    polygoneObj = trans.GetObject(dwgPolygone, OpenMode.ForRead);
                    RaumpolygonEntity polygone = new RaumpolygonEntity
                    {
                        Id = polygoneObj.Id.ToString(),
                        Handle = polygoneObj.Handle.ToString(), //Referenz Fläche
                        Name = selectedLayer
                    };
                    polygones.Add(polygone);
                }
            }
            return polygones;
        }

        private ObjectIdCollection SelectAllPolygones(string layerName)
        {
            // Build a filter list so that only entities
            // on the specified layer are selected
            //TypedValue[] tvs = new TypedValue[1]
            //{
            //    new TypedValue((int)DxfCode.LayerName, layerName)
            //};

            //SelectionFilter sf = new SelectionFilter(tvs);
            //PromptSelectionResult psr = ed.SelectAll(sf);

            //if (psr.Status == PromptStatus.OK)
            //    return new ObjectIdCollection(psr.Value.GetObjectIds());
            //else
            //    return new ObjectIdCollection();

            ObjectIdCollection selObjects = null;

            try
            {
                PromptSelectionResult psr = null;

                TypedValue[] tvs = new TypedValue[]
                {
                    new TypedValue(Convert.ToInt32(DxfCode.Operator), "<and"),
                    new TypedValue(Convert.ToInt32(DxfCode.LayerName), layerName),
                    new TypedValue(Convert.ToInt32(DxfCode.Operator), "<or"),
                    new TypedValue(Convert.ToInt32(DxfCode.Start), "POLYLINE"),
                    new TypedValue(Convert.ToInt32(DxfCode.Start), "LWPOLYLINE"),
                    new TypedValue(Convert.ToInt32(DxfCode.Start), "POLYLINE2D"),
                    new TypedValue(Convert.ToInt32(DxfCode.Start), "POLYLINE3d"),
                    new TypedValue(Convert.ToInt32(DxfCode.Operator), "or>"),
                    new TypedValue(Convert.ToInt32(DxfCode.Operator), "and>")
                };

                SelectionFilter sf = new SelectionFilter(tvs);
                psr = ed.SelectAll(sf);

                if (psr.Status == PromptStatus.OK)
                    selObjects = new ObjectIdCollection(psr.Value.GetObjectIds());
                else
                    selObjects = new ObjectIdCollection();
            }
            catch (System.Exception ex)
            {
                ed.WriteMessage($"Thron the exceprion during the selecting polylines: {ex.Message}\n{ex.StackTrace}");
            }

            return selObjects;
        }

        //for room blocks
        public List<RaumstempelEntity> GetBlocksFromLayer(string selectedLayerInfo)
        {
            if (string.IsNullOrEmpty(selectedLayerInfo)) return null;

            List<RaumstempelEntity> blDefEntities = new List<RaumstempelEntity>();

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
                    if (entity is BlockReference br && entity.Layer == selectedLayerInfo)
                    {
                        var btr = trans.GetObject(br.BlockTableRecord, OpenMode.ForRead) as BlockTableRecord;
                        RaumstempelEntity bEntity = new RaumstempelEntity
                        {
                            Id = br.Id.ToString(),
                            Handle = br.Handle.ToString(),
                            BlockDefinitionName = br.Name,
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
