using System.Windows;
using Microsoft.Win32;

namespace MangaK_System.GUI.Views.Components.Dialogs
{
    public partial class CreateSeriesDialog : Window
    {
        public string SeriesTitle => TitleTextBox.Text.Trim();
        public string SeriesDescription => DescriptionTextBox.Text.Trim();
        public string? LocalCoverPath => string.IsNullOrWhiteSpace(CoverFileTextBox.Text) ? null : CoverFileTextBox.Text.Trim();
        public string? LocalNameFilePath => string.IsNullOrWhiteSpace(NameFileTextBox.Text) ? null : NameFileTextBox.Text.Trim();

        public CreateSeriesDialog()
        {
            InitializeComponent();
        }

        private void BrowseCover_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Select Cover Image",
                Filter = "Image Files (*.jpg;*.jpeg;*.png;*.webp;*.gif)|*.jpg;*.jpeg;*.png;*.webp;*.gif|All Files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                CoverFileTextBox.Text = dialog.FileName;
            }
        }

        private void BrowseNameFile_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Select Manuscript / Name File",
                Filter = "Documents (*.pdf;*.zip;*.rar;*.docx)|*.pdf;*.zip;*.rar;*.docx|All Files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                NameFileTextBox.Text = dialog.FileName;
            }
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SeriesTitle))
            {
                ErrorMessageTextBlock.Text = "Please enter a series title.";
                ErrorMessageTextBlock.Visibility = Visibility.Visible;
                return;
            }

            if (string.IsNullOrWhiteSpace(SeriesDescription))
            {
                ErrorMessageTextBlock.Text = "Please enter a series description.";
                ErrorMessageTextBlock.Visibility = Visibility.Visible;
                return;
            }

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
