using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Taktamir.Core.Domain._01.Jobs;
using Taktamir.Core.Domain._03.Users;
using Taktamir.Core.Domain._06.Wallets;
using Taktamir.Endpoint.Models.Dtos.AdminDto;
using Taktamir.Endpoint.Models.Dtos.CustomerDtos;
using Taktamir.Endpoint.Models.Dtos.JobDtos;
using Taktamir.Endpoint.Models.Dtos.UserDtos;
using Taktamir.framework.Common;
using Taktamir.infra.Data.sql._02.Jobs;
using Taktamir.infra.Data.sql._06.Wallets;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Taktamir.Endpoint.Controllers.Admin
{
    [Authorize(Roles =UserRoleApp.Admin)]
    [Route("api/[controller]")]
    [ApiController]
    public class JobAdminController : ControllerBase
    {
        private readonly IJobRepository _JobService;
        private readonly IMapper _mapper;
        private readonly IWalletRepository _walletRepository;

        public JobAdminController(IJobRepository JobService,IMapper mapper,IWalletRepository walletRepository)
        {
            _JobService = JobService;
            _mapper = mapper;
            _walletRepository = walletRepository;
        }
        [HttpGet("AllJobs")]
        public async Task<IActionResult> GetAllJobs(int page = 1, int pageSize = 10)
        {
            var result = await _walletRepository.GetAllWorksbyAdmin(page, pageSize);



            var resultUsers = new List<ReadUsersAdmindto>();
            foreach (var user in result.Item1)
            {
                var Userdata = new ReadUsersAdmindto();
                Userdata.id = user.Id;
                Userdata.FullNameUser = $"{user.Firstname} {user.LastName}";
                Userdata.IsActive = user.IsActive;
                Userdata.PhoneNumber = user.PhoneNumber;
                Userdata.SpecialtyDtos = _mapper.Map<List<SpecialtyDto>>(user.Specialties);
                if (user.Wallet.Orders.Count()>0)
                {

                    foreach (var order in user.Wallet.Orders)
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
             return new JsonResult(new { Paginationdata = result.Item2, Data = resultUsers });
        }

       

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id,CancellationToken cancellationToken)
        {
            var job =await _JobService.GetByIdAsync(cancellationToken,id); 
            var result = _mapper.Map<ReadJobDto>(job);
            result.StatusJob=JobsUtills.SetStatusJob((int)job.StatusJob);
            result.ReservationStatusResult = JobsUtills.SetReservationStatus((int)job.ReservationStatus);
            result.Customer = _mapper.Map<ReadCustomerDto>(job.Customer);
            return Ok(result);
        }

    
      //  [HttpPut("{id}")]
        [HttpPost("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] ReadJobDto model, CancellationToken cancellationToken)
        {
            if (id <= 0) return BadRequest("Invalid Id");
            if (!ModelState.IsValid) return BadRequest(model);
            var updatejob = _mapper.Map<Job>(model);
            await _JobService.UpdateAsync(updatejob, cancellationToken);
            var result = await _JobService.GetByIdAsync(cancellationToken, updatejob.Id);
            Response.StatusCode = StatusCodes.Status204NoContent;
            return Ok(result);
        }
    }
}
