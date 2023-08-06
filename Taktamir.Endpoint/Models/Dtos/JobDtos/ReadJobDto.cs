using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Taktamir.Core.Domain._01.Jobs;
using Taktamir.Endpoint.Models.Dtos.CustomerDtos;
using Taktamir.Endpoint.Models.Dtos.UserDtos;

namespace Taktamir.Endpoint.Models.Dtos.JobDtos
{
    public class ReadJobDto
    {
        public int Id { get; set; }
        public string Name_Device { get; set; }
        public string Problems { get; set; }
        public string StatusJob { get; set; }
        [MaxLength(200)]
        public string Description { get; set; }
        public bool UsedTokcet { get; set; } = false;
        public bool Reservation { get; set; } = false;
        public string ReservationStatusResult { get; set; }
        public ReadCustomerDto Customer { get; set; }
    }
}
