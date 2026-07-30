using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MangaK_System.DAL.Entity.Enums;
using Mangak_System._1_DAL.Data;

namespace MangaK_System.BLL.PublishingSchedule
{
    public class PublishingScheduleService : IPublishingScheduleService
    {
        private readonly AppDbContext _dbContext;

        public PublishingScheduleService(AppDbContext context)
        {
            _dbContext = context;
        }

        public async Task<System.Collections.Generic.List<GetPublishingScheduleResponse>> GetAllPublishingSchedulesAsync(Guid userId)
        {
            var user = await _dbContext.Set<Mangak_System._1_DAL.Entity.User>().FirstOrDefaultAsync(u => u.Id == userId && u.Status == UserStatus.Active);
            if (user == null) throw new UnauthorizedAccessException("User not found or not active.");

            var allowedRoles = new[] { UserRole.Editorial, UserRole.Tantou, UserRole.Admin };
            if (!allowedRoles.Contains(user.Role))
                throw new UnauthorizedAccessException("Only EditorialBoard, TantouEditor, Admin can view all publishing schedules.");

            var schedule = await _dbContext.Set<Mangak_System._1_DAL.Entity.PublishingSchedule>()
                .Include(p => p.Series)
                .ThenInclude(s => s.CreatedBy)
                .Include(p => p.DecidedBy)
                .OrderBy(p => p.PublishDate)
                .ToListAsync();

            return schedule.Select(p => new GetPublishingScheduleResponse()
            {
                ScheduleId      = p.Id,
                SeriesId        = p.SeriesId,
                SeriesTitle     = p.Series.Title,
                SeriesCoverFile = p.Series.CoverFile,
                SeriesStatus    = p.Series.Status,
                MangakaName     = p.Series.CreatedBy.AuthorName ?? $"{p.Series.CreatedBy.FirstName} {p.Series.CreatedBy.LastName}",
                PublishDate     = p.PublishDate,
                PublishPeriod   = p.PublishPeriod,
                DecidedByName   = p.DecidedBy != null ? $"{p.DecidedBy.FirstName} {p.DecidedBy.LastName}" : string.Empty,
                CreatedAt       = p.CreatedAt,
                UpdatedAt       = p.UpdatedAt
            }).ToList();
        }

        public async Task<bool> CreatePublishingScheduleAsync(Guid seriesId, DateTime publishDate, string publishPeriod, Guid decidedById)
        {
            var board = await _dbContext.Set<Mangak_System._1_DAL.Entity.User>().FindAsync(decidedById);
            if (board == null) throw new UnauthorizedAccessException("User not found");
            
            if (board.Role != UserRole.Editorial)
                throw new UnauthorizedAccessException("Only Editorial Board can create PublishSchedule.");

            var series = await _dbContext.Set<Mangak_System._1_DAL.Entity.Series>()
                .Include(s => s.PublishingSchedule)
                .FirstOrDefaultAsync(s => s.Id == seriesId);

            if (series == null) throw new KeyNotFoundException("Series not found");
            
            if (series.Status != SeriesStatus.Approved)
                throw new InvalidOperationException($"Series must be in approved status. Current: {series.Status}");

            if (series.PublishingSchedule != null)
                throw new InvalidOperationException("Publishing Schedule already exists");
            
            if (publishDate <= DateTime.UtcNow)
                throw new ArgumentException("Publish date must be in the future");

            // Validate based on period
            string period = publishPeriod.ToLower();
            if (period == "weekly" && publishDate < DateTime.UtcNow.AddDays(7))
            {
                throw new ArgumentException("For Weekly series, Publish Date must be at least 7 days from today.");
            }
            if (period == "monthly" && publishDate < DateTime.UtcNow.AddDays(30))
            {
                throw new ArgumentException("For Monthly series, Publish Date must be at least 30 days from today.");
            }

            var schedule = new Mangak_System._1_DAL.Entity.PublishingSchedule
            {
                Id = Guid.NewGuid(),
                SeriesId = seriesId,
                PublishPeriod = publishPeriod,
                PublishDate = publishDate,
                DecidedById = decidedById,
                CreatedAt = DateTimeOffset.UtcNow
            };

            series.PublishingSchedule = schedule;
            series.Status = SeriesStatus.Scheduled;
            series.UpdatedAt = DateTimeOffset.UtcNow;

            var statusChangeFeedback = new Mangak_System._1_DAL.Entity.Feedback
            {
                Id        = Guid.NewGuid(),
                SenderId  = decidedById,
                Content   = "Editorial Board has scheduled series " + series.Title,
                SeriesId  = series.Id,
                Type      = FeedbackType.StatusChange,
                CreatedAt = DateTimeOffset.UtcNow,
                IsRead    = false
            };
            
            _dbContext.Set<Mangak_System._1_DAL.Entity.PublishingSchedule>().Add(schedule);
            _dbContext.Set<Mangak_System._1_DAL.Entity.Feedback>().Add(statusChangeFeedback);
            
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
