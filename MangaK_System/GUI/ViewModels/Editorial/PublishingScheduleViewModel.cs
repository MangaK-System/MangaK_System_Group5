using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using MangaK_System.BLL.PublishingSchedule;
using MangaK_System.BLL.Series;
using MangaK_System.DAL.Entity.Enums;
using MangaK_System.GUI.Services;
using MangaK_System.GUI.Views.Components.Dialogs;

namespace MangaK_System.GUI.ViewModels.Editorial
{
    public class PublishingScheduleViewModel : BaseViewModel
    {
        private bool _isLoading;
        private string _statusMessage = string.Empty;

        public ObservableCollection<GetPublishingScheduleResponse> ScheduleList { get; } = new();
        public ObservableCollection<GetAllSeriesResponse> ApprovedSeriesList { get; } = new();

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
        public ICommand ScheduleSeriesCommand { get; }

        public PublishingScheduleViewModel()
        {
            RefreshCommand = new RelayCommand(async _ => await LoadDataAsync());
            ScheduleSeriesCommand = new RelayCommand(param => ExecuteScheduleSeries(param as GetAllSeriesResponse));

            _ = LoadDataAsync();
        }

        public async Task LoadDataAsync()
        {
            if (UserSession.CurrentUser == null) return;

            IsLoading = true;
            StatusMessage = string.Empty;

            try
            {
                using var scope = App.ServiceProvider.CreateScope();
                var scheduleService = scope.ServiceProvider.GetRequiredService<IPublishingScheduleService>();
                var seriesService = scope.ServiceProvider.GetRequiredService<ISeriesService>();

                // Load existing schedules
                var schedules = await scheduleService.GetAllPublishingSchedulesAsync(UserSession.CurrentUser.Id);
                ScheduleList.Clear();
                foreach (var item in schedules)
                {
                    ScheduleList.Add(item);
                }

                // Load approved series waiting to be scheduled
                var allSeries = await seriesService.GetAllSeriesAsync(UserSession.CurrentUser.Id);
                var approved = allSeries.Where(s => s.Status == SeriesStatus.Approved).ToList();

                ApprovedSeriesList.Clear();
                foreach (var item in approved)
                {
                    ApprovedSeriesList.Add(item);
                }

                if (ScheduleList.Count == 0 && ApprovedSeriesList.Count == 0)
                {
                    StatusMessage = "No publishing schedules or approved series found.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading data: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ExecuteScheduleSeries(GetAllSeriesResponse? series)
        {
            if (series == null || UserSession.CurrentUser == null) return;

            var dialog = new CreateScheduleDialog(series.Title)
            {
                Owner = Application.Current.MainWindow
            };

            if (dialog.ShowDialog() == true)
            {
                _ = SubmitScheduleAsync(series.SeriesId, dialog.SelectedPublishDate, dialog.SelectedPeriod);
            }
        }

        private async Task SubmitScheduleAsync(Guid seriesId, DateTime publishDate, string publishPeriod)
        {
            if (UserSession.CurrentUser == null) return;

            IsLoading = true;

            try
            {
                using var scope = App.ServiceProvider.CreateScope();
                var scheduleService = scope.ServiceProvider.GetRequiredService<IPublishingScheduleService>();

                await scheduleService.CreatePublishingScheduleAsync(
                    seriesId: seriesId,
                    publishDate: publishDate,
                    publishPeriod: publishPeriod,
                    decidedById: UserSession.CurrentUser.Id
                );

                MessageBox.Show($"Publishing schedule created successfully! Series is now Scheduled.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to set schedule: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
