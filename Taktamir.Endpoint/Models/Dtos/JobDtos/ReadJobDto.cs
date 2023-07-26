using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
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

        public ReadCustomerDto Customer { get; set; }
     

        public static string SetStatusJob(int statusjob)
        {
            switch (statusjob)
            {
                case 0:
                    return "not yet status change ";
                case 1:
                    //return StatusJobDto.Completed.ToString();
                    return "not yet status change ";
                case 2:
                    return StatusJobDto.Cancel.ToString();
                case 3:
                    return StatusJobDto.Doing.ToString();
                case 4:
                    return StatusJobDto.Carry_off.ToString();
                case 5:
                    return StatusJobDto.High_time.ToString();
                case 6:
                    return StatusJobDto.waiting.ToString();
                default:
                    throw new ArgumentException("Invalid status job value.");
            }
        }


    }
}
