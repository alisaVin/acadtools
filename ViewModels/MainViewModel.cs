using ACADTools.Commands;
using ACADTools.Models.Importing;
using ACADTools.Services.Contracts;
using ACADTools.Services.Implementations;
using Microsoft.Extensions.DependencyInjection; //für IOC-Container
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;


namespace ACADTools.ViewModels
{
    public class MainViewModel : DefaultViewModel
    {
        private readonly ACADApplication _acadApplication;
        private readonly DefaultViewModel _defaultViewModel;
        //private readonly IAcadDocumentService _acadDocumentService; //<--------------------------------- Dependency Injection
        private ICsvService _csvService;
        //public IAsyncRelayCommand StartCommonCommand { get; set; }//  <------------------------------------- INTEGRIEREN!!!

        public MainViewModel()
        {
            //_acadDocumentService = acadDocumentService;

            //IOC Container
            using (var serviceProvider = new ServiceCollection()
                                             .AddTransient<IAcadDocumentService, AcadDocumentService>()
                                             .AddTransient<ICsvService, CsvService>().BuildServiceProvider()) //creates service provider with new service instances
            {
                var acadDocumentService = serviceProvider.GetRequiredService<AcadDocumentService>();
                acadDocumentService.GetNameOfCurrentDwg(); //get the name of dwg with dependency injection 
            }

            _acadApplication = new ACADApplication();
            _defaultViewModel = new DefaultViewModel();
            _csvService = new CsvService();
            LayerNames = _acadApplication.GetLayerNamesFromCAD()
                                         .OrderBy(x => x).ToList(); //sorted alphabeticaly
            Polygons = new ObservableCollection<RaumpolygonEntity>();
            Raumstempeln = new ObservableCollection<RaumstempelEntity>();
            Texts = new ObservableCollection<TextRaumstempelEntity>();

            //StartCommonCommand = new AsyncRelayCommand(StartProcessingAsync); //CanStart <---------------------------------- TESTEN!!!

            SelectedFilePathCsv = _acadApplication.GetFullPathOfCurrentDwg().Replace(".dwg", ".csv"); //später mit service implementieren //getrennt starten
            CurrentDwgName = _acadApplication.GetNameOfCurrentDwg();
        }


        //private async Task StartProcessingAsync()
        //{
        //    // 1) Gather (in CAD context)
        //    //var gatherResult = await Application.DocumentManager
        //    //    .ExecuteInCommandContextAsync(_ => GatherCadData(SelectedLayer, SelectedLayerInfo), null);
        //}


        /// <summary>
        /// Gets the Command object bound to the Start button.
        /// The button automatically disabled if CanExecute predicate returns false.
        /// </summary>
        //public RelayCommand GetBlocksFromSelectedLayerCommand => new RelayCommand(execute => LoadPolygonesFromSelectedLayer());
        public RelayCommand StartCommonCommand => new RelayCommand(
            execute => ExecuteMultipleProcesses(),
            canExecute => !string.IsNullOrEmpty(SelectedLayer) && !string.IsNullOrEmpty(SelectedLayerInfo));

        public RelayCommand ResetPropertiesCommand => new RelayCommand(
            execute => ResetProperies(),
            canExecute => !string.IsNullOrEmpty(SelectedLayer) && !string.IsNullOrEmpty(SelectedLayerInfo)); //&& CheckedBlock == true && CheckedHatching == true

        private void ExecuteMultipleProcesses()
        {
            LoadPolygonesFromSelectedLayer();

            if (!CheckedBlock && !CheckedBlockAttributes)
                LoadTextFromSelectedLayer();
            else
                LoadBlocksFromSelectedLayer();

            CreateAndSaveCsvFile(SelectedFilePathCsv, Polygons.ToList(), Raumstempeln.ToList(), Texts.ToList());  // VERBESSERN!!!!
        }

        /// <summary>
        /// Load the polygons entities to the view model
        /// </summary>
        public void LoadPolygonesFromSelectedLayer()
        {
            if (!string.IsNullOrEmpty(SelectedLayer))
            {
                var polygons = _acadApplication.GetPolygonesFromLayer(SelectedLayer, DecimalPlaces); //Flächen combobox
                Polygons.Clear();

                foreach (var polygon in polygons)
                {
                    Polygons.Add(polygon);
                }
                PolygonsCount = Polygons.Count;
            }
        }

        /// <summary>
        /// Loads blocks enitites with room information to the view model
        /// </summary>
        public void LoadBlocksFromSelectedLayer()
        {
            if (!string.IsNullOrEmpty(SelectedLayerInfo))
            {
                var blocks = _acadApplication.GetBlocksFromLayer(SelectedLayerInfo); //Flächen combobox
                Raumstempeln.Clear();

                foreach (var block in blocks)
                {
                    Raumstempeln.Add(block);
                }
            }
        }

        /// <summary>
        /// Loads text entities with room information to the view model
        /// </summary>
        private void LoadTextFromSelectedLayer()
        {
            var texts = _acadApplication.GetAllRoomTextes(SelectedLayerInfo);
            Texts.Clear();

            foreach (var text in texts)
            {
                Texts.Add(text);
            }
        }

        /// <summary>
        /// Creates a new .csv file and saves that with room polygones and room information data records
        /// </summary>
        /// <param name="saveFilePath">Path to save .csv file</param>
        private void CreateAndSaveCsvFile(string saveFilePath, List<RaumpolygonEntity> polygons, List<RaumstempelEntity> raumstempeln, List<TextRaumstempelEntity> texts)
        {
            _csvService.CreateAndSaveCsv(saveFilePath, polygons, raumstempeln, texts);
        }

        /// <summary>
        /// Reset input properies to the default values
        /// </summary>
        public void ResetProperies()
        {
            CopyPropertiesFrom(_defaultViewModel);
        }

        /// <summary>
        /// Copy default properties from the default view model
        /// </summary>
        /// <param name="source">Instance of the default view model</param>
        private void CopyPropertiesFrom(DefaultViewModel source)
        {
            var properties = typeof(DefaultViewModel).GetProperties()
                .Where(p => p.CanRead && p.CanWrite);

            foreach (var prop in properties)
            {
                var value = prop.GetValue(source);
                prop.SetValue(this, value);
            }
        }

        //public BlockItem BlockReftoBlockItem(BlockReference blockRef)
        //{
        //    if (blockRef == null)
        //    {
        //        // Handle the case where blockRef is null
        //        return default;
        //    }

        //    BlockItem item = new BlockItem
        //    {
        //        IsChecked = true,
        //        Name = blockRef.Name,
        //        Description = GetAttribute(blockRef),
        //        BlockCount = 0  // Set BlockCount to 0 initially
        //    };

        //    return item;
        //}
    }
}
