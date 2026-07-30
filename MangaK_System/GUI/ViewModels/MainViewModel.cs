namespace MangaK_System.GUI.ViewModels
{
    /// <summary>
    /// ViewModel trung tâm – đóng vai trò như "Router" của ứng dụng.
    /// 
    /// Giống React Router: MainWindow không render nội dung cứng,
    /// mà chỉ render CurrentPage. Khi CurrentPage thay đổi,
    /// WPF tự tìm DataTemplate tương ứng trong Window.Resources và hiển thị View đúng.
    /// 
    /// Cách điều hướng:
    ///   CurrentPage = new LandingViewModel();   // → hiển thị LandingPage
    ///   CurrentPage = new LoginViewModel(this); // → hiển thị LoginPage
    /// </summary>
    public class MainViewModel : BaseViewModel
    {
        // ─── Backing field cho thuộc tính CurrentPage ───────────────────────
        private BaseViewModel _currentPage;

        /// <summary>
        /// Trang hiện tại đang được hiển thị trong MainWindow.
        /// Khi set giá trị mới, WPF tự động tìm DataTemplate phù hợp và render View.
        /// </summary>
        public BaseViewModel CurrentPage
        {
            get => _currentPage;
            set => SetProperty(ref _currentPage, value);
        }

        /// <summary>
        /// Khởi tạo MainViewModel – mặc định hiển thị LandingPage.
        /// </summary>
        public MainViewModel()
        {
            // Điểm vào mặc định: LandingPage
            // Truyền "this" để LandingViewModel có thể điều hướng về sau
            _currentPage = new LandingViewModel(this);
        }

        // ─── Các phương thức điều hướng (Navigation Methods) ──────────────

        /// <summary>
        /// Chuyển sang trang Landing (trang chủ).
        /// </summary>
        public void NavigateToLanding()
        {
            CurrentPage = new LandingViewModel(this);
        }

        /// <summary>
        /// Chuyển sang trang đăng nhập.
        /// </summary>
        public void NavigateToLogin()
        {
            CurrentPage = new LoginViewModel(this);
        }

        /// <summary>
        /// Chuyển sang layout chính sau đăng nhập thành công.
        /// </summary>
        /// <param name="role">Vai trò người dùng (quyết định menu sidebar)</param>
        /// <param name="roleName">Tên hiển thị (vd: "Tantou Editor")</param>
        /// <param name="avatarUrl">URL ảnh avatar (tuỳ chọn)</param>
        public void NavigateToMainLayout(UserRole role, string roleName, string avatarUrl = "")
        {
            CurrentPage = new MainLayoutViewModel(this, role, roleName, avatarUrl);
        }
    }
}
