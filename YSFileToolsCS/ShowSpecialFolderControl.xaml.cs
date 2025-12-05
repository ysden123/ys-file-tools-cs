using System.Windows;
using System.Windows.Controls;

namespace YSFileToolsCS
{
    /// <summary>
    /// Interaction logic for ShowSpecialFolderControl1.xaml
    /// </summary>
    public partial class ShowSpecialFolderControl : UserControl
    {
        public ShowSpecialFolderControl()
        {
            InitializeComponent();


            FolderList.Items.Add("Select folder");

            var names = Enum.GetNames(typeof(Environment.SpecialFolder)).ToList<string>();
            names.Sort();


            var values = new List<Environment.SpecialFolder>();
            foreach (var name in names)
            {
                Enum.TryParse<Environment.SpecialFolder>(name, out var result);
                values.Add(result);
            }

            foreach (var item in values)
            {
                FolderList.Items.Add(item);
            }
        }

        private void FolderList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Environment.SpecialFolder selectedItem = (Environment.SpecialFolder)FolderList.SelectedItem;
                FolderText.Text = Environment.GetFolderPath(selectedItem);
            }
            catch (Exception)
            {
                FolderText.Text = string.Empty;
            }
        }

        private void CopyToClipBoardButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (FolderText.Text.Length > 0)
            {
                Clipboard.SetText(FolderText.Text);
            }
        }
    }
}
