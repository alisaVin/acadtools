using ACADTools.ViewModels;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
            MainViewModel vm = new MainViewModel();
            DataContext = vm;
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

        private void ResetControlsButton_Click(object sender, RoutedEventArgs e)
        {
            ResetControlsInputs(this);
        }

        private void ResetControlsInputs(DependencyObject parent)
        {
            int childCount = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < childCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);

                switch (child)
                {
                    case TextBox textBox:
                        textBox.Text = string.Empty;
                        break;

                    case ComboBox comboBox:
                        comboBox.SelectedIndex = -1;
                        comboBox.SelectedItem = null;
                        break;

                    case CheckBox checkBox:
                        checkBox.IsChecked = false;
                        break;
                }

                ResetControlsInputs(child);
            }
        }
    }
}
