using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using MangaK_System.BLL.User;
using MangaK_System.GUI.Services;
using DalUserRole = MangaK_System.DAL.Entity.Enums.UserRole;
using GuiUserRole = MangaK_System.GUI.ViewModels.UserRole;

namespace MangaK_System.GUI.ViewModels
{
    /// <summary>
    /// ViewModel cho trang Đăng nhập (LoginPage).
    /// Quản lý trạng thái form đăng nhập: email, mật khẩu, hiển thị lỗi,
    /// và kết nối trực tiếp với IUserService trong tầng BLL để xác thực người dùng.
    /// </summary>
    public class LoginViewModel : BaseViewModel
    {
        // Tham chiếu tới MainViewModel để điều hướng trang
        private readonly MainViewModel _mainViewModel;

        // ─── Backing fields ────────────────────────────────────────────────
        private string _email = string.Empty;
        private string _password = string.Empty;
        private bool _isPasswordVisible = false;
        private string _errorMessage = string.Empty;
        private bool _isLoading = false;

        // ─── Thuộc tính binding với View ──────────────────────────────────

        /// <summary>
        /// Địa chỉ email người dùng nhập vào.
        /// </summary>
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        /// <summary>
        /// Mật khẩu người dùng nhập vào.
        /// </summary>
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        /// <summary>
        /// Trạng thái hiển thị mật khẩu (true = hiện, false = ẩn).
        /// </summary>
        public bool IsPasswordVisible
        {
            get => _isPasswordVisible;
            set => SetProperty(ref _isPasswordVisible, value);
        }

        /// <summary>
        /// Thông báo lỗi hiển thị bên dưới form khi đăng nhập thất bại.
        /// </summary>
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        /// <summary>
        /// Trạng thái đang xử lý đăng nhập (true = đang gọi Service).
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (SetProperty(ref _isLoading, value))
                {
                    // Thông báo RequerySuggested để tự động refresh trạng thái CanExecute của LoginCommand
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        // ─── Commands ─────────────────────────────────────────────────────

        /// <summary>
        /// Command gắn vào nút "Log In".
        /// </summary>
        public ICommand LoginCommand { get; }

        /// <summary>
        /// Command gắn vào nút "Back to Home".
        /// </summary>
        public ICommand BackToHomeCommand { get; }

        /// <summary>
        /// Command gắn vào icon mắt – bật/tắt hiển thị mật khẩu.
        /// </summary>
        public ICommand TogglePasswordVisibilityCommand { get; }

        /// <summary>
        /// Khởi tạo LoginViewModel.
        /// </summary>
        /// <param name="mainViewModel">MainViewModel – dùng để điều hướng</param>
        public LoginViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;

            // Quay lại trang Landing
            BackToHomeCommand = new RelayCommand(_ => _mainViewModel.NavigateToLanding());

            // Bật/tắt hiển thị mật khẩu
            TogglePasswordVisibilityCommand = new RelayCommand(_ => IsPasswordVisible = !IsPasswordVisible);

            // Thực hiện đăng nhập bất đồng bộ
            LoginCommand = new RelayCommand(
                async _ => await ExecuteLoginAsync(),
                _ => !IsLoading // Vô hiệu hoá nút khi đang xử lý
            );
        }

        // ─── Logic xử lý kết nối BLL & DAL ───────────────────────────────

        /// <summary>
        /// Thực hiện xác thực đăng nhập qua IUserService (BLL).
        /// </summary>
        private async Task ExecuteLoginAsync()
        {
            // Reset thông báo lỗi cũ
            ErrorMessage = string.Empty;

            // 1. Kiểm tra dữ liệu đầu vào (Validation)
            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Vui lòng nhập địa chỉ email.";
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Vui lòng nhập mật khẩu.";
                return;
            }

            IsLoading = true;

            try
            {
                // 2. Lấy IUserService từ DI ServiceProvider của App
                using var scope = App.ServiceProvider.CreateScope();
                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

                // 3. Gọi LoginAsync trong BLL (BLL sẽ truy vấn DAL AppDbContext và verify password hash bằng BCrypt)
                var user = await userService.LoginAsync(Email.Trim(), Password);

                if (user == null)
                {
                    ErrorMessage = "Email hoặc mật khẩu không chính xác.";
                    return;
                }

                // 4. Ánh xạ Role từ DAL sang GUI và kiểm tra quyền truy cập Desktop App
                if (!TryMapRole(user.Role, out GuiUserRole guiRole, out string roleDisplayName))
                {
                    ErrorMessage = "Tài khoản của bạn không có quyền truy cập ứng dụng Desktop.";
                    return;
                }

                // 5. Lưu thông tin người dùng vào Session
                UserSession.Login(user);

                // 6. Xây dựng tên hiển thị (Ưu tiên AuthorName -> "FirstName LastName" -> Email)
                string displayName = !string.IsNullOrWhiteSpace(user.AuthorName)
                    ? user.AuthorName
                    : $"{user.FirstName} {user.LastName}".Trim();

                if (string.IsNullOrWhiteSpace(displayName))
                {
                    displayName = user.Email;
                }

                // 7. Chuyển sang Giao diện chính (MainLayout) theo Role tương ứng
                _mainViewModel.NavigateToMainLayout(guiRole, roleDisplayName, user.AvatarUrl ?? string.Empty);
            }
            catch (UnauthorizedAccessException ex)
            {
                // BLL ném UnauthorizedAccessException khi Email không tồn tại hoặc sai mật khẩu
                if (ex.Message.Contains("Email not found", StringComparison.OrdinalIgnoreCase))
                {
                    ErrorMessage = "Email không tồn tại trong hệ thống.";
                }
                else if (ex.Message.Contains("Incorrect password", StringComparison.OrdinalIgnoreCase))
                {
                    ErrorMessage = "Mật khẩu không chính xác.";
                }
                else
                {
                    ErrorMessage = ex.Message;
                }
            }
            catch (ArgumentException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Đã xảy ra lỗi hệ thống: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Ánh xạ UserRole từ tầng DAL sang GuiUserRole ở tầng GUI.
        /// </summary>
        private static bool TryMapRole(DalUserRole dalRole, out GuiUserRole guiRole, out string roleDisplayName)
        {
            switch (dalRole)
            {
                case DalUserRole.Mangaka:
                    guiRole = GuiUserRole.Mangaka;
                    roleDisplayName = "Mangaka";
                    return true;

                case DalUserRole.Tantou:
                    guiRole = GuiUserRole.Tantou;
                    roleDisplayName = "Tantou Editor";
                    return true;

                case DalUserRole.Editorial:
                    guiRole = GuiUserRole.Editorial;
                    roleDisplayName = "Editorial Board";
                    return true;

                case DalUserRole.Admin:
                    guiRole = GuiUserRole.Admin;
                    roleDisplayName = "Admin";
                    return true;

                default:
                    guiRole = default;
                    roleDisplayName = string.Empty;
                    return false;
            }
        }
    }
}
