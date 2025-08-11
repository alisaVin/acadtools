using ACADTools.Services.Contracts;
using Autodesk.AutoCAD.Customization;
using System;
using app = Autodesk.AutoCAD.ApplicationServices.Application;

namespace ACADTools.Commands
{
    public class ACADCommandRibbon
    {
        private const string _tabName = "ACADTools_v2";
        private const string _panelName = "Tools panel";

        private readonly IIconService _iconService;
        private CustomizationSection _cs;
        private string _currentWorkspace;

        public ACADCommandRibbon(IIconService iconService)
        {
            _iconService = iconService;
        }

        /// <summary>
        /// Initialize the common parameters
        /// </summary>
        private void InitializeParameters()
        {
            _cs = new CustomizationSection((string)app.GetSystemVariable("MENUNAME"));
            _currentWorkspace = (string)app.GetSystemVariable("WSCURRENT");
        }

        /// <summary>
        /// Create a new ribbon panel with ribbon buttons in AutoCAD UI
        /// </summary>
        public void CreateRegisterTab()
        {
            var doc = app.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;

            try
            {
                InitializeParameters();
                RemoveExistingTabs();
                CreateRibbonUI();

                if (_cs.IsModified)
                {
                    _cs.Save();
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
        /// Remove the existeing ribbon tabs of plug-in
        /// </summary>
        /// <param name="cs">Customization section instance</param>
        /// <param name="currentWorkspace">Current workspace variable </param>
        /// <param name="tabName">Name of the current ribbon tab</param>
        /// <param name="panelName">Name of the corresponding ribbon panel</param>
        public void RemoveExistingTabs()
        {
            try
            {
                // If parameters have not yet been initialized
                if (_cs == null)
                {
                    InitializeParameters();
                }

                // Remove workspace references
                int curWsIndex = _cs.Workspaces.IndexOfWorkspaceName(_currentWorkspace);
                if (curWsIndex >= 0)
                {
                    WSRibbonRoot wSRibbonRoot = _cs.Workspaces[curWsIndex].WorkspaceRibbonRoot;
                    for (int i = wSRibbonRoot.WorkspaceTabs.Count - 1; i >= 0; i--)
                    {
                        WSRibbonTabSourceReference tabRef = wSRibbonRoot.WorkspaceTabs[i];
                        if (tabRef.TabId == "ID_" + _tabName)
                        {
                            wSRibbonRoot.WorkspaceTabs.RemoveAt(i);
                        }
                    }
                }

                // Remouve tab sources
                RibbonRoot root = _cs.MenuGroup.RibbonRoot;
                for (int i = root.RibbonTabSources.Count - 1; i >= 0; i--)
                {
                    string tabSourceName = root.RibbonTabSources[i].Name;
                    if (tabSourceName == _tabName)
                    {
                        root.RibbonTabSources.RemoveAt(i);
                    }
                }

                // Remouve panel sources
                for (int i = root.RibbonPanelSources.Count - 1; i >= 0; i--)
                {
                    string ribPanelSourceName = root.RibbonPanelSources[i].Name;
                    if (ribPanelSourceName == _panelName)
                    {
                        root.RibbonPanelSources.RemoveAt(i);
                    }
                }
                _cs.Save();
                app.ReloadAllMenus();
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
        private void CreateRibbonUI()
        {
            //RibbonRoot ist das Stammobjekt für alle CUI-Elemente im Zusammenhang mit dem Menüband,
            //die aus der MenuGroup-Eigenschaft des zuvor erstellten CustomizationSection-Objekts abgerufen werden können.
            RibbonRoot root = _cs.MenuGroup.RibbonRoot;
            RibbonPanelSourceCollection panels = root.RibbonPanelSources;
            MacroGroup macroGroup = _cs.MenuGroup.MacroGroups[0];

            //Erstellt die RibbonTabSource und fügt die zu der RibbonTabSourceCollection hinzu
            //RibbonTabSource definiert grundlegende Informationen zum Menüband-Tab, wie z. B. Tab-Name, Text, ID usw.
            RibbonTabSource tabSrc = new RibbonTabSource(root);
            tabSrc.Text = tabSrc.Name = _tabName;
            tabSrc.ElementID = tabSrc.Id = "ID_" + _tabName;
            root.RibbonTabSources.Add(tabSrc);

            //Erstellt die RibbonPanelSource und fügt sie der RibbonPanelSourceCollection hinzu.
            RibbonPanelSource panelSrc = new RibbonPanelSource(root);
            panelSrc.Text = panelSrc.Name = _panelName;
            panelSrc.ElementID = panelSrc.Id = "ID_" + _panelName;
            panels.Add(panelSrc);

            //Erstellt die RibbonPanelSourceReference und fügt die zu der RibbonPanelSourceReferenceCollection hinzu
            //Die RibbonPanelSourceReference gibt an, wohin die zugehörige RibbonPanelSource verschoben werden soll,
            //wahrscheinlich in die Ribbon Panel Source Referenzensammlung einer gewünschten RibbonTabSource.
            //So kann ein einzelnes Ribbon Panel von verschiedenen Ribbon Tabs gehostet werden,
            //genauso wie ein Block als verschiedene Blockreferenzen eingefügt werden kann.
            RibbonPanelSourceReference ribPanelSourceRef = new RibbonPanelSourceReference(tabSrc);
            ribPanelSourceRef.PanelId = panelSrc.ElementID;
            tabSrc.Items.Add(ribPanelSourceRef);

            // Layout anpassen
            RibbonRow largeRow = new RibbonRow(panelSrc);
            panelSrc.Items.Add(largeRow);
            RibbonRowPanel subField = new RibbonRowPanel(largeRow);
            largeRow.Items.Add(subField);

            //Rows erstellen
            RibbonRow firstRow = new RibbonRow(subField);
            subField.Items.Add(firstRow);
            RibbonRow secondRow = new RibbonRow(subField);
            subField.Items.Add(secondRow);

            //Buttons erstellen
            RibbonCommandButton generateBtn = new RibbonCommandButton(largeRow);
            generateBtn.Text = "Flächenlisten generieren";
            string smallGenerateIcon = _iconService.GetIconPath("icons8-new-file-16");
            string largeGenerateIcon = _iconService.GetIconPath("icons8-new-file-32");

            MenuMacro menuMac1 = macroGroup.CreateMenuMacro("generateBtn_macro", "^C^CGENERATEAREALISTS", "generateBtn_tag", "Bereitet die Blockdefinitionen zum CSV-Import vor",
                                                            MacroType.Any, smallGenerateIcon, largeGenerateIcon, "generateBtn_labelID");
            generateBtn.MacroID = menuMac1.ElementID;
            generateBtn.ButtonStyle = RibbonButtonStyle.LargeWithText;
            generateBtn.KeyTip = "Genarate Key Tip";
            generateBtn.TooltipTitle = "Flächen generieren";
            largeRow.Items.Add(generateBtn);


            RibbonCommandButton infoBtn = new RibbonCommandButton(firstRow);
            infoBtn.Text = "Info";
            string smallInfoIcon = _iconService.GetIconPath("icons8-information-16");
            string largeInfoIcon = _iconService.GetIconPath("icons8-information-32");

            MenuMacro menuMac2 = macroGroup.CreateMenuMacro("infoBtn_macro", "^C^CINFOACADTOOLS", "infoBtn_tag", "Informationen zur aktuellen Plug-In Version",
                                                            MacroType.Any, smallInfoIcon, largeInfoIcon, "infoBtn_labelID");
            infoBtn.MacroID = menuMac2.ElementID;
            infoBtn.ButtonStyle = RibbonButtonStyle.SmallWithText;
            infoBtn.KeyTip = "Info Key Tip";
            infoBtn.TooltipTitle = "Info";
            firstRow.Items.Add(infoBtn);


            RibbonCommandButton closeBtn = new RibbonCommandButton(secondRow);
            closeBtn.Text = "ACADTools beenden";
            string smallCloseIcon = _iconService.GetIconPath("icons8-close-16");
            string largeCloseIcon = _iconService.GetIconPath("icons8-close-32");

            MenuMacro menuMacClose = macroGroup.CreateMenuMacro("closeBtn_macro", "^C^CCLOSEACADTOOLS", "closeBtn_tag", "Beendet die Anwendung vom ACADTools Plug-In",
                                                            MacroType.Any, smallCloseIcon, largeCloseIcon, "closeBtn_labelID");
            closeBtn.MacroID = menuMacClose.ElementID;
            closeBtn.ButtonStyle = RibbonButtonStyle.SmallWithText;
            closeBtn.KeyTip = "Close Key Tip";
            closeBtn.TooltipTitle = "ACADTools beenden";
            secondRow.Items.Add(closeBtn);

            //Erstellt die WorkspaceRibbonTabSourceReference
            //Sie verfügt über eine WorkspaceTabs-Sammlung, die neue Instanzen der WSRibbonTabSourceReference willkommen heißt.
            WSRibbonTabSourceReference tabSrcRef = WSRibbonTabSourceReference.Create(tabSrc);

            //Gibt den Ribbon Root vom Workspace
            //Die WSRibbonRoot ist das Stammobjekt für einen bestimmten Arbeitsbereich, soweit es das Ribbon betrifft.
            int curWsIndex = _cs.Workspaces.IndexOfWorkspaceName(_currentWorkspace);
            WSRibbonRoot wsRibRoot = _cs.Workspaces[curWsIndex].WorkspaceRibbonRoot;

            //Legt den Besitzer der Ribbon Tab Source Reference fest und fügt die zu der Workspace Ribbon Tab Collection hinzu
            tabSrcRef.SetParent(wsRibRoot);
            wsRibRoot.WorkspaceTabs.Add(tabSrcRef);
        }
    }
}
///<summary>
///Die AutoCAD-Systemvariable MENUNAME gibt den Dateinamen des Menüs (CUI oder CUIx, soweit es das Menüband betrifft) an.
///Das Objekt CustomizationSection ist das Stammobjekt für die AutoCAD CUI .NET API, und alles sollte von dort aus beginnen.
///<params name="cs">Die CustomizationSection-Instanz sollte möglichst einzeln verwaltet werden. Andernfalls können Synchronisierungsprobleme auftreten.</params>
///<params name="curWorkspace">Eine weitere AutoCAD-Systemvariable WSCURRENT gibt den aktuellen Arbeitsbereichsnamen an.</params>
/// </summary>
