using ACADTools.Commands;
using ACADTools.Models;
using System.Collections.ObjectModel;
using System.Linq;


namespace ACADTools.ViewModels
{
    public class MainViewModel : DefaultViewModel
    {
        private readonly ACADApplication _acadApplication;
        private readonly DefaultViewModel _defaultViewModel;

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
            canExecute => !string.IsNullOrEmpty(SelectedLayer) && !string.IsNullOrEmpty(SelectedLayerInfo)); //&& CheckedBlock == true && CheckedHatching == true

        private void ProcessBothLayers()
        {
            LoadPolygonesFromSelectedLayer();
            LoadBlocksFromSelectedLayer();
        }

        public MainViewModel()
        {
            _acadApplication = new ACADApplication();
            _defaultViewModel = new DefaultViewModel();
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
            StartRoomNumber = Polygons.Count + 1;
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
            //MessageBox.Show($"You have {Raumstempeln.Count} blocks to import");
        }

        public void ResetProperies()
        {
            //try to make this later https://www.codeproject.com/Articles/158591/Resetting-a-View-Model-in-WPF-MVVM-applications-wi

            CopyPropertiesFrom(_defaultViewModel);

        }

        // Hilfsmethode für viele Properties ANSCHAUEN
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
