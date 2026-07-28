using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MangaK_System.DAL.Abstraction;
using MangaK_System.DAL.Entity.Enums;

namespace Mangak_System._1_DAL.Entity;

public class User : BaseEntity<Guid>, IAuditableEntity
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public required string PasswordHash { get; set; }
    public string? Phone { get; set; }
    public string? AvatarUrl { get; set; }
    public string? AuthorName { get; set; }
    public string? Bio { get; set; }
    public UserRole Role { get; set; }
    public UserStatus Status { get; set; }
    // Tantou quản lý Mangaka
    public Guid? SupervisorId { get; set; }
    public User? Supervisor { get; set; }
    // Navigation
    public ICollection<Series> CreatedSeries { get; set; } = new List<Series>();
    public ICollection<Series> ApprovedSeries { get; set; } = new List<Series>();
    public ICollection<Series> ReviewedSeries { get; set; } = new List<Series>();
    public ICollection<PublishingSchedule> PublishingSchedules { get; set; } = new List<PublishingSchedule>();
    public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
    public ICollection<User> Mangakas { get; set; } = new List<User>();
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}