using Microsoft.Win32;
using Serilog;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace YSFileToolsCS
{
    /// <summary>
    /// Interaction logic for FindDuplicatesControl.xaml
    /// </summary>
    public partial class FindDuplicatesControl : UserControl
    {
        private static readonly ILogger _logger = Log.ForContext<FindDuplicatesControl>();

        /*private readonly OpenFolderDialog dialog;*/

        public FindDuplicatesControl()
        {
            InitializeComponent();
            var properties = new AppProperties();
            string? folder = properties.GetProperty(AppProperties.IMAGES_FOLDER);
            if (folder != null)
            {
                DirectoryText.Text = folder;
            }
        }

        private void ChooseDirButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog dialog = new()
            {
                Title = "Choose initial directory"
            };
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
                DuplicatesListText.Text = "";
                return;
            }

            DuplicatesListText.Text = "Finding duplicates... This might take a while.";
            var currentCursor = DuplicatesListText.Cursor;
            DuplicatesListText.Cursor = Cursors.Wait;
            FindButton.IsEnabled = false;

            try
            {
                var files = await GetDuplicatesList(DirectoryText.Text);
                if (files != null)
                {
                    DuplicatesListText.Text = "";

                    foreach (var file in files)
                    {
                        DuplicatesListText.Text += file;
                        DuplicatesListText.Text += "\n";
                    }
                }
                DuplicatesListText.Text += "\nDone";
            }
            catch (Exception ex)
            {
                DuplicatesListText.Text = $"Exception: {ex.Message}";
                _logger.Error($"Error finding duplicates: {ex.Message}");
            }

            DuplicatesListText.Cursor = currentCursor;
            FindButton.IsEnabled = true;
        }

        private static async Task<IEnumerable<string>> GetDuplicatesList(string directory)
        {
            return await Task.Run(() =>
            {
                EnumerationOptions enumerationOptions = new()
                {
                    RecurseSubdirectories = true
                };

                Dictionary<string, List<string>> filesByHash = [];
                var files = Directory.GetFiles(directory, "*", enumerationOptions);

                // Find candidates: files with equal size
                var candidates = new Dictionary<long, List<string>>();
                foreach (var file in files)
                {
                    var fileInfo = new FileInfo(file);
                    if (candidates.TryGetValue(fileInfo.Length, out var listOfFiles))
                    {
                        listOfFiles.Add(file);
                    }
                    else
                    {
                        candidates[fileInfo.Length] = [file];
                    }
                }

                var filesWithEqiulSize = (from candidate in candidates
                                          where candidate.Value.Count > 1
                                          select candidate.Value).ToList();

                foreach (var file4check in filesWithEqiulSize)
                {
                    try
                    {
                        foreach (var file in file4check)
                        {
                            var fileHash = CalculateMD5(file);
                            if (filesByHash.TryGetValue(fileHash, out var listOfFiles))
                            {
                                listOfFiles.Add(file);
                            }
                            else
                            {
                                filesByHash[fileHash] = [file];
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Ignore files that cannot be read
                        _logger.Error($"Error processing files for duplicate check: {ex.Message}");
                    }
                }

                List<string> duplicatesFileList = [];
                foreach (var hashWithFiles in filesByHash)
                {
                    var filesduplicatedFiles = hashWithFiles.Value;
                    if (filesduplicatedFiles.Count > 1)
                    {
                        duplicatesFileList.AddRange(filesduplicatedFiles);
                        duplicatesFileList.Add("");
                    }
                }

                return duplicatesFileList;
            });
        }

        /// <summary>
        /// Computes the MD5 hash of the contents of the specified file and returns it as a hexadecimal string.
        /// </summary>
        /// <param name="filePath">The path to the file whose MD5 hash is to be calculated. Must refer to an existing file.</param>
        /// <returns>A string containing the MD5 hash of the file's contents, represented as a 32-character lowercase hexadecimal
        /// value.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the file specified by <paramref name="filePath"/> does not exist.</exception>
        public static string CalculateMD5(string filePath)
        {
            if (!File.Exists(filePath))
            {
                _logger.Error($"No file found for path: {filePath}");
                throw new FileNotFoundException($"No file found for path: {filePath}");
            }

            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    byte[] hashBytes = md5.ComputeHash(stream);

                    var sb = new StringBuilder();
                    foreach (byte b in hashBytes)
                    {
                        sb.Append(b.ToString("x2"));
                    }

                    return sb.ToString();
                }
            }
        }

        private void OnDirectoryTextChanged(object sender, TextChangedEventArgs e)
        {
            FindButton.IsEnabled = DirectoryText.Text.Length > 0;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            DirectoryText.Focus();
            Keyboard.Focus(DirectoryText);
        }
    }
}
