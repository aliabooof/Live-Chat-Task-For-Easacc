using Microsoft.AspNetCore.Http;

namespace LiveChat.Application.Interfaces
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file, string folder);
        bool IsValidImage(IFormFile file);
        bool IsValidDocument(IFormFile file);
        bool IsValidVoice(IFormFile file);
    }
}
