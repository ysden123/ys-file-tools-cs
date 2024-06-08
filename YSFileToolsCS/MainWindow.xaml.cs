using System.Windows;

namespace YSFileToolsCS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            if (fvi != null && fvi.FileVersion != null)
            {
                string version = fvi.FileVersion;
                Title = $"{Title} {version}";
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MenuItem_EmptyDirs_Click(object sender, RoutedEventArgs e)
        {
            CC.Content = new EmptyDirsControl();
        }

        private void MenuItem_Extensions_Click(object sender, RoutedEventArgs e)
        {
            CC.Content = new ExtensionControl();
        }

        private void MenuItem_AnalyzeKeyWords_Click(object sender, RoutedEventArgs e)
        {
            CC.Content = new KeyWordsControl();
        }

        private void MenuItem_ShowOnMap_Click(object sender, RoutedEventArgs e)
        {
            CC.Content = new ShowOnMapControl();
        }

        private void MenuItem_ShowSpecialFolders_Click(object sender, RoutedEventArgs e)
        {
            CC.Content = new ShowSpecialFolderControl();
        }
    }
}