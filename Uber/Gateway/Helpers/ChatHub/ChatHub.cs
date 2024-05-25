using Microsoft.AspNetCore.SignalR;

namespace Gateway.Helpers.ChatHub
{
    public class ChatHub : Hub<IChatHub>
    {
        public async Task SendMessage(string message)
        {

            await Clients.All.SendMessage(message);
        }

        public Task JoinGroup(string groupName)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }
    }
}
