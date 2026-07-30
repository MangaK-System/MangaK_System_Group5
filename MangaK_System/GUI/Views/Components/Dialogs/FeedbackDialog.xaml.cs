using System;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using MangaK_System.BLL.Feedback;
using MangaK_System.GUI.Services;

namespace MangaK_System.GUI.Views.Components.Dialogs
{
    public partial class FeedbackDialog : Window
    {
        private readonly Guid _seriesId;

        public FeedbackDialog(Guid seriesId, string seriesTitle)
        {
            InitializeComponent();
            _seriesId = seriesId;
            SeriesTitleTextBlock.Text = $"Feedback History: {seriesTitle}";

            _ = LoadFeedbacksAsync();
        }

        private async Task LoadFeedbacksAsync()
        {
            if (UserSession.CurrentUser == null) return;

            StatusTextBlock.Text = "Loading feedback history...";
            StatusTextBlock.Visibility = Visibility.Visible;

            try
            {
                using var scope = App.ServiceProvider.CreateScope();
                var feedbackService = scope.ServiceProvider.GetRequiredService<IFeedbackService>();

                var list = await feedbackService.GetFeedbackListAsync(_seriesId, UserSession.CurrentUser.Id);

                FeedbackListView.ItemsSource = list;

                if (list.Count == 0)
                {
                    StatusTextBlock.Text = "No feedback history for this series yet.";
                    StatusTextBlock.Visibility = Visibility.Visible;
                }
                else
                {
                    StatusTextBlock.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"Error loading feedbacks: {ex.Message}";
                StatusTextBlock.Visibility = Visibility.Visible;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
