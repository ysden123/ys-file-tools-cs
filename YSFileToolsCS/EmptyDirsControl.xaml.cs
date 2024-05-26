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
        private readonly OpenFolderDialog dialog;
        private readonly EnumerationOptions enumerationOptions;
        public EmptyDirsControl()
        {
            InitializeComponent();
            dialog = new OpenFolderDialog();
            enumerationOptions = new EnumerationOptions();
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
                EmptyListText.Text = "";
                return;
            }

            EmptyListText.Text = "Waiting...";

            
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
