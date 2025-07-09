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
        }

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
    }
}
