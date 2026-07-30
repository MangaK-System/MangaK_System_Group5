using MangaK_System.GUI.ViewModels;
using System.Windows;

namespace MangaK_System.GUI.Views.Windows
{
    /// <summary>
    /// Code-behind cho AppWindow – cửa sổ chính của ứng dụng.
    /// 
    /// Nhiệm vụ duy nhất: Khởi tạo MainViewModel và gán làm DataContext.
    /// MainViewModel sẽ điều khiển toàn bộ việc chuyển trang.
    /// </summary>
    public partial class AppWindow : Window
    {
        public AppWindow()
        {
            InitializeComponent();

            // Gán MainViewModel làm DataContext cho toàn bộ cửa sổ.
            // MainViewModel tự động set CurrentPage = LandingViewModel (trang mặc định).
            DataContext = new MainViewModel();
        }
    }
}
