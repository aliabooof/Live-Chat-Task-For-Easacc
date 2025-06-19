using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveChat.Domain.Entities
{
    public class User
    {
        public Guid ID { get; set; }
        public string UserName { get; set; } = string.Empty;

        public bool IsAdmin { get; set; }
        public bool IsOnline { get; set; }
        public DateTime LastSeen { get; set; }
        public string? ConnectionId { get; set; }
        public List<ChatMessage> SentMessages { get; set; } = new();
        public List<ChatMessage> ReceivedMessages { get; set; } = new();

    }
}
