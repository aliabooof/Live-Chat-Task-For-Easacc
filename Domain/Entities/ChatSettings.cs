using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveChat.Domain.Entities
{
    public class ChatSettings
    {
        public int MaxCharacters { get; set; } = 500;
        public int MaxVoiceMinutes { get; set; } = 5;
        public int InactivityTimeoutMinutes { get; set; } = 1;
    }
}
