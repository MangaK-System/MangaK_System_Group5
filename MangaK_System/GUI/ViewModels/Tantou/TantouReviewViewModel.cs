using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using MangaK_System.BLL.Series;
using MangaK_System.DAL.Entity.Enums;
using MangaK_System.GUI.Services;
using MangaK_System.GUI.Views.Components.Dialogs;

namespace MangaK_System.GUI.ViewModels.Tantou
{
    public class TantouReviewViewModel : BaseViewModel
    {
        private bool _isLoading;
        private string _statusMessage = string.Empty;

        public ObservableCollection<GetAllSeriesResponse> SeriesList { get; } = new();

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public ICommand RefreshCommand { get; }
        public ICommand ReviewSeriesCommand { get; }
        public ICommand ViewDetailsCommand { get; }

        public TantouReviewViewModel()
        {
            RefreshCommand = new RelayCommand(async _ => await LoadSeriesAsync());
            ReviewSeriesCommand = new RelayCommand(param => ExecuteReviewSeries(param as GetAllSeriesResponse));
            ViewDetailsCommand = new RelayCommand(param => ExecuteViewDetails(param as GetAllSeriesResponse));

            _ = LoadSeriesAsync();
        }

        private void ExecuteViewDetails(GetAllSeriesResponse? series)
        {
            if (series == null) return;
            var dialog = new SeriesDetailDialog(series)
            {
                Owner = Application.Current.MainWindow
            };
            dialog.ShowDialog();
        }

        public async Task LoadSeriesAsync()
        {
            if (UserSession.CurrentUser == null) return;

            IsLoading = true;
            StatusMessage = string.Empty;

            try
            {
                using var scope = App.ServiceProvider.CreateScope();
                var seriesService = scope.ServiceProvider.GetRequiredService<ISeriesService>();

                var list = await seriesService.GetAllSeriesAsync(UserSession.CurrentUser.Id);

                // Filter for Tantou review: status = Processing
                var pendingList = list.Where(s => s.Status == SeriesStatus.Processing).ToList();

                SeriesList.Clear();
                foreach (var item in pendingList)
                {
                    SeriesList.Add(item);
                }

                if (SeriesList.Count == 0)
                {
                    StatusMessage = "No series currently pending for Tantou review.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading series: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ExecuteReviewSeries(GetAllSeriesResponse? series)
        {
            if (series == null || UserSession.CurrentUser == null) return;

            var dialog = new ReviewNoteDialog(series.Title)
            {
                Owner = Application.Current.MainWindow
            };

            if (dialog.ShowDialog() == true)
            {
                _ = SubmitReviewAsync(series.SeriesId, dialog.IsApproved, dialog.Note);
            }
        }

        private async Task SubmitReviewAsync(Guid seriesId, bool isApproved, string note)
        {
            if (UserSession.CurrentUser == null) return;

            IsLoading = true;

            try
            {
                using var scope = App.ServiceProvider.CreateScope();
                var seriesService = scope.ServiceProvider.GetRequiredService<ISeriesService>();

                await seriesService.ReviewSeriesByTantouEditorAsync(
                    seriesId: seriesId,
                    editorId: UserSession.CurrentUser.Id,
                    isApproved: isApproved,
                    note: note
                );

                string resultText = isApproved
                    ? "Approved & Submitted to Editorial Board for final approval."
                    : "Rejected with feedback sent to author.";

                MessageBox.Show($"Review submitted successfully! {resultText}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                await LoadSeriesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to submit review: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
