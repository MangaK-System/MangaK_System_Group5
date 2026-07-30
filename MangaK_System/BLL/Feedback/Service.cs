using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mangak_System._1_DAL.Data;
using Mangak_System._1_DAL.Entity;
using MangaK_System.DAL.Entity.Enums;
using Microsoft.EntityFrameworkCore;

namespace MangaK_System.BLL.Feedback
{
    public class FeedbackService : IFeedbackService
    {
        private readonly AppDbContext _context;

        public FeedbackService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Mangak_System._1_DAL.Entity.Feedback>> GetFeedbackListAsync(Guid seriesId, Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new UnauthorizedAccessException("User not found.");

            var series = await _context.Series
                .Include(s => s.CreatedBy)
                .FirstOrDefaultAsync(s => s.Id == seriesId);

            if (series == null)
                throw new KeyNotFoundException("Series not found.");

            bool canView = false;

            if (user.Role == UserRole.Admin || user.Role == UserRole.Editorial)
            {
                canView = true;
            }
            else if (user.Role == UserRole.Mangaka)
            {
                canView = (series.CreatedById == userId);
            }
            else if (user.Role == UserRole.Tantou)
            {
                canView = (series.CreatedBy != null && series.CreatedBy.SupervisorId == userId);
            }

            if (!canView)
                throw new UnauthorizedAccessException("You are not authorized to view feedbacks for this series.");

            var feedbacks = await _context.Feedbacks
                .Include(f => f.Sender)
                .Where(f => f.SeriesId == seriesId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            return feedbacks;
        }

        public async Task<Mangak_System._1_DAL.Entity.Feedback> SendFeedbackAsync(Guid seriesId, Guid senderId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Feedback content cannot be empty.");

            var user = await _context.Users.FindAsync(senderId);
            if (user == null)
                throw new UnauthorizedAccessException("User not found.");

            var series = await _context.Series.FindAsync(seriesId);
            if (series == null)
                throw new KeyNotFoundException("Series not found.");

            var feedback = new Mangak_System._1_DAL.Entity.Feedback
            {
                Id = Guid.NewGuid(),
                SeriesId = seriesId,
                SenderId = senderId,
                Content = content,
                Type = FeedbackType.Manual,
                CreatedAt = DateTimeOffset.UtcNow,
                IsRead = false
            };

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            return feedback;
        }
    }
}
