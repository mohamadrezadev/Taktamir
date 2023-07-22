using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Taktamir.Core.Domain._01.Jobs;
using Taktamir.Endpoint.Models.Dtos.JobDtos;
using Taktamir.infra.Data.sql._02.Jobs;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Taktamir.Endpoint.Controllers.Admin
{
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
            var jobs = _JobService.Entities.Include(p =>  p.Customer ).Include(p=>p.User).ToList();
            var result=_mapper.Map<List<Job>>(jobs);
            return Ok(result);
        }
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var job = _JobService.GetById(id); 
            var result = _mapper.Map<Job>(job);
            return Ok(result);
        }

        // PUT api/<JobsController>/5
        [HttpPut("{id}")]
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
