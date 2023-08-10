using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Security.Claims;
using Taktamir.Core.Domain._01.Jobs;
using Taktamir.Core.Domain._03.Users;
using Taktamir.Core.Domain._05.Messages;
using Taktamir.Core.Domain._06.Wallets;
using Taktamir.Core.Domain._07.Suppliess;
using Taktamir.Endpoint.Models.Dtos.JobDtos;
using Taktamir.Endpoint.Models.Dtos.UserDtos;
using Taktamir.Endpoint.Models.Dtos.WalletDtos;
using Taktamir.framework.Common;
using Taktamir.framework.Common.JobsUtill;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Taktamir.Endpoint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize (Roles =UserRoleApp.Technician + "," + UserRoleApp.Admin )]
    public class UsersController : ControllerBase
    {
        #region AddServices 
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IJobRepository _jobRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly ISuppliesRepository _suppliesRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRoomRepository _roomRepository;

        public UsersController(IUserRepository userRepository,IMapper mapper,
            IJobRepository jobRepository,IWalletRepository walletRepository, IHttpContextAccessor httpContextAccessor,
            ISuppliesRepository suppliesRepository,IOrderRepository orderRepository,IRoomRepository roomRepository)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _jobRepository=jobRepository;
            _walletRepository = walletRepository;
            _suppliesRepository = suppliesRepository;
            _orderRepository = orderRepository;
            _httpContextAccessor = httpContextAccessor;
            _roomRepository = roomRepository;
        }

        #endregion
        
        /// <summary>
        /// Retrieves a user by ID.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/users/Profile
        ///
        /// </remarks>
        /// <returns>The user with the specified ID.</returns>
        /// <response code="200">Returns the user.</response>
        /// <response code="404">If the user is not found.</response>
        [HttpGet("user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> me()
        {
            var findUser = await _userRepository.Entities.Include(p => p.Wallet).Include(p => p.Specialties)
                .FirstOrDefaultAsync(p => p.PhoneNumber.Equals(_httpContextAccessor.HttpContext.User.FindFirstValue("MobilePhone")));
            if (findUser == null) return NotFound("User dos not exist");
            var result = _mapper.Map<ReadUserDto>(findUser);
            result.specialties = _mapper.Map<List<SpecialtyDto>>(findUser.Specialties);
            result.Wallet = _mapper.Map<ReadWalletDto>(findUser.Wallet);
            return Ok(result);
        }

        [HttpPost("ActiveAccount")]
        public async Task<IActionResult> ActiveAccount([FromBody]  IsActiveAccount isActiveAccount, CancellationToken cancellationToken)
        {
            var findUser = await _userRepository.Entities.Include(p => p.Wallet).Include(p=>p.Room).Include(p => p.Specialties)
               .FirstOrDefaultAsync(p => p.PhoneNumber.Equals(_httpContextAccessor.HttpContext.User.FindFirstValue("MobilePhone")));
            findUser.IsActive= isActiveAccount.IsActive;
            await _userRepository.UpdateAsync(findUser, cancellationToken);
            return Ok();
        }

        // [HttpPut(nameof(Update_user))]
        [HttpPost(nameof(Update_user))]
        public async Task<IActionResult> Update_user([FromBody] UpdateUserDto model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return BadRequest($" model invalid {model}");


            var findUser = await _userRepository.Entities.Include(p => p.Wallet).Include(p=>p.Room)
                .FirstOrDefaultAsync(p => p.PhoneNumber.Equals(_httpContextAccessor.HttpContext.User.FindFirstValue("MobilePhone")));
            if (findUser == null) return NotFound("not Exist User");
            findUser.Firstname = model.Firstname;
            findUser.LastName = model.LastName;
            findUser.Profile_url = model.Profile_url;
            findUser.SerialNumber = model.SerialNumber;
            findUser.Email = model.Email;
            findUser.Update_at = DateTime.Now;
            findUser.IsCompleteprofile = true;
            findUser.Specialties = _mapper.Map<List<Specialty>>(model.specialties);

            var findroom = await _roomRepository.GetByIdAsync(cancellationToken, findUser.Room.RoomId);
            if (findroom!=null)
            {
                findroom.NameRoom = $"{findUser.Firstname} {findUser.LastName}";
               // await _roomRepository.UpdateAsync(findroom,cancellationToken);
                findUser.Room = findroom;
            }
            await _userRepository.UpdateAsync(findUser, cancellationToken);
            var result = _mapper.Map<ReadUserDto>(findUser);
            result.specialties = model.specialties;
            if (!findUser.IsConfirmedAccount)
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return new JsonResult(result);
            }
            return Ok(result);

        }

       
        [HttpGet("Book_a_job")]
        public async Task<IActionResult> Book_a_job(int IdJob,CancellationToken cancellationToken)
        {
            var findjob =await _jobRepository.GetByIdAsync(cancellationToken, IdJob);
            if (findjob == null || IdJob <= 0) return BadRequest("Inalid Id job");
            var findUser = await _userRepository.Entities.Include(p => p.Wallet)
                    .FirstOrDefaultAsync(p => p.PhoneNumber.Equals(_httpContextAccessor.HttpContext.User.FindFirstValue("MobilePhone")));
            if (findUser.IsConfirmedAccount)
            {
                if (findjob.ReservationStatus.Equals(ReservationStatus.WatingforReserve))
                {

                    var walletuser = await _walletRepository.Entities.Include(p => p.Orders).FirstOrDefaultAsync(p => p.Id == findUser.Wallet.Id);
                    var neworder = new Order();
                    var neworderjob = new OrderJob() { Order = neworder, Job = findjob };
                    neworder.OrderJobs.Add(neworderjob);

                    findjob.StatusJob = StatusJob.waiting;
                    findjob.ReservationStatus = ReservationStatus.ReservedByTec;
                    walletuser.Orders.Add(neworder);
                    await _jobRepository.UpdateAsync(findjob, cancellationToken);
                    await _walletRepository.UpdateAsync(walletuser, cancellationToken);
                    return Ok("Reserved job successfuly");
                }
                Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
                return new JsonResult("This job Already wating for reservetion by another Tecnecian ");
            }
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return BadRequest("can not book a job ");
        }
        

        [HttpGet("MyJobs")]
        public async Task<IActionResult> Myjobs(int page = 1, int pageSize = 10)
        {
            var findUser = await _userRepository.Entities.Include(p => p.Wallet)
                .FirstOrDefaultAsync(p => p.PhoneNumber.Equals(_httpContextAccessor.HttpContext.User.FindFirstValue("MobilePhone")));
            var walletuser = await _walletRepository.Entities.Include(p => p.Orders).ThenInclude(p=>p.OrderJobs).ThenInclude(p=>p.Job).ThenInclude(p=>p.Customer).FirstOrDefaultAsync(p => p.Id == findUser.Wallet.Id);
            var Resultwallet = await _userRepository.JobsUser(findUser.Wallet.Id, page, pageSize);
           
            var result = new List<ReadOrderDto>();
            var listorderjobs = new List<ReadJobDto>();
            foreach (var order in Resultwallet.Item1.Orders)
            {
                    foreach (var orderjob in order.OrderJobs)
                    {
                        var jobdto = _mapper.Map<ReadJobDto>(orderjob.Job);
                        jobdto.ReservationStatusResult = JobsUtills.SetReservationStatus((int)orderjob.Job.ReservationStatus);
                        jobdto.StatusJob = JobsUtills.SetStatusJob((int)orderjob.Job.StatusJob);
                        var orderdto = new ReadOrderDto() { spent = order.spent, Total = order.Total, Id = order.Id };
                        orderdto.JobsOrder.Add(jobdto);
                        result.Add(orderdto);
                    }
            }
            return new JsonResult(new { PaginationData = Resultwallet.Item2, orders =  result });
        }


        //[HttpPut("Change_of_work_status")]
        [HttpPost("Change_of_work_status")]
        public async Task<IActionResult> Change_of_work_status(int orderid, ChangestatusJob model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return BadRequest(model);
            var order=await _orderRepository.Entities.Include(p=>p.OrderJobs).ThenInclude(p=>p.Job).FirstOrDefaultAsync(p=>p.Id==orderid);
            var idjob=order.OrderJobs.Select(p=>p.Job.Id).FirstOrDefault();
            var findjob = await _jobRepository.GetByIdAsync(cancellationToken, idjob);
            if (findjob.ReservationStatus.Equals(ReservationStatus.ConfirmeByAdmin))
            {
                if (!order.OrderJobs.Any(p => p.Job.Id == findjob.Id)) return NotFound("job dos not exist order ");
                
                findjob.StatusJob = model.StatusJobDto;
                order.Description = model.Description;
                await _jobRepository.UpdateAsync(findjob, cancellationToken);
                foreach (var job in order.OrderJobs)
                {
                    job.Job = findjob;
                }
                await _orderRepository.UpdateAsync(order, cancellationToken);
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Ok("sucessful");
                    
            }
            Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
            return new JsonResult(new { message = "Can not change status job becuse confirmetion by admin please waiting ..." });
        }

        // [HttpPut("End_of_the_work")]
        [HttpPost("End_of_the_work")]
        public async Task<IActionResult> End_of_the_work(int orderid,UpdateStatusjobdEndwork model,CancellationToken cancellationToken)
        {
            var findUserwallet = await _userRepository.Entities.Include(p => p.Wallet).Include(p=>p.Room).Include(p => p.Specialties)
              .FirstOrDefaultAsync(p => p.PhoneNumber.Equals(_httpContextAccessor.HttpContext.User.FindFirstValue("MobilePhone")));

            var order = await _orderRepository.Entities.Include(p => p.OrderJobs).ThenInclude(p => p.Job).ThenInclude(p=>p.Supplies).FirstOrDefaultAsync(p => p.Id == orderid);
            var jobid = order.OrderJobs.Select(p => p.Job.Id).FirstOrDefault();
            var findjob = await _jobRepository.Entities.Include(p=>p.Customer).Include(p=>p.Supplies).FirstOrDefaultAsync(p=>p.Id==jobid);
            
            if (!order.OrderJobs.Any(p => p.Job.Id == findjob.Id)) return NotFound("job dos not exist order ");
            
            
            
            foreach (var item in model.suppliessDtos)
            {
                var s = new Supplies() {Price=item.Price,Name=item.Name};
                order.Total += item.Price ;
                findjob.Supplies.Add(s);
            }
            order.spent = model.Spent;
            order.Description = model.DescriptionOrder;
            findjob.Description = model.DescriptionOrder;
            findjob.StatusJob = StatusJob.Completed;
            findjob.usTagged = model.UsedTokcet;

            foreach (var item in order.OrderJobs)
            {
                item.Job = findjob;
            }
            order.CodemeliiCustomer = model.CodemeliiCustomer;
            await _jobRepository.UpdateAsync(findjob,cancellationToken);
            await _orderRepository.UpdateAsync(order, cancellationToken);

            findUserwallet.Wallet.Diposit(order.spent);
            await _userRepository.UpdateAsync(findUserwallet, cancellationToken);
            
            return Ok();
        }


    }
}


