using Mangak_System._1_DAL.Entity;

namespace MangaK_System.GUI.Services
{
    /// <summary>
    /// Lưu trữ thông tin phiên làm việc (session) của người dùng hiện tại sau khi đăng nhập thành công.
    /// Dùng trong toàn bộ tầng GUI.
    /// </summary>
    public static class UserSession
    {
        /// <summary>
        /// Đối tượng User hiện tại đang đăng nhập. Null nếu chưa đăng nhập.
        /// </summary>
        public static User? CurrentUser { get; private set; }

        /// <summary>
        /// Lưu phiên đăng nhập mới.
        /// </summary>
        /// <param name="user">Người dùng đã đăng nhập thành công</param>
        public static void Login(User user)
        {
            CurrentUser = user ?? throw new ArgumentNullException(nameof(user));
        }

        /// <summary>
        /// Xóa phiên đăng nhập (khi người dùng Đăng xuất).
        /// </summary>
        public static void Logout()
        {
            CurrentUser = null;
        }

        /// <summary>
        /// Kiểm tra xem người dùng đã đăng nhập hay chưa.
        /// </summary>
        public static bool IsLoggedIn => CurrentUser != null;
    }
}
