using System.ComponentModel.DataAnnotations;
using Taktamir.Endpoint.Models.Dtos.CustomerDtos;

namespace Taktamir.Endpoint.Models.Dtos.JobDtos
{
    public class ReadJobDto
    {
        public int Id { get; set; }
        public string Name_Device { get; set; }
        public string Problems { get; set; }

        public StatusJobDto StatusJob { get; set; }
        [MaxLength(200)]
        public string Description { get; set; }
        public bool UsedTokcet { get; set; } = false;
        public bool Reservation { get; set; } = false;

        public ReadCustomerDto Customer { get; set; }

    }
}
