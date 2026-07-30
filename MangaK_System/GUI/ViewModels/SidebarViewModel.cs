using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MangaK_System.GUI.ViewModels
{
    // ═══════════════════════════════════════════════════════
    // ENUM: UserRole – các vai trò trong hệ thống MangaK
    // Tương đương với roleDisplayNames trong Layout.jsx
    // ═══════════════════════════════════════════════════════

    /// <summary>
    /// Các vai trò người dùng trong hệ thống MangaK.
    /// </summary>
    public enum UserRole
    {
        Mangaka,
        Tantou,      // Tantou Editor
        Editorial,   // Editorial Board
        Admin
    }

    // ═══════════════════════════════════════════════════════
    // CLASS: MenuItem – một mục trong Sidebar menu
    // Tương đương với object { icon, label, path, key } trong Sidebar.jsx
    // ═══════════════════════════════════════════════════════

    /// <summary>
    /// Đại diện cho một mục menu trong Sidebar.
    /// </summary>
    public class MenuItem : BaseViewModel
    {
        private bool _isActive;

        /// <summary>Nhãn hiển thị (vd: "Dashboard", "Series Management")</summary>
        public string Label { get; init; } = string.Empty;

        /// <summary>Icon unicode hoặc ký tự đại diện cho menu item</summary>
        public string IconGlyph { get; init; } = string.Empty;

        /// <summary>Key định danh duy nhất (vd: "dashboard", "series")</summary>
        public string Key { get; init; } = string.Empty;

        /// <summary>
        /// Trạng thái active – true khi đây là trang đang xem.
        /// Tương đương isActive trong Sidebar.jsx (dựa vào location.pathname).
        /// </summary>
        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }
    }

    // ═══════════════════════════════════════════════════════
    // CLASS: SidebarViewModel
    // Tương đương với Sidebar.jsx (state + menu items theo role)
    // ═══════════════════════════════════════════════════════

    /// <summary>
    /// ViewModel cho Sidebar – quản lý menu items theo vai trò người dùng
    /// và trạng thái thu gọn/mở rộng của sidebar.
    /// </summary>
    public class SidebarViewModel : BaseViewModel
    {
        private readonly MainViewModel _mainViewModel;
        private bool _isCollapsed = false;
        private MenuItem? _selectedItem;

        // ─── Thuộc tính ───────────────────────────────────────

        /// <summary>
        /// Trạng thái thu gọn sidebar (true = chỉ hiện icon, false = hiện icon + chữ).
        /// Tương đương state isOpen trong Sidebar.jsx (đảo chiều).
        /// </summary>
        public bool IsCollapsed
        {
            get => _isCollapsed;
            set => SetProperty(ref _isCollapsed, value);
        }

        /// <summary>
        /// Chiều rộng sidebar tính bằng pixel.
        /// 220px khi mở, 64px khi thu gọn.
        /// </summary>
        public double SidebarWidth => IsCollapsed ? 64 : 220;

        /// <summary>Menu item đang được chọn (trang đang xem)</summary>
        public MenuItem? SelectedItem
        {
            get => _selectedItem;
            set
            {
                // Bỏ active item cũ
                if (_selectedItem != null)
                    _selectedItem.IsActive = false;

                SetProperty(ref _selectedItem, value);

                // Đánh dấu active item mới
                if (_selectedItem != null)
                    _selectedItem.IsActive = true;
            }
        }

        /// <summary>
        /// Danh sách menu items theo role hiện tại.
        /// Tương đương menuItems[userRole] trong Sidebar.jsx.
        /// </summary>
        public ObservableCollection<MenuItem> MenuItems { get; } = [];

        // ─── Commands ─────────────────────────────────────────

        /// <summary>Toggle thu gọn/mở rộng Sidebar</summary>
        public ICommand ToggleCollapseCommand { get; }

        /// <summary>Chọn một menu item để điều hướng</summary>
        public ICommand SelectItemCommand { get; }

        // ─── Constructor ──────────────────────────────────────

        /// <summary>
        /// Callback được gọi khi người dùng click vào mục menu để chuyển nội dung trang.
        /// </summary>
        public Action<string, UserRole>? OnNavigationRequested { get; set; }

        /// <summary>
        /// Khởi tạo SidebarViewModel với role người dùng.
        /// </summary>
        /// <param name="mainViewModel">Router trung tâm để điều hướng</param>
        /// <param name="role">Vai trò người dùng</param>
        public SidebarViewModel(MainViewModel mainViewModel, UserRole role)
        {
            _mainViewModel = mainViewModel;

            // Build menu items theo role (giống menuItems object trong Sidebar.jsx)
            BuildMenuItems(role);

            // Command toggle sidebar
            ToggleCollapseCommand = new RelayCommand(_ =>
            {
                IsCollapsed = !IsCollapsed;
                OnPropertyChanged(nameof(SidebarWidth));
            });

            // Command chọn item: cập nhật active state và điều hướng
            SelectItemCommand = new RelayCommand(param =>
            {
                if (param is MenuItem item)
                {
                    SelectedItem = item;
                    OnNavigationRequested?.Invoke(item.Key, role);
                }
            });

            // Active item đầu tiên (Dashboard) mặc định
            if (MenuItems.Count > 0)
                SelectedItem = MenuItems[0];
        }

        // ─── Khởi tạo menu items theo role ──────────────────

        /// <summary>
        /// Xây dựng danh sách menu items dựa theo role.
        /// Tương đương object menuItems trong Sidebar.jsx.
        /// </summary>
        private void BuildMenuItems(UserRole role)
        {
            MenuItems.Clear();

            var items = role switch
            {
                // admin: Account Management
                UserRole.Admin => new[]
                {
                    new MenuItem { Key = "accounts", Label = "Account Management", IconGlyph = "👥" },
                },

                // mangaka: Series Management
                UserRole.Mangaka => new[]
                {
                    new MenuItem { Key = "series", Label = "Series Management", IconGlyph = "📂" },
                },

                // tantou: Series Review
                UserRole.Tantou => new[]
                {
                    new MenuItem { Key = "series", Label = "Series Review", IconGlyph = "📂" },
                },

                // editorial: Series Approval + Publishing Schedule
                UserRole.Editorial => new[]
                {
                    new MenuItem { Key = "series",   Label = "Series Approval",    IconGlyph = "🔍" },
                    new MenuItem { Key = "schedule", Label = "Publishing Schedule", IconGlyph = "📅" },
                },

                _ => Array.Empty<MenuItem>()
            };

            foreach (var item in items)
                MenuItems.Add(item);
        }

        // ─── Logic điều hướng ────────────────────────────────

        /// <summary>
        /// Điều hướng tới trang tương ứng với menu item được chọn.
        /// TODO: Thêm case khi tạo từng ViewModel tương ứng.
        /// </summary>
        private void NavigateTo(string key, UserRole role)
        {
            // TODO: Gán CurrentPage trong _mainViewModel theo key + role
            // Ví dụ:
            // if (key == "dashboard" && role == UserRole.Admin)
            //     _mainViewModel.CurrentPage = new AdminDashboardViewModel(_mainViewModel);
        }
    }
}
