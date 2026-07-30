using System.Windows.Input;

namespace MangaK_System.GUI.ViewModels
{
    /// <summary>
    /// Triển khai ICommand dạng delegate – cho phép bind Command trong XAML
    /// mà không cần tạo class riêng cho từng Command.
    /// 
    /// Cách dùng:
    ///   MyCommand = new RelayCommand(_ => DoSomething());
    ///   MyCommand = new RelayCommand(_ => DoSomething(), _ => CanDoSomething());
    /// </summary>
    public class RelayCommand : ICommand
    {
        // Hành động thực thi khi Command được gọi
        private readonly Action<object?> _execute;

        // Hàm kiểm tra Command có thể thực thi không (tuỳ chọn)
        private readonly Func<object?, bool>? _canExecute;

        /// <summary>
        /// Sự kiện WPF dùng để cập nhật trạng thái CanExecute.
        /// Gắn vào CommandManager.RequerySuggested để tự động refresh.
        /// </summary>
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        /// <summary>
        /// Khởi tạo RelayCommand.
        /// </summary>
        /// <param name="execute">Hành động được thực thi</param>
        /// <param name="canExecute">Điều kiện cho phép thực thi (null = luôn cho phép)</param>
        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// Kiểm tra Command có thể thực thi không.
        /// </summary>
        public bool CanExecute(object? parameter) =>
            _canExecute == null || _canExecute(parameter);

        /// <summary>
        /// Thực thi hành động của Command.
        /// </summary>
        public void Execute(object? parameter) => _execute(parameter);
    }
}
