using System.ComponentModel.DataAnnotations;

namespace Taktamir.Endpoint.Models.Dtos.ChatDto
{
    public class CreateChatGroupDto
    {
        public long UserId { get; set; }
        public IFormFile ImageFile { get; set; }
        public string GroupName { get; set; }
    }
}
