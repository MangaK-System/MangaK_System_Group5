using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;
using Mangak_System._1_DAL.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// Khái báo using ở đây
using CloudinaryService = MangaK_System.BLL.CloudinaryService;
using MediaService = MangaK_System.BLL.MediaService;
using SeriesService = MangaK_System.BLL.Series;
using PublishingScheduleService = MangaK_System.BLL.PublishingSchedule;
using UserService = MangaK_System.BLL.User;
using FeedbackService = MangaK_System.BLL.Feedback;
using CategoryService = MangaK_System.BLL.Category;

namespace MangaK_System
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();

            // Đọc appsettings.json
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            // =============================
            // Database
            // =============================
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // =============================
            // Đăng ký Service ở đây
            // =============================
            services.AddScoped<SeriesService.ISeriesService, SeriesService.SeriesService>();
            services.AddScoped<PublishingScheduleService.IPublishingScheduleService, PublishingScheduleService.PublishingScheduleService>();
            services.AddScoped<MediaService.IService, CloudinaryService.Service>();
            services.AddScoped<UserService.IUserService, UserService.UserService>();
            services.AddScoped<FeedbackService.IFeedbackService, FeedbackService.FeedbackService>();
            services.AddScoped<CategoryService.IService, CategoryService.Service>();
            //==============================
            ServiceProvider = services.BuildServiceProvider();

            try
            {
                using var scope = ServiceProvider.CreateScope();

                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                db.Database.Migrate();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Database Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                Shutdown();
            }

            base.OnStartup(e);
        }
    }

}
