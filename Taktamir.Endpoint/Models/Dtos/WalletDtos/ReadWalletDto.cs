using System.ComponentModel.DataAnnotations.Schema;
using Taktamir.Core.Domain._03.Users;
using Taktamir.Core.Domain._06.Wallets;

namespace Taktamir.Endpoint.Models.Dtos.WalletDtos
{
    public class ReadWalletDto
    {
        public ReadWalletDto()
        {
            Orders = new List<ReadOrderDto> ();
        }
        public int Id { get; set; }
        public double Balance { get;  set; } 
        public ICollection<ReadOrderDto> Orders { get; set; }
    }
}
