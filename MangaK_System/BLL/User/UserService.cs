using System;
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
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Email and password cannot be empty.");
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Email not found.");
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Incorrect password.");
            }

            return user;
        }

        public async Task<Mangak_System._1_DAL.Entity.User?> GetProfileAsync(Guid userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);
            
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            return user;
        }
    }
}
