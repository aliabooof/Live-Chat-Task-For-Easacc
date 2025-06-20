using LiveChat.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveChat.Infrastructure.Services
{
    public class FileService : IFileService
    {
        private readonly string[] _allowedImageTypes = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private readonly string[] _allowedDocTypes = { ".pdf", ".doc", ".docx", ".txt", ".xlsx", ".pptx" };
        private readonly string[] _allowedVoiceTypes = { ".mp3", ".wav", ".ogg", ".m4a" };
        public bool IsValidDocument(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return _allowedDocTypes.Contains(extension) && file.Length < 10 * 1024 * 1024;
        }

        public bool IsValidImage(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return _allowedImageTypes.Contains(extension) && file.Length < 10 * 1024 * 1024; 
        }

        public bool IsValidVoice(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return _allowedVoiceTypes.Contains(extension) && file.Length < 25 * 1024 * 1024; 
        }

        public async Task<string> SaveFileAsync(IFormFile file, string folder)
        {
            var uploadsFolder = Path.Combine("wwwroot", "uploads", folder);
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/{folder}/{uniqueFileName}";
        }
    }
}
