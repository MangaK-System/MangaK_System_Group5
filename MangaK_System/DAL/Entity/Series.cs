using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MangaK_System.DAL.Abstraction;
using MangaK_System.DAL.Entity.Enums;

namespace Mangak_System._1_DAL.Entity;

public class Series : BaseEntity<Guid>, IAuditableEntity
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public string? CoverFile { get; set; }
    public string? NameFile { get; set; }
    public string? NameFilePublicId { get; set; }
    public SeriesStatus Status { get; set; }

    public Guid CreatedById { get; set; }
    public User CreatedBy { get; set; } = null!;

    public Guid? ApprovedById { get; set; }
    public User? ApprovedBy { get; set; }

    public Guid? ReviewedById { get; set; }
    public User? ReviewedBy { get; set; }

    // Quan hệ 1-1
    public PublishingSchedule? PublishingSchedule { get; set; }
    // Quan hệ nhiều-nhiều
    public ICollection<CategorySeries> CategorySeries { get; set; } = new List<CategorySeries>();
    // Feedback
    public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
