using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace YSFileToolsCS
{
    /// <summary>
    /// Interaction logic for EmptyDirsControl.xaml
    /// </summary>
    public partial class EmptyDirsControl : UserControl
    {
        public EmptyDirsControl()
        {
            InitializeComponent();
        }

        private void ChooseDirButton_Click(object sender, RoutedEventArgs e)
        {
            dynamic dialog = new OpenFolderDialog();
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
                EmptyListText.Text = "";
                return;
            }

            EmptyListText.Text = "Waiting...";

            var enumerationOptions = new EnumerationOptions();
            enumerationOptions.RecurseSubdirectories = true;

            var directories = Directory.EnumerateDirectories(DirectoryText.Text, "*", enumerationOptions);
            EmptyListText.Text = "";

            foreach (var directory in directories)
            {
                if (!Directory.EnumerateFileSystemEntries(directory).Any())
                {
                    EmptyListText.Text += directory;
                    EmptyListText.Text += "\n";
                }
            }
        }
    }
}
