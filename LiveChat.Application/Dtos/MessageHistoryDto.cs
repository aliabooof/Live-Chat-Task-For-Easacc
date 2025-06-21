using LiveChat.Domain.Enums;

namespace LiveChat.Application.Dtos

{
    public class MessageHistoryDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public MessageType Type { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsSent { get; set; }
        public bool IsSeen { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public int? VoiceDurationSeconds { get; set; }

        public UserDto Sender { get; set; } = null!;
        public UserDto Receiver { get; set; } = null!;
    }
}
