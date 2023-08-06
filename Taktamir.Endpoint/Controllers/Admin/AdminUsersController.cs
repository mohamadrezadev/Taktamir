using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
using Taktamir.framework.Common;
using Taktamir.framework.Common.JobsUtill;
using Taktamir.infra.Data.sql._02.Jobs;
using Taktamir.infra.Data.sql._03.Users;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Taktamir.Endpoint.Controllers.Admin
{
    [Authorize(Roles = UserRoleApp.Admin)]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminUsersController : ControllerBase
    {
        #region Add services and respos
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
        #endregion
        
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
        public async  Task<IActionResult> GetAllusers(int page = 1, int pageSize = 10)
        {
            
            var Result = await _userRepository.GetAllUsersAsync(page,pageSize);
            if (Result.Item1.Count() <= 0) return NotFound("not Yet Users Active Account");
            var result = _mapper.Map<List<ReadUserDto>>(Result.Item1);
            Result.Item1.ForEach(user =>
            {
                result.ForEach(item =>
                {
                    item.StatusAccount = AccountUtills.SetConfermetionAccount(user.IsConfirmedAccount);
                });
            });
            Response.StatusCode = (int)HttpStatusCode.OK;
            return new JsonResult(new { PaginationData = Result.Item2 ,Users = result});


        }
        
        
        //کاربران تایید نشده
        [HttpGet("Unverified_users")]
        public async Task<IActionResult> Unverified_users(int page = 1, int pageSize = 10)
        {
            //var users=await _userRepository.Entities.Where(p=>!p.IsConfirmedAccount).ToListAsync();
            var Result=await _userRepository.GetAllUsersAsync(page,pageSize);
            var ResponseData=_mapper.Map<List<ReadUserDto>>(Result.Item1);
            Result.Item1.ForEach(user =>
            {
                ResponseData.ForEach(item =>
                {
                    item.StatusAccount = AccountUtills.SetConfermetionAccount(user.IsConfirmedAccount);
                });
            });
            
            Response.StatusCode = (int)HttpStatusCode.OK;
            return new JsonResult(new { PaginationData = Result.Item2, Users = ResponseData });
        }
       
        
        //کاربران تایید شده
        [HttpGet("Verified_user_account")]
        public async Task<IActionResult> Verified_user_account(int page = 1, int pageSize = 10)
        {
            var Result=await _userRepository.Verified_user_account(page,pageSize);
            var ResponseData = _mapper.Map<List<ReadUserDto>>(Result.Item1);
            Result.Item1.ForEach(user =>
            {
                ResponseData.ForEach(item =>
                {
                    item.StatusAccount = AccountUtills.SetConfermetionAccount(user.IsConfirmedAccount);
                });
            });
            Response.StatusCode = (int)HttpStatusCode.OK;
            return new JsonResult(new { PaginationData = Result.Item2, Users = ResponseData });
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
        public async Task<IActionResult> Work_pending_approval(int page = 1, int pageSize = 10)
        {
            
            var result=await _walletRepository.GetAll_Work_pending_Orders(page, pageSize);


            var resultUsers=new List<ReadUserDto>();
            foreach (var user in result.Item1)
            {
                var userdto = _mapper.Map<ReadUserDto>(user);
                userdto.Wallet.Orders.Clear();
                foreach (var order in user.Wallet.Orders)
                {
                    var orderdto = new ReadOrderDto(); 

                    orderdto.Total = order.Total;
                    orderdto.Id = order.Id;

                    foreach (var job in order.OrderJobs)
                    {
                        var jobdto = _mapper.Map<ReadJobDto>(job.Job);
                        jobdto.StatusJob= JobsUtills.SetStatusJob((int)job.Job.StatusJob);
                        jobdto.ReservationStatusResult = JobsUtills.SetReservationStatus((int)job.Job.ReservationStatus);
                        orderdto.JobsOrder.Add(jobdto);
                    }

                    userdto.Wallet.Orders.Add(orderdto);
                }

                resultUsers.Add(userdto);
            }
            


            return new JsonResult (new { PaginationMetadata=result.Item2,Data= resultUsers });
           
        }

        //تایید رزرو کار 
        //[HttpPut("Work_booking_confirmation")]
        [HttpGet("Work_booking_confirmation")]
        public async Task<IActionResult> Work_booking_confirmation(int Idorder,CancellationToken cancellationToken)
        {
            var order = await _orderRepository.Entities.Include(p => p.OrderJobs)
                .ThenInclude(p => p.Job).ThenInclude(p => p.Customer).Where(p=>p.Id==Idorder).ToListAsync();
           var idJob= order.SelectMany(p=>p.OrderJobs).Select(p => p.Job.Id).ToList();
         
            var Findjob = await _jobRepository.GetByIdAsync(cancellationToken, idJob[0]);
            
            if (Findjob == null) return NotFound("Job dos not Exist");
            if (Findjob.ReservationStatus.Equals(ReservationStatus.ReservedByTec))
            {
                Findjob.StatusJob = StatusJob.Doing;
                Findjob.ReservationStatus = ReservationStatus.ConfirmeByAdmin;
                Findjob.Reservation = true;
                await _jobRepository.UpdateAsync(Findjob, cancellationToken);
                return Ok("Confirmed");
            }
            Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
            return new JsonResult("can not comfirme job becuse already confirmed or not Reserve yet ");
        }
        [HttpGet("Work_booking_Reject")]
        public async Task <IActionResult> work_booking_reject(int Idorder, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.Entities.Include(p => p.OrderJobs)
                .ThenInclude(p => p.Job).ThenInclude(p => p.Customer).Where(p => p.Id == Idorder).FirstOrDefaultAsync();
            var idJob = order.OrderJobs.Select(p => p.Job.Id).FirstOrDefault();

            var Findjob = await _jobRepository.GetByIdAsync(cancellationToken, idJob);

            if (Findjob == null) return NotFound("Job dos not Exist");
            if (Findjob.ReservationStatus.Equals(ReservationStatus.ReservedByTec))
            {
                Findjob.StatusJob = StatusJob.Doing;
                Findjob.ReservationStatus = ReservationStatus.WatingforReserve;
                Findjob.Reservation = true;
                await _jobRepository.UpdateAsync(Findjob, cancellationToken);
                await _orderRepository.DeleteAsync(order, cancellationToken);
                return Ok("Reject booking job ");
            }
            Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
            return new JsonResult("can not comfirme job becuse already confirmed or not Reserve yet ");
        }
    }
}
