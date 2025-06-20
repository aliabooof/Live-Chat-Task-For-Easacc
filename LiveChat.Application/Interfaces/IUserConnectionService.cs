using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveChat.Application.Interfaces
{
    public interface IUserConnectionService
    {
        void AddConnection(Guid userId, string connectionId);
        void RemoveConnection(string connectionId);
        List<string> GetConnectionIds(Guid userId);
        Guid? GetUserId(string connectionId);
        void StartInactivityTimer(Guid userId, string connectionId);
        void ResetInactivityTimer(Guid userId, string connectionId);
        void StopInactivityTimer(Guid userId, string connectionId);
    }
}
