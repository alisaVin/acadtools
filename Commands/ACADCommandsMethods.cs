using Autodesk.AutoCAD.Runtime;

namespace ACADTools.Commands
{
    public class ACADCommandsMethods
    {
        #region Main command method
        [CommandMethod("ACADTOOLS")]
        public void ModelessWpfDialogCmd()
        {
            //var dialog = new MainView();
            //var result = app.ShowModalWindow(dialog);
            ACADCommandsCore.CreateRegisterTab();
        }
        #endregion

        #region Commands to register this plugin
        [CommandMethod("REGACADTOOL")]
        public static void RegisterAppOnDemand()
        {
            DemandLoading.RegisterForAutoLoading();
        }

        [CommandMethod("UNREGACADTOOL")]
        public static void UnregisterApp()
        {
            DemandLoading.UnregisterForAutoLoading();
        }
        #endregion

    }
}
