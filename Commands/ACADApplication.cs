using ACADTools.Models.Importing;
using ACADTools.Models.Processing;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.BoundaryRepresentation;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.IO;
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

        //später als service bereitstellen ???
        #region Current Dwg Methods
        public string GetFullPathOfCurrentDwg()
        {
            HostApplicationServices hs = HostApplicationServices.Current;
            string fullPath = hs.FindFile(doc.Name, db, FindFileHint.Default);
            return fullPath;
        }

        public string GetNameOfCurrentDwg()
        {
            string fileName = Path.GetFileName(doc.Name);
            return fileName;
        }

        /// <summary>
        /// Get names of the current CAD-drawing layers
        /// </summary>
        /// <returns>List of layer names</returns>
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
        #endregion

        #region Work with polygons
        /// <summary>
        /// Get all room polygones entities from the selected layer
        /// </summary>
        /// <param name="selectedLayer">Name of selected layer</param>
        /// <returns>List of room polygones entities</returns>
        public List<RaumpolygonEntity> GetPolygonesFromLayer(string selectedLayer, int decimalPlaces)
        {
            if (string.IsNullOrEmpty(selectedLayer)) return new List<RaumpolygonEntity>(); //empty List

            List<RaumpolygonEntity> polygones = new List<RaumpolygonEntity>();
            ObjectIdCollection dwgPolygonIds = GetAllPolygonIds(selectedLayer);

            if (dwgPolygonIds == null || dwgPolygonIds.Count == 0)
                return polygones;

            using (var trans = db.TransactionManager.StartOpenCloseTransaction())
            {
                try
                {
                    foreach (ObjectId dwgPolygonId in dwgPolygonIds)
                    {
                        DBObject polygoneObj = trans.GetObject(dwgPolygonId, OpenMode.ForRead);
                        if (polygoneObj == null) continue;

                        double area = GetPolygonArea(trans, dwgPolygonId);
                        double perimeter = GetPolygonPerimeter(trans, dwgPolygonId);

                        RaumpolygonEntity polygone = new RaumpolygonEntity
                        {
                            Id = polygoneObj.Id.ToString().Replace("(", "").Replace(")", ""),
                            Handle = polygoneObj.Handle.Value.ToString("X").ToUpperInvariant(), //Referenz Fläche
                            Name = selectedLayer,
                            Area = Math.Round(area, decimalPlaces),
                            Perimeter = Math.Round(perimeter, decimalPlaces)
                        };
                        polygones.Add(polygone);
                    }
                }
                catch (System.Exception ex)
                {
                    ed.WriteMessage($"Thrown the exceprion during the loading of polygone's entities: {ex.Message}\n{ex.StackTrace}");
                }
            }
            return polygones;
        }

        /// <summary>
        /// Get all polygones ids from the current drawing
        /// </summary>
        /// <param name="layerName">Name of the selected layer with polygones</param>
        /// <returns>Collection of ObjectIds from the selected layer</returns>
        private ObjectIdCollection GetAllPolygonIds(string layerName)
        {
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
                ed.WriteMessage($"Thrown the exceprion during the polygon ids export: {ex.Message}\n{ex.StackTrace}");
            }
            return selObjects;
        }

        /// <summary>
        /// Get area of polygon
        /// </summary>
        /// <param name="trans">Transaction instance</param>
        /// <param name="dwgPolygonId">ObjectId of polygon</param>
        /// <returns>Dooble value of area</returns>
        private double GetPolygonArea(OpenCloseTransaction trans, ObjectId dwgPolygonId)
        {
            try
            {
                Entity ent = trans.GetObject(dwgPolygonId, OpenMode.ForRead) as Entity;
                if (ent == null)
                    return 0.0;  //Fehler anschreiben später

                switch (ent)
                {
                    case Polyline polyline:
                        if (polyline.Closed)
                            return Math.Abs(polyline.Area);
                        break;

                    case Polyline2d polyline2D:
                        if (polyline2D.Closed)
                            return Math.Abs(polyline2D.Area);
                        break;


                    case Polyline3d polyline3D:
                        if (polyline3D.Closed)
                            return Math.Abs(polyline3D.Area);
                        break;
                }
            }
            catch (System.Exception ex)
            {
                ed.WriteMessage($"Thrown the exceprion during the calculating polylines areas: {ex.Message}\n{ex.StackTrace}");
            }

            return 0.0;
        }

        /// <summary>
        /// Get perimeter of polygon
        /// </summary>
        /// <param name="trans">Transaction instance</param>
        /// <param name="dwgPolygonId">ObjectId of polygon</param>
        /// <returns>Double value of perimeter</returns>
        private double GetPolygonPerimeter(OpenCloseTransaction trans, ObjectId dwgPolygonId)
        {
            try
            {
                Entity ent = trans.GetObject(dwgPolygonId, OpenMode.ForRead) as Entity;
                if (ent == null)
                    return 0.0;  //Fehler anschreiben später

                switch (ent)
                {
                    case Polyline polyline:
                        return Math.Abs(polyline.Length);

                    case Polyline2d polyline2D:
                        return Math.Abs(polyline2D.Length);

                    case Polyline3d polyline3D:
                        return Math.Abs(polyline3D.Length);
                }
            }
            catch (System.Exception ex)
            {
                ed.WriteMessage($"Thrown the exceprion during the calculating polylines areas: {ex.Message}\n{ex.StackTrace}");
            }
            return 0.0;
        }
        #endregion

        #region Work with blocks
        /// <summary>
        /// Get all blocks entities with room information 
        /// </summary>
        /// <param name="selectedLayerInfo">Name of the selected layer with blocks</param>
        /// <returns>List of room information entities</returns>
        public List<RaumstempelEntity> GetBlocksFromLayer(string selectedLayerInfo)
        {
            List<RaumstempelEntity> blEntities = new List<RaumstempelEntity>();
            List<RaumstempelEntity> errBlEntities = new List<RaumstempelEntity>();
            if (string.IsNullOrEmpty(selectedLayerInfo)) return blEntities;

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
                            Id = br.Id.ToString().Replace("(", "").Replace(")", ""),
                            Handle = br.Handle.Value.ToString("X").ToUpperInvariant(),
                            BlockDefinitionName = br.Name,
                            //InsertionPoint = br.Position
                        };
                        blEntities.Add(bEntity);
                    }
                }
            }
            return blEntities;
        }
        #endregion



        #region Work with texts
        /// <summary>
        /// Load all room texts entities with room information
        /// </summary>
        /// <param name="selectedLayer">Name of the selected layer with texts</param>
        /// <returns>List of text entities</returns>
        public List<TextRaumstempelEntity> GetAllRoomTextes(string selectedLayer)
        {
            List<TextRaumstempelEntity> texts = new List<TextRaumstempelEntity>();
            if (string.IsNullOrEmpty(selectedLayer)) return texts;

            TypedValue[] tvs = new[]
            {
                new TypedValue((int)DxfCode.Start, "TEXT,MTEXT"), // only DBText or MText
                new TypedValue((int)DxfCode.LayerName, selectedLayer), // on the selected Layer
            };
            SelectionFilter filter = new SelectionFilter(tvs);
            PromptSelectionResult selection = ed.SelectAll(filter);

            if (selection.Status != PromptStatus.OK)
                return texts;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                foreach (ObjectId id in selection.Value.GetObjectIds())
                {
                    TextRaumstempelEntity textEntity = new TextRaumstempelEntity();
                    switch (id.ObjectClass.DxfName)
                    {
                        case "TEXT":
                            DBText text = tr.GetObject(id, OpenMode.ForRead) as DBText;
                            textEntity.Id = text.Id.ToString().ToUpperInvariant();
                            textEntity.Handle = text.Handle.Value.ToString("X").ToUpperInvariant();
                            textEntity.Text = text.TextString;
                            textEntity.InsertPoint = text.Position;
                            break;

                        case "MTEXT":
                            MText mText = tr.GetObject(id, OpenMode.ForRead) as MText;
                            textEntity.Id = mText.Id.ToString().ToUpperInvariant();
                            textEntity.Handle = mText.Handle.Value.ToString("X").ToUpperInvariant();
                            //textEntity.Text = mText.Text;
                            textEntity.Text = mText.Contents;
                            textEntity.InsertPoint = mText.Location;
                            break;

                        default: break;
                    }
                    texts.Add(textEntity);
                }
                tr.Commit();
            }
            return texts;
        }
        #endregion



        #region Find points in the polygon
        public bool FindBlockInsertionPointInsidePolygon(Transaction tr, ObjectId polygonId, List<RaumstempelDTO> blocks) //, ObjectIdCollection polygonIds
        {
            try
            {
                var polyline = tr.GetObject(polygonId, OpenMode.ForRead) as Polyline;
                if (polyline == null || !polyline.Closed) return false;   //<---------------Exception handling

                DBObjectCollection curves = new DBObjectCollection();
                curves.Add(polyline);

                using (DBObjectCollection regions = Region.CreateFromCurves(curves))
                using (Region region = (Region)regions[0])
                {
                    foreach (var block in blocks)
                    {
                        if (GetPointContainment(region, block.InsertionPoint) == PointContainment.Inside)
                            return true;
                        //can also create a list of objects, that should be added on the ERR_OBJEKTE layer
                    }
                }
                return false;
            }
            catch (System.Exception) { return false; }
        }

        private PointContainment GetPointContainment(Region region, Point3d point)
        {
            PointContainment result = PointContainment.Outside;
            using (Brep brep = new Brep(region))
            {
                if (brep != null)
                {
                    using (BrepEntity ent = brep.GetPointContainment(point, out result))
                    {
                        if (ent is Autodesk.AutoCAD.BoundaryRepresentation.Face)
                        {
                            result = PointContainment.Inside;
                        }
                    }
                }
            }
            return result;
        }
        #endregion

        public void AddErrorObjLayer(Transaction tr)
        {
            using (tr)
            {
                var lt = tr.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
                if (!lt.Has("ERR_OBJEKTE"))
                {
                    lt.UpgradeOpen();
                    var newLayer = new LayerTableRecord();
                    newLayer.Name = "ERR_OBJEKTE";
                    newLayer.LineWeight = LineWeight.LineWeight018;
                    newLayer.Description = "Der Layer mit Error-Objekte";
                    newLayer.Color = Autodesk.AutoCAD.Colors.Color.FromRgb(255, 0, 0);
                    lt.Add(newLayer);
                    tr.AddNewlyCreatedDBObject(newLayer, true);
                }
                tr.Commit();
                db.Clayer = lt["ERR_OBJEKTE"];
            }
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