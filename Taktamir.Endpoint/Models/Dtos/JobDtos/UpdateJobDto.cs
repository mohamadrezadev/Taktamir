using System.ComponentModel.DataAnnotations;
using Taktamir.Endpoint.Models.Dtos.Suppliess;
using Taktamir.framework.Common.JobsUtill;

namespace Taktamir.Endpoint.Models.Dtos.JobDtos
{
    public class UpdateJobDto
    {
        public string Name_Device { get; set; }
        public string Problems { get; set; }
        public StatusJob StatusJob { get; set; } = StatusJob.Completed;
        [MaxLength(200)]
        public string Description { get; set; }
        public bool UsedTokcet { get; set; } = false;
        public ICollection <SuppliessDto> suppliessDtos { get; set; }

    }
    public class ChangestatusJob
    {
        public string Description { get; set; }
        public StatusJob StatusJobDto { get; set; }

    }
   
}
