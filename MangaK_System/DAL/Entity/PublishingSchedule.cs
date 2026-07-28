using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MangaK_System.DAL.Abstraction;

namespace Mangak_System._1_DAL.Entity;

public class PublishingSchedule : BaseEntity<Guid>, IAuditableEntity
{
    public DateTime PublishDate { get; set; }
    public string? PublishPeriod { get; set; }

    public Guid SeriesId { get; set; }
    public Series Series { get; set; } = null!;

    public Guid? DecidedById { get; set; }
    public User? DecidedBy { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}