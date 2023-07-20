using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Taktamir.Core.Domain._01.Common;
using Taktamir.Core.Domain._01.Jobs;
using Taktamir.Core.Domain._4.Customers;
using Taktamir.Endpoint.Models.Dtos.CustomerDtos;
using Taktamir.Endpoint.Models.Dtos.JobDtos;

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
        public async Task<IActionResult> Get()
        {
            var jobs = await _jobRepository.Entities.Include(j => j.Customer).ToListAsync();
            var result = _mapper.Map<List<ReadJobDto>>(jobs);
            return Ok(result);
           
        }

        // GET api/<JobsController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            if (id <= 0) return BadRequest();
            var findjob=await _jobRepository.GetJobBtid(id);

            var resutl=_mapper.Map<ReadJobDto>(findjob);
            resutl.Customer=_mapper.Map<ReadCustomerDto>(resutl.Customer);
            return Ok(resutl);
        }

        // POST api/<JobsController>
        [HttpPost]
        public async Task<IActionResult> Post( CreateJobDto model,CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return  BadRequest(model);
            var customer = _mapper.Map<Customer>(model.CustomerDto);
            var newjob = _mapper.Map<Job>(model);         
            await _customerRepository.AddAsync(customer, cancellationToken);
            newjob.Customer= customer;
            newjob.Customer.Id= customer.Id;
           //await _jobRepository.AddAsync(newjob, cancellationToken);
            await _jobRepository.AddAsync(newjob,cancellationToken);
            return Created($"/api/Jobs/get/{newjob.Id}", newjob.Id);
        }

    }
}
