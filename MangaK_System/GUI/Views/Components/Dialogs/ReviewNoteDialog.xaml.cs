using System.Windows;

namespace MangaK_System.GUI.Views.Components.Dialogs
{
    public partial class ReviewNoteDialog : Window
    {
        public bool IsApproved { get; private set; }
        public string Note => NoteTextBox.Text.Trim();

        public ReviewNoteDialog(string seriesTitle)
        {
            InitializeComponent();
            SeriesTitleTextBlock.Text = $"Review: {seriesTitle}";
        }

        private void ApproveButton_Click(object sender, RoutedEventArgs e)
        {
            IsApproved = true;
            DialogResult = true;
            Close();
        }

        private void RejectButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Note))
            {
                ErrorMessageTextBlock.Text = "Feedback note is required when rejecting a series.";
                ErrorMessageTextBlock.Visibility = Visibility.Visible;
                return;
            }

            IsApproved = false;
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
