using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace YSFileToolsCS
{
    /// <summary>
    /// Interaction logic for EmptyDirsControl.xaml
    /// </summary>
    public partial class EmptyDirsControl : UserControl
    {
        private readonly OpenFolderDialog dialog;
        public EmptyDirsControl()
        {
            InitializeComponent();
            dialog = new OpenFolderDialog();
        }

        private void ChooseDirButton_Click(object sender, RoutedEventArgs e)
        {
            dialog.Title = "Choose initial directory";
            var properties = new AppProperties();
            string? folder = properties.GetProperty(AppProperties.IMAGES_FOLDER);
            if (folder != null)
            {
                dialog.InitialDirectory = folder;
            }
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                DirectoryText.Text = dialog.FolderName;
                properties.SetProperty(AppProperties.IMAGES_FOLDER, dialog.FolderName);
                properties.SaveProperties();
            }
        }

        private async void FindButton_Click(object sender, RoutedEventArgs e)
        {
            if (DirectoryText.Text.Length == 0)
            {
                EmptyListText.Text = "";
                return;
            }

            EmptyListText.Text = "Waiting...";
            var currentCursor = EmptyListText.Cursor;
            EmptyListText.Cursor = Cursors.Wait;
            FindButton.IsEnabled = false;

            try
            {
                var directories = await GetEmptyList(DirectoryText.Text);
                if (directories != null)
                {
                    EmptyListText.Text = "";

                    foreach (var directory in directories)
                    {
                        EmptyListText.Text += directory;
                        EmptyListText.Text += "\n";
                    }
                }
                EmptyListText.Text += "\nDone";
            }
            catch (Exception ex)
            {
                EmptyListText.Text = $"Exception: {ex.Message}";
            }

            EmptyListText.Cursor = currentCursor;
            FindButton.IsEnabled = true;
        }

        private static async Task<IEnumerable<string>> GetEmptyList(string directory)
        {
            return await Task.Run(() =>
            {
                EnumerationOptions enumerationOptions = new()
                {
                    RecurseSubdirectories = true
                };
                var directories = Directory.EnumerateDirectories(directory, "*", enumerationOptions);
                var emptyDirList = new List<string>();
                foreach (var directory in directories)
                {
                    if (!Directory.EnumerateFileSystemEntries(directory).Any())
                    {
                        emptyDirList.Add(directory);
                    }
                }
                return emptyDirList;
            });
        }
    }
}
