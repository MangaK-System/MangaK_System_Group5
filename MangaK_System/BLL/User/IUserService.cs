using System;
using System.Threading.Tasks;
using Mangak_System._1_DAL.Entity;

namespace MangaK_System.BLL.User
{
    public interface IUserService
    {
        Task<Mangak_System._1_DAL.Entity.User?> LoginAsync(string email, string password);
    }
}
