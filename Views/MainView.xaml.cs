using ACADTools.ViewModels;
using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace ACADTools.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        //CheckBoxes Action
        private void chkHatchingGroup_Checked(object sender, RoutedEventArgs e)
        {
            bool isEnabled = uiHatchingGroup.IsEnabled;
            if (isEnabled == false)
                uiHatchingGroup.IsEnabled = true;
            else
                uiHatchingGroup.IsEnabled = false;
        }

        private void chkBlockGroup_Checked(object sender, RoutedEventArgs e)
        {
            bool isEnabled = uiBlockGroup.IsEnabled;
            if (isEnabled == false)
                uiBlockGroup.IsEnabled = true;
            else
                uiBlockGroup.IsEnabled = false;
        }

        //Save the csv file in the chosen directory
        private void SelectDirectoryBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV Datei|*.csv";
            saveFileDialog.Title = "Importdatei speichern unter";
            saveFileDialog.InitialDirectory = "C:\\";
            saveFileDialog.FileName = "Zeichnung1";
            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName != "")
            {
                FileStream fs = saveFileDialog.OpenFile() as FileStream;
                fs.Close();
            }

        }

    }
}
