// API/utility/FileService.cs
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
        private const int MaxFileSize = 10 * 1024 * 1024; // 10MB (قابل تنظیم)
        private static readonly string[] AllowedImageTypes = { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };

        public FileService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string?> UploadAsync(
            IFormFile? file,
            string folderPath,
            string? existingUrl = null,
            ResizeOptions? resizeOptions = null)
        {
            // اگر فایل نباشد
            if (file == null || file.Length == 0)
                return null;

            // اعتبارسنجی حجم
            if (file.Length > MaxFileSize)
                throw new InvalidOperationException("حجم فایل نمی‌تواند بیشتر از 10 مگابایت باشد.");

            // مسیر کامل پوشه
            var fullFolderPath = Path.Combine(_env.WebRootPath, folderPath.Trim('/'));
            Directory.CreateDirectory(fullFolderPath);

            // پسوند فایل
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(extension))
                extension = ".bin"; // fallback

            // نام منحصربه‌فرد
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(fullFolderPath, fileName);

            // اگر فایل تصویر باشد و resizeOptions داده شده باشد
            if (IsImage(file.ContentType) && resizeOptions != null)
            {
                try
                {
                    using var image = await Image.LoadAsync(file.OpenReadStream());
                    image.Mutate(x => x.Resize(resizeOptions));
                    await image.SaveAsync(filePath);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"خطا در پردازش تصویر: {ex.Message}", ex);
                }
            }
            else
            {
                // ذخیره مستقیم فایل (غیرتصویری یا بدون resize)
                using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);
            }

            // حذف فایل قبلی
            if (!string.IsNullOrWhiteSpace(existingUrl))
            {
                var oldFilePath = Path.Combine(_env.WebRootPath, existingUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(oldFilePath) && !oldFilePath.Equals(filePath, StringComparison.OrdinalIgnoreCase))
                {
                    try { File.Delete(oldFilePath); }
                    catch { /* نادیده بگیر */ }
                }
            }

            // برگرداندن مسیر نسبی
            return $"/{folderPath.Trim('/')}/{fileName}";
        }

        private static bool IsImage(string contentType)
        {
            return AllowedImageTypes.Contains(contentType.ToLowerInvariant());
        }
    }
}