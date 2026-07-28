using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MangaK_System.BLL.MediaService;

public interface IService
{
    Task<string> UploadImageAsync(string filePath);
    Task<(string FileUrl, string PublicId)> UploadFileAsync(string filePath);
}