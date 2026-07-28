using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MangaK_System.DAL.Abstraction;

namespace Mangak_System._1_DAL.Entity;

public class Category : BaseEntity<Guid>, IAuditableEntity
{
    public string Name { get; set; } = null!;
    public ICollection<CategorySeries> CategorySeries { get; set; } = new List<CategorySeries>();
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}