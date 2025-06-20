using Live_Chat.Hubs;
using LiveChat.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Live_Chat.Services
{
    public class ChatNotifier:IChatNotifier
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatNotifier(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task SendInactivityWarningAsync(string connectionId, string message)
        {
            return _hubContext.Clients.Client(connectionId).SendAsync("InactivityWarning", message);
        }
    }

}
