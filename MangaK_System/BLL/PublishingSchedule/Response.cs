using System;
using MangaK_System.DAL.Entity.Enums;

namespace MangaK_System.BLL.PublishingSchedule
{
    public class GetPublishingScheduleResponse
    {
        public Guid ScheduleId { get; set; }
        public Guid SeriesId { get; set; }
        public string SeriesTitle { get; set; } = null!;
        public string? SeriesCoverFile { get; set; }
        public SeriesStatus SeriesStatus { get; set; }
        public string MangakaName { get; set; } = null!;
        public DateTime PublishDate { get; set; }
        public string? PublishPeriod { get; set; }
        public string DecidedByName { get; set; } = null!;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
