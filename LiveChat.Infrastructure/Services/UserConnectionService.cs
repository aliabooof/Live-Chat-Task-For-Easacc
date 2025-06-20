using LiveChat.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveChat.Infrastructure.Services
{
    public class UserConnectionService : IUserConnectionService
    {
        private readonly ConcurrentDictionary<string, Guid> _connections = new();
        private readonly ConcurrentDictionary<Guid, List<string>> _userConnections = new();
        private readonly ConcurrentDictionary<Guid, Timer> _inactivityTimers = new();
        private readonly IServiceProvider _serviceProvider;

       

        public UserConnectionService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            
        }

        public void AddConnection(Guid userId, string connectionId)
        {
            _connections[connectionId] = userId;
            _userConnections.AddOrUpdate(userId,
                new List<string> { connectionId },
                (key, list) => { list.Add(connectionId); return list; });
        }


        public List<string> GetConnectionIds(Guid userId)
        {
            return _userConnections.TryGetValue(userId, out var connections)
               ? connections.ToList()
               : new List<string>();
        }

        public Guid? GetUserId(string connectionId)
        {
            return _connections.TryGetValue(connectionId, out var userId) ? userId : null;
        }

        public void RemoveConnection(string connectionId)
        {
            if (_connections.TryRemove(connectionId, out var userId))
            {
                if (_userConnections.TryGetValue(userId, out var connections))
                {
                    connections.Remove(connectionId);
                    if (!connections.Any())
                    {
                        _userConnections.TryRemove(userId, out _);
                        StopInactivityTimer(userId, connectionId);
                    }
                }
            }
        }

        public void ResetInactivityTimer(Guid userId, string connectionId)
        {
            StartInactivityTimer(userId, connectionId);
        }

        public void StartInactivityTimer(Guid userId, string connectionId)
        {
            StopInactivityTimer(userId, connectionId);

            var timer = new Timer(async _ =>
            {
                using var scope = _serviceProvider.CreateScope();
                var notifier = scope.ServiceProvider.GetRequiredService<IChatNotifier>();

                await notifier.SendInactivityWarningAsync(connectionId,
                    "The chat will be terminated because we have not received a response from you.");

            }, null, TimeSpan.FromMinutes(1), Timeout.InfiniteTimeSpan);

            _inactivityTimers[userId] = timer;
        }



        public void StopInactivityTimer(Guid userId, string connectionId)
        {
            if (_inactivityTimers.TryRemove(userId, out var timer))
            {
                timer.Dispose();
            }
        }
    }
}
