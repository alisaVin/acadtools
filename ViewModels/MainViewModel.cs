using ACADTools.Commands;
using ACADTools.Models;
using ACADTools.Services.Contracts;
using ACADTools.Services.Implementations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;


namespace ACADTools.ViewModels
{
    public class MainViewModel : DefaultViewModel
    {
        private readonly ACADApplication _acadApplication;
        private readonly DefaultViewModel _defaultViewModel;
        //private readonly IAcadDocumentService _acadDocumentService;
        private ICsvService _csvService;

        public MainViewModel()
        {
            _acadApplication = new ACADApplication();
            _defaultViewModel = new DefaultViewModel();
            _csvService = new CsvService();
            LayerNames = _acadApplication.GetLayerNamesFromCAD()
                                         .OrderBy(x => x).ToList(); //sorted alphabeticaly
            Polygons = new ObservableCollection<RaumpolygonEntity>();
            Raumstempeln = new ObservableCollection<RaumstempelEntity>();
            SelectedFilePathCsv = _acadApplication.GetFullPathOfCurrentDwg().Replace(".dwg", ".csv"); //später mit service implementieren
            CurrentDwgName = _acadApplication.GetNameOfCurrentDwg();
        }

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
            if (CheckedBlock && CheckedBlockAttributes)
            {
                LoadPolygonesFromSelectedLayer();
                LoadBlocksFromSelectedLayer();
            }
            else if (CheckedText)
            {
                LoadTextFromSelectedLayer();
            }

            CreateAndSaveCsvFile(SelectedFilePathCsv, Polygons.ToList(), Raumstempeln.ToList());  // VERBESSERN!!!!
        }



        /// <summary>
        /// Load the polygones entities to the view model
        /// </summary>
        public void LoadPolygonesFromSelectedLayer()
        {
            if (!string.IsNullOrEmpty(SelectedLayer))
            {
                var polygones = _acadApplication.GetPolygonesFromLayer(SelectedLayer, DecimalPlaces); //Flächen combobox
                Polygons.Clear();

                foreach (var polygone in polygones)
                {
                    Polygons.Add(polygone);
                }
            }
            StartRoomNumber = Polygons.Count + 1;
        }

        /// <summary>
        /// Load blocks enitites with room information to the view model
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


        private void LoadTextFromSelectedLayer()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new .csv file and saves that with room polygones and room information data records
        /// </summary>
        /// <param name="saveFilePath">Path to save .csv file</param>
        private void CreateAndSaveCsvFile(string saveFilePath, List<RaumpolygonEntity> polygons, List<RaumstempelEntity> raumstempeln)
        {
            _csvService.CreateAndSaveCsv(saveFilePath, polygons, raumstempeln);
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
