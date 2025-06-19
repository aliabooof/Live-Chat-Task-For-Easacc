using LiveChat.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LiveChat.Infrastructure.Data
{
    public class ChatDbContext : DbContext
    {
        public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<ChatMessage> Messages { get; set; }

        
    }
}

