using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.QuickTime;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

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
                Filter = "Image files (*.dng;*.jpg;*.jpeg;*.tif;*.mp4)|*.dng;*.jpg;*.jpeg;*.tif;*.mp4"
            };

            var result = openFileDialog.ShowDialog();
            if (result != null && result == true)
                this.FileText.Text = openFileDialog.FileName;
        }

        private void ShowOnMap_Click(object sender, RoutedEventArgs e)
        {
            if (FileText.Text.Length == 0)
                return;

            if (FileText.Text.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase))
            {
                ExtractGpsFromMP4(FileText.Text);
                return;
            }

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

                if (gps.TryGetGeoLocation(out GeoLocation location))
                {
                    var url = $"https://www.google.com/maps/search/?api=1&query={location.Latitude.ToString()},{location.Longitude.ToString()}";
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                else
                {
                    MessageBox.Show("No location found");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ExtractGpsFromMP4(string filePath)
        {
            try
            {
                // Open the MP4 file as a stream and parse it using the QuickTime reader
                using (var stream = File.OpenRead(filePath))
                {
                    var directories = QuickTimeMetadataReader.ReadMetadata(stream);

                    // QuickTime/MP4 GPS data is housed inside QuickTimeMetadataDirectory
                    var qtDirectory = directories.OfType<QuickTimeMetadataHeaderDirectory>().FirstOrDefault();

                    if (qtDirectory != null)
                    {
                        // Look for the standard location identifier tag
                        // Apple/Android typically use TagLocationRole or custom string identifiers like "+37.7749-122.4194/"
                        var locationTag = qtDirectory.Tags.FirstOrDefault(t => t.Name.Contains("Location") || t.Name.Contains("XYZ"));

                        if (locationTag != null && !string.IsNullOrEmpty(locationTag.Description))
                        {
                            MessageBox.Show($"Found GPS Metadata: {locationTag.Description}");

                            // Optional: Parse the ISO 6709 string format (+37.7749-122.4194/)
                            //todo: remove:ParseIso6709Coordinates(locationTag.Description);

                            var coordinates = locationTag.Description.Trim().TrimEnd('/').Split(new char[] { '+', '-' }, StringSplitOptions.RemoveEmptyEntries);
                            var url = $"http://maps.google.com/?q={coordinates[0]},{coordinates[1]}";
                            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                        }
                        else
                        {
                            MessageBox.Show("No static GPS location tag found in QuickTime metadata.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("No QuickTime metadata directory detected.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading video metadata: {ex.Message}");
            }
        }
    }
}
