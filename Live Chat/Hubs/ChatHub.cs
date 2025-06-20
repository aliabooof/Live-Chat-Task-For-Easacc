using LiveChat.Application.Interfaces;
using LiveChat.Domain.Entities;
using LiveChat.Domain.Enums;
using Microsoft.AspNetCore.SignalR;

namespace Live_Chat.Hubs
{
    public class ChatHub:Hub
    {
        private readonly IChatService _chatService;
        private readonly IUserConnectionService _connectionService;

        public ChatHub(IChatService chatService, IUserConnectionService connectionService)
        {
            _chatService = chatService;
            _connectionService = connectionService;
        }

        public async Task JoinChat(string username, bool isAdmin = false)
        {
            var user = await _chatService.CreateOrGetUserAsync(username);
            if (user != null)
            {
                await _chatService.UpdateUserConnectionAsync(user.ID, Context.ConnectionId);
                _connectionService.AddConnection(user.ID, Context.ConnectionId);

                await Groups.AddToGroupAsync(Context.ConnectionId, isAdmin ? "Admins" : "Users");

                if (!isAdmin)
                {
                    _connectionService.StartInactivityTimer(user.ID, Context.ConnectionId);
                }

                await Clients.Group("Admins").SendAsync("UserJoined", user);
            }
        }

        public async Task SendMessage(string receiverUsername, string content, string messageType)
        {
            var sender = await _chatService.GetUserByConnectionIdAsync(Context.ConnectionId);
            if (sender == null) return;

            var receiver = await _chatService.CreateOrGetUserAsync(receiverUsername);
            if (receiver == null) return;

            var message = new ChatMessage
            {
                Content = content,
                Type = Enum.Parse<MessageType>(messageType),
                SenderId = sender.ID,
                ReceiverId = receiver.ID
            };

            var savedMessage = await _chatService.SaveMessageAsync(message);

           
            if (!sender.IsAdmin)
            {
                _connectionService.ResetInactivityTimer(sender.ID, Context.ConnectionId);
            }

            
            var receiverConnections = _connectionService.GetConnectionIds(receiver. ID);
            foreach (var connId in receiverConnections)
            {
                await Clients.Client(connId).SendAsync("ReceiveMessage", savedMessage);
            }

           
            await Clients.Caller.SendAsync("MessageSent", savedMessage);
        }

        public async Task MarkMessageSeen(string messageId)
        {
            if (Guid.TryParse(messageId, out var id))
            {
                await _chatService.MarkMessageAsSeenAsync(id);
                await Clients.All.SendAsync("MessageSeen", messageId);
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var user = await _chatService.GetUserByConnectionIdAsync(Context.ConnectionId);
            if (user != null)
            {
                _connectionService.RemoveConnection(Context.ConnectionId);
                await _chatService.UpdateUserConnectionAsync(user.ID, null);
                await Clients.Group("Admins").SendAsync("UserLeft", user);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}

