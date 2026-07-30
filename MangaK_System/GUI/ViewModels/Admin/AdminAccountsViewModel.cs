using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DalUserRole = MangaK_System.DAL.Entity.Enums.UserRole;
using DalUserStatus = MangaK_System.DAL.Entity.Enums.UserStatus;
using Mangak_System._1_DAL.Data;
using Mangak_System._1_DAL.Entity;

namespace MangaK_System.GUI.ViewModels.Admin
{
    public class UserAccountItem : BaseViewModel
    {
        public User User { get; }
        public string Email => User.Email;
        public string FirstName => User.FirstName;
        public string LastName => User.LastName;
        public string AuthorName => User.AuthorName ?? string.Empty;
        public string Role => User.Role.ToString();
        public string Status => User.Status.ToString();
        public bool IsMangaka => User.Role == DalUserRole.Mangaka;

        public string SupervisorName => User.Supervisor != null
            ? $"{User.Supervisor.FirstName} {User.Supervisor.LastName}"
            : "Unassigned";

        private Guid? _selectedSupervisorId;
        public Guid? SelectedSupervisorId
        {
            get => _selectedSupervisorId;
            set => SetProperty(ref _selectedSupervisorId, value);
        }

        public UserAccountItem(User user)
        {
            User = user;
            _selectedSupervisorId = user.SupervisorId;
        }
    }

    public class AdminAccountsViewModel : BaseViewModel
    {
        private bool _isLoading;
        private string _statusMessage = string.Empty;

        public ObservableCollection<UserAccountItem> UserList { get; } = new();
        public ObservableCollection<User> TantouEditors { get; } = new();

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

        public ICommand RefreshCommand { get; }
        public ICommand AssignSupervisorCommand { get; }

        public AdminAccountsViewModel()
        {
            RefreshCommand = new RelayCommand(async _ => await LoadUsersAsync());
            AssignSupervisorCommand = new RelayCommand(param => ExecuteAssignSupervisor(param as UserAccountItem));

            _ = LoadUsersAsync();
        }

        public async Task LoadUsersAsync()
        {
            IsLoading = true;
            StatusMessage = string.Empty;

            try
            {
                using var scope = App.ServiceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // Nạp danh sách Tantou Editors
                var tantous = await dbContext.Users
                    .Where(u => u.Role == DalUserRole.Tantou && u.Status == DalUserStatus.Active)
                    .ToListAsync();

                TantouEditors.Clear();
                foreach (var tantou in tantous)
                {
                    TantouEditors.Add(tantou);
                }

                // Nạp danh sách Users kèm thông tin Supervisor
                var users = await dbContext.Users
                    .Include(u => u.Supervisor)
                    .OrderBy(u => u.Role)
                    .ThenBy(u => u.FirstName)
                    .ToListAsync();

                UserList.Clear();
                foreach (var user in users)
                {
                    UserList.Add(new UserAccountItem(user));
                }

                if (UserList.Count == 0)
                {
                    StatusMessage = "No user accounts found.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading users: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ExecuteAssignSupervisor(UserAccountItem? item)
        {
            if (item == null) return;
            _ = UpdateSupervisorAsync(item);
        }

        private async Task UpdateSupervisorAsync(UserAccountItem item)
        {
            IsLoading = true;

            try
            {
                using var scope = App.ServiceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var dbUser = await dbContext.Users.FindAsync(item.User.Id);
                if (dbUser != null)
                {
                    dbUser.SupervisorId = item.SelectedSupervisorId;
                    dbUser.UpdatedAt = DateTimeOffset.UtcNow;

                    await dbContext.SaveChangesAsync();

                    var assignedTantou = TantouEditors.FirstOrDefault(t => t.Id == item.SelectedSupervisorId);
                    string tantouName = assignedTantou != null
                        ? $"{assignedTantou.FirstName} {assignedTantou.LastName}"
                        : "Unassigned";

                    MessageBox.Show($"Supervisor '{tantouName}' assigned for Mangaka '{item.FirstName} {item.LastName}' successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    await LoadUsersAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to assign supervisor: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
