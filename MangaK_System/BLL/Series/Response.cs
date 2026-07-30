using System;
using System.Collections.Generic;
using MangaK_System.DAL.Entity.Enums;

namespace MangaK_System.BLL.Series
{
    public class GetAllSeriesResponse
    {
        public Guid SeriesId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string? NameFile { get; set; }
        public List<string> Categories { get; set; } = new List<string>();
        public string? CoverFile { get; set; }
        public SeriesStatus Status { get; set; }
        public string MangakaName { get; set; } = null!;
        public DateTimeOffset CreateAt { get; set; }
    }
}
