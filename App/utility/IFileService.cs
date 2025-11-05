using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace App.utility
{
    public interface IFileService
    {
        Task<string?> UploadProfilePictureAsync(IFormFile? file, string? oldPath = null);
    }
}