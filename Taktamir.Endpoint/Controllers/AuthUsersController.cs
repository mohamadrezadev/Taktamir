using AutoMapper;
using IraniValidator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading;
using Taktamir.Core.Domain._03.Users;
using Taktamir.Core.Domain._08.Verifycodes;
using Taktamir.Endpoint.Models.Dtos.Common;
using Taktamir.Endpoint.Models.Dtos.UserDtos;
using Taktamir.Services.JwtServices;
using Taktamir.Services.SmsServices;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Taktamir.Endpoint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthUsersController : ControllerBase
    {
        private readonly IVerifycodeRepository _verifycodeRepository;
        private readonly ISmsService _smsService;
        private readonly RoleManager<Role> _roleManager;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<User> _userManager;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthUsersController> _logger;
        private readonly IMapper _mapper;

        public AuthUsersController(IVerifycodeRepository verifycodeRepository,ISmsService smsService,IUserRepository userRepository,
             UserManager<User> userManager, RoleManager<Role> roleManager,IJwtService jwtService ,ILogger<AuthUsersController> logger
            ,IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _verifycodeRepository = verifycodeRepository;
            _smsService = smsService;
            _roleManager = roleManager;
            _userManager = userManager;
            _jwtService = jwtService;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _mapper = mapper;
        }
        [HttpGet("signup_phone_number")]
        public async Task<IActionResult> signup_phone_number([Required] string phone_number,CancellationToken cancellationToken)
        {
            
            if (!phone_number.IsValidMobile(oprator: OpratorType.AllOpprator)) return BadRequest("Invalid phone numbe");
            if (!Request.QueryString.HasValue || phone_number.Length < 10) return BadRequest("phone number field is required!");
            var IsSendcode= await _smsService.SendVerifycode(phone_number);
            if (!IsSendcode.Item1) return Problem("An error occurred. Please try again",null,StatusCodes.Status503ServiceUnavailable);
            var verifycode = new Verifycode()
            {
                phone_number = phone_number,
                code=IsSendcode.Item2 
            };
            await _verifycodeRepository.add_or_update_verifycode(verifycode);
            return Ok($"Send Code to phone number :{phone_number}");
        }

        [HttpPost]
        public async Task<IActionResult> Verify_code(ConfirmMobileDto model,CancellationToken cancellationToken){
            var token = new Response_jwt();
            StringBuilder st = new StringBuilder();
            if (!ModelState.IsValid) return BadRequest(model);
            if (!model.Phone_number.IsValidMobile(oprator: OpratorType.AllOpprator)) return BadRequest("Invalid phone numbe");
            if (model.Code==null)return BadRequest("Invalid sms code ");
            var CheckVerificationCode = await _verifycodeRepository.Isvalidcode(model.Phone_number, model.Code);
            if (!CheckVerificationCode.Item1) return BadRequest("Invalid Code or phone Number ....!");
            var finduser =await _userRepository.Entities.Include(p=>p.Wallet).FirstOrDefaultAsync(p=>p.PhoneNumber.Equals(model.Phone_number));
            if (finduser == null)
            {
                var newuser = new User()
                {
                    PhoneNumber = model.Phone_number,
                    UserName = model.Phone_number,
                    Create_at = DateTime.Now,
                    IsCompleteprofile = false,
                    IsActive = false,
                    SecurityStamp = Guid.NewGuid().ToString(),
                   
                };
                token = generateLoginToken(newuser);
                newuser.Access_Token = token.Access_Token;
                newuser.Refresh_Token = token.Refresh_Token;
                await _userRepository.AddAsync(newuser,cancellationToken);
                var createdrol = await _roleManager.CreateAsync(new Role() { Name = UserRoleApp.Technician, NormalizedName = UserRoleApp.Technician });
                if (!createdrol.Succeeded)
                {
                    foreach (var item in createdrol.Errors)
                    {
                        st.Append($"{item.Code} {item.Description}");
                    }
                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    return Problem(st.ToString());
                }
                await _userManager.AddToRoleAsync(newuser,UserRoleApp.Technician);
                token.Role = UserRoleApp.Technician;
                var role = await _userManager.GetRolesAsync(finduser);
                token.Role = role.ToString();
                _logger.LogInformation($"created user{newuser}");
                Response.StatusCode = (int)HttpStatusCode.UpgradeRequired;
                return Created($"api/AuthUsers/",token);
            }
            token = generateLoginToken(finduser);
            finduser.Refresh_Token = token.Refresh_Token;
            finduser.Access_Token = token.Access_Token;
            
            var resultUpdate = await _userManager.UpdateAsync(finduser);
            if (!resultUpdate.Succeeded)
            {
                foreach (var item in resultUpdate.Errors)
                {
                    st.Append($"{item.Code} {item.Description}");
                }
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Problem(st.ToString());
            }
            var r= await _userManager.GetRolesAsync(finduser);
           
            token.Role= string.Concat(r.SelectMany(p => p));
            if (!finduser.IsCompleteprofile)
            {
                Response.StatusCode = (int)HttpStatusCode.UpgradeRequired;
                return new JsonResult(token);
            }
            Response.StatusCode = (int)HttpStatusCode.OK;
            return Ok(token);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> refreshtoken([Required] string refreshToken,CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return BadRequest("invalid refreshtoken");
            }
            var userid = _jwtService.DecodeRefreshToken(refreshToken);
            var finduser= await _userRepository.GetByIdAsync(cancellationToken, int.Parse(userid));
            if (finduser == null) return NotFound("Not founded");
            var Access_Token = _jwtService.CreateToken(finduser);
            finduser.Access_Token = new JwtSecurityTokenHandler().WriteToken(Access_Token);
            await _userRepository.UpdateAsync(finduser, cancellationToken);
            var r = await _userManager.GetRolesAsync(finduser);

           
            var response = new Response_jwt()
            {
                Access_Token = finduser.Access_Token,
                Refresh_Token = refreshToken,
                phone_number = finduser.PhoneNumber,
                Expiration = Access_Token.ValidTo,
                Role= string.Concat(r.SelectMany(p => p))
            };
            Response.StatusCode = (int)HttpStatusCode.Created;
            return Ok(response);
        }



        [HttpPut(nameof(Update_user))]
        [Authorize]
        public async Task<IActionResult> Update_user([FromBody] UpdateUserDto model,CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)return BadRequest($" model invalid {model}");
            var findUser = await _userRepository.Entities.Include(p => p.Wallet)
                .FirstOrDefaultAsync(p => p.PhoneNumber.Equals(_httpContextAccessor.HttpContext.User.FindFirstValue("MobilePhone")));
            if (findUser == null) return NotFound("not Exist User");
            findUser.Firstname= model.Firstname;
            findUser.LastName= model.LastName;
            findUser.Profile_url= model.Profile_url;
            findUser.SerialNumber= model.SerialNumber;
            findUser.Email= model.Email;
            findUser.Update_at = DateTime.Now;
            findUser.IsCompleteprofile = true;
            findUser.Specialties= _mapper.Map<List<Specialty>>(model.specialties);
            await _userRepository.UpdateAsync(findUser, cancellationToken);
            var result = _mapper.Map<ReadUserDto>(findUser);
            return Ok(result);

        }



        private Response_jwt generateLoginToken(User user)
        {
            var JST = new JwtSecurityTokenHandler();
            var Access_Token = _jwtService.CreateToken(user);
            var refreshtoken = _jwtService.GenerateRefreshToken(user.Id.ToString());
            var response = new Response_jwt()
            {
                Access_Token = JST.WriteToken(Access_Token),
                Refresh_Token = JST.WriteToken(refreshtoken),
                Expiration = Access_Token.ValidTo,
                phone_number = user.PhoneNumber,
            };
            
            return response;
        }
       
    }
}
