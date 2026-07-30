using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MangaK_System.BLL.Series;

namespace MangaK_System.GUI.Views.Components.Dialogs
{
    public partial class SeriesDetailDialog : Window
    {
        private readonly Guid _seriesId;
        private readonly string _seriesTitle;
        private readonly string? _nameFileUrl;

        public SeriesDetailDialog(GetAllSeriesResponse series)
        {
            InitializeComponent();

            _seriesId = series.SeriesId;
            _seriesTitle = series.Title;
            TitleTextBlock.Text = series.Title;
            AuthorAndDateTextBlock.Text = $"Author: {series.MangakaName}  •  Created: {series.CreateAt:dd/MM/yyyy HH:mm}";
            DescriptionTextBlock.Text = string.IsNullOrWhiteSpace(series.Description) ? "No description provided." : series.Description;
            StatusTextBlock.Text = series.Status.ToString();

            // Style Status Badge
            SetStatusStyle(series.Status.ToString());

            // Categories
            if (series.Categories != null && series.Categories.Count > 0)
            {
                CategoriesTextBlock.Text = string.Join(", ", series.Categories);
                CategoriesTextBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#374151"));
            }
            else
            {
                CategoriesTextBlock.Text = "(No category selected)";
                CategoriesTextBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9CA3AF"));
            }

            // Cover Image
            if (!string.IsNullOrWhiteSpace(series.CoverFile))
            {
                try
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(series.CoverFile, UriKind.RelativeOrAbsolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    CoverImage.Source = bitmap;
                }
                catch
                {
                    CoverFallbackTextBlock.Visibility = Visibility.Visible;
                }
            }
            else
            {
                CoverFallbackTextBlock.Visibility = Visibility.Visible;
            }

            // NameFile (Manuscript)
            _nameFileUrl = series.NameFile;
            if (!string.IsNullOrWhiteSpace(_nameFileUrl))
            {
                DownloadFileButton.Visibility = Visibility.Visible;
                NoFileTextBlock.Visibility = Visibility.Collapsed;
            }
            else
            {
                DownloadFileButton.Visibility = Visibility.Collapsed;
                NoFileTextBlock.Visibility = Visibility.Visible;
            }
        }

        private void SetStatusStyle(string status)
        {
            switch (status)
            {
                case "Processing":
                    StatusBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FEF3C7"));
                    StatusTextBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D97706"));
                    break;
                case "Pending":
                    StatusBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DBEAFE"));
                    StatusTextBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2563EB"));
                    break;
                case "Approved":
                    StatusBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DCFCE7"));
                    StatusTextBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#16A34A"));
                    break;
                case "Scheduled":
                    StatusBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F3E8FF"));
                    StatusTextBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9333EA"));
                    break;
                case "Rejected":
                    StatusBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FEE2E2"));
                    StatusTextBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DC2626"));
                    break;
                default:
                    StatusBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F3F4F6"));
                    StatusTextBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#374151"));
                    break;
            }
        }

        private void DownloadFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_nameFileUrl)) return;

            try
            {
                // Mở URL hoặc File Path mặc định bằng trình duyệt / ứng dụng của hệ thống
                Process.Start(new ProcessStartInfo(_nameFileUrl) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not open file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FeedbackButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FeedbackDialog(_seriesId, _seriesTitle)
            {
                Owner = this
            };
            dialog.ShowDialog();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
