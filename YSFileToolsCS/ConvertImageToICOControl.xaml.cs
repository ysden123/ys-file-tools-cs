using ImageMagick;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace YSFileToolsCS
{
    /// <summary>
    /// Interaction logic for ConvertImageToICOControl.xaml
    /// </summary>
    public partial class ConvertImageToICOControl : UserControl
    {
        private readonly OpenFileDialog openFileDialog = new();
        private readonly OpenFolderDialog saveFolderDialog = new();
        private readonly AppProperties properties = new();

        public ConvertImageToICOControl()
        {
            InitializeComponent();
            string? sourceFile = properties.GetProperty(AppProperties.IMAGE_TO_CONVERT_FILE);
            if (sourceFile != null)
            {
                ImageFilePathTextBox.Text = sourceFile;
            }

            string? targetFolder = properties.GetProperty(AppProperties.TO_IMAGE_TO_CONVERT_PATH);
            if (targetFolder != null)
            {
                TargetFolderTextBox.Text = targetFolder;
            }
        }

        private void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            if (ImageFilePathTextBox.Text.Length == 0 ||
                TargetFolderTextBox.Text.Length == 0)
            {
                return;
            }

            try
            {
                using var image = new MagickImage(ImageFilePathTextBox.Text);
                uint[] sizes = { 16, 24, 32, 48, 64, 128, 256 };
                using var collection = new MagickImageCollection();
                foreach (var size in sizes)
                {
                    var resizedImage = (MagickImage)image.Clone();

                    resizedImage.Resize(size, size);
                    collection.Add(resizedImage);
                }

                collection.Write(TargetFolderTextBox.Text);

                properties.SetProperty(AppProperties.IMAGE_TO_CONVERT_FILE, ImageFilePathTextBox.Text);
                properties.SetProperty(AppProperties.TO_IMAGE_TO_CONVERT_PATH, TargetFolderTextBox.Text);
                properties.SaveProperties();

                MessageBox.Show("Image file converted successfully.", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error converting image file: " + ex.Message, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SelectSourceFileButton_Click(object sender, RoutedEventArgs e)
        {
            string? sourceFile = properties.GetProperty(AppProperties.IMAGE_TO_CONVERT_FILE);
            openFileDialog.Title = "Select Image File";
            openFileDialog.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.tiff|All Files|*.*";
            if (sourceFile != null)
            {
                openFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(sourceFile);
                openFileDialog.FileName = System.IO.Path.GetFileName(sourceFile);
            }
            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                ImageFilePathTextBox.Text = openFileDialog.FileName;
                properties.SetProperty(AppProperties.IMAGE_TO_CONVERT_FILE, ImageFilePathTextBox.Text);
                properties.SaveProperties();
            }
        }

        private void SelectTargetFolderButton_Click(object sender, RoutedEventArgs e)
        {
            string? targetFolder = properties.GetProperty(AppProperties.TO_IMAGE_TO_CONVERT_PATH);
            saveFolderDialog.Title = "Select Target folder";
            if (targetFolder != null)
            {
                saveFolderDialog.InitialDirectory = System.IO.Path.GetDirectoryName(targetFolder);
            }
            bool? result = saveFolderDialog.ShowDialog();
            if (result == true)
            {
                TargetFolderTextBox.Text = System.IO.Path.Combine(TargetFolderTextBox.Text,
                    System.IO.Path.GetFileName(ImageFilePathTextBox.Text).Replace(System.IO.Path.GetExtension(ImageFilePathTextBox.Text), ".ico"));
                properties.SetProperty(AppProperties.TO_IMAGE_TO_CONVERT_PATH, TargetFolderTextBox.Text);
                properties.SaveProperties();
            }
        }

        private void ImageFilePathTextBoxChanged(object sender, TextChangedEventArgs e)
        {
            ConvertButton.
                IsEnabled = ImageFilePathTextBox.Text.Length > 0 && TargetFolderTextBox.Text.Length > 0;
        }

        private void TargetFolderTextBoxChanged(object sender, TextChangedEventArgs e)
        {
            ConvertButton.
                IsEnabled = ImageFilePathTextBox.Text.Length > 0 && TargetFolderTextBox.Text.Length > 0;
        }
    }
}
