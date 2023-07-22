using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Taktamir.Core.Domain._01.Jobs;
using Taktamir.Core.Domain._03.Users;
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

        public UsersController(IUserRepository userRepository,IMapper mapper,
            IJobRepository jobRepository,IWalletRepository walletRepository,
            ISuppliesRepository suppliesRepository,IOrderRepository orderRepository)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _jobRepository=jobRepository;
            _walletRepository = walletRepository;
            _suppliesRepository = suppliesRepository;
            _orderRepository = orderRepository;
        }


        /// <summary>
        /// Retrieves a user by ID.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/users/2
        ///
        /// </remarks>
        /// <param name="id">The ID of the user to retrieve.</param>
        /// <returns>The user with the specified ID.</returns>
        /// <response code="200">Returns the user.</response>
        /// <response code="404">If the user is not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id )
        {
            
            var user = await _userRepository.Entities.Include(p => p.Wallet).FirstOrDefaultAsync(p => p.Id == id);
            if (user == null) return NotFound();
            var result = _mapper.Map<ReadUserDto>(user);
            return Ok();
        }

        /// <summary>
        /// Retrieves a list of all orders for a user.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/jobs/AllMyOrders/2
        ///
        /// </remarks>
        /// <param name="userid">The ID of the user to retrieve orders for.</param>
        /// <returns>A list of all orders for the user.</returns>
        /// <response code="200">Returns the list of orders.</response>
        /// <response code="404">If the user is not found.</response>
        [HttpGet("AllMyOrders/{userid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AllMyOrders(int userid)
        {
            var orders =await _walletRepository.GetAllOrders(userid);
            var result = _mapper.Map<List<ReadOrderDto>>(orders);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a list of all jobs.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/jobs
        ///
        /// </remarks>
        /// <returns>A list of all jobs.</returns>
        /// <response code="200">Returns the list of jobs.</response>
        [HttpGet("Jobs")]
        public async Task<IActionResult> jobs()
        {
            var jobs =await _jobRepository.GetAllJobs();
            var result = _mapper.Map<List<ReadJobDto>>(jobs);
            return Ok(result);
        }

        /// <summary>
        /// Books a job for a user and creates an order for the job.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/jobs/BookingJob?idjob=1?userid=2
        ///
        /// </remarks>
        /// <param name="idjob">The ID of the job to book.</param>
        /// <param name="userid">The ID of the user to book the job for.</param>
        /// <returns>No content.</returns>
        /// <response code="200">The job was booked successfully.</response>
        /// <response code="400">If the request is invalid.</response>
        /// <response code="404">If the job or user is not found.</response>
        [HttpPost("BookingJob")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> BookingJob(int idjob ,int userid)
        {
            CancellationToken cancellation = CancellationToken.None;
            if (idjob <= 0 || userid <= 0) return BadRequest();
            
            var wallet=await _walletRepository.Entities.Include(p=>p.Orders).FirstOrDefaultAsync(p=>p.Id == userid);
            if (wallet == null) return NotFound("Not found wallet");

            var job = await _jobRepository.GetByIdAsync(cancellation, idjob);
            job.Reservation = true;
            job.StatusJob =(int) StatusJob.Doing;
            job.User= await _userRepository.GetByIdAsync(cancellation, userid);
            await _jobRepository.UpdateAsync(job, cancellation);
            var neworder = new Order()
            {
                Wallet = wallet,
                Jobs = job,
            };
            await _orderRepository.UpdateAsync(neworder, cancellation);
 
            wallet.Orders.Add(neworder);
            await _walletRepository.UpdateAsync(wallet, cancellation);
            return Ok();
        }




        /// <summary>
        /// Changes the status of a job by ID.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/jobs/ChangeStatusjob/1
        ///     {
        ///         "status": 1,
        ///         "Description": "Job is in progress"
        ///     }
        ///
        /// </remarks>
        /// <param name="idjob">The ID of the job to update.</param>
        /// <param name="statusJobDto">The new status and description for the job.</param>
        /// <returns>No content.</returns>
        /// <response code="200">The job status was updated successfully.</response>
        /// <response code="404">If the job is not found.</response>
        [HttpPut("ChangeStatusjob/{idjob}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangeStatushob(int idjob, [FromBody] ChangeStatusJobDto statusJobDto)
        {
            CancellationToken cancellationToken =CancellationToken.None;
            var job = await _jobRepository.GetByIdAsync(cancellationToken,idjob);
            if (job == null) return NotFound("Not Found Job");
            job.StatusJob=(int)statusJobDto.status;
            job.Description = statusJobDto.Description;
            await _jobRepository.UpdateAsync(job, cancellationToken);
            Response.StatusCode = StatusCodes.Status204NoContent;
            return Ok();
        }
        
        /// <summary>
        /// Marks a job as completed and adds job completion details.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/jobs/Completework/1
        ///     {
        ///         "description": "Job completed successfully.",
        ///         "suppliesDto": [
        ///             {
        ///                 "name": "Supply 1",
        ///                 "price": 10.99
        ///             },
        ///             {
        ///                 "name": "Supply 2",
        ///                 "price": 20.99
        ///             }
        ///         ]
        ///     }
        ///
        /// </remarks>
        /// <param name="idjob">The ID of the job to mark as completed.</param>
        /// <param name="completeJob">The completion details for the job.</param>
        /// <returns>No content.</returns>
        /// <response code="204">The job was marked as completed successfully.</response>
        /// <response code="400">If the request is invalid.</response>
        /// <response code="404">If the job is not found.</response>
        [HttpPut("Completework/{idjob}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]  
        public  IActionResult CompleteJob(int idjob, [FromBody] CompleteJobDto completeJob )
        {
 
            
            var job = _jobRepository.GetById(idjob);
            if (job == null) return NotFound("Job not found");
            job.StatusJob = (int)StatusJob.Completed;
            job.Description = completeJob.Description;
            completeJob.SuppliessDto.ForEach(item =>
            {
                job.Supplies.Add(new Supplies() { Name = item.Name, Price = item.Price });
            });
            _suppliesRepository.UpdateRange(job.Supplies);
            _jobRepository.Update(job);
            Response.StatusCode= StatusCodes.Status204NoContent;
            return Ok();
        }

        [HttpPost("Change_Status_Account_User")]
        public IActionResult StatusAccountUser(int userId,bool IsActive)
        {
            var user = _userRepository.GetById(userId);
            if (user == null) return NotFound("User Not Found");
            user.IsActive = IsActive;
            _userRepository.Update(user);
            return Ok("Changed Status Account User");
        }
    }
}
