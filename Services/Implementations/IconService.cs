using ACADTools.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace ACADTools.Services.Implementations
{
    /// <summary>
    /// Extrahiert Icons für Ribbon Buttons aus den Projekt Ressourcen
    /// </summary>
    public class IconService : IIconService
    {
        private static readonly Dictionary<string, string> _cachedPaths = new Dictionary<string, string>();
        private static readonly string _tempDir = Path.Combine(Path.GetTempPath(), "ACADTools_Icons");

        public string GetIconPath(string name)
        {
            if (!Directory.Exists(_tempDir))
                Directory.CreateDirectory(_tempDir);

            // Cache prüfen --> cash verweist auf das leere Verzeichnis, soll geleert werden
            if (_cachedPaths.ContainsKey(name))
                return _cachedPaths[name];

            try
            {
                string filePath = Path.Combine(_tempDir, $"{name}.png");

                // Wenn die Datei bereits extrahiert
                if (File.Exists(filePath))
                {
                    _cachedPaths[name] = filePath;
                    return filePath;
                }

                // Über Properties.Resources (wenn als Resource hinzugefügt)
                object resource = Properties.Resources.ResourceManager.GetObject(name.Replace("-", "_"));
                if (resource is Bitmap bitmap)
                {
                    using (var clonedBitmap = new Bitmap(bitmap))
                    {
                        clonedBitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                        _cachedPaths[name] = filePath;
                        return filePath;
                    }
                }

                // Fallback: Standard AutoCAD Icon
                return "RCDATA_16_MODIFY";
            }
            catch (Exception ex)
            {
                var ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument?.Editor;
                ed?.WriteMessage($"Icon extraction failed for '{name}': {ex.Message}\n");
                return "RCDATA_16_MODIFY";
            }
        }

        /// <summary>
        /// Aufräumen der temporären Dateien beim Shutdown !!!! Mal testen (vielleicht wird nicht benötigt)
        /// </summary>
        public void CleanUp()
        {
            try
            {
                if (Directory.Exists(_tempDir))
                {
                    Directory.Delete(_tempDir, true);
                    _cachedPaths.Clear();
                }
            }
            catch { }
        }
    }
}

// Direkt aus Assembly (wenn als Embedded Resource)
//var assembly = System.Reflection.Assembly.GetExecutingAssembly();
//var resourceNames = assembly.GetManifestResourceNames();

//// Mögliche Ressourcennamen durchsuchen
//string[] possibleNames = {
//                    $"ACADTools.Resources.{name}.png",
//                    $"ACADTools.Resources.{name}",
//                    $"ACADTools.{name}.png",
//                    resourceNames.FirstOrDefault(r => r.Contains(name))
//                };

//foreach (var resourceName in possibleNames.Where(n => !string.IsNullOrEmpty(n)))
//{
//    using (var stream = assembly.GetManifestResourceStream(resourceName))
//    {
//        if (stream != null)
//        {
//            using (var fileStream = File.Create(filePath))
//            {
//                stream.CopyTo(fileStream);
//            }
//            _cachedPaths[name] = filePath;
//            return filePath;
//        }
//    }
//}
