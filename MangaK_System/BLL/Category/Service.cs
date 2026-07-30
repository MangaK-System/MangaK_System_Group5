using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mangak_System._1_DAL.Data;
using Mangak_System._1_DAL.Entity;

namespace MangaK_System.BLL.Category
{
    public class Service : IService
    {
        private readonly AppDbContext _context;

        public Service(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Mangak_System._1_DAL.Entity.Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }
    }
}
