namespace LiveChat.Application.Dtos

{
    public class UserDto
    {
        public Guid ID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }
        public bool IsOnline { get; set; }
        public DateTime LastSeen { get; set; }
        public string? ConnectionId { get; set; }
    }
}
