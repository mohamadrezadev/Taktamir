using Taktamir.Endpoint.Models.Dtos.WalletDtos;

namespace Taktamir.Endpoint.Models.Dtos.UserDtos
{
    public class ReadUserorderDto
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public virtual ICollection<ReadOrderDto> ReadOrderDtos { get; set; }
    }


}
