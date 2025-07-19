using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace YSFileToolsCS
{
    /// <summary>
    /// Interaction logic for ExtensionControl.xaml
    /// </summary>
    public partial class ExtensionControl : UserControl
    {
        private readonly OpenFolderDialog dialog;
        public ExtensionControl()
        {
            InitializeComponent();
            dialog = new OpenFolderDialog();
        }

        private void ChooseDirButton_Click(object sender, RoutedEventArgs e)
        {
            dialog.Title = "Choose initial directory";
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                DirectoryText.Text = dialog.FolderName;
            }
        }

        private async void FindButton_Click(object sender, RoutedEventArgs e)
        {
            if (DirectoryText.Text.Length == 0)
            {
                ExtensionListText.Text = "";
                return;
            }

            ExtensionListText.Text = "Wait...";

            ExtensionListText.Text = "Waiting...";
            var currentCursor = ExtensionListText.Cursor;
            ExtensionListText.Cursor = Cursors.Wait;
            FindButton.IsEnabled = false;

            try
            {
                var extensions = await GetExtensions(DirectoryText.Text);
                if (extensions != null)
                {
                    ExtensionListText.Text = "";
                    foreach (var extension in extensions.OrderBy(item => item.Key))
                    {
                        ExtensionListText.Text += $"{extension.Key.Replace(".", "")} - {extension.Value}\n";
                    }
                }

                ExtensionListText.Text += "\nDone";
            }
            catch (Exception ex)
            {
                ExtensionListText.Text = $"Exception: {ex.Message}";
            }
            FindButton.IsEnabled = true;
            ExtensionListText.Cursor = currentCursor;
        }

        private static async Task<Dictionary<string, int>> GetExtensions(string directory)
        {
            var result = await Task.Run(() =>
            {
                var enumerationOptions = new EnumerationOptions
                {
                    RecurseSubdirectories = true
                };

                var files = Directory.GetFiles(directory, "*", enumerationOptions);
                var extensions = new Dictionary<string, int>();
                foreach (var file in files)
                {
                    FileInfo fileInfo = new(file);
                    var ext = fileInfo.Extension;

                    if (extensions.TryGetValue(ext, out int count))
                    {
                        extensions[ext] = count + 1;
                    }
                    else
                    {
                        extensions[ext] = 1;
                    }
                }
                return extensions;
            });
            return result;
        }
    }
}
