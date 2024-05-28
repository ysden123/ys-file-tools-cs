using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using Microsoft.Win32;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace YSFileToolsCS
{
    /// <summary>
    /// Interaction logic for ShowOnMapControl.xaml
    /// </summary>
    public partial class ShowOnMapControl : UserControl
    {
        public ShowOnMapControl()
        {
            InitializeComponent();
        }

        private void ChooseFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Choose image file",
                Filter = "Image files (*.dng;*.jpg;*.jpeg;*.tif)|*.dng;*.jpg;*.jpeg;*.tif"
            };

            var result = openFileDialog.ShowDialog();
            if (result != null && result == true)
                this.FileText.Text = openFileDialog.FileName;
        }

        private void ShowOnMap_Click(object sender, RoutedEventArgs e)
        {
            if (FileText.Text.Length == 0)
                return;

            try
            {
                var gps = ImageMetadataReader.ReadMetadata(FileText.Text)
                             .OfType<GpsDirectory>()
                             .FirstOrDefault();
                if (gps == null)
                {
                    MessageBox.Show("File doesn't contain GPS info.");
                    return;
                }

                var location = gps.GetGeoLocation();
                if (location == null)
                {
                    MessageBox.Show("No location found");
                }
                else
                {
                    var url = $"https://www.google.com/maps/search/?api=1&query={location.Latitude.ToString()},{location.Longitude.ToString()}";
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
