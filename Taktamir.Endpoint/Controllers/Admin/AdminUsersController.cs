using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using System.Net.WebSockets;
using System.Security.Principal;
using Taktamir.Core.Domain._01.Jobs;
using Taktamir.Core.Domain._03.Users;
using Taktamir.Core.Domain._06.Wallets;
using Taktamir.Endpoint.Models.Dtos.UserDtos;
using Taktamir.Endpoint.Models.Dtos.WalletDtos;
using Taktamir.infra.Data.sql._02.Jobs;
using Taktamir.infra.Data.sql._03.Users;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Taktamir.Endpoint.Controllers.Admin
{
    [Authorize(Roles = UserRoleApp.Admin)]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminUsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IJobRepository _jobRepository;
        private readonly IMapper _mapper;

        public AdminUsersController(IUserRepository userRepository,IMapper mapper,
            IOrderRepository orderRepository,IJobRepository jobRepository)
        {
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _jobRepository = jobRepository;
            _mapper = mapper;
        }
 
        //[HttpPut("VerifyUser")]
        [HttpPost("VerifyUser")]
        public IActionResult VerifyUser(int userid)
        {
            var user = _userRepository.GetById(userid);
            if (user == null) return NotFound("User not found");
            user.confirme(true);
            _userRepository.Update(user);
            return Ok("verify user Sucssesfuly");
        }
        //همه کاربران 
        [HttpGet("AllUsers")]
        public async  Task<IActionResult> GetAllusers()
        {
            var users =await _userRepository.Entities.Include(p=>p.Wallet).ToListAsync();
            if (users.Count()<=0) return NotFound("not Yet Users Active Account");
           
            var result = _mapper.Map<List<ReadUserDto>>(users);
            return Ok(result);
        
        }
        //کاربران تایید نشده
        [HttpGet("Unverified_users")]
        public async Task<IActionResult> Unverified_users()
        {
            var users=await _userRepository.Entities.Where(p=>!p.IsConfirmedAccount).ToListAsync();
            var result=_mapper.Map<List<ReadUserDto>>(users);
            return Ok(result);
        }
        //کاربران تایید شده
        [HttpGet("Verified_user_account")]
        public async Task<IActionResult> Verified_user_account()
        {
            var Users = await _userRepository.Entities.Where(p => p.IsConfirmedAccount).ToListAsync();
            var result=_mapper.Map<List<ReadUserDto>>(Users);
            return Ok(result);
        }
        //تایید حساب 
        //[HttpPut("User_account_verification")]
        [HttpPost("User_account_verification")]
        public async Task<IActionResult> User_account_verification(int UserId, CancellationToken  cancellationToken )
        {
            var FindUser = await _userRepository.GetByIdAsync(cancellationToken, UserId);
            if (FindUser == null) return NotFound("User dos not Exist");
            FindUser.confirme(true);
            await _userRepository.UpdateAsync(FindUser,cancellationToken);
            return Ok("The user account has been verified");
        }

        //کار های در انتضار تایید
        [HttpGet("Work_pending_approval")]
        public async Task<IActionResult> Work_pending_approval()
        {
            var users = await _userRepository.Entities.Include(w => w.Wallet)
                .ThenInclude(o => o.Orders)
                .ThenInclude(j => j.Jobs.Where(j=>j.StatusJob==(int)StatusJob.waiting))
                .ThenInclude(c => c.Customer).ToListAsync();

            var result=_mapper.Map<List<ReadUserDto>>(users);
            return Ok(result);
        }

        //تایید رزرو کار 
        //[HttpPut("Work_booking_confirmation")]
        [HttpPost("Work_booking_confirmation")]
        public async Task<IActionResult> Work_booking_confirmation(int idjob,CancellationToken cancellationToken)
        {
            var findjob=await _jobRepository.GetByIdAsync(cancellationToken,idjob);
            if (findjob == null) return NotFound("Job dos not Exist");
            findjob.StatusJob = (int)StatusJob.Doing;
            findjob.ReservationStatus = ReservationStatus.ConfirmeByidmin;
            findjob.Reservation = true;
            await _jobRepository.UpdateAsync(findjob,cancellationToken);
            return Ok("Confirmed");
        }

    }
}
