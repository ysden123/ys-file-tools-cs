using Serilog;
using System.IO;
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
            var folder = YSCommon.Utils.GetAssemblyFolderInLocalData("ysfiletoolscs");
#if DEBUG
            string fileName = Path.Combine(folder, "logs", "ysfiletoolscs-debug.log");
            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .Enrich.WithThreadId()
               .WriteTo.File(fileName,
               rollingInterval: RollingInterval.Month,
               outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {SourceContext} [{ThreadId}] {Message:lj}{NewLine}{Exception}")
           .CreateLogger();
#else
            string fileName = Path.Combine(folder, "logs", "ysfiletoolscs.log");
            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Error()
               .Enrich.WithThreadId()
               .WriteTo.File(fileName,
               rollingInterval: RollingInterval.Month,
               outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {SourceContext} [{ThreadId}] {Message:lj}{NewLine}{Exception}")
           .CreateLogger();
#endif
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

        private void MenuItem_ConvertImageToICO_Click(object sender, RoutedEventArgs e)
        {
            CC.Content = new ConvertImageToICOControl();
        }

        private void MenuItem_FindDuplicates_Click(object sender, RoutedEventArgs e)
        {
            CC.Content = new FindDuplicatesControl();
        }

        private void MenuItem_ExtractEXIF_Click(object sender, RoutedEventArgs e)
        {
            CC.Content = new ExtractEXIFControl();
        }
    }
}