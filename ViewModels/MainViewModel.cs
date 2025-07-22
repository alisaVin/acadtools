using ACADTools.Commands;
using ACADTools.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace ACADTools.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ACADApplication _acadApplication;

        //Properties
        public List<string> LayerNames { get; set; }
        public ObservableCollection<BlockDefinitionEntity> Blocks { get; set; }
        public ObservableCollection<RaumstempelEntity> Raumstempeln { get; set; }

        /// <summary>
        /// Gets the Command object bound to the OK button.
        /// The button automatically disabled if CanExecute predicate returns false.
        /// </summary>
        public RelayCommand GetBlocksFromSelectedLayerCommand { get; }


        //Selected layer item
        private string _selectedLayer;
        public string SelectedLayer
        {
            get => _selectedLayer;
            set
            {
                _selectedLayer = value;
                OnPropertyChanged(nameof(SelectedLayer));
            }
        }

        //Selected layer info
        private string _selectedLayerInfo;

        public string SelectedLayerInfo
        {
            get { return _selectedLayerInfo; }
            set
            {
                _selectedLayerInfo = value;
                OnPropertyChanged(nameof(SelectedLayerInfo));
            }
        }

        public MainViewModel()
        {
            _acadApplication = new ACADApplication();
            LayerNames = _acadApplication.GetLayerNamesFromCAD();
            Blocks = new ObservableCollection<BlockDefinitionEntity>();
            Raumstempeln = new ObservableCollection<RaumstempelEntity>();

            //Crete the command once
            GetBlocksFromSelectedLayerCommand = new RelayCommand(o => LoadBlocksFromSelectedLayer(), o => true);
        }


        public void LoadBlocksFromSelectedLayer()
        {
            if (!string.IsNullOrEmpty(SelectedLayer))
            {
                var blocks = _acadApplication.GetBlocksFromLayer(SelectedLayer);
                Blocks.Clear();

                foreach (var block in blocks)
                {
                    Blocks.Add(block);
                }
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
