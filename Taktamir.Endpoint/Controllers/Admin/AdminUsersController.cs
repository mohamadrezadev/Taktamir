using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Security.Principal;
using System.Text;
using Taktamir.Core.Domain._01.Jobs;
using Taktamir.Core.Domain._03.Users;
using Taktamir.Core.Domain._06.Wallets;
using Taktamir.Endpoint.Models.Dtos.JobDtos;
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
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly IWalletRepository _walletRepository;
        private readonly IMapper _mapper;

        public AdminUsersController(IUserRepository userRepository,IMapper mapper,
            IOrderRepository orderRepository,IJobRepository jobRepository,
            IWalletRepository walletRepository 
            ,RoleManager<Role> roleManager,UserManager<User> userManager)
        {
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _jobRepository = jobRepository;
            _roleManager = roleManager;
            _userManager = userManager;
            _walletRepository = walletRepository;
            _mapper = mapper;
        }
        [HttpGet("UpgradeAccounttoAdmin")]
        public async Task<IActionResult> UpgradeAccounttoAdmin(int userid,CancellationToken cancellationToken)
        {
            var st = new StringBuilder();
            var user =await _userRepository.GetByIdAsync(cancellationToken, userid);
            if (user == null) return NotFound("user notFound");
            var findRoleadmin=await _roleManager.FindByNameAsync(UserRoleApp.Admin);
            
            
            if (findRoleadmin == null)
            {
               var res= await _roleManager.CreateAsync(new Role() { Name = UserRoleApp.Admin, NormalizedName = UserRoleApp.Admin });
               if (!res.Succeeded)
                {
                    foreach (var item in res.Errors)
                    {
                        st.Append(item);
                    }
                    return Problem(st.ToString());
                }
            }
            var result = await _userManager.AddToRoleAsync(user, UserRoleApp.Admin);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    st.Append(item);
                }
                return Problem(st.ToString());
            }

            return Ok("Add Role to this Account");
        }
        //[HttpPut("VerifyUser")]
        [HttpPost("VerifyAccountUser")]
        public IActionResult VerifyUser(int userid)
        {
            if(  Request.HttpContext.User.Identity.IsAuthenticated && Request.HttpContext.User.IsInRole(UserRoleApp.Admin))
            {
                var user = _userRepository.GetById(userid);
                if (user == null) return NotFound("User not found");
                user.confirme(true);
                _userRepository.Update(user);
                return Ok("verify user Sucssesfuly");
            }
            return Forbid();
            
        }
        //همه کاربران 
        [HttpGet("AllUsers")]
        public async  Task<IActionResult> GetAllusers()
        {
            var users = await _userRepository.Entities.Include(p => p.Wallet).ToListAsync();
            if (users.Count() <= 0) return NotFound("not Yet Users Active Account");

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
            //Where(p => p.Job.ReservationStatus.Equals(ReservationStatus.ReservedByTec)


            var users = await _userRepository.Entities.Include(w => w.Wallet)
                .ThenInclude(o => o.Orders).ThenInclude(p=>p.OrderJobs)
                .ThenInclude(p=>p.Job).ThenInclude(p=>p.Customer).ToListAsync();
            
            var resultUsers=new List<ReadUserDto>();
            foreach (var user in users)
            {
                var userdto = _mapper.Map<ReadUserDto>(user);

                foreach (var order in user.Wallet.Orders)
                {
                    var orderdto = new ReadOrderDto(); // Create a new instance for each order

                    orderdto.Total = order.Total;
                    orderdto.Id = order.Id;

                    foreach (var job in order.OrderJobs)
                    {
                        var jobdto = _mapper.Map<ReadJobDto>(job.Job);
                        orderdto.JobsOrder.Add(jobdto);
                    }

                    userdto.Wallet.Orders.Add(orderdto);
                }

                resultUsers.Add(userdto);
            }
            //foreach (var user in users)
            //{
            //    var userdto = _mapper.Map<ReadUserDto>(user);
            //    var orderdto = new ReadOrderDto();

            //    foreach (var order in user.Wallet.Orders)
            //    { 
            //        orderdto.Total= order.Total;
            //        orderdto.Id= order.Id;
            //        foreach (var job in order.OrderJobs)
            //        {
            //            var jobdto = _mapper.Map<ReadJobDto>(job.Job);
            //            orderdto.JobsOrder.Add(jobdto);
            //        }
            //        userdto.Wallet.Orders.Add(orderdto);
                    
            //    }

            //    resultUsers.Add(userdto);
            //}

            return Ok(resultUsers);
           
        }

        //تایید رزرو کار 
        //[HttpPut("Work_booking_confirmation")]
        [HttpPost("Work_booking_confirmation")]
        public async Task<IActionResult> Work_booking_confirmation(int idjob,CancellationToken cancellationToken)
        {
            var findjob=await _jobRepository.GetByIdAsync(cancellationToken,idjob);
            if (findjob == null) return NotFound("Job dos not Exist");
            if (findjob.ReservationStatus.Equals(ReservationStatus.ReservedByTec))
            {
                findjob.StatusJob = (int)StatusJob.Doing;
                findjob.ReservationStatus = ReservationStatus.ConfirmeByidmin;
                findjob.Reservation = true;
                await _jobRepository.UpdateAsync(findjob, cancellationToken);
                return Ok("Confirmed");
            }
            Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
            return new JsonResult("can not comfirme job becuse already confirmed or not Reserve yet");
        }

    }
}
