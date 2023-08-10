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
using Taktamir.Endpoint.Models.Dtos.WalletDtos;
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
        private readonly ITokenService _tokenService;

        public AuthUsersController(IVerifycodeRepository verifycodeRepository,ISmsService smsService,IUserRepository userRepository,
             UserManager<User> userManager, RoleManager<Role> roleManager,IJwtService jwtService ,ILogger<AuthUsersController> logger
            ,IMapper mapper, IHttpContextAccessor httpContextAccessor, ITokenService tokenService)
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
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));

        }
        [HttpGet("signup_phone_number")]
        public async Task<IActionResult> signup_phone_number([Required] string phone_number,CancellationToken cancellationToken)
        {
            
            if (!phone_number.IsValidMobile(oprator: OpratorType.AllOpprator)) return BadRequest("Invalid phone numbe");
            if (!Request.QueryString.HasValue || phone_number.Length < 10) return BadRequest("phone number field is required!");
            var IsSendcode= await _smsService.SendVerifycode(phone_number);
         //   if (!IsSendcode.Item1) return Problem("An error occurred. Please try again",null,StatusCodes.Status503ServiceUnavailable);
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
            var finduser =await _userRepository.Entities.Include(p=>p.Wallet).Include(p=>p.Room).FirstOrDefaultAsync(p=>p.PhoneNumber.Equals(model.Phone_number));
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
                var claimsregister = new List<Claim>
                {
                    new Claim("MobilePhone", newuser.PhoneNumber),
                    new Claim("Name", newuser.UserName),
                    new Claim("Id", newuser.Id.ToString()),
                   
                };
                

                await _userRepository.AddAsync(newuser,cancellationToken);
               
                claimsregister.Add(new Claim(ClaimTypes.NameIdentifier, newuser.Id.ToString()));
                
                var findrole = await _roleManager.Roles.FirstOrDefaultAsync(p => p.Name.Equals(UserRoleApp.Technician));
                if (findrole == null)
                {
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
                }
               
                var res= await _userManager.AddToRoleAsync(newuser,UserRoleApp.Technician);
                if (!res.Succeeded)
                {
                    foreach(var item in res.Errors)
                    {
                        st.Append(item);
                    }
                    return Problem(st.ToString());
                }
               
                token.Role = UserRoleApp.Technician;
                var role = await _userManager.GetRolesAsync(newuser);
                foreach (var item in role)
                {
                    claimsregister.Add(new Claim(ClaimTypes.Role, item));
                }
                newuser.Access_Token = _tokenService.GenerateAccessToken(claimsregister);
                newuser.RefreshToken = _tokenService.GenerateRefreshToken();
                newuser.RefreshTokenExpiryTime = DateTime.Now.AddDays(30);
                await _userRepository.UpdateAsync(newuser, cancellationToken);
                token.Access_Token = newuser.Access_Token;
                token.Refresh_Token = newuser.RefreshToken;
                token.phone_number = newuser.PhoneNumber;
                _logger.LogInformation($"created user{newuser}");
                Response.StatusCode = (int)HttpStatusCode.UpgradeRequired;
                return Created($"api/AuthUsers/",token);
            }
           
            var roles = await _userManager.GetRolesAsync(finduser);
            token.Role = string.Concat(roles.SelectMany(p => p));
            token.phone_number = finduser.PhoneNumber;
            var claims = new List<Claim>
            {   new Claim("MobilePhone", finduser.PhoneNumber),
                new Claim("Name", finduser.UserName),
                new Claim("Id", finduser.Id.ToString()),
                
            };
            foreach (var item in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, item));
            }
            token.Access_Token = _tokenService.GenerateAccessToken(claims);
            token.Refresh_Token = finduser.RefreshToken;
            if (DateTime.UtcNow > finduser.RefreshTokenExpiryTime)
            {
                Response.StatusCode =(int) HttpStatusCode.Unauthorized;
                return Unauthorized("Token expired");
            }
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
          
            if (!finduser.IsCompleteprofile)
            {
                Response.StatusCode = (int)HttpStatusCode.UpgradeRequired;
                return new JsonResult(token);
            }
            if (!finduser.IsConfirmedAccount) 
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return new JsonResult(token);
            }
            Response.StatusCode = (int)HttpStatusCode.OK;
            
            return Ok(token);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> refreshtoken([Required] string refreshToken,CancellationToken cancellationToken)
        {
            var response = new Response_jwt();
            if (string.IsNullOrEmpty(refreshToken))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return BadRequest("invalid refreshtoken");
            }
            var finduser =await _userRepository.Entities.Include(p=>p.Wallet).Include(p=>p.Room).FirstOrDefaultAsync(p => p.RefreshToken.Equals(refreshToken));
            if (finduser == null) {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return BadRequest("invalid refreshtoken");
            }
            if (DateTime.UtcNow > finduser.RefreshTokenExpiryTime)
            {
                finduser.RefreshToken = _tokenService.GenerateRefreshToken();
                finduser.RefreshTokenExpiryTime = DateTime.Now.AddDays(30);
            }
            var roles = await _userManager.GetRolesAsync(finduser);
            response.Role = string.Concat(roles.SelectMany(p => p));
            response.phone_number = finduser.PhoneNumber;
            var claims = new List<Claim>
            {   new Claim("MobilePhone", finduser.PhoneNumber),
                new Claim("Name", finduser.UserName),
                new Claim("Id", finduser.Id.ToString()),
                
            };
            foreach (var item in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, item));
            }
            response.Access_Token = _tokenService.GenerateAccessToken(claims);
            response.Refresh_Token=finduser.RefreshToken;
            await _userRepository.UpdateAsync(finduser, cancellationToken);
            Response.StatusCode = (int)HttpStatusCode.Created;
            return Ok(response);
        }



       

      
       
    }
}
