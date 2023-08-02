using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Taktamir.Core.Domain._01.Jobs;
using Taktamir.Core.Domain._03.Users;
using Taktamir.Core.Domain._05.Messages;
using Taktamir.Core.Domain._06.Wallets;
using Taktamir.Core.Domain._07.Suppliess;
using Taktamir.Endpoint.Models.Dtos.JobDtos;
using Taktamir.Endpoint.Models.Dtos.UserDtos;
using Taktamir.Endpoint.Models.Dtos.WalletDtos;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Taktamir.Endpoint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
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
        [Authorize]
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


        [Authorize]
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
            return Ok(result);

        }

        [Authorize]
        [HttpGet("Book_a_job")]
        public async Task<IActionResult> Book_a_job(int IdJob,CancellationToken cancellationToken)
        {
            var findjob =await _jobRepository.GetByIdAsync(cancellationToken, IdJob);
            if (findjob == null || IdJob <= 0) return BadRequest("Inalid Id job");
            var findUser = await _userRepository.Entities.Include(p => p.Wallet)
              .FirstOrDefaultAsync(p => p.PhoneNumber.Equals(_httpContextAccessor.HttpContext.User.FindFirstValue("MobilePhone")));
            var walletuser = await _walletRepository.Entities.Include(p => p.Orders).FirstOrDefaultAsync(p => p.Id == findUser.Wallet.Id);
            var neworder = new Order();
            neworder.Jobs.Add(findjob);
            findjob.ReservationStatus = ReservationStatus.ReservedByTec;
            findjob.StatusJob =(int) StatusJob.waiting;
            walletuser.Orders.Add(neworder);
            await _jobRepository.UpdateAsync(findjob, cancellationToken);
            await _walletRepository.UpdateAsync(walletuser, cancellationToken);

            return Ok();
        }
        
        [Authorize]
        [HttpGet("MyJobs")]
        public async Task<IActionResult> Myjobs()
        {
            var findUser = await _userRepository.Entities.Include(p => p.Wallet)
                .FirstOrDefaultAsync(p => p.PhoneNumber.Equals(_httpContextAccessor.HttpContext.User.FindFirstValue("MobilePhone")));
            var walletuser = await _walletRepository.Entities.Include(p => p.Orders).FirstOrDefaultAsync(p => p.Id == findUser.Wallet.Id);
            var orders = new List<Order>();
            foreach (var orderitem in walletuser.Orders)
            {
                var order =await _orderRepository.Entities.Include(p => p.Jobs).ThenInclude(p=>p.Customer).FirstOrDefaultAsync(p => p.Id == orderitem.Id);
                orders.Add(order);
            }
            var result = _mapper.Map<List<ReadOrderDto>>(orders);
          
            return Ok(result);
        }

        [Authorize]
        //[HttpPut("Change_of_work_status")]
        [HttpPost("Change_of_work_status")]
        public async Task<IActionResult> Change_of_work_status(int orderid,int jobid, UpdateJobDto model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return BadRequest(model);
            var order=await _orderRepository.Entities.Include(p=>p.Jobs).FirstOrDefaultAsync(p=>p.Id==orderid);
            var findjob = await _jobRepository.GetByIdAsync(cancellationToken, jobid);
            if (!order.Jobs.Any(p => p.Id == findjob.Id)) return NotFound("job dos not exist order ");   
            findjob.StatusJob =(int) model.StatusJob;
            findjob.Description=model.Description;
            await _jobRepository.UpdateAsync(findjob, cancellationToken);
            return Ok();
        }
        [Authorize]
       // [HttpPut("End_of_the_work")]
        [HttpPost("End_of_the_work")]
        public async Task<IActionResult> End_of_the_work(int orderid,int jobid,UpdateJobDto model,CancellationToken cancellationToken)
        {
            var findUser = await _userRepository.Entities.Include(p => p.Wallet)
                .FirstOrDefaultAsync(p => p.PhoneNumber.Equals(_httpContextAccessor.HttpContext.User.FindFirstValue("MobilePhone")));
            var order=await _orderRepository.Entities.Include(p=>p.Jobs).FirstOrDefaultAsync(p=>p.Id == orderid);
            var findjob = await _jobRepository.GetByIdAsync(cancellationToken, jobid);
            if (!order.Jobs.Any(p => p.Id == findjob.Id)) return NotFound("job dos not exist order ");
            findjob.Description = model.Description;
            findjob.StatusJob=(int) model.StatusJob;
            foreach (var item in model.suppliessDtos)
            {
                var s = await _suppliesRepository.Entities.FirstOrDefaultAsync(p => p.Name.Equals(item.Name));
                order.Total += item.Price ;
                order.spent += s.Price;
                findjob.Supplies.Add(s);
            }
            findjob.usTagged = model.UsedTokcet;

            findUser.Wallet.Diposit(order.Total);

            await _jobRepository.UpdateAsync(findjob,cancellationToken);
            await _orderRepository.UpdateAsync(order, cancellationToken);
            
            return Ok();
        }




    }
}


///// <summary>
///// Retrieves a list of all jobs.
///// </summary>
///// <remarks>
///// Sample request:
/////
/////     GET /api/jobs
/////
///// </remarks>
///// <returns>A list of all jobs.</returns>
///// <response code="200">Returns the list of jobs.</response>
//[HttpGet("Jobs")]
//public async Task<IActionResult> jobs()
//{
//    var jobs =await _jobRepository.GetAllJobs();
//    var result = _mapper.Map<List<ReadJobDto>>(jobs);
//    return Ok(result);
//}   

