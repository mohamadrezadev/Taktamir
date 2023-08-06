using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using Taktamir.Core.Domain._01.Common;
using Taktamir.Core.Domain._01.Jobs;
using Taktamir.Core.Domain._4.Customers;
using Taktamir.Endpoint.Models.Dtos.CustomerDtos;
using Taktamir.Endpoint.Models.Dtos.JobDtos;
using Taktamir.framework.Common;
using Taktamir.framework.Common.JobsUtill;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Taktamir.Endpoint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly IJobRepository _jobRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        public JobsController(IJobRepository jobRepository,ICustomerRepository customerRepository,IMapper mapper)
        {
            _jobRepository = jobRepository;
            _customerRepository = customerRepository;
            _mapper = mapper;
        }
        // GET: api/<JobsController>
        [HttpGet]
        public async Task<IActionResult> Get(int page = 1, int pageSize = 10)
        
        {
            var result = await _jobRepository.GetAllJobsAsync(page,pageSize);
           

            var response =new List<ReadJobDto>();
            foreach (var item in result.Item1)
            {

                var dto = new ReadJobDto();
                dto = _mapper.Map<ReadJobDto>(item);
                dto.Customer = _mapper.Map<ReadCustomerDto>(item.Customer);
                dto.ReservationStatusResult = JobsUtills.SetReservationStatus((int)item.ReservationStatus);
                response.Add(dto);
            }
            Response.StatusCode = (int)HttpStatusCode.OK;
            var res = new { PaginationData = result.Item2, Jobs = response};
            return Ok(res);
           
        }


        // GET api/<JobsController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            if (id <= 0) return BadRequest();
            var findjob=await _jobRepository.GetJobBtid(id);

            var resutl=_mapper.Map<ReadJobDto>(findjob);
            resutl.ReservationStatusResult = JobsUtills.SetReservationStatus((int)findjob.ReservationStatus);
            resutl.Customer=_mapper.Map<ReadCustomerDto>(resutl.Customer);
            return Ok(resutl);
        }

        
        [HttpPost]
        public async Task<IActionResult> Post( CreateJobDto model,CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return  BadRequest(model);
            var newjob = _mapper.Map<Job>(model); 
            newjob.ReservationStatus = ReservationStatus.WatingforReserve;
            newjob.Customer = _mapper.Map<Customer>(model.CustomerDto);

            await _jobRepository.AddAsync(newjob, cancellationToken);          
            return Created($"/api/Jobs/get/{newjob.Id}", newjob.Id);
        }
       
    }
}
