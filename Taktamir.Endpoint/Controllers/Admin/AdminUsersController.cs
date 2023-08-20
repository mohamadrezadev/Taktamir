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
using Taktamir.Endpoint.Models.Dtos.AdminDto;
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
            var isAdmin = await _userManager.IsInRoleAsync(user,UserRoleApp.Admin);
            if (!isAdmin )
            {
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

            Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
            return Content("Add Role to this Account");
        }
        
        //تایید حساب 
        //[HttpPut("VerifyUser")]
        [HttpPost("VerifyAccountUser")]
        public async Task<IActionResult> VerifyUser(int userid,CancellationToken cancellationToken)
        {
            var user =await _userRepository.GetByIdAsync(cancellationToken, userid);
            if (user == null) return NotFound("User not found");
            user.confirme(true);
            await  _userRepository.UpdateAsync(user,cancellationToken);
            return Ok("verify user Sucssesfuly");

        }

        [HttpPost("RejectAccountUser")]
        public async Task<IActionResult> RejectUser(int Userid,CancellationToken cancellationToken)
        {
            var FindUser=await _userRepository.Entities
                .Include(p=>p.Specialties).Include(p=>p.Wallet).ThenInclude(p=>p.Orders)
                .FirstOrDefaultAsync(p=>p.Id== Userid);
            if (FindUser == null) return NotFound("User dos not Exist");
            FindUser.confirme(false);
            await _userRepository.DeleteAsync(FindUser,cancellationToken);
            return Ok();
        }

        //همه کاربران 
        [HttpGet("AllUsers")]
        public async  Task<IActionResult> GetAllusers(int page = 1, int pageSize = 10)
        {
            
            var Result = await _userRepository.GetAllUsersAsync(page,pageSize);
            if (Result.Item1.Count() <= 0) return NotFound("not Yet Users Active Account");
            
            var result = new List<ReadUserDto>();
            foreach (var user in Result.Item1)
            {
                var Specialties = _mapper.Map<List<SpecialtyDto>>(user.Specialties);

                var userdto=_mapper.Map<ReadUserDto>(user);
                userdto.StatusAccount = AccountUtills.SetConfermetionAccount(user.IsConfirmedAccount);
                userdto.specialties = Specialties;
                var rols =await _userManager.GetRolesAsync(user);
                userdto.Role =string.Join(" , ", rols);
                result.Add(userdto);

            }
            Response.StatusCode = (int)HttpStatusCode.OK;
            return new JsonResult(new { PaginationData = Result.Item2 ,Users = result});


        }
        
        
        //کاربران تایید نشده
        [HttpGet("Unverified_users")]
        public async Task<IActionResult> Unverified_users(int page = 1, int pageSize = 10)
        {
            //var users=await _userRepository.Entities.Where(p=>!p.IsConfirmedAccount).ToListAsync();
            var Result=await _userRepository.Unverified_users(page,pageSize);
            var ResponseData=_mapper.Map<List<ReadUserDto>>(Result.Item1);
            Result.Item1.ForEach(user =>
            {
                var Specialties = _mapper.Map<List<SpecialtyDto>>(user.Specialties);
                ResponseData.ForEach(item =>
                {
                    item.StatusAccount = AccountUtills.SetConfermetionAccount(user.IsConfirmedAccount);
                    item.specialties = Specialties;
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
                var Specialties = _mapper.Map<List<SpecialtyDto>>(user.Specialties);
                ResponseData.ForEach(item =>
                {
                    item.StatusAccount = AccountUtills.SetConfermetionAccount(user.IsConfirmedAccount);
                    item.specialties = Specialties;
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


            var resultUsers=new List<ReadUsersAdmindto>();
            foreach (var user in result.Item1)
            {
                var Userdata = new ReadUsersAdmindto();
                Userdata.id=user.Id;
                Userdata.FullNameUser = $"{user.Firstname} {user.LastName}";
                Userdata.IsActive = user.IsActive;
                Userdata.PhoneNumber = user.PhoneNumber;
                Userdata.SpecialtyDtos= _mapper.Map<List<SpecialtyDto>>(user.Specialties);
              
                foreach (var order in user.Wallet.Orders)
                {
                    if (order.OrderJobs.Count > 0)
                    {
                        var orderdto = new OrderUserAdmindto();
                        orderdto.Id = order.Id;

                        foreach (var job in order.OrderJobs)
                        {
                            var jobdto = new jobsAdminDto();
                            jobdto.Id = job.Id;
                            jobdto.Problems = job.Job.Problems;
                            jobdto.Name_Device = job.Job.Name_Device;
                            jobdto.ReservationStatusResult = JobsUtills.SetReservationStatus((int)job.Job.ReservationStatus);
                            jobdto.StatusJob = JobsUtills.SetStatusJob((int)job.Job.StatusJob);
                            jobdto.Customer = new ReadCustomerAdminDto()
                            {
                                Id = job.Job.Customer.Id,
                                Address = job.Job.Customer.Address,
                                Phone = job.Job.Customer.Phone,
                                FullNameCustomer = job.Job.Customer.FullNameCustomer,
                                PhoneNumber = job.Job.Customer.PhoneNumber,
                            };
                            orderdto.Jobs.Add(jobdto);
                        }


                        Userdata.orders.Add(orderdto);
                    }
                   
                }
                resultUsers.Add(Userdata);
            }
            


            return new JsonResult (new { PaginationData = result.Item2,Data= resultUsers });
           
        }

        //تایید رزرو کار 
        //[HttpPut("Work_booking_confirmation")]
        [HttpGet("Work_booking_confirmation")]
        public async Task<IActionResult> Work_booking_confirmation(int Idorder,CancellationToken cancellationToken)
        {
            var order = await _orderRepository.Entities.Include(p => p.OrderJobs)
                .ThenInclude(p => p.Job).ThenInclude(p => p.Customer).Where(p=>p.Id==Idorder).ToListAsync();
            if (order == null)
            {
                return BadRequest();
            }
            var idJob= order.SelectMany(p=>p.OrderJobs.Select(p => p.Job.Id)).ToList();
          
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
                .ThenInclude(p => p.Job).ThenInclude(p => p.Customer).FirstOrDefaultAsync(p => p.Id == Idorder);
        
            if (order==null)
            {
                return BadRequest();
            }
            var idJob = order.OrderJobs.Select(p =>p.Job.Id).SingleOrDefault();
        
            var Findjob = await _jobRepository.Entities.Include(p => p.Customer).FirstOrDefaultAsync(p => p.Id == idJob);
            if (Findjob == null) return NotFound("Job dos not Exist");
            if (Findjob.ReservationStatus.Equals(ReservationStatus.ReservedByTec)|| Findjob.ReservationStatus.Equals(ReservationStatus.WatingforReserve))
            {
                Findjob.StatusJob = StatusJob.waiting;
                Findjob.ReservationStatus = ReservationStatus.WatingforReserve;
                Findjob.Reservation = true;
                await _jobRepository.UpdateAsync(Findjob, cancellationToken);
                order.OrderJobs.Clear();
                await _orderRepository.DeleteAsync(order, cancellationToken);
                return Ok("Reject booking job ");
            }
            Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
            return new JsonResult("can not comfirme job becuse already confirmed or not Reserve yet ");
        }
    }
}
