using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Taktamir.Core.Domain._03.Users;
using Taktamir.Core.Domain._07.Suppliess;
using Taktamir.Core.Domain._4.Customers;
using Taktamir.Endpoint.Models.Dtos.CustomerDtos;

namespace Taktamir.Endpoint.Models.Dtos.JobDtos
{
    public class CreateJobDto
    {
      
        public string Name_Device { get; set; }
        public string Problems { get; set; }
        [MaxLength(200)]
        public string Description { get; set; }
        public CreateCustomerDto CustomerDto  { get; set; }
        
       
    }
}
