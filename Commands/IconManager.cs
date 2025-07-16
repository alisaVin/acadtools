using System;
using System.Drawing;
using System.IO;

namespace ACADTools.Commands
{
    /// <summary>
    /// Kombiniert Embedded Resources mit temporärer Extraktion für maximale Deployment-Sicherheit
    /// </summary>
    public static class IconManager
    {
        public static string GetIconPath(string name)
        {
            try
            {
                object resource = Properties.Resources.ResourceManager.GetObject(name);
                string tempDir = Path.Combine(Path.GetTempPath(), "ACADTools_Icons");
                if (!Directory.Exists(tempDir))
                    Directory.CreateDirectory(tempDir);

                if (resource != null)
                {
                    string fileName = $"{name}.";
                    string filePath = Path.Combine(tempDir, fileName);

                    if (File.Exists(filePath))
                        return filePath;

                    // Eventuelle Dateisperrung vermeiden
                    if (resource is Bitmap bitmap)
                    {
                        using (var clonedBitmap = new Bitmap(bitmap))
                            clonedBitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Bmp);
                    }
                    return filePath;
                }
                else
                {
                    return "RCDATA_16_MODIFY";
                    throw new ArgumentException($"Resource '{resource}' not found in VS Resources"); //???
                }

            }
            catch (System.Exception ex)
            {
                var ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
                ed.WriteMessage($"Thrown the exception during the icons extraction: {ex.Message} \n {ex.StackTrace}");
                return null;
            }

        }
    }
}
