using LiveChat.Application.Interfaces;
using LiveChat.Infrastructure.Services;

namespace Live_Chat.DependencyInjection
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplication (this IServiceCollection services)
        {
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<IFileService, FileService>();
            services.AddSingleton<IUserConnectionService, UserConnectionService>();

            return services;
        }
    }
}
