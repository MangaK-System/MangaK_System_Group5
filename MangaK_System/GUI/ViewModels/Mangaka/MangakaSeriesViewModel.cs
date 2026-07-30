using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using MangaK_System.BLL.MediaService;
using MangaK_System.BLL.Series;
using MangaK_System.GUI.Services;
using MangaK_System.GUI.Views.Components.Dialogs;

namespace MangaK_System.GUI.ViewModels.Mangaka
{
    public class MangakaSeriesViewModel : BaseViewModel
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
        public ICommand OpenCreateSeriesCommand { get; }
        public ICommand ViewDetailsCommand { get; }

        public MangakaSeriesViewModel()
        {
            RefreshCommand = new RelayCommand(async _ => await LoadSeriesAsync());
            OpenCreateSeriesCommand = new RelayCommand(_ => ExecuteOpenCreateSeries());
            ViewDetailsCommand = new RelayCommand(param => ExecuteViewDetails(param as GetAllSeriesResponse));

            // Load data when initialized
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

                SeriesList.Clear();
                foreach (var item in list)
                {
                    SeriesList.Add(item);
                }

                if (SeriesList.Count == 0)
                {
                    StatusMessage = "No series created yet. Click 'Create Series' to start!";
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

        private void ExecuteOpenCreateSeries()
        {
            if (UserSession.CurrentUser == null) return;

            var dialog = new CreateSeriesDialog
            {
                Owner = Application.Current.MainWindow
            };

            if (dialog.ShowDialog() == true)
            {
                _ = CreateSeriesAsync(dialog.SeriesTitle, dialog.SeriesDescription, dialog.LocalCoverPath, dialog.LocalNameFilePath);
            }
        }

        private async Task CreateSeriesAsync(string title, string description, string? localCoverPath, string? localNameFilePath)
        {
            if (UserSession.CurrentUser == null) return;

            IsLoading = true;

            try
            {
                using var scope = App.ServiceProvider.CreateScope();
                var seriesService = scope.ServiceProvider.GetRequiredService<ISeriesService>();
                var mediaService = scope.ServiceProvider.GetRequiredService<IService>();

                string? coverUrl = null;
                string? nameFileUrl = null;
                string? nameFilePublicId = null;

                // 1. Upload Cover Image sang Cloudinary nếu người dùng đã chọn file
                if (!string.IsNullOrWhiteSpace(localCoverPath))
                {
                    coverUrl = await mediaService.UploadImageAsync(localCoverPath);
                }

                // 2. Upload Name/Manuscript file sang Cloudinary nếu có
                if (!string.IsNullOrWhiteSpace(localNameFilePath))
                {
                    var result = await mediaService.UploadFileAsync(localNameFilePath);
                    nameFileUrl = result.FileUrl;
                    nameFilePublicId = result.PublicId;
                }

                // 3. Tạo Series trong DB với URL file đã upload
                await seriesService.CreateSeriesAsync(
                    title: title,
                    description: description,
                    coverFile: coverUrl,
                    nameFile: nameFileUrl,
                    nameFilePublicId: nameFilePublicId,
                    createdById: UserSession.CurrentUser.Id
                );

                MessageBox.Show($"Series '{title}' created and uploaded successfully! Submitted to Tantou Editor for review.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                await LoadSeriesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to create series: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
