using MangaK_System.GUI.ViewModels;
using System.Windows.Controls;

namespace MangaK_System.GUI.Views.Pages
{
    /// <summary>
    /// Code-behind cho LoginPage.
    /// 
    /// Lý do cần code-behind (ngoại lệ của MVVM thuần túy):
    /// WPF PasswordBox không hỗ trợ Two-Way Binding trực tiếp vì lý do bảo mật.
    /// Giải pháp: Lắng nghe sự kiện PasswordChanged và đẩy giá trị vào ViewModel thủ công.
    /// </summary>
    public partial class LoginPage : UserControl
    {
        public LoginPage()
        {
            InitializeComponent();

            // Lắng nghe sự kiện thay đổi mật khẩu trong PasswordBox
            PasswordBox.PasswordChanged += OnPasswordChanged;
        }

        /// <summary>
        /// Đồng bộ giá trị PasswordBox sang ViewModel mỗi khi người dùng nhập.
        /// Đây là cách xử lý chuẩn cho PasswordBox trong MVVM.
        /// </summary>
        private void OnPasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            // Lấy ViewModel từ DataContext (được gán tự động qua DataTemplate)
            if (DataContext is LoginViewModel vm)
            {
                vm.Password = PasswordBox.Password;
            }
        }
    }
}
