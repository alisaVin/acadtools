using ACADTools.Commands;
using ACADTools.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;


namespace ACADTools.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ACADApplication _acadApplication;

        //Properties
        public ObservableCollection<RaumpolygonEntity> Polygons { get; set; }
        public ObservableCollection<RaumstempelEntity> Raumstempeln { get; set; }
        public List<string> LayerNames { get; set; }

        //Selected layer item
        private string _selectedLayer;
        public string SelectedLayer
        {
            get => _selectedLayer;
            set { _selectedLayer = value; OnPropertyChanged(nameof(SelectedLayer)); }
        }

        //Selected layer info
        private string _selectedLayerInfo;
        public string SelectedLayerInfo
        {
            get => _selectedLayerInfo;
            set { _selectedLayerInfo = value; OnPropertyChanged(nameof(SelectedLayerInfo)); }
        }

        //for check boxes binding
        private bool _checkedHatching;
        public bool CheckedHatching
        {
            get => _checkedHatching;
            set { _checkedHatching = value; OnPropertyChanged(nameof(CheckedHatching)); }
        }

        private bool _checkedBlock;
        public bool CheckedBlock
        {
            get => _checkedBlock;
            set { _checkedBlock = value; OnPropertyChanged(nameof(CheckedBlock)); }
        }

        /// <summary>
        /// Gets the Command object bound to the Start button.
        /// The button automatically disabled if CanExecute predicate returns false.
        /// </summary>
        //public RelayCommand GetBlocksFromSelectedLayerCommand => new RelayCommand(execute => LoadPolygonesFromSelectedLayer());
        public RelayCommand GetObjtectsBySelectedLayersCommand => new RelayCommand(
            execute => ProcessBothLayers(),
            canExecute => !string.IsNullOrEmpty(SelectedLayer) && !string.IsNullOrEmpty(SelectedLayerInfo));

        public RelayCommand ResetPropertiesCommand => new RelayCommand(
            execute => ResetProperies(),
            canExecute => !string.IsNullOrEmpty(SelectedLayer) && !string.IsNullOrEmpty(SelectedLayerInfo) && CheckedBlock == true && CheckedHatching == true);

        private void ProcessBothLayers()
        {
            LoadPolygonesFromSelectedLayer();
            LoadBlocksFromSelectedLayer();
        }

        public MainViewModel()
        {
            _acadApplication = new ACADApplication();
            LayerNames = _acadApplication.GetLayerNamesFromCAD()
                                         .OrderBy(x => x).ToList(); //sorted alphabeticaly
            Polygons = new ObservableCollection<RaumpolygonEntity>();
            Raumstempeln = new ObservableCollection<RaumstempelEntity>();
        }

        //for room polygones
        public void LoadPolygonesFromSelectedLayer()
        {
            if (!string.IsNullOrEmpty(SelectedLayer))
            {
                var polygones = _acadApplication.GetPolygonesFromLayer(SelectedLayer); //Flächen combobox
                Polygons.Clear();

                foreach (var polygone in polygones)
                {
                    Polygons.Add(polygone);
                }
            }
            MessageBox.Show($"You have {Polygons.Count} polygones to import");
        }

        //for room info blocks
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
            MessageBox.Show($"You have {Raumstempeln.Count} blocks to import");
        }

        public void ResetProperies()
        {
            //try to make this later https://www.codeproject.com/Articles/158591/Resetting-a-View-Model-in-WPF-MVVM-applications-wi

            SelectedLayer = null;
            SelectedLayerInfo = null;
            CheckedHatching = false;
            CheckedBlock = false;
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
