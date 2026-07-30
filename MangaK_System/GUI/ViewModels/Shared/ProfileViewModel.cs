using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MangaK_System.BLL.User;
using MangaK_System.GUI.Services;

namespace MangaK_System.GUI.ViewModels.Shared
{
    /// <summary>
    /// ViewModel xem thông tin cá nhân (Profile) của người dùng.
    /// Sử dụng IUserService.GetProfileAsync(userId) đúng theo BLL.
    /// </summary>
    public class ProfileViewModel : BaseViewModel
    {
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private string _authorName = string.Empty;
        private string _phone = string.Empty;
        private string _bio = string.Empty;
        private string _email = string.Empty;
        private string _roleName = string.Empty;
        private string _avatarUrl = string.Empty;
        private bool _isLoading;
        private string _statusMessage = string.Empty;

        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }

        public string LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }

        public string AuthorName
        {
            get => _authorName;
            set => SetProperty(ref _authorName, value);
        }

        public string Phone
        {
            get => _phone;
            set => SetProperty(ref _phone, value);
        }

        public string Bio
        {
            get => _bio;
            set => SetProperty(ref _bio, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string RoleName
        {
            get => _roleName;
            set => SetProperty(ref _roleName, value);
        }

        public string AvatarUrl
        {
            get => _avatarUrl;
            set => SetProperty(ref _avatarUrl, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public ProfileViewModel()
        {
            _ = LoadProfileAsync();
        }

        private async Task LoadProfileAsync()
        {
            if (UserSession.CurrentUser == null) return;

            IsLoading = true;
            StatusMessage = string.Empty;

            try
            {
                using var scope = App.ServiceProvider.CreateScope();
                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

                // Gọi đúng hàm GetProfileAsync trong BLL
                var profile = await userService.GetProfileAsync(UserSession.CurrentUser.Id);
                if (profile != null)
                {
                    FirstName = profile.FirstName ?? string.Empty;
                    LastName = profile.LastName ?? string.Empty;
                    AuthorName = profile.AuthorName ?? string.Empty;
                    Phone = profile.Phone ?? string.Empty;
                    Bio = profile.Bio ?? string.Empty;
                    Email = profile.Email ?? string.Empty;
                    RoleName = profile.Role.ToString();
                    AvatarUrl = profile.AvatarUrl ?? string.Empty;
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading profile: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
