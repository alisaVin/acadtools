using ACADTools.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ACADTools.ViewModels
{
    public class DefaultViewModel : ViewModelBase
    {
        public DefaultViewModel()
        {
            SelectedLayer = null;
            SelectedLayerInfo = null;
            CheckedHatching = false;
            CheckedBlock = false;
            CheckedBlockAttributes = false;
            CheckedText = false;
            RoomNumberPrefixedText = "Raum";
            StartRoomNumber = 1;
            RoomNumberSubstitutedText = string.Empty;
            AreaPrefixedText = string.Empty;
            AreaSubstitutedText = string.Empty;
            AreaFactor = 1;
            AreaDecimalPlaces = 2;
            AreaSelectedCulture = string.Empty;
            RoomNumberCsv = true;
            AreaSizeCsv = true;
            AreaSizeExplandedCsv = false;
            DrawingNameCsv = true;
            AreaReferenceCsv = false;
            HatchingReferenceCsv = false;
            AreaInfoReferenceCsv = false;
            HeaderCsv = true;
        }

        #region Collections
        public ObservableCollection<RaumpolygonEntity> Polygons { get; set; }
        public ObservableCollection<RaumstempelEntity> Raumstempeln { get; set; }
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

        //Hatching GroupBox
        private bool _checkedHatching;
        public bool CheckedHatching
        {
            get => _checkedHatching;
            set { _checkedHatching = value; OnPropertyChanged(); }
        }

        //Block GroupBox
        private bool _checkedBlock;
        public bool CheckedBlock
        {
            get => _checkedBlock;
            set { _checkedBlock = value; OnPropertyChanged(); }
        }

        private bool _checkedBlockAttributes;
        public bool CheckedBlockAttributes
        {
            get { return _checkedBlockAttributes; }
            set { _checkedBlockAttributes = value; OnPropertyChanged(); }
        }

        //Text GroupBox
        private bool _checkedText;

        public bool CheckedText
        {
            get { return _checkedText; }
            set { _checkedText = value; OnPropertyChanged(); }
        }

        //Generation of room numbers GroupBox
        private string _roomNumberPrefixedText;
        public string RoomNumberPrefixedText
        {
            get { return _roomNumberPrefixedText; }
            set { _roomNumberPrefixedText = value; OnPropertyChanged(); }
        }

        private int _startRoomNumber;
        public int StartRoomNumber
        {
            get { return _startRoomNumber; }
            set { _startRoomNumber = value; OnPropertyChanged(); }
        }

        private string _roomNumberSubstitutedText;
        public string RoomNumberSubstitutedText
        {
            get { return _roomNumberSubstitutedText; }
            set { _roomNumberSubstitutedText = value; OnPropertyChanged(); }
        }

        //Area size GroupBox
        private string _areaPrefixedText;
        public string AreaPrefixedText
        {
            get { return _areaPrefixedText; }
            set { _areaPrefixedText = value; OnPropertyChanged(); }
        }

        private string _areaSubstitutedText;
        public string AreaSubstitutedText
        {
            get { return _areaSubstitutedText; }
            set { _areaSubstitutedText = value; OnPropertyChanged(); }
        }

        private int _areaFactor;
        public int AreaFactor
        {
            get { return _areaFactor; }
            set { _areaFactor = value; OnPropertyChanged(); }
        }

        private int _areaDecimalPlaces;

        public int AreaDecimalPlaces
        {
            get { return _areaDecimalPlaces; }
            set { _areaDecimalPlaces = value; OnPropertyChanged(); }
        }

        private string _areaSelectedCulture;

        public string AreaSelectedCulture
        {
            get { return _areaSelectedCulture; }
            set { _areaSelectedCulture = value; OnPropertyChanged(); }
        }

        //Settings for CSV file
        private bool _roomNumberCsv;
        public bool RoomNumberCsv
        {
            get { return _roomNumberCsv; }
            set { _roomNumberCsv = value; OnPropertyChanged(); }
        }

        private bool _areaSizeCsv;
        public bool AreaSizeCsv
        {
            get { return _areaSizeCsv; }
            set { _areaSizeCsv = value; OnPropertyChanged(); }
        }

        private bool _areaSizeExplandedCsv;
        public bool AreaSizeExplandedCsv
        {
            get { return _areaSizeExplandedCsv; }
            set { _areaSizeExplandedCsv = value; OnPropertyChanged(); }
        }

        private bool _drawingNameCsv;
        public bool DrawingNameCsv
        {
            get { return _drawingNameCsv; }
            set { _drawingNameCsv = value; OnPropertyChanged(); }
        }

        private bool _areaReferenceCsv;
        public bool AreaReferenceCsv
        {
            get { return _areaReferenceCsv; }
            set { _areaReferenceCsv = value; OnPropertyChanged(); }
        }

        private bool _hatchingReferenceCsv;
        public bool HatchingReferenceCsv
        {
            get { return _hatchingReferenceCsv; }
            set { _hatchingReferenceCsv = value; OnPropertyChanged(); }
        }

        private bool _areaInfoReferenceCsv;
        public bool AreaInfoReferenceCsv
        {
            get { return _areaInfoReferenceCsv; }
            set { _areaInfoReferenceCsv = value; OnPropertyChanged(); }
        }

        private bool _headerCsv;
        public bool HeaderCsv
        {
            get { return _headerCsv; }
            set { _headerCsv = value; OnPropertyChanged(); }
        }
        #endregion

    }
}
