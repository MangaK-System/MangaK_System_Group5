using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mangak_System._1_DAL.Entity;

namespace MangaK_System.BLL.User
{
    public interface IUserService
    {
        Task<Mangak_System._1_DAL.Entity.User?> LoginAsync(string email, string password);
        Task<Mangak_System._1_DAL.Entity.User?> GetProfileAsync(Guid userId);
        Task<List<Mangak_System._1_DAL.Entity.User>> GetAllUsersAsync();
        Task<bool> UpdateSupervisorAsync(Guid userId, Guid? supervisorId);
    }
}
