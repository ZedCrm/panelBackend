using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp.Processing;

namespace App.utility
{
    public interface IFileService
    {
             Task<string?> UploadAsync(
            IFormFile? file,
            string folderPath,
            string? existingUrl = null,
            ResizeOptions? resizeOptions = null);
    }
}