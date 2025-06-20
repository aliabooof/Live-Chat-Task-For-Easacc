using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveChat.Application.Interfaces
{
    public interface IChatNotifier
    {
        Task SendInactivityWarningAsync(string connectionId, string message);
    }
}
