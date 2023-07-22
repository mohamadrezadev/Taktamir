using System.ComponentModel.DataAnnotations;

namespace Taktamir.Endpoint.Models.Dtos.UserDtos
{
    public class UpdateUserDto
    {
      
        [Required]
        public string Firstname { get; set; }
        [Required]
        public string LastName { get; set; }
        public string Profile_url { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string SerialNumber { get; set; }
        public virtual ICollection<SpecialtyDto> specialties { get; set; }

    }
}
