using LiveChat.Application.Interfaces;
using LiveChat.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Live_Chat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IFileService _fileService;

        public ChatController(IChatService chatService, IFileService fileService)
        {
            _chatService = chatService;
            _fileService = fileService;
        }

        [HttpGet("active-users")]
        public async Task<ActionResult<List<User>>> GetActiveUsers()
        {
            var users = await _chatService.GetActiveUsersAsync();
            return Ok(users);
        }

        [HttpGet("history/{userId}/{contactId}")]
        public async Task<ActionResult<List<ChatMessage>>> GetChatHistory(Guid userId, Guid contactId)
        {
            var messages = await _chatService.GetChatHistoryAsync(userId, contactId);
            return Ok(messages);
        }

        [HttpPost("upload")]
        public async Task<ActionResult<string>> UploadFile(IFormFile file, [FromForm] string type)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            var isValid = type.ToLower() switch
            {
                "image" => _fileService.IsValidImage(file),
                "document" => _fileService.IsValidDocument(file),
                "voice" => _fileService.IsValidVoice(file),
                _ => false
            };

            if (!isValid)
                return BadRequest("Invalid file type or size");

            var filePath = await _fileService.SaveFileAsync(file, type);
            return Ok(new { filePath });
        }

        [HttpGet("settings")]
        public ActionResult<ChatSettings> GetSettings()
        {
            return Ok(new ChatSettings());
        }
    }
}
    

