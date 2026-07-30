using System.Windows.Input;

namespace MangaK_System.GUI.ViewModels
{
    /// <summary>
    /// ViewModel cho trang Đăng nhập (LoginPage).
    /// Quản lý trạng thái form đăng nhập: email, mật khẩu, hiển thị lỗi.
    /// </summary>
    public class LoginViewModel : BaseViewModel
    {
        // Tham chiếu tới MainViewModel để điều hướng quay lại trang chủ
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
        /// Lưu ý: trong WPF, PasswordBox không hỗ trợ binding trực tiếp,
        /// nên cần xử lý qua code-behind hoặc Attached Property.
        /// </summary>
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        /// <summary>
        /// Trạng thái hiển thị mật khẩu (true = hiện, false = ẩn).
        /// Điều khiển việc dùng TextBox hay PasswordBox trong View.
        /// </summary>
        public bool IsPasswordVisible
        {
            get => _isPasswordVisible;
            set => SetProperty(ref _isPasswordVisible, value);
        }

        /// <summary>
        /// Thông báo lỗi hiển thị bên dưới form khi đăng nhập thất bại.
        /// Rỗng = không có lỗi.
        /// </summary>
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        /// <summary>
        /// Trạng thái đang xử lý đăng nhập (true = đang gọi API/Service).
        /// Dùng để vô hiệu hoá nút Log In và hiển thị loading indicator.
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
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
        /// <param name="mainViewModel">
        /// MainViewModel – dùng để điều hướng quay lại trang chủ
        /// </param>
        public LoginViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;

            // Quay lại trang Landing
            BackToHomeCommand = new RelayCommand(_ => _mainViewModel.NavigateToLanding());

            // Bật/tắt hiển thị mật khẩu
            TogglePasswordVisibilityCommand = new RelayCommand(_ => IsPasswordVisible = !IsPasswordVisible);

            // Thực hiện đăng nhập
            LoginCommand = new RelayCommand(
                _ => ExecuteLogin(),
                _ => !IsLoading // Chỉ cho phép bấm khi không đang loading
            );
        }

        // ─── Logic xử lý ──────────────────────────────────────────────────

        /// <summary>
        /// Xử lý đăng nhập khi người dùng bấm nút "Log In".
        /// TODO: Kết nối với BLL/Service thực tế để xác thực người dùng.
        /// </summary>
        private void ExecuteLogin()
        {
            // Xoá lỗi cũ
            ErrorMessage = string.Empty;

            // Validate đơn giản trước khi gọi service
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

            // TODO: Gọi AuthService để xác thực
            // Ví dụ:
            // IsLoading = true;
            // var result = await _authService.LoginAsync(Email, Password);
            // if (result.IsSuccess)
            //     _mainViewModel.CurrentPage = new DashboardViewModel(_mainViewModel);
            // else
            //     ErrorMessage = result.Message;
            // IsLoading = false;
        }
    }
}
