using System;
using System.Threading.Tasks;

namespace MangaK_System.BLL.PublishingSchedule
{
    public interface IPublishingScheduleService
    {
        Task<bool> CreatePublishingScheduleAsync(Guid seriesId, DateTime publishDate, string publishPeriod, Guid decidedById);
        Task<System.Collections.Generic.List<GetPublishingScheduleResponse>> GetAllPublishingSchedulesAsync(Guid userId);
    }
}
