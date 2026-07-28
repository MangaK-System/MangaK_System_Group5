using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace MangaK_System.BLL.CloudinaryService;

public record CloudinaryOptions
{
    [Required] public string CloudName { get; set; } = string.Empty;
    [Required] public string ApiKey { get; set; } = string.Empty;
    [Required] public string ApiSecret { get; set; } = string.Empty;
}
