using ACADTools.Services;
using ACADTools.Views;
using Autodesk.AutoCAD.Runtime;
using app = Autodesk.AutoCAD.ApplicationServices.Application;

namespace ACADTools.Commands
{
    /// <summary>
    /// Definition of AutoCAD CommandMethods
    /// </summary>
    public class ACADCommandMethods
    {
        #region Main command method
        [CommandMethod("ACADTOOLS")]
        public void ModelessWpfDialogCmd()
        {
            ACADCommandRibbon.CreateRegisterTab();
        }
        #endregion

        #region Generate area lists dialog window
        [CommandMethod("GENERATEAREALISTS")]
        public void GenerateAreaListsDialogCmd()
        {
            var mainDialog = new MainView();
            //Modeless dialog
            app.ShowModelessWindow(mainDialog);
            //Modal dialog
            //var result = app.ShowWindow(mainDialog);
        }
        #endregion

        #region Close ACADTools
        [CommandMethod("CLOSEACADTOOLS")]
        public void CloseAcadTools()
        {
            ACADCommandRibbon.RemoveExistingTabs();
            IconService.Cleanup();
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
            DemandLoadingService.RegisterForAutoLoading();
        }

        [CommandMethod("UNREGACADTOOL")]
        public static void UnregisterApp()
        {
            DemandLoadingService.UnregisterForAutoLoading();
        }
        #endregion

    }
}
