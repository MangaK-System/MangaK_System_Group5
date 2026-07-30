using System.Windows.Input;

namespace MangaK_System.GUI.ViewModels
{
    /// <summary>
    /// ViewModel cho trang Landing (trang chủ).
    /// Chứa logic và dữ liệu hiển thị trên LandingPage.
    /// </summary>
    public class LandingViewModel : BaseViewModel
    {
        // Tham chiếu tới MainViewModel để điều hướng sang trang khác
        private readonly MainViewModel _mainViewModel;

        // ─── Commands ─────────────────────────────────────────────────────

        /// <summary>
        /// Command gắn vào nút "Enter System".
        /// Khi bấm → chuyển sang trang đăng nhập.
        /// </summary>
        public ICommand EnterSystemCommand { get; }

        // ─── Thông tin hiển thị trên Landing Page ─────────────────────────

        /// <summary>
        /// Số lượng Mangaka (hiển thị trong phần thống kê).
        /// </summary>
        public string MangakaCount => "500+";

        /// <summary>
        /// Số lượng Trợ lý (Assistants).
        /// </summary>
        public string AssistantCount => "2.4K+";

        /// <summary>
        /// Số lượng tác phẩm đã xuất bản.
        /// </summary>
        public string PublishedWorksCount => "1,200+";

        /// <summary>
        /// Khởi tạo LandingViewModel.
        /// </summary>
        /// <param name="mainViewModel">
        /// MainViewModel – dùng để gọi điều hướng sang trang Login
        /// </param>
        public LandingViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;

            // Khi nút "Enter System" được bấm → gọi NavigateToLogin()
            EnterSystemCommand = new RelayCommand(_ => _mainViewModel.NavigateToLogin());
        }
    }
}
