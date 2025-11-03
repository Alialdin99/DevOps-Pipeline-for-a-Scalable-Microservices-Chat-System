using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
namespace ChattingService.Hubs
{
    public class ChatHub: Hub   
    {
        public async Task SendMessage(string senderId, string receiverId, string message)
        {
            await Clients.User(receiverId).SendAsync("ReceiveMessage", senderId, message);
        }
    }
}
