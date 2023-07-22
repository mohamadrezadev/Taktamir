using System.ComponentModel.DataAnnotations;

namespace Taktamir.Endpoint.Models.Dtos.UserDtos
{
    public class ConfirmMobileDto  
    {
        [Required(ErrorMessage = "Verification code is required.")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format. Please enter a valid phone number.")]
        public string Phone_number { get; set; }
    }

}
