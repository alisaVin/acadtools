using System.IO;
using System.Reflection;

namespace ACADTools.Commands
{
    /// <summary>
    /// Kombiniert Embedded Resources mit temporärer Extraktion für maximale Deployment-Sicherheit
    /// </summary>
    public static class IconManager
    {
        private static readonly Assembly _assembly;

        static IconManager()
        {
            _assembly = Assembly.GetExecutingAssembly();
        }

        public static string GetIcon(string name)
        {

            var allResNames = _assembly.GetManifestResourceNames();

            foreach (var resName in allResNames)
            {
                if (resName != null && resName == name)
                {
                    using (Stream resStream = _assembly.GetManifestResourceStream(resName))
                    {

                    }
                }


            }
            return "RCDATA_16_OSNAP";
        }
    }
}
