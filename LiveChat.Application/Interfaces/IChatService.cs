using LiveChat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveChat.Application.Interfaces
{
    public interface IChatService
    {
        Task<List<User>> GetActiveUsersAsync();
        Task<List<ChatMessage>> GetChatHistoryAsync(Guid userId, Guid contactId);
        Task<ChatMessage> SaveMessageAsync(ChatMessage message);
        Task MarkMessageAsSeenAsync(Guid messageId);
        Task<User?> GetUserByConnectionIdAsync(string connectionId);
        Task UpdateUserConnectionAsync(Guid userId, string? connectionId);
        Task<User?> CreateOrGetUserAsync(string username);
    }
}
