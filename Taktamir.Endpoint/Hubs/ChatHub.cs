using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Taktamir.Core.Domain._03.Users;
using Taktamir.Core.Domain._03.Users.UserGroups;
using Taktamir.Core.Domain._09.Chats;

namespace Taktamir.Endpoint.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatRepository _chatRepository;
        private readonly IChatGroupRespository _chatGroup;
        private readonly IUserGroupRepository _userGroup;

        public ChatHub(IChatRepository chatRepository,
            IChatGroupRespository chatGroup, IUserGroupRepository userGroup)
        {
            _chatRepository = chatRepository;
            _chatGroup = chatGroup;
            _userGroup = userGroup;
        }


        //public override Task OnConnectedAsync()
        //{
        //    Clients.Caller.SendAsync("Welcome", Context.ConnectionId);
        //    return base.OnConnectedAsync();
        //}
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        //public async Task SendPrivateMessage(string userId, string message)
        //{
        //    var senderId = Context.User.Identity.Name;
        //   // var receiverConnectionId = await _userService.GetUserConnectionId(userId);
        //    //if (receiverConnectionId != null)
        //    //{
        //    //    var chatMessage = new ChatMessage { SenderId = senderId, ReceiverId = userId, Text = message };
        //    //    await Clients.Client(receiverConnectionId).SendAsync("ReceivePrivateMessage", chatMessage);
        //    //}
        //}

    }

    public class ChatMessage
    {
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string Text { get; set; }
    }
}
    


    
  

