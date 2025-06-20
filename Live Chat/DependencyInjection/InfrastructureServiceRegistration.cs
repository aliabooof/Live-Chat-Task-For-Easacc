using LiveChat.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Live_Chat.DependencyInjection
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,IConfiguration configuration) 
        {
            services.AddDbContext<ChatDbContext>(op => op.UseSqlServer(configuration.GetConnectionString("ChatConnection")));
            return services;
        }
    }
}
