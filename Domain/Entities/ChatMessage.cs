using LiveChat.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveChat.Domain.Entities
{
    public class ChatMessage
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public MessageType Type { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsSent { get; set; }
        public bool IsSeen { get; set; }
        public Guid SenderId { get; set; }
        public User Sender { get; set; } = null!;
        public Guid ReceiverId { get; set; }
        public User Receiver { get; set; } = null!;
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public int? VoiceDurationSeconds { get; set; }
    }

   
}
