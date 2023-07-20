using System.ComponentModel.DataAnnotations;

namespace Taktamir.Endpoint.Models.Dtos.JobDtos
{
    public class UpdateJobDto
    {
        public string Name_Device { get; set; }
        public string Problems { get; set; }
        public StatusJobDto StatusJob { get; set; }
        [MaxLength(200)]
        public string Description { get; set; }
        public bool UsedTokcet { get; set; } = false;
        public bool Reservation { get; set; } = false;

    }
}
