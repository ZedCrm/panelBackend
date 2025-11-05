using App.utility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Threading.Tasks;

namespace API.utility
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;
        private const int MaxFileSize = 2 * 1024 * 1024; // 2MB
        private const int ImageSize = 200;

        public FileService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string?> UploadProfilePictureAsync(IFormFile? file, string? oldPath = null)
        {
            if (file == null || file.Length == 0) return null;

            if (file.Length > MaxFileSize)
                throw new InvalidOperationException("حجم فایل بیش از 2 مگابایت است.");

            if (!file.ContentType.StartsWith("image/"))
                throw new InvalidOperationException("فقط فایل‌های تصویری مجاز هستند.");

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using var image = await Image.LoadAsync(file.OpenReadStream());
            image.Mutate(x => x.Resize(ImageSize, ImageSize));
            await image.SaveAsync(filePath);

            if (!string.IsNullOrEmpty(oldPath))
            {
                var oldFilePath = Path.Combine(_env.WebRootPath, oldPath.TrimStart('/'));
                if (File.Exists(oldFilePath))
                    File.Delete(oldFilePath);
            }

            return $"/uploads/{fileName}";
        }
    }
}