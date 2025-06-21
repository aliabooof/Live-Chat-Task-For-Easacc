using LiveChat.Domain.Entities;

namespace Live_Chat.Dtos
{
    public class ChatMessageDto
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public string MessageType { get; set; }
        public string SenderUsername { get; set; }
        public string ReceiverUsername { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsSeen { get; set; }


        public static ChatMessageDto ToDto(ChatMessage message, string senderUsername, string receiverUsername)
        {
            return new ChatMessageDto
            {
                Id = message.Id.ToString(),
                Content = message.Content,
                MessageType = message.Type.ToString(),
                SenderUsername = senderUsername,
                ReceiverUsername = receiverUsername,
                Timestamp = message.Timestamp,
                IsSeen = message.IsSeen
            };
        }
    }
}