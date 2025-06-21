
using LiveChat.Application.Dtos;
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

        public async Task<UserDto> JoinChat(string username, bool isAdmin = false)
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

                return new UserDto
                {
                    ID = user.ID,
                    UserName = user.UserName,
                    IsAdmin = user.IsAdmin,
                    IsOnline = user.IsOnline,
                    LastSeen = user.LastSeen
                    
                }; 
            }
            return null;
        }

        public async Task SendMessage(string receiverUsername, string content, string messageType)
        {
            var senderConnectionId = Context.ConnectionId;
            var sender = await _chatService.GetUserByConnectionIdAsync(senderConnectionId);
            if (sender == null) return;

            var receiver = await _chatService.CreateOrGetUserAsync(receiverUsername);
            if (receiver == null) return;

            var type = Enum.TryParse(typeof(MessageType), messageType, true, out var parsedObj)
                        ? (MessageType)parsedObj
                        : MessageType.Text;


            var message = new ChatMessage
            {
                Id = Guid.NewGuid(),
                Content = content,
                Type = type,
                Timestamp = DateTime.UtcNow,
                IsSent = true,
                IsSeen = false,
                SenderId = sender.ID,
                ReceiverId = receiver.ID,
                Sender = sender,
                Receiver = receiver
            };

            await _chatService.SaveMessageAsync(message);

           
            var messageDto = new MessageHistoryDto
            {
                Id = message.Id,
                Content = message.Content,
                Type = message.Type,
                Timestamp = message.Timestamp,
                IsSent = message.IsSent,
                IsSeen = message.IsSeen,
                SenderId = sender.ID,
                ReceiverId = receiver.ID,
                FileName = message.FileName,
                FilePath = message.FilePath,
                VoiceDurationSeconds = message.VoiceDurationSeconds,
                Sender = new UserDto
                {
                    ID = sender.ID,
                    UserName = sender.UserName,
                    IsAdmin = sender.IsAdmin,
                    IsOnline = sender.IsOnline,
                    LastSeen = sender.LastSeen,
                    ConnectionId = sender.ConnectionId
                },
                Receiver = new UserDto
                {
                    ID = receiver.ID,
                    UserName = receiver.UserName,
                    IsAdmin = receiver.IsAdmin,
                    IsOnline = receiver.IsOnline,
                    LastSeen = receiver.LastSeen,
                    ConnectionId = receiver.ConnectionId
                }
            };

            
            if (!string.IsNullOrEmpty(receiver.ConnectionId))
            {
                await Clients.Client(receiver.ConnectionId).SendAsync("ReceiveMessage", messageDto);
            }

            await Clients.Client(senderConnectionId).SendAsync("MessageSent", messageDto);
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

