using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace YSFileToolsCS
{
    /// <summary>
    /// Interaction logic for KeyWordsControl.xaml
    /// </summary>
    public partial class KeyWordsControl : UserControl
    {
        public KeyWordsControl()
        {
            InitializeComponent();
        }

        private void ChooseFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Choose file with key words";
            openFileDialog.Filter = "txt files (*.txt)|*.txt";
            var properties = new AppProperties();
            string? fileName = properties.GetProperty(AppProperties.KEYWORDS_FILE);
            if (fileName != null)
            {
                openFileDialog.FileName = fileName;
            }
            var result = openFileDialog.ShowDialog();
            if (result != null && result == true)
            {
                this.FileText.Text = openFileDialog.FileName;
                properties.SetProperty(AppProperties.KEYWORDS_FILE, openFileDialog.FileName);
                properties.SaveProperties();
            }
        }

        private void Analyze_Click(object sender, RoutedEventArgs e)
        {
            this.StatisticsText.Text = string.Empty;
            this.DuplicatesText.Text = string.Empty;

            if (this.FileText.Text.Length == 0)
                return;

            var ka = new KeyAnalizer(this.FileText.Text);
            var allKeys = ka.ReadAllKeys();
            this.StatisticsText.Text += $"Max level={KeyAnalizer.MaxLevel(allKeys)}\n";
            this.StatisticsText.Text += $"Count of keys is {allKeys.Count}\n";

            foreach (var entry in KeyAnalizer.FindDuplicates(allKeys))
            {
                entry.Value.ForEach(keyWord =>
                {
                    if (keyWord.Parent != null)
                        this.DuplicatesText.Text += $"{keyWord.Key} in {keyWord.Parent.Key}\n";
                    else
                        this.DuplicatesText.Text += $"{keyWord.Key}\n";
                });
                this.DuplicatesText.Text += "\n";
            }
        }
    }
}
