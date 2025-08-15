using ACADTools.Models.Importing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ACADTools.ViewModels
{
    public class DefaultViewModel : ViewModelBase
    {
        public DefaultViewModel()
        {
            SelectedLayer = null;
            SelectedLayerInfo = null;
            CheckedBlock = false;
            CheckedBlockAttributes = false;
            CheckedBlockInsertPoint = null;
            CheckedText = false;
            AreaSelectedCulture = null;
            DecimalPlaces = 2;
            HeaderCsv = true;
            DrawingNameCsv = true;
            AreaReferenceCsv = false;
            AreaInfoReferenceCsv = false;
            PolygonsCount = 0;
            //SelectedFilePathCsv = string.Empty; //mal schauen und besser implementieren
        }

        #region Collections
        public ObservableCollection<RaumpolygonEntity> Polygons { get; set; }
        public ObservableCollection<RaumstempelEntity> Raumstempeln { get; set; }
        public ObservableCollection<TextRaumstempelEntity> Texts { get; set; }
        public List<string> LayerNames { get; set; }
        #endregion

        #region Properies
        //Selected layer item (Flächen)
        private string _selectedLayer;
        public string SelectedLayer
        {
            get => _selectedLayer;
            set { _selectedLayer = value; OnPropertyChanged(); } //für abhängige Properties -> OnPropertyChanged(nameof(DependentProperty))
        }

        //Selected layer info (FlächenInfo)
        private string _selectedLayerInfo;
        public string SelectedLayerInfo
        {
            get => _selectedLayerInfo;
            set { _selectedLayerInfo = value; OnPropertyChanged(); }
        }

        //Block GroupBox
        private bool _checkedBlock;
        public bool CheckedBlock
        {
            get => _checkedBlock;
            set { _checkedBlock = value; OnPropertyChanged(); }
        }

        private string _checkedBlockInsertPoint;
        public string CheckedBlockInsertPoint
        {
            get => _checkedBlockInsertPoint;
            set { _checkedBlockInsertPoint = value; OnPropertyChanged(); }
        }

        public enum BlockInsertPointValues
        {
            Einfügepunkt,
            Mittelpunkt
        }

        public IEnumerable<BlockInsertPointValues> CheckedBlockInsertPointValues
        {
            get => Enum.GetValues(typeof(BlockInsertPointValues)).Cast<BlockInsertPointValues>();
        }

        private bool _checkedBlockAttributes;
        public bool CheckedBlockAttributes
        {
            get => _checkedBlockAttributes;
            set { _checkedBlockAttributes = value; OnPropertyChanged(); }
        }

        //Text GroupBox
        private bool _checkedText;
        public bool CheckedText
        {
            get => _checkedText;
            set { _checkedText = value; OnPropertyChanged(); }
        }

        //Flächengröße GroupBox
        private string _areaSelectedCulture;
        public string AreaSelectedCulture
        {
            get => _areaSelectedCulture;
            set { _areaSelectedCulture = value; OnPropertyChanged(); }
        }

        public enum CommonCulture
        {
            German,
            English
        }

        public IEnumerable<CommonCulture> CommonCultureValues
        {
            get => Enum.GetValues(typeof(CommonCulture)).Cast<CommonCulture>();
        }

        private int _decimalPlaces;
        public int DecimalPlaces
        {
            get => _decimalPlaces;
            set { _decimalPlaces = value; OnPropertyChanged(); }
        }

        //Settings for CSV file
        private bool _headerCsv;
        public bool HeaderCsv
        {
            get => _headerCsv;
            set { _headerCsv = value; OnPropertyChanged(); }
        }

        private bool _drawingNameCsv;
        public bool DrawingNameCsv
        {
            get => _drawingNameCsv;
            set { _drawingNameCsv = value; OnPropertyChanged(); }
        }

        private bool _areaSizeCsv;
        public bool AreaSizeCsv
        {
            get => _areaSizeCsv;
            set { _areaSizeCsv = value; OnPropertyChanged(); }
        }

        private bool _areaReferenceCsv;
        public bool AreaReferenceCsv
        {
            get => _areaReferenceCsv;
            set { _areaReferenceCsv = value; OnPropertyChanged(); }
        }

        private bool _areaInfoReferenceCsv;
        public bool AreaInfoReferenceCsv
        {
            get => _areaInfoReferenceCsv;
            set { _areaInfoReferenceCsv = value; OnPropertyChanged(); }
        }

        private bool _areaPerimeterCsv;
        public bool AreaPerimeterCsv
        {
            get => _areaPerimeterCsv;
            set { _areaPerimeterCsv = value; OnPropertyChanged(); }
        }

        private string _selectedFilePathCsv;
        public string SelectedFilePathCsv
        {
            get => _selectedFilePathCsv;
            set { _selectedFilePathCsv = value; OnPropertyChanged(); }
        }

        private string _currentDwgName;
        public string CurrentDwgName
        {
            get => _currentDwgName;
            set { _currentDwgName = value; OnPropertyChanged(); }
        }

        private int _polygonsCount;

        public int PolygonsCount
        {
            get => _polygonsCount;
            set { _polygonsCount = value; OnPropertyChanged(); }
        }
        #endregion
    }
}
