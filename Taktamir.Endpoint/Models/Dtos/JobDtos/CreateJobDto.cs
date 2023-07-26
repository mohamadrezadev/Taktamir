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
        [Required(ErrorMessage = "Device name is required")]
        [StringLength(50, ErrorMessage = "Device name cannot be longer than 50 characters")]
        public string Name_Device { get; set; }

        [Required(ErrorMessage = "Problem description is required")]
        [StringLength(200, ErrorMessage = "Problem description cannot be longer than 200 characters")]
        public string Problems { get; set; }

        [MaxLength(200, ErrorMessage = "Description cannot be longer than 200 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Customer information is required")]
        public CreateCustomerDto CustomerDto { get; set; }
    }

}
