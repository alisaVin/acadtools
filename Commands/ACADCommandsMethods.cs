using ACADTools.Views;
using Autodesk.AutoCAD.Runtime;
using app = Autodesk.AutoCAD.ApplicationServices.Application;

namespace ACADTools.Commands
{
    public class ACADCommandsMethods
    {
        #region Main command method
        [CommandMethod("ACADTOOLS")]
        public void ModelessWpfDialogCmd()
        {

            ACADCommandsRibbon.CreateRegisterTab();
        }
        #endregion

        #region Generate area lists dialog window
        [CommandMethod("GENERATEAREALISTS")]
        public void GenerateAreaListsDialogCmd()
        {
            var mainDialog = new MainView();
            var result = app.ShowModalWindow(mainDialog);
        }
        #endregion

        #region Close ACADTools
        [CommandMethod("CLOSEACADTOOLS")]
        public void CloseAcadTools()
        {
            ACADCommandsRibbon.RemoveExistingTabs();
        }
        #endregion

        #region Info to ACADTools
        [CommandMethod("INFOACADTOOLS")]
        public void InfoAcadTools()
        {
            var infoDialog = new InfoView();
            var res = app.ShowModalWindow(infoDialog);
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
