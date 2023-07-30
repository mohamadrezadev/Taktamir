using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Taktamir.Core.Domain._03.Users;
using Taktamir.Core.Domain._09.Chats;
using Taktamir.Endpoint.Hubs;
using Taktamir.Endpoint.Models.Dtos.ChatDto;

namespace Taktamir.Endpoint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatsController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatsController(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [Route("send")]
        [HttpPost]
        public async Task<IActionResult> SendRequest([FromBody] string username,string reciver,string text)
        {
            await _hubContext.Clients.Client(reciver).SendAsync("ReceiveOne", username,text);
            return Ok();
        }

        [Route("receive")]
        [HttpGet]
        public IActionResult Receive()
        {
            return Ok();
        }
        [HttpPost]
        [HubMethodName("SendMessage")]
        public async Task SendMessage(string user, string message)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    
    }
}
