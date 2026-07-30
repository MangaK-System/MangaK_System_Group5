using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MangaK_System.BLL.MediaService;
using Microsoft.Extensions.Configuration;

namespace MangaK_System.BLL.CloudinaryService;

public class Service : IService
{
    private readonly Cloudinary _cloudinary;
    private readonly CloudinaryOptions _cloudinaryOptions = new();

    public Service(IConfiguration configuration)
    {
        configuration.GetSection(nameof(CloudinaryOptions)).Bind(_cloudinaryOptions);

        _cloudinary = new Cloudinary(new Account(
            _cloudinaryOptions.CloudName,
            _cloudinaryOptions.ApiKey,
            _cloudinaryOptions.ApiSecret));
    }

    public async Task<string> UploadImageAsync(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path is empty.");

        if (!File.Exists(filePath))
            throw new FileNotFoundException("File not found.");

        if (!IsImageFile(filePath))
            throw new ArgumentException("Invalid image file.");

        await using var stream = File.OpenRead(filePath);

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(Path.GetFileName(filePath), stream)
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.Error != null)
            throw new Exception(uploadResult.Error.Message);

        return uploadResult.SecureUrl.ToString();
    }

    private bool IsImageFile(string filePath)
    {
        var allowedExtensions = new[]
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".gif",
            ".webp"
        };

        var extension = Path.GetExtension(filePath).ToLowerInvariant();

        return allowedExtensions.Contains(extension);
    }

    public async Task<(string FileUrl, string PublicId)> UploadFileAsync(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("File not found.");

        await using var stream = File.OpenRead(filePath);

        var uploadParams = new RawUploadParams
        {
            File = new FileDescription(Path.GetFileName(filePath), stream),
            Folder = "mangaksystem/documents",
            UseFilename = true,
            UniqueFilename = true
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.Error != null)
            throw new Exception(uploadResult.Error.Message);

        return
        (
            uploadResult.SecureUrl.ToString(),
            uploadResult.PublicId
        );
    }

}
