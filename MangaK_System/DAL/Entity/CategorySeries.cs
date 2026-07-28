using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mangak_System._1_DAL.Entity;

public class CategorySeries
{
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public Guid SeriesId { get; set; }
    public Series Series { get; set; } = null!;
}
