using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Mangak_System._1_DAL.Entity;

namespace MangaK_System.BLL.Feedback
{
    public interface IFeedbackService
    {
        Task<List<Mangak_System._1_DAL.Entity.Feedback>> GetFeedbackListAsync(Guid seriesId, Guid userId);
    }
}
