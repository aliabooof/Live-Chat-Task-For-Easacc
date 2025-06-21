using LiveChat.Application.Interfaces;
using LiveChat.Domain.Entities;
using LiveChat.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveChat.Application.Dtos;

namespace LiveChat.Infrastructure.Services
{
    public class ChatService : IChatService
    {
        private readonly ChatDbContext _context;

        public ChatService(ChatDbContext context)
        {
            _context = context;
        }
        public async Task<User?> CreateOrGetUserAsync(string username)
        {
            var user = _context.Users.FirstOrDefault(u=>u.UserName == username);
            if (user == null) 
            {
                user = new User() { 
                    UserName = username,
                    ID = Guid.NewGuid(),
                    IsAdmin = false,
                    IsOnline = true,
                };
               await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
            }
            return user;
            
        }

        public async Task<List<User>> GetActiveUsersAsync()=>
            await _context.Users.Where(u => u.IsOnline && !u.IsAdmin).ToListAsync();


        public async Task<List<MessageHistoryDto>> GetChatHistoryAsync(Guid userId, Guid contactId) =>
            await _context.Messages.Where(m => (m.SenderId == userId && m.ReceiverId == contactId) ||
                    (m.SenderId == contactId && m.ReceiverId == userId))
        .OrderBy(m => m.Timestamp)
        .Select(m => new MessageHistoryDto
        {
            Id = m.Id,
            Content = m.Content,
            Type = m.Type,
            Timestamp = m.Timestamp,
            IsSent = m.IsSent,
            IsSeen = m.IsSeen,
            SenderId = m.SenderId,
            ReceiverId = m.ReceiverId,
            FileName = m.FileName,
            FilePath = m.FilePath,
            VoiceDurationSeconds = m.VoiceDurationSeconds,
            Sender = new UserDto
            {
                ID = m.Sender.ID,
                UserName = m.Sender.UserName,
                IsAdmin = m.Sender.IsAdmin,
                IsOnline = m.Sender.IsOnline,
                LastSeen = m.Sender.LastSeen
            },
            Receiver = new UserDto
            {
                ID = m.Receiver.ID,
                UserName = m.Receiver.UserName,
                IsAdmin = m.Receiver.IsAdmin,
                IsOnline = m.Receiver.IsOnline,
                LastSeen = m.Receiver.LastSeen
            }
        })
        .ToListAsync();

        

        public async Task<User?> GetUserByConnectionIdAsync(string connectionId)=>
            await _context.Users.FirstOrDefaultAsync(u => u.ConnectionId == connectionId);
        

        public async Task MarkMessageAsSeenAsync(Guid messageId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if(message != null)
            {
                message.IsSeen = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ChatMessage> SaveMessageAsync(ChatMessage message)
        {
            message.Id = Guid.NewGuid();
            message.Timestamp = DateTime.UtcNow;
            message.IsSent = true;

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .FirstAsync(m => m.Id == message.Id);
        }

        public async Task UpdateUserConnectionAsync(Guid userId, string? connectionId)
        {
            var user = await _context.Users.FindAsync(userId);
            if(user != null)
            {
                user.ConnectionId = connectionId;
                user.IsOnline = !String.IsNullOrEmpty(connectionId);
                user.LastSeen = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

    }
}
