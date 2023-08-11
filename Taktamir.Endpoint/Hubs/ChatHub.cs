using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Validations;
using Newtonsoft.Json;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading;
using Taktamir.Core.Domain._03.Users;
using Taktamir.Core.Domain._05.Messages;
using Taktamir.Endpoint.Models.Dtos.HubDto;

namespace Taktamir.Endpoint.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IUserRepository _userRepository;
        private IRoomRepository _roomRepository;
        private readonly UserManager<User> _userManager;
        private readonly IDictionary<string, Room> _connections;
        private IHttpContextAccessor _httpContextAccessor;
        private readonly string _botUser;
        private readonly IMessagesRepository _messagesRepository;
        private readonly RoleManager<Role> _roleManager;

        public ChatHub(IUserRepository userRepository,
            UserManager<User> userManager, IDictionary<string, Room> connections,
            IHttpContextAccessor httpContextAccessor, IRoomRepository roomRepository,
            IMessagesRepository messagesRepository,RoleManager<Role> roleManager)
        {
            _userRepository = userRepository;
            _roomRepository = roomRepository;
            _userManager = userManager;
            _connections = connections;
            _httpContextAccessor = httpContextAccessor;
            _botUser = "MyChat Bot";
            _messagesRepository = messagesRepository;
            _roleManager = roleManager;
        }



        public async Task SendMessage(string message,string NameRoom)
        {
            var mobile = Context.User.FindFirstValue("MobilePhone");
            var user = await _userRepository.Entities.FirstOrDefaultAsync(p => p.PhoneNumber == mobile.ToString());
            var room = await _roomRepository.Entities.Include(p=>p.User).ThenInclude(w=>w.Wallet).Include(p=>p.Messages).FirstOrDefaultAsync(p => p.NameRoom.Equals(NameRoom));
            var isroleadmin = await _userManager.IsInRoleAsync(user ,UserRoleApp.Admin);
            if (room == null)
                return;
            
            var newmessage = new Message()
            {
                Room = room,
                Text = message,
                Timestamp = DateTime.Now,
                RoomId=room.RoomId,
                
            };
            newmessage.Sender = isroleadmin ? "Admin" : $"{user.Firstname} {user.LastName}";
           

            room.Messages.Add(newmessage);
            await _roomRepository.UpdateAsync(room, CancellationToken.None);
            var userIds = room.UsersId.ToList();

            var serializerSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            newmessage.Room = null;
           
            var messageJson = JsonConvert.SerializeObject(newmessage, serializerSettings);
      
            await Clients.Group(room.NameRoom).SendAsync("notification", new { user.Firstname, user.LastName, newmessage.Sender });

            await Clients.Group(room.NameRoom).SendAsync("ReceiveMessage", $"{user.Firstname}{user.LastName}", messageJson);

        }
        public async Task SendPrivateMessage(string userId, string message)
        {
            await Clients.User(userId).SendAsync("ReceiveMessage", userId, message);
        }



        [Authorize (Roles =UserRoleApp.Technician)]
        public async Task JoinRoom()
        {
            
            CancellationToken cancellationToken = CancellationToken.None;

            var mobile = Context.User.FindFirstValue("MobilePhone");
            var findUser = await _userRepository.Entities.Include(p=>p.Room).FirstOrDefaultAsync(p=>p.PhoneNumber== mobile.ToString());
            var findroom = await _roomRepository.Entities.Include(p => p.Messages).FirstOrDefaultAsync(p => p.RoomId == findUser.Room.RoomId);
            if (findroom == null)
            {
                var newroom = new Room() { NameRoom = findUser.Firstname + findUser.LastName, };
                newroom.UsersId.Add(findUser.Id.ToString());
                await _roomRepository.AddAsync(findUser.Room, CancellationToken.None);
                var message = new Message() { Text = "Welcom", Room = newroom, Timestamp = DateTime.Now };
                await _messagesRepository.AddAsync(message, CancellationToken.None);
                await Clients.Group(newroom.NameRoom).SendAsync("ReceiveMessage", message);
                await Groups.AddToGroupAsync(findUser.Id.ToString(), findUser.Room.NameRoom, cancellationToken);
            }

            if (findroom.NameRoom == null || findroom.NameRoom==" ")
            {
                findroom.NameRoom = $"{findUser.Firstname} {findUser.LastName}";
                await _roomRepository.UpdateAsync(findroom, cancellationToken);
            }
            if (findroom.UsersId == null || !findroom.UsersId.Any(p => p.Equals(findUser.Id.ToString())))
            {
                var UsersId = new List<string>() { findUser.Id.ToString() };
                findroom.UsersId = UsersId;
                await _roomRepository.UpdateAsync(findroom, cancellationToken);
            }

            try
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, findroom.NameRoom);
                // await Clients.Group(findroom.NameRoom).SendAsync("ReceiveMessage", _botUser, $"{findadmin.Firstname + findadmin.LastName} has joined room");
                var messageList = findroom.Messages.ToList();
               // _connections[Context.ConnectionId] = findroom;
                var serializerSettings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                var messageJson = JsonConvert.SerializeObject(messageList, serializerSettings);
                await Clients.Group(findroom.NameRoom).SendAsync("AllMessage", messageJson);
  
                await SendUsersConnected(findroom.NameRoom);
                
                
                //await Groups.AddToGroupAsync(Context.ConnectionId, findroom.NameRoom);
                //await Clients.Group(findUser.Room.NameRoom).SendAsync("ReceiveMessage", _botUser, $"{findUser.Firstname + findUser.LastName} has joined room");
              
                //await Clients.Group(findroom.NameRoom).SendAsync("AllMessage", findroom.Messages.SelectMany(p=>p.Text).ToList());
                ////await Clients.Others.SendAsync("ReceiveMessage", _botUser, $"{findUser.Firstname + findUser.LastName} has joined chat");
                //await SendUsersConnected(findUser.Room.NameRoom);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message); ;
                throw;
            }

        }

        [Authorize(Roles =UserRoleApp.Admin)]
        public async Task JonAdmintoroom(string Nameroom)
        {
            try
            {
                var mobile = Context.User.FindFirstValue("MobilePhone");
                var findadmin = await _userRepository.Entities.FirstOrDefaultAsync(p => p.PhoneNumber == mobile.ToString());
                var findroom = await _roomRepository.Entities.Include(p=>p.Messages).FirstOrDefaultAsync(p => p.NameRoom == Nameroom);
                if (findroom==null)
                    await Clients.Caller.SendAsync("Erorr", "not found room");
                
                await Groups.AddToGroupAsync(Context.ConnectionId, findroom.NameRoom);
                var messageList = findroom.Messages.ToList();
                var serializerSettings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                var messageJson = JsonConvert.SerializeObject(messageList, serializerSettings);
                await Clients.Group(findroom.NameRoom).SendAsync("AllMessage", messageJson);
                await SendUsersConnected(findroom.NameRoom);

            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Erorr", ex.Message);
            }
           

        }
     
        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out Room room))
            {
                _connections.Remove(Context.ConnectionId);
                Clients.Group(room.NameRoom).SendAsync("ReceiveMessage", _botUser, $"{room.User.Firstname + room.User.LastName} has left");
                SendUsersConnected(room.NameRoom);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public Task SendUsersConnected(string room)
        {
            var usersid = _roomRepository.Entities.Where(r => r.NameRoom == room).Select(p => p.UsersId).ToList();

            //.Where(c => c.Nameroom == room)
            //.Select(c => c.User);

            return Clients.Group(room).SendAsync("UsersInRoom", usersid);
        }

    }


}






