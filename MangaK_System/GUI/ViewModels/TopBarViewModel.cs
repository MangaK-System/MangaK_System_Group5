using System.Windows.Input;

namespace MangaK_System.GUI.ViewModels
{
    /// <summary>
    /// ViewModel cho thanh tiêu đề (TopBar) – phần đầu trang chính sau đăng nhập.
    /// Quản lý thông tin vai trò người dùng và menu Đăng xuất.
    /// </summary>
    public class TopBarViewModel : BaseViewModel
    {
        private readonly MainViewModel _mainViewModel;

        // ─── Backing fields ────────────────────────────────────
        private string _roleName = string.Empty;
        private string _avatarUrl = string.Empty;
        private bool _isMenuDropdownOpen = false;

        // ─── Thuộc tính binding ────────────────────────────────

        /// <summary>
        /// Tên vai trò hiển thị (vd: "Mangaka", "Tantou Editor").
        /// </summary>
        public string RoleName
        {
            get => _roleName;
            set => SetProperty(ref _roleName, value);
        }

        /// <summary>
        /// URL/đường dẫn ảnh avatar người dùng.
        /// </summary>
        public string AvatarUrl
        {
            get => _avatarUrl;
            set => SetProperty(ref _avatarUrl, value);
        }

        /// <summary>
        /// Trạng thái mở/đóng dropdown menu (hamburger).
        /// </summary>
        public bool IsMenuDropdownOpen
        {
            get => _isMenuDropdownOpen;
            set => SetProperty(ref _isMenuDropdownOpen, value);
        }

        // ─── Commands ─────────────────────────────────────────

        /// <summary>
        /// Callback được gọi khi người dùng chọn mục Profile trong menu hamburger.
        /// </summary>
        public Action? OnProfileRequested { get; set; }

        /// <summary>Mở/đóng dropdown menu (Profile, Logout)</summary>
        public ICommand ToggleMenuDropdownCommand { get; }

        /// <summary>Điều hướng sang trang Profile</summary>
        public ICommand GoToProfileCommand { get; }

        /// <summary>Đăng xuất và quay về Landing</summary>
        public ICommand LogoutCommand { get; }

        // ─── Constructor ──────────────────────────────────────

        /// <summary>
        /// Khởi tạo TopBarViewModel.
        /// </summary>
        /// <param name="mainViewModel">Router trung tâm</param>
        /// <param name="roleName">Tên vai trò hiển thị</param>
        /// <param name="avatarUrl">Đường dẫn ảnh avatar (tuỳ chọn)</param>
        public TopBarViewModel(MainViewModel mainViewModel, string roleName, string avatarUrl = "")
        {
            _mainViewModel = mainViewModel;
            _roleName = roleName;
            _avatarUrl = string.IsNullOrEmpty(avatarUrl) ? "" : avatarUrl;

            // Toggle dropdown menu
            ToggleMenuDropdownCommand = new RelayCommand(_ =>
            {
                IsMenuDropdownOpen = !IsMenuDropdownOpen;
            });

            // Điều hướng Profile
            GoToProfileCommand = new RelayCommand(_ =>
            {
                IsMenuDropdownOpen = false;
                OnProfileRequested?.Invoke();
            });

            // Đăng xuất → giải phóng session và quay về Landing
            LogoutCommand = new RelayCommand(_ =>
            {
                IsMenuDropdownOpen = false;
                MangaK_System.GUI.Services.UserSession.Logout();
                _mainViewModel.NavigateToLanding();
            });
        }
    }
}
