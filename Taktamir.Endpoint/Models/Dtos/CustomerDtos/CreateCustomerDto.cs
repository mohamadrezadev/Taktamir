using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Taktamir.Core.Domain._01.Jobs;

namespace Taktamir.Endpoint.Models.Dtos.CustomerDtos
{
    public class CreateCustomerDto
    {
        
        [NotNull]
        [MaxLength(100)]
        public string FullNameCustomer { get; set; }
        [NotNull]
        [MaxLength(11, ErrorMessage = "Invalid PhoneNumber")]
        public string PhoneNumber { get; set; }
        [NotNull]
        [MaxLength(11, ErrorMessage = "Invalid Phone")]
        public string Phone { get; set; }
        [NotNull]
        [MaxLength(200)]
        public string Address { get; set; }
        [NotNull]
        [MaxLength(10, ErrorMessage = "Invalid Identification_code")]
        public string Identification_code { get; set; }
        
    }
}
