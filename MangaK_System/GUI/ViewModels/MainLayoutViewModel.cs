namespace MangaK_System.GUI.ViewModels
{
    // ═══════════════════════════════════════════════════════
    // CLASS: MainLayoutViewModel
    // Tương đương với Layout.jsx
    //
    // Layout.jsx là shell sau khi đăng nhập, gồm:
    //   [Sidebar] | [TopBar + Breadcrumb + PageHeader + Content]
    //
    // ViewModel này làm chủ toàn bộ vùng đó,
    // giữ tham chiếu đến SidebarViewModel và TopBarViewModel.
    // ═══════════════════════════════════════════════════════

    /// <summary>
    /// ViewModel cho layout chính sau đăng nhập.
    /// Là "shell" bao gồm Sidebar, TopBar, và vùng nội dung trang.
    ///
    /// Tương đương Layout.jsx – render Sidebar + HeaderPage + Outlet.
    /// </summary>
    public class MainLayoutViewModel : BaseViewModel
    {
        private readonly MainViewModel _mainViewModel;
        private BaseViewModel? _currentContent;
        private string _pageTitle = string.Empty;
        private string _pageSubtitle = string.Empty;

        // ─── Sub-ViewModels ───────────────────────────────────

        /// <summary>
        /// ViewModel của Sidebar bên trái.
        /// Tương đương component &lt;Sidebar&gt; trong Layout.jsx.
        /// </summary>
        public SidebarViewModel Sidebar { get; }

        /// <summary>
        /// ViewModel của TopBar bên trên.
        /// Tương đương component &lt;HeaderPage&gt; trong Layout.jsx.
        /// </summary>
        public TopBarViewModel TopBar { get; }

        // ─── Thuộc tính nội dung trang ───────────────────────

        /// <summary>
        /// Nội dung trang hiện tại được hiển thị trong vùng main.
        /// Tương đương &lt;Outlet&gt; trong Layout.jsx –
        /// WPF dùng ContentControl + DataTemplate để render.
        /// </summary>
        public BaseViewModel? CurrentContent
        {
            get => _currentContent;
            set => SetProperty(ref _currentContent, value);
        }

        /// <summary>
        /// Tiêu đề trang (vd: "Series Management").
        /// Tương đương pageHeader.title trong Layout.jsx.
        /// </summary>
        public string PageTitle
        {
            get => _pageTitle;
            set
            {
                SetProperty(ref _pageTitle, value);
                OnPropertyChanged(nameof(HasPageHeader));
            }
        }

        /// <summary>
        /// Mô tả ngắn dưới tiêu đề trang.
        /// Tương đương pageHeader.subtitle trong Layout.jsx.
        /// </summary>
        public string PageSubtitle
        {
            get => _pageSubtitle;
            set => SetProperty(ref _pageSubtitle, value);
        }

        /// <summary>
        /// True khi có tiêu đề trang (PageTitle không rỗng).
        /// Tương đương điều kiện {pageHeader && ...} trong Layout.jsx.
        /// </summary>
        public bool HasPageHeader => !string.IsNullOrEmpty(PageTitle);

        // ─── Constructor ──────────────────────────────────────

        /// <summary>
        /// Khởi tạo MainLayoutViewModel sau khi đăng nhập thành công.
        /// </summary>
        /// <param name="mainViewModel">Router trung tâm</param>
        /// <param name="role">Vai trò người dùng (quyết định menu sidebar)</param>
        /// <param name="roleName">Tên hiển thị (vd: "Tantou Editor")</param>
        /// <param name="avatarUrl">URL ảnh avatar</param>
        public MainLayoutViewModel(
            MainViewModel mainViewModel,
            UserRole role,
            string roleName,
            string avatarUrl = "")
        {
            _mainViewModel = mainViewModel;

            // Khởi tạo sidebar với menu items theo role
            Sidebar = new SidebarViewModel(mainViewModel, role);
            Sidebar.OnNavigationRequested = HandleNavigation;

            // Khởi tạo topbar với thông tin người dùng
            TopBar = new TopBarViewModel(mainViewModel, roleName, avatarUrl);

            // Mở trang mặc định ban đầu dựa theo role (bỏ Dashboard)
            string defaultKey = role == UserRole.Admin ? "accounts" : "series";
            HandleNavigation(defaultKey, role);
        }

        /// <summary>
        /// Xử lý điều hướng hiển thị trang con (sub-page) tương ứng với menu item.
        /// </summary>
        private void HandleNavigation(string key, UserRole role)
        {
            switch (role)
            {
                case UserRole.Mangaka:
                    CurrentContent = new Mangaka.MangakaSeriesViewModel();
                    SetPageHeader("Series Management", "Create new manga series and track review status in real-time.");
                    break;

                case UserRole.Tantou:
                    CurrentContent = new Tantou.TantouReviewViewModel();
                    SetPageHeader("Series Review", "Review pending series submitted by your assigned Mangakas.");
                    break;

                case UserRole.Editorial:
                    if (key == "schedule")
                    {
                        CurrentContent = new Editorial.PublishingScheduleViewModel();
                        SetPageHeader("Publishing Schedule", "Manage and set publishing release dates and cycles.");
                    }
                    else
                    {
                        CurrentContent = new Editorial.EditorialApprovalViewModel();
                        SetPageHeader("Series Approval", "Final approval for series reviewed by Tantou Editors.");
                    }
                    break;

                case UserRole.Admin:
                    CurrentContent = new Admin.AdminAccountsViewModel();
                    SetPageHeader("Account Management", "Manage user accounts and roles in system.");
                    break;

                default:
                    ClearPageHeader();
                    break;
            }
        }

        // ─── Helper: set tiêu đề trang ───────────────────────

        /// <summary>
        /// Cập nhật tiêu đề và mô tả của trang đang hiển thị.
        /// Gọi từ ViewModel con khi chuyển nội dung.
        /// Tương đương setPageHeader() trong Layout.jsx.
        /// </summary>
        public void SetPageHeader(string title, string subtitle = "")
        {
            PageTitle = title;
            PageSubtitle = subtitle;
        }

        /// <summary>
        /// Xóa tiêu đề trang (ẩn page header section).
        /// </summary>
        public void ClearPageHeader()
        {
            PageTitle = string.Empty;
            PageSubtitle = string.Empty;
        }
    }
}
