using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Taktamir.Core.Domain._03.Users;
using Taktamir.Core.Domain._05.Messages;
using Taktamir.Endpoint.Hubs;
using Taktamir.Endpoint.Models.Dtos.ChatDto;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Taktamir.Endpoint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles =UserRoleApp.Admin)]
    public class ChatsController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IDictionary<string, UserConnection> _connections;
        private readonly IRoomRepository _roomRepository;

        public ChatsController(IHubContext<ChatHub> hubContext, IUserRepository userRepository,
            IDictionary<string, UserConnection> connections,IRoomRepository roomRepository,
            UserManager<User>userManager,RoleManager<Role> roleManager)
        {
            _userRepository = userRepository;
            _roleManager = roleManager;
            _userManager = userManager;
            _hubContext = hubContext;
            _connections = connections;
            _roomRepository = roomRepository;

        }

        [HttpGet]
        public async Task< IActionResult> GetAll()
        {
           
            var Rooms = await _roomRepository.Entities.ToListAsync();
            return Ok(Rooms);
        }
      
        private Task SendUsersConnected(string room)
        {
            var users = _connections.Values
                .Where(c => c.Nameroom == room)
                .Select(c => c.User);

            return _hubContext.Clients.Group(room).SendAsync("UsersInRoom", users);
        }


    }
}
