using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Taktamir.Core.Domain._01.Jobs;
using Taktamir.Core.Domain._03.Users;
using Taktamir.Endpoint.Models.Dtos.CustomerDtos;
using Taktamir.Endpoint.Models.Dtos.JobDtos;
using Taktamir.Endpoint.Models.Dtos.UserDtos;
using Taktamir.infra.Data.sql._02.Jobs;

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

        public JobAdminController(IJobRepository JobService,IMapper mapper)
        {
            _JobService = JobService;
            _mapper = mapper;
        }
        [HttpGet]
        public IActionResult Get()
        {
            var jobs = _JobService.Entities.Include(p =>  p.Customer ).ToList();
            var result=_mapper.Map<List<ReadJobDto>>(jobs);
            jobs.ForEach(item =>
            {
                result.ForEach(itemdto =>
                {
                    itemdto.Customer=_mapper.Map<ReadCustomerDto>(item.Customer);
                    itemdto.StatusJob=  ReadJobDto.SetStatusJob(item.StatusJob);
                    itemdto.ReservationStatusResult = ReadJobDto.SetReservationStatus((int)item.ReservationStatus);
                });
                
            });
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id,CancellationToken cancellationToken)
        {
            var job =await _JobService.GetByIdAsync(cancellationToken,id); 
            var result = _mapper.Map<ReadJobDto>(job);
            result.StatusJob=ReadJobDto.SetStatusJob(job.StatusJob);
            result.ReservationStatusResult = ReadJobDto.SetReservationStatus((int)job.ReservationStatus);
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
