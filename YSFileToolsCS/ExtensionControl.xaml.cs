using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;

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

        private void FindButton_Click(object sender, RoutedEventArgs e)
        {
            if (DirectoryText.Text.Length == 0)
            {
                ExtensionListText.Text = "";
                return;
            }

            ExtensionListText.Text = "Wait...";

            var enumerationOptions = new EnumerationOptions();
            enumerationOptions.RecurseSubdirectories = true;

            var files = Directory.GetFiles(DirectoryText.Text, "*", enumerationOptions);
            var extensions = new Dictionary<string, int>();
            foreach (var file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
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

            ExtensionListText.Text = "";
            foreach (var extension in extensions.OrderBy(item => item.Key))
            {
                ExtensionListText.Text += $"{extension.Key.Replace(".", "")} - {extension.Value}\n";
            }
        }
    }
}
