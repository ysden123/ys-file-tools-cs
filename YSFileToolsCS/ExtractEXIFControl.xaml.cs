using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using Microsoft.Win32;
using Serilog;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace YSFileToolsCS
{
    /// <summary>
    /// Interaction logic for ExtractEXIFControl.xaml
    /// </summary>
    public partial class ExtractEXIFControl : UserControl
    {
        private static readonly ILogger _logger = Log.ForContext<ExtractEXIFControl>();
        public ExtractEXIFControl()
        {
            InitializeComponent();
        }

        private async void ExtractEXIFButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog dialog = new()
            {
                Title = "Choose initial directory",
                Multiselect = true
            };
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                var currentCursor = ExtractEXIFText.Cursor;
                ExtractEXIFText.Cursor = Cursors.Wait;
                ExtractEXIFButton.IsEnabled = false;

                string[]? folders = dialog.FolderNames;
                _logger.Debug($"Selected {folders.Length} folders.");
                var exifInfo = await ExtractExifDataAsync(folders);
                ExtractEXIFText.Text = exifInfo.ToString();

                ExtractEXIFText.Cursor = currentCursor;
                ExtractEXIFButton.IsEnabled = true;
            }
            else
            {
                ExtractEXIFText.Text = "No folder selected.\n";
            }
        }

        private static async Task<StringBuilder> ExtractExifDataAsync(string[]? folders)
        {
            return await Task.Run(() =>
            {
                StringBuilder exifData = new();
                if (folders == null || folders.Length == 0)
                {
                    _logger.Debug("No folders provided for EXIF extraction.");
                    return exifData;
                }
                foreach (string folder in folders)
                {
                    _logger.Debug($"Selected folder: {folder}");
                    foreach (string imagePath in System.IO.Directory.GetFiles(folder))
                    {
                        try
                        {
                            IEnumerable<MetadataExtractor.Directory> directories = ImageMetadataReader.ReadMetadata(imagePath);
                            var exitIfo = new ExifInfo();
                            foreach (var dir in directories.OfType<ExifSubIfdDirectory>())
                            {
                                foreach (var tag in dir.Tags)
                                {
                                    exitIfo.AddTag(tag.Name, tag.Description);
                                }
                            }

                            _logger.Debug($"Image: {imagePath}, EXIF Info: {exitIfo}");
                            exifData.Append($"Image: {imagePath}, EXIF Info: {exitIfo}\n");
                        }
                        catch (Exception ex)
                        {
                            _logger.Error($"Error reading metadata from image {imagePath}: {ex.Message}");
                        }
                    }
                }
                return exifData;
            });
        }
    }
}
