using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaK_System.DAL.Abstraction
{
    public abstract class BaseEntity<T>
    {
        public T Id { get; set; } = default!;

        public bool IsDeleted { get; set; } = false;
    }
}
