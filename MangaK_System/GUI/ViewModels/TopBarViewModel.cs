using System.Windows.Input;

namespace MangaK_System.GUI.ViewModels
{
    // ═══════════════════════════════════════════════════════
    // CLASS: TopBarViewModel
    // Tương đương với HeaderPage.jsx + HeaderMenu.jsx
    //
    // Trách nhiệm:
    //   - Hiển thị avatar, tên role người dùng ("Welcome back!")
    //   - Số lượng feedback chưa đọc (badge trên chuông)
    //   - Dropdown menu: Profile, Logout
    //   - Không có theme toggle (desktop app không cần dark/light mode toggle)
    // ═══════════════════════════════════════════════════════

    /// <summary>
    /// ViewModel cho thanh tiêu đề (TopBar) – phần đầu trang chính sau đăng nhập.
    /// Kết hợp logic của HeaderPage.jsx và HeaderMenu.jsx.
    /// </summary>
    public class TopBarViewModel : BaseViewModel
    {
        private readonly MainViewModel _mainViewModel;

        // ─── Backing fields ────────────────────────────────────
        private string _roleName = string.Empty;
        private string _avatarUrl = string.Empty;
        private int _unreadFeedbackCount = 0;
        private bool _isFeedbackDropdownOpen = false;
        private bool _isMenuDropdownOpen = false;

        // ─── Thuộc tính binding ────────────────────────────────

        /// <summary>
        /// Tên vai trò hiển thị (vd: "Mangaka", "Tantou Editor").
        /// Tương đương prop roleName trong HeaderPage.jsx.
        /// </summary>
        public string RoleName
        {
            get => _roleName;
            set => SetProperty(ref _roleName, value);
        }

        /// <summary>
        /// URL/đường dẫn ảnh avatar người dùng.
        /// Tương đương state currentUserAvatar trong HeaderPage.jsx.
        /// </summary>
        public string AvatarUrl
        {
            get => _avatarUrl;
            set => SetProperty(ref _avatarUrl, value);
        }

        /// <summary>
        /// Số lượng feedback chưa đọc – hiển thị badge đỏ trên icon chuông.
        /// Tương đương unreadFeedbackCount trong HeaderPage.jsx.
        /// </summary>
        public int UnreadFeedbackCount
        {
            get => _unreadFeedbackCount;
            set
            {
                SetProperty(ref _unreadFeedbackCount, value);
                // Thông báo cho View cập nhật HasUnreadFeedback
                OnPropertyChanged(nameof(HasUnreadFeedback));
                OnPropertyChanged(nameof(UnreadFeedbackBadge));
            }
        }

        /// <summary>True khi có feedback chưa đọc (hiện badge)</summary>
        public bool HasUnreadFeedback => UnreadFeedbackCount > 0;

        /// <summary>
        /// Text hiển thị trên badge (giới hạn "99+").
        /// Tương đương: unreadFeedbackCount > 99 ? '99+' : unreadFeedbackCount
        /// </summary>
        public string UnreadFeedbackBadge =>
            UnreadFeedbackCount > 99 ? "99+" : UnreadFeedbackCount.ToString();

        /// <summary>
        /// Trạng thái mở/đóng dropdown feedback.
        /// Tương đương isDropdownOpen trong HeaderPage.jsx.
        /// </summary>
        public bool IsFeedbackDropdownOpen
        {
            get => _isFeedbackDropdownOpen;
            set => SetProperty(ref _isFeedbackDropdownOpen, value);
        }

        /// <summary>
        /// Trạng thái mở/đóng dropdown menu (hamburger).
        /// Tương đương isOpen trong HeaderMenu.jsx.
        /// </summary>
        public bool IsMenuDropdownOpen
        {
            get => _isMenuDropdownOpen;
            set => SetProperty(ref _isMenuDropdownOpen, value);
        }

        // ─── Commands ─────────────────────────────────────────

        /// <summary>Mở/đóng dropdown danh sách feedback</summary>
        public ICommand ToggleFeedbackDropdownCommand { get; }

        /// <summary>Mở/đóng dropdown menu (Profile, Logout)</summary>
        public ICommand ToggleMenuDropdownCommand { get; }

        /// <summary>
        /// Điều hướng sang trang Profile.
        /// Tương đương handleProfile() trong HeaderMenu.jsx.
        /// </summary>
        public ICommand GoToProfileCommand { get; }

        /// <summary>
        /// Đăng xuất và quay về Landing.
        /// Tương đương handleLogout() trong HeaderMenu.jsx.
        /// </summary>
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

            // Toggle dropdown feedback
            ToggleFeedbackDropdownCommand = new RelayCommand(_ =>
            {
                IsFeedbackDropdownOpen = !IsFeedbackDropdownOpen;
                // Đóng menu kia nếu đang mở
                if (IsFeedbackDropdownOpen) IsMenuDropdownOpen = false;
            });

            // Toggle dropdown menu
            ToggleMenuDropdownCommand = new RelayCommand(_ =>
            {
                IsMenuDropdownOpen = !IsMenuDropdownOpen;
                // Đóng feedback dropdown nếu đang mở
                if (IsMenuDropdownOpen) IsFeedbackDropdownOpen = false;
            });

            // Điều hướng Profile
            GoToProfileCommand = new RelayCommand(_ =>
            {
                IsMenuDropdownOpen = false;
                // TODO: _mainViewModel.NavigateToProfile();
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
