using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MangaK_System.DAL.Abstraction;
using MangaK_System.DAL.Entity.Enums;

namespace Mangak_System._1_DAL.Entity;

public class Feedback : BaseEntity<Guid>, IAuditableEntity
{
    public Guid SenderId { get; set; }
    public User Sender { get; set; } = null;
    public string Content { get; set; } = null!;
    public FeedbackType Type { get; set; } = FeedbackType.Manual;
    public bool IsRead { get; set; } = false;
    public Guid SeriesId { get; set; }
    public Series Series { get; set; } = null!;

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
