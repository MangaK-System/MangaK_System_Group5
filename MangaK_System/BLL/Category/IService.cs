using System.Collections.Generic;
using System.Threading.Tasks;
using Mangak_System._1_DAL.Entity;

namespace MangaK_System.BLL.Category
{
    public interface IService
    {
        Task<List<Mangak_System._1_DAL.Entity.Category>> GetAllCategoriesAsync();
    }
}
