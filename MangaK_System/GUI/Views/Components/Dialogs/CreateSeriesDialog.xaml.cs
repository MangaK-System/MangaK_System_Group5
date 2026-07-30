using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using MangaK_System.BLL.Category;
using MangaK_System.GUI.ViewModels;

namespace MangaK_System.GUI.Views.Components.Dialogs
{
    public class CategorySelectionItem : BaseViewModel
    {
        public string Name { get; set; } = string.Empty;
        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }

    public partial class CreateSeriesDialog : Window
    {
        public ObservableCollection<CategorySelectionItem> CategoryList { get; } = new();

        public string SeriesTitle => TitleTextBox.Text.Trim();
        public string SeriesDescription => DescriptionTextBox.Text.Trim();
        public string? LocalCoverPath => string.IsNullOrWhiteSpace(CoverFileTextBox.Text) ? null : CoverFileTextBox.Text.Trim();
        public string? LocalNameFilePath => string.IsNullOrWhiteSpace(NameFileTextBox.Text) ? null : NameFileTextBox.Text.Trim();

        public List<string> SelectedCategoryNames => CategoryList
            .Where(c => c.IsSelected)
            .Select(c => c.Name)
            .ToList();

        public CreateSeriesDialog()
        {
            InitializeComponent();
            CategoriesItemsControl.ItemsSource = CategoryList;

            _ = LoadCategoriesAsync();
        }

        private async Task LoadCategoriesAsync()
        {
            try
            {
                using var scope = App.ServiceProvider.CreateScope();
                var categoryService = scope.ServiceProvider.GetRequiredService<IService>();

                var categories = await categoryService.GetAllCategoriesAsync();

                CategoryList.Clear();
                foreach (var cat in categories)
                {
                    CategoryList.Add(new CategorySelectionItem { Name = cat.Name });
                }

                // Fallback default list if DB has no categories yet
                if (CategoryList.Count == 0)
                {
                    var defaultNames = new[] { "Action", "Adventure", "Comedy", "Drama", "Fantasy", "Romance", "Sci-Fi", "Slice of Life" };
                    foreach (var name in defaultNames)
                    {
                        CategoryList.Add(new CategorySelectionItem { Name = name });
                    }
                }
            }
            catch
            {
                var defaultNames = new[] { "Action", "Adventure", "Comedy", "Drama", "Fantasy", "Romance", "Sci-Fi", "Slice of Life" };
                CategoryList.Clear();
                foreach (var name in defaultNames)
                {
                    CategoryList.Add(new CategorySelectionItem { Name = name });
                }
            }
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
