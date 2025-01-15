using Microsoft.AspNetCore.SignalR;
using System.Security.Cryptography.Xml;

namespace MEDCHGWEB.Services
{
    public class NotificationHub : Hub
    {
        public async Task SendNotification(List<string> userID)
        {
            await Clients.All.SendAsync("ReceiveNotification", userID);
        }

    }

}
