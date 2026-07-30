using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MangaK_System.GUI.ViewModels
{
    /// <summary>
    /// Lớp ViewModel cơ sở – triển khai INotifyPropertyChanged
    /// để tự động thông báo cho View khi dữ liệu thay đổi (tương tự useState trong React).
    /// Tất cả ViewModel đều kế thừa từ lớp này.
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        // Sự kiện được WPF lắng nghe để cập nhật giao diện
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Kích hoạt sự kiện PropertyChanged cho thuộc tính được chỉ định.
        /// Gọi phương thức này khi một thuộc tính thay đổi giá trị.
        /// </summary>
        /// <param name="propertyName">
        /// Tên thuộc tính vừa thay đổi.
        /// [CallerMemberName] sẽ tự điền tên thuộc tính nếu không truyền tham số.
        /// </param>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Gán giá trị mới cho trường (backing field) và kích hoạt PropertyChanged nếu giá trị thay đổi.
        /// Dùng thay vì gán trực tiếp trong setter để tránh notify thừa.
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu của thuộc tính</typeparam>
        /// <param name="field">Tham chiếu tới backing field</param>
        /// <param name="value">Giá trị mới</param>
        /// <param name="propertyName">Tên thuộc tính (tự động điền)</param>
        /// <returns>True nếu giá trị thực sự thay đổi</returns>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