///// <summary>
///// Books a job for a user and creates an order for the job.
///// </summary>
///// <remarks>
///// Sample request:
/////
/////     POST /api/jobs/BookingJob?idjob=1?userid=2
/////
///// </remarks>

///// <returns>No content.</returns>
///// <response code="200">The job was booked successfully.</response>
///// <response code="400">If the request is invalid.</response>
///// <response code="404">If the job or user is not found.</response>
//[HttpPost("BookingJob")]
//[ProducesResponseType(StatusCodes.Status200OK)]
//[ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
//[ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
//[Authorize(Roles = UserRoleApp.Technician)]
//public async Task<IActionResult> Job_booking(int idjob, CancellationToken cancellation )
//{

//    var findUser = await _userRepository.Entities.Include(p => p.Wallet).Include(p => p.Specialties)
//     .FirstOrDefaultAsync(p => p.PhoneNumber.Equals(_httpContextAccessor.HttpContext.User.FindFirstValue("MobilePhone")));
//    var walletUser = await _walletRepository.Entities.Include(p => p.Orders).FirstOrDefaultAsync(p => p.Id == findUser.Wallet.Id);

//    if (idjob <= 0 || findUser==null) return BadRequest();

//    var findjob = await _jobRepository.GetByIdAsync(cancellation,idjob);
//    if (findjob == null)   return BadRequest("Job dos not exist");

//    findjob.StatusJob =(int) StatusJob.waiting;
//    findjob.ReservationStatus = ReservationStatus.ReservedByTec;
//    await _jobRepository.UpdateAsync(findjob, cancellation);
//    var neworder = new Order();
//    neworder.Jobs.Add(findjob);
//    walletUser.Orders.Add(neworder);
//    await _walletRepository.UpdateAsync(walletUser, cancellation);
//    return Ok();
//}




///// <summary>
///// Changes the status of a job by ID.
///// </summary>
///// <remarks>
///// Sample request:
/////
/////     PUT /api/jobs/ChangeStatusjob/1
/////     {
/////         "status": 1,
/////         "Description": "Job is in progress"
/////     }
/////
///// </remarks>
///// <param name="idjob">The ID of the job to update.</param>
///// <param name="statusJobDto">The new status and description for the job.</param>
///// <returns>No content.</returns>
///// <response code="200">The job status was updated successfully.</response>
///// <response code="404">If the job is not found.</response>
//[HttpPut("ChangeStatusjob/{idjob}")]
//[ProducesResponseType(StatusCodes.Status200OK)]
//[ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
//public async Task<IActionResult> ChangeStatushob(int idjob, [FromBody] ChangeStatusJobDto statusJobDto)
//{
//    CancellationToken cancellationToken =CancellationToken.None;
//    var job = await _jobRepository.GetByIdAsync(cancellationToken,idjob);
//    if (job == null) return NotFound("Not Found Job");
//    job.StatusJob=(int)statusJobDto.status;
//    job.Description = statusJobDto.Description;
//    await _jobRepository.UpdateAsync(job, cancellationToken);
//    Response.StatusCode = StatusCodes.Status204NoContent;
//    return Ok();
//}

///// <summary>
///// Marks a job as completed and adds job completion details.
///// </summary>
///// <remarks>
///// Sample request:
/////
/////     PUT /api/jobs/Completework/1
/////     {
/////         "description": "Job completed successfully.",
/////         "suppliesDto": [
/////             {
/////                 "name": "Supply 1",
/////                 "price": 10.99
/////             },
/////             {
/////                 "name": "Supply 2",
/////                 "price": 20.99
/////             }
/////         ]
/////     }
/////
///// </remarks>
///// <param name="idjob">The ID of the job to mark as completed.</param>
///// <param name="completeJob">The completion details for the job.</param>
///// <returns>No content.</returns>
///// <response code="204">The job was marked as completed successfully.</response>
///// <response code="400">If the request is invalid.</response>
///// <response code="404">If the job is not found.</response>
//[HttpPut("Completework/{idjob}")]
//[ProducesResponseType(StatusCodes.Status204NoContent)]
//[ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
//[ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]  
//public  IActionResult CompleteJob(int idjob, [FromBody] CompleteJobDto completeJob )
//{


//    var job = _jobRepository.GetById(idjob);
//    if (job == null) return NotFound("Job not found");
//    job.StatusJob = (int)StatusJob.Completed;
//    job.Description = completeJob.Description;
//    completeJob.SuppliessDto.ForEach(item =>
//    {
//        job.Supplies.Add(new Supplies() { Name = item.Name, Price = item.Price });
//    });
//    _suppliesRepository.UpdateRange(job.Supplies);
//    _jobRepository.Update(job);
//    Response.StatusCode= StatusCodes.Status204NoContent;
//    return Ok();
//}

//[HttpPost("Change_Status_Account_User")]
//public IActionResult StatusAccountUser(int userId,bool IsActive)
//{
//    var user = _userRepository.GetById(userId);
//    if (user == null) return NotFound("User Not Found");
//    user.IsActive = IsActive;
//    _userRepository.Update(user);
//    return Ok("Changed Status Account User");
//}