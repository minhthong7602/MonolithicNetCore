using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace MonolithicNetCore.Web.SignalHub
{
    public class LogServer : Hub
    {
        public async Task SubscribeAdmin()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "AdminLog");
        }
    }
}
