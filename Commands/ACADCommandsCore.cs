using Autodesk.AutoCAD.Customization;
using System;
using app = Autodesk.AutoCAD.ApplicationServices.Application;

namespace ACADTools.Commands
{
    public class ACADCommandsCore
    {
        /// <summary>
        /// 
        /// </summary>
        public static void CreateRegisterTab()
        {
            var doc = app.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;
            string appName = "ACADTools_v2";
            string panelName = "Tools panel";

            try
            {
                ///<summary>
                ///Die AutoCAD-Systemvariable MENUNAME gibt den Dateinamen des Menüs (CUI oder CUIx, soweit es das Menüband betrifft) an.
                ///Das Objekt CustomizationSection ist das Stammobjekt für die AutoCAD CUI .NET API, und alles sollte von dort aus beginnen.
                ///<params name="cs">Die CustomizationSection-Instanz sollte möglichst einzeln verwaltet werden. Andernfalls können Synchronisierungsprobleme auftreten.</params>
                ///<params name="curWorkspace">Eine weitere AutoCAD-Systemvariable WSCURRENT gibt den aktuellen Arbeitsbereichsnamen an.</params>
                /// </summary>

                CustomizationSection cs = new CustomizationSection((string)app.GetSystemVariable("MENUNAME"));
                string curWorkspace = (string)app.GetSystemVariable("WSCURRENT");

                RemoveExistingTabs(cs, curWorkspace, appName, panelName);
                CreateRibbonUI(cs, curWorkspace, appName, panelName);

                if (cs.IsModified)
                {
                    cs.Save();
                    app.ReloadAllMenus();
                    ed.WriteMessage("CUI IS UPDATED AND RIBBON REFRESHED\n");
                }
            }
            catch (Exception ex)
            {
                ed.WriteMessage($"Exception thrown: {ex.Message} \n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Entfernt existierende Ribbon Tabs 
        /// </summary>
        /// <param name="cs">CustomizationSection Instanz</param>
        /// <param name="currentWorkspace">WSCURRENT</param>
        /// <param name="tabName">Name der aktuellen Ribbon Tab</param>
        /// <param name="panelName">Name des zugehörigen Ribbon Panels</param>
        private static void RemoveExistingTabs(CustomizationSection cs, string currentWorkspace, string tabName, string panelName)
        {
            try
            {
                // Workspace-Referenzen entfernen 
                int curWsIndex = cs.Workspaces.IndexOfWorkspaceName(currentWorkspace);
                if (curWsIndex >= 0)
                {
                    WSRibbonRoot wSRibbonRoot = cs.Workspaces[curWsIndex].WorkspaceRibbonRoot;
                    for (int i = wSRibbonRoot.WorkspaceTabs.Count - 1; i >= 0; i--)
                    {
                        WSRibbonTabSourceReference tabRef = wSRibbonRoot.WorkspaceTabs[i];
                        if (tabRef.TabId == "ID_" + tabName)
                        {
                            wSRibbonRoot.WorkspaceTabs.RemoveAt(i);
                        }
                    }
                }

                // TabSources entfernen
                RibbonRoot root = cs.MenuGroup.RibbonRoot;
                for (int i = root.RibbonTabSources.Count - 1; i >= 0; i--)
                {
                    string tabSourceName = root.RibbonTabSources[i].Name;
                    if (tabSourceName == tabName)
                    {
                        root.RibbonTabSources.RemoveAt(i);
                    }
                }

                // PanelSources entfernen 
                for (int i = root.RibbonPanelSources.Count - 1; i >= 0; i--)
                {
                    string ribPanelSourceName = root.RibbonPanelSources[i].Name;
                    if (ribPanelSourceName == panelName)
                    {
                        root.RibbonPanelSources.RemoveAt(i);
                    }
                }

                cs.Save();
            }
            catch (Exception ex)
            {
                var ed = app.DocumentManager.MdiActiveDocument.Editor;
                ed.WriteMessage($"Cleanup failed: {ex.Message} \n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Erstellt neue Ribbon-Elemente im aktuellen Workspace
        /// </summary>
        /// <param name="cs">CustomizationSection Instanz</param>
        /// <param name="currentWorkspace">WSCURRENT</param>
        /// <param name="tabName">Name der aktuellen Ribbon Tab</param>
        /// <param name="panelName">Name des zugehörigen Ribbon Panels</param>
        private static void CreateRibbonUI(CustomizationSection cs, string currentWorkspace, string tabName, string panelName)
        {
            //RibbonRoot ist das Stammobjekt für alle CUI-Elemente im Zusammenhang mit dem Menüband,
            //die aus der MenuGroup-Eigenschaft des zuvor erstellten CustomizationSection-Objekts abgerufen werden können.
            RibbonRoot root = cs.MenuGroup.RibbonRoot;
            RibbonPanelSourceCollection panels = root.RibbonPanelSources;
            MacroGroup macroGroup = cs.MenuGroup.MacroGroups[0];

            //Erstellt die RibbonTabSource und fügt die zu der RibbonTabSourceCollection hinzu
            //RibbonTabSource definiert grundlegende Informationen zum Menüband-Tab, wie z. B. Tab-Name, Text, ID usw.
            RibbonTabSource tabSrc = new RibbonTabSource(root);
            tabSrc.Text = tabSrc.Name = tabName;
            tabSrc.ElementID = tabSrc.Id = "ID_" + tabName;
            root.RibbonTabSources.Add(tabSrc);

            //Erstellt die RibbonPanelSource und fügt sie der RibbonPanelSourceCollection hinzu.
            RibbonPanelSource panelSrc = new RibbonPanelSource(root);
            panelSrc.Text = panelSrc.Name = panelName;
            panelSrc.ElementID = panelSrc.Id = "ID_" + panelName;
            panels.Add(panelSrc);

            //Erstellt die RibbonPanelSourceReference und fügt die zu der RibbonPanelSourceReferenceCollection hinzu
            //Die RibbonPanelSourceReference gibt an, wohin die zugehörige RibbonPanelSource verschoben werden soll,
            //wahrscheinlich in die Ribbon Panel Source Referenzensammlung einer gewünschten RibbonTabSource.
            //So kann ein einzelnes Ribbon Panel von verschiedenen Ribbon Tabs gehostet werden,
            //genauso wie ein Block als verschiedene Blockreferenzen eingefügt werden kann.
            RibbonPanelSourceReference ribPanelSourceRef = new RibbonPanelSourceReference(tabSrc);
            ribPanelSourceRef.PanelId = panelSrc.ElementID;
            tabSrc.Items.Add(ribPanelSourceRef);

            //Rows erstellen
            RibbonRow row1 = new RibbonRow(panelSrc);
            panelSrc.Items.Add(row1);

            //Buttons erstellen
            RibbonCommandButton button1 = new RibbonCommandButton(row1);
            button1.Text = "Flächen generieren";
            string smallIcon1 = IconManager.GetIconPath("generate_16");
            string largeIcon1 = IconManager.GetIconPath("generate_32");

            MenuMacro menuMac1 = macroGroup.CreateMenuMacro("button1_macro", "^C^Cbutton1_command", "button1_tag", "button1_help",
                                                            MacroType.Any, smallIcon1, largeIcon1, "button1_labelID");
            button1.MacroID = menuMac1.ElementID;
            button1.ButtonStyle = RibbonButtonStyle.LargeWithText;
            button1.KeyTip = "button1 Key Tip";
            button1.TooltipTitle = "Flächen generieren Tooltip Title";
            row1.Items.Add(button1);


            RibbonCommandButton button2 = new RibbonCommandButton(row1);
            button2.Text = "Info";
            string smallIcon2 = IconManager.GetIconPath("info_16");
            string largeIcon2 = IconManager.GetIconPath("info_32");

            MenuMacro menuMac2 = macroGroup.CreateMenuMacro("button2_macro", "^C^Cbutton2_command", "button2_tag", "button2_help",
                                                            MacroType.Any, smallIcon2, largeIcon2, "button2_labelID");
            button2.MacroID = menuMac2.ElementID;
            button2.ButtonStyle = RibbonButtonStyle.SmallWithText;
            button2.KeyTip = "button2 Key Tip";
            button2.TooltipTitle = "Info Tooltip Title";
            row1.Items.Add(button2);

            //Erstellt die WorkspaceRibbonTabSourceReference
            //Sie verfügt über eine WorkspaceTabs-Sammlung, die neue Instanzen der WSRibbonTabSourceReference willkommen heißt.
            WSRibbonTabSourceReference tabSrcRef = WSRibbonTabSourceReference.Create(tabSrc);

            //Gibt den Ribbon Root vom Workspace
            //Die WSRibbonRoot ist das Stammobjekt für einen bestimmten Arbeitsbereich, soweit es das Ribbon betrifft.
            int curWsIndex = cs.Workspaces.IndexOfWorkspaceName(currentWorkspace);
            WSRibbonRoot wsRibRoot = cs.Workspaces[curWsIndex].WorkspaceRibbonRoot;

            //Legt den Besitzer der Ribbon Tab Source Reference fest und fügt die zu der Workspace Ribbon Tab Collection hinzu
            tabSrcRef.SetParent(wsRibRoot);
            wsRibRoot.WorkspaceTabs.Add(tabSrcRef);
        }
    }
}


//ed.WriteMessage($"Workspace tabs count: {cs.Workspaces[cs.Workspaces.IndexOfWorkspaceName(curWorkspace)].WorkspaceRibbonRoot.WorkspaceTabs.Count}\n");

//Prüft Tab in Workspace UND TabSources
//private static bool TabAlreadyExists(CustomizationSection cs, string tabName, string currentWorkspace)
//{
//    try
//    {
//        //Prüft TabSource
//        RibbonRoot ribbonRoot = cs.MenuGroup.RibbonRoot;
//        RibbonTabSourceCollection ribTabSrc = ribbonRoot.RibbonTabSources;
//        RibbonTabSource existingTabSource = null;

//        foreach (RibbonTabSource tab in ribTabSrc)
//        {
//            if (tab.Name == tabName)
//            {
//                existingTabSource = tab;
//                break;
//            }
//        }

//        //Prüft Workspace-Referenz
//        int curWsIndex = cs.Workspaces.IndexOfWorkspaceName(currentWorkspace);
//        if (curWsIndex >= 0)
//        {
//            WSRibbonRoot wSRibbonRoot = cs.Workspaces[curWsIndex].WorkspaceRibbonRoot;
//            WorkspaceRibbonTabCollection wsRibTabs = wSRibbonRoot.WorkspaceTabs;

//            foreach (WSRibbonTabSourceReference tabRef in wsRibTabs)
//            {
//                if (tabRef.TabId == tabName + "_tabSourceID")
//                {
//                    return true; // Tab existiert bereits im Workspace
//                }
//            }
//        }
//        return false;
//    }
//    catch (System.Exception ex)
//    {
//        var ed = app.DocumentManager.MdiActiveDocument.Editor;
//        ed.WriteMessage($"Thrown the exception by TabAlreadyExists(): {ex.Message} \n{ex.StackTrace}");
//        return false;
//    }
//}


//RibbonControl ribbon = ComponentManager.Ribbon;
//if (ribbon != null)
//{
//    RibbonTab ribbonTab = ribbon.FindTab("ACADTOOLS_new");
//    if (ribbonTab != null)
//        ribbon.Tabs.Remove(ribbonTab);

//    ribbonTab = new RibbonTab();
//    ribbonTab.Title = "ACADTOOLS_new";
//    ribbonTab.Id = $"{ribbonTab.Title}_PanelTabID";
//    ribbon.Tabs.Add(ribbonTab);
//    addContent(ribbonTab);
//}

//public static RibbonPanelSource GetRibbonPanel(CustomizationSection cs, string panelName)
//{
//    System.Collections.ObjectModel.Collection<CustomizationSection> csList = new System.Collections.ObjectModel.Collection<CustomizationSection>();
//    csList.Add(cs);
//    return RibbonRoot.FindPanelSource(csList, cs.MenuGroup.Name, panelName + "_PanelSourceID");
//}

//static void addContent(RibbonTab rtab)
//{
//    rtab.Panels.Add(AddOnePanel());
//}

//static RibbonPanel AddOnePanel()
//{
//    RibbonPanelSource rps = new RibbonPanelSource();
//    rps.Title = "ACADTOOLS_NEW";
//    RibbonPanel rpannel = new RibbonPanel();
//    rpannel.Source = rps;

//    //Create a Command Item that the Dialog Launcher can use,
//    // for this test it is just a place holder.
//    RibbonButton rci = new RibbonButton();
//    rci.Name = "ACADToolsCommand";

//    //assign the Command Item to the DialgLauncher which auto-enables
//    // the little button at the lower right of a Panel
//    rps.DialogLauncher = rci;

//    RibbonButton rbtn = new RibbonButton();
//    rbtn.Name = "Flächenliste generieren";
//    rbtn.ShowText = true;
//    rbtn.Description = "Erstellt eine CSV-Datei nach den eingegebenen Parameter";
//    rbtn.Text = "Flächenliste generieren";

//    RibbonButton rbtn2 = new RibbonButton();
//    rbtn2.Name = "Info";
//    rbtn2.ShowText = true;
//    rbtn2.Description = "Information über ACADTools Plug-in";
//    rbtn2.Text = "Info";

//    RibbonButton rbtn3 = new RibbonButton();
//    rbtn3.Name = "Beenden";
//    rbtn3.ShowText = true;
//    rbtn3.Description = "ACADTools Plug-in beenden und Registerkarte schließen";
//    rbtn3.Text = "Beenden";

//    //Add button to the tab
//    rps.Items.Add(rbtn);
//    rps.Items.Add(rbtn2);
//    rps.Items.Add(rbtn3);

//    return rpannel;
//}
