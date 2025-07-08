using ACADTools.Views;
using Autodesk.AutoCAD.Runtime;
using app = Autodesk.AutoCAD.ApplicationServices.Application;

namespace ACADTools.Commands
{
    public class ACADCommands
    {
        [CommandMethod("ACADTOOLS")]
        public void ModelessWpfDialogCmd()
        {
            var dialog = new MainView();
            var result = app.ShowModalWindow(dialog);
        }
    }
}
