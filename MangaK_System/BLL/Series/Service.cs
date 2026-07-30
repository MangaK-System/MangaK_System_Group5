using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MangaK_System.DAL.Entity.Enums;
using Mangak_System._1_DAL.Entity;
using Mangak_System._1_DAL.Data;

namespace MangaK_System.BLL.Series
{
    public class SeriesService : ISeriesService
    {
        private readonly AppDbContext _dbContext;

        public SeriesService(AppDbContext context)
        {
            _dbContext = context;
        }

        public async Task<Mangak_System._1_DAL.Entity.Series> CreateSeriesAsync(string title, string description, string? coverFile, string? nameFile, string? nameFilePublicId, Guid createdById)
        {
            var series = new Mangak_System._1_DAL.Entity.Series
            {
                Id = Guid.NewGuid(),
                Title = title,
                Description = description,
                CoverFile = coverFile,
                NameFile = nameFile,
                NameFilePublicId = nameFilePublicId,
                CreatedById = createdById,
                Status = SeriesStatus.Processing,
                CreatedAt = DateTimeOffset.UtcNow
            };

            _dbContext.Set<Mangak_System._1_DAL.Entity.Series>().Add(series);
            await _dbContext.SaveChangesAsync();

            return series;
        }

        public async Task<System.Collections.Generic.List<GetAllSeriesResponse>> GetAllSeriesAsync(Guid userId)
        {
            var user = await _dbContext.Set<Mangak_System._1_DAL.Entity.User>().FindAsync(userId);
            
            var query = _dbContext.Set<Mangak_System._1_DAL.Entity.Series>()
                .Include(s => s.CreatedBy)
                .Include(s => s.CategorySeries)
                .ThenInclude(cs => cs.Category) 
                .OrderByDescending(s => s.CreatedAt)
                .AsQueryable();

            if (user != null)
            {
                if (user.Role == UserRole.Mangaka)
                    query = query.Where(s => s.CreatedById == userId);

                if (user.Role == UserRole.Tantou)
                    query = query.Where(s => s.CreatedBy.SupervisorId == userId);
            }
            
            var seriesList = await query.ToListAsync();
            
            return seriesList.Select(s => new GetAllSeriesResponse()
            {
                SeriesId = s.Id,
                Title = s.Title,
                Categories = s.CategorySeries.Select(cs => cs.Category.Name).ToList(),
                CoverFile = s.CoverFile,
                Status = s.Status,
                MangakaName = s.CreatedBy.AuthorName ?? $"{s.CreatedBy.FirstName} {s.CreatedBy.LastName}",
                CreateAt = s.CreatedAt
            }).ToList();
        }
        public async Task<bool> ReviewSeriesByTantouEditorAsync(Guid seriesId, Guid editorId, bool isApproved, string? note = null)
        {
            var editor = await _dbContext.Set<Mangak_System._1_DAL.Entity.User>().FindAsync(editorId);
            if(editor == null || editor.Role != UserRole.Tantou)
                throw new UnauthorizedAccessException("Only Tantou Editor can review series at this stage");

            var series = await _dbContext.Set<Mangak_System._1_DAL.Entity.Series>()
                .Include(s => s.CreatedBy)
                .FirstOrDefaultAsync(s => s.Id == seriesId);
            
            if(series == null) throw new KeyNotFoundException("Series not found");

            if (series.CreatedBy.SupervisorId != editorId)
                throw new UnauthorizedAccessException("You are not assigned to review this series.");

            if(series.Status != SeriesStatus.Processing)
                throw new InvalidOperationException($"Series must be in processing status. Current: {series.Status}");

            if (series.ReviewedById != null && series.ReviewedById != editorId)
                throw new UnauthorizedAccessException("This series is already being handled by another Tantou Editor.");

            if (!isApproved && string.IsNullOrWhiteSpace(note))
                throw new ArgumentException("Feedback note is required when rejecting a series.");

            if (isApproved)
            {
                series.Status = SeriesStatus.Pending;
                var statusFeedback = new Mangak_System._1_DAL.Entity.Feedback
                {
                    Id = Guid.NewGuid(),
                    SenderId = editor.Id,
                    Content = "Tantou submitted series to Editorial Board",
                    SeriesId = series.Id,
                    Type = FeedbackType.StatusChange,
                    CreatedAt = DateTimeOffset.UtcNow,
                    IsRead = false
                };
                _dbContext.Set<Mangak_System._1_DAL.Entity.Feedback>().Add(statusFeedback);
            }
            else
            {
                series.Status = SeriesStatus.Rejected;
                var statusFeedback = new Mangak_System._1_DAL.Entity.Feedback
                {
                    Id = Guid.NewGuid(),
                    SenderId = editor.Id,
                    Content = "Tantou rejected series " + series.Title,
                    SeriesId = series.Id,
                    Type = FeedbackType.StatusChange,
                    CreatedAt = DateTimeOffset.UtcNow,
                    IsRead = false
                };
                _dbContext.Set<Mangak_System._1_DAL.Entity.Feedback>().Add(statusFeedback);
            }

            series.ReviewedById = editorId;
            series.UpdatedAt = DateTimeOffset.UtcNow;

            if (!string.IsNullOrWhiteSpace(note))
            {
                var feedback = new Mangak_System._1_DAL.Entity.Feedback
                {
                    Id = Guid.NewGuid(),
                    SenderId = editor.Id,
                    Content = note,
                    SeriesId = series.Id,
                    Type = FeedbackType.Manual,
                    CreatedAt = DateTimeOffset.UtcNow,
                    IsRead = false
                };
                _dbContext.Set<Mangak_System._1_DAL.Entity.Feedback>().Add(feedback);
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReviewSeriesByEditorialBoardAsync(Guid seriesId, Guid boardId, bool isApproved, string? note = null)
        {
            var board = await _dbContext.Set<Mangak_System._1_DAL.Entity.User>().FindAsync(boardId);
            if(board == null || board.Role != UserRole.Editorial)
                throw new UnauthorizedAccessException("Only Editorial Board can review series at this stage");

            var series = await _dbContext.Set<Mangak_System._1_DAL.Entity.Series>().FindAsync(seriesId);
            if(series == null) throw new KeyNotFoundException("Series not found");

            if(series.Status != SeriesStatus.Pending)
                throw new InvalidOperationException($"Series must be pending review from Tantou Editor first. Current: {series.Status}");

            if (!isApproved && string.IsNullOrWhiteSpace(note))
                throw new ArgumentException("Feedback note is required when rejecting a series.");

            if (isApproved)
            {
                series.Status = SeriesStatus.Approved;
                series.ApprovedById = boardId;
                var statusFeedback = new Mangak_System._1_DAL.Entity.Feedback
                {
                    Id = Guid.NewGuid(),
                    SenderId = board.Id,
                    Content = "Editorial Board has approved series " + series.Title,
                    SeriesId = series.Id,
                    Type = FeedbackType.StatusChange,
                    CreatedAt = DateTimeOffset.UtcNow,
                    IsRead = false
                };
                _dbContext.Set<Mangak_System._1_DAL.Entity.Feedback>().Add(statusFeedback);
            }
            else
            {
                series.Status = SeriesStatus.Rejected;
                var statusFeedback = new Mangak_System._1_DAL.Entity.Feedback
                {
                    Id = Guid.NewGuid(),
                    SenderId = board.Id,
                    Content = "Editorial Board has rejected series " + series.Title,
                    SeriesId = series.Id,
                    Type = FeedbackType.StatusChange,
                    CreatedAt = DateTimeOffset.UtcNow,
                    IsRead = false
                };
                _dbContext.Set<Mangak_System._1_DAL.Entity.Feedback>().Add(statusFeedback);
            }

            series.UpdatedAt = DateTimeOffset.UtcNow;

            if (!string.IsNullOrWhiteSpace(note))
            {
                var feedback = new Mangak_System._1_DAL.Entity.Feedback
                {
                    Id = Guid.NewGuid(),
                    SenderId = board.Id,
                    Content = note,
                    SeriesId = series.Id,
                    Type = FeedbackType.Manual,
                    CreatedAt = DateTimeOffset.UtcNow,
                    IsRead = false
                };
                _dbContext.Set<Mangak_System._1_DAL.Entity.Feedback>().Add(feedback);
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
