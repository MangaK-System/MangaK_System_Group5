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

namespace MangaK_System.GUI.ViewModels.Editorial
{
    public class EditorialApprovalViewModel : BaseViewModel
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
        public ICommand ApproveSeriesCommand { get; }
        public ICommand ViewDetailsCommand { get; }

        public EditorialApprovalViewModel()
        {
            RefreshCommand = new RelayCommand(async _ => await LoadSeriesAsync());
            ApproveSeriesCommand = new RelayCommand(param => ExecuteReviewSeries(param as GetAllSeriesResponse));
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

                // Filter for Editorial approval: status = Pending
                var pendingList = list.Where(s => s.Status == SeriesStatus.Pending).ToList();

                SeriesList.Clear();
                foreach (var item in pendingList)
                {
                    SeriesList.Add(item);
                }

                if (SeriesList.Count == 0)
                {
                    StatusMessage = "No series currently pending for Editorial Board approval.";
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
                _ = SubmitEditorialReviewAsync(series.SeriesId, dialog.IsApproved, dialog.Note);
            }
        }

        private async Task SubmitEditorialReviewAsync(Guid seriesId, bool isApproved, string note)
        {
            if (UserSession.CurrentUser == null) return;

            IsLoading = true;

            try
            {
                using var scope = App.ServiceProvider.CreateScope();
                var seriesService = scope.ServiceProvider.GetRequiredService<ISeriesService>();

                await seriesService.ReviewSeriesByEditorialBoardAsync(
                    seriesId: seriesId,
                    boardId: UserSession.CurrentUser.Id,
                    isApproved: isApproved,
                    note: note
                );

                string resultText = isApproved
                    ? "Approved! This series can now be scheduled for publishing."
                    : "Rejected.";

                MessageBox.Show($"Editorial review completed! {resultText}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

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
