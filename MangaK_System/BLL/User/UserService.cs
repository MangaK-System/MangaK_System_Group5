using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mangak_System._1_DAL.Data;
using Mangak_System._1_DAL.Entity;

namespace MangaK_System.BLL.User
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Mangak_System._1_DAL.Entity.User?> LoginAsync(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return null;

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!isPasswordValid)
                return null;

            return user;
        }

        public async Task<Mangak_System._1_DAL.Entity.User?> GetProfileAsync(Guid userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<List<Mangak_System._1_DAL.Entity.User>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(u => u.Supervisor)
                .OrderBy(u => u.Role)
                .ThenBy(u => u.FirstName)
                .ToListAsync();
        }

        public async Task<bool> UpdateSupervisorAsync(Guid userId, Guid? supervisorId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.SupervisorId = supervisorId;
            user.UpdatedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
