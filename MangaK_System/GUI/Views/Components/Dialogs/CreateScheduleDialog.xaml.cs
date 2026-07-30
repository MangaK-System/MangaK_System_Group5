using System;
using System.Windows;
using System.Windows.Controls;

namespace MangaK_System.GUI.Views.Components.Dialogs
{
    public partial class CreateScheduleDialog : Window
    {
        public DateTime SelectedPublishDate => PublishDatePicker.SelectedDate ?? DateTime.UtcNow.AddDays(8);
        public string SelectedPeriod => (PeriodComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Weekly";

        public CreateScheduleDialog(string seriesTitle)
        {
            InitializeComponent();
            SeriesTitleTextBlock.Text = $"Schedule: {seriesTitle}";
            PublishDatePicker.SelectedDate = DateTime.UtcNow.AddDays(8); // Default to 8 days in future
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (PublishDatePicker.SelectedDate == null)
            {
                ErrorMessageTextBlock.Text = "Please select a valid publish date.";
                ErrorMessageTextBlock.Visibility = Visibility.Visible;
                return;
            }

            if (PublishDatePicker.SelectedDate <= DateTime.UtcNow)
            {
                ErrorMessageTextBlock.Text = "Publish date must be in the future.";
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
