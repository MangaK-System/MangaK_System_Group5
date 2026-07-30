using System;
using System.Threading.Tasks;

namespace MangaK_System.BLL.Series
{
    public interface ISeriesService
    {
        Task<Mangak_System._1_DAL.Entity.Series> CreateSeriesAsync(string title, string description, string? coverFile, string? nameFile, string? nameFilePublicId, Guid createdById);
        Task<System.Collections.Generic.List<GetAllSeriesResponse>> GetAllSeriesAsync(Guid userId);
        Task<bool> ReviewSeriesByTantouEditorAsync(Guid seriesId, Guid editorId, bool isApproved, string? note = null);
        Task<bool> ReviewSeriesByEditorialBoardAsync(Guid seriesId, Guid boardId, bool isApproved, string? note = null);
    }
}
