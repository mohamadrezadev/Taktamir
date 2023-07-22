using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Taktamir.Core.Domain._03.Users;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Taktamir.Endpoint.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminUsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public AdminUsersController(IUserRepository userRepository,IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }
       
        [HttpGet("{userid}")]
        public IActionResult VerifyUser(int userid)
        {
            var user = _userRepository.GetById(userid);
            if (user == null) return NotFound("User not found");
            user.IsActive = true;
            _userRepository.Update(user);
            return Ok("verify user Sucssesfuly");
        }

        [HttpGet]
        public IActionResult GetStatususers()
        {
            var users = _userRepository.Entities.ToList();
            var result = _mapper.Map<List<User>>(users);
            return Ok(result);
        
        }
      
    }
}
