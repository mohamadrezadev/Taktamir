using System.ComponentModel.DataAnnotations.Schema;
using Taktamir.Core.Domain._03.Users;
using Taktamir.Core.Domain._06.Wallets;

namespace Taktamir.Endpoint.Models.Dtos.WalletDtos
{
    public class ReadWalletDto
    {
        public int Id { get; set; }
        public double Balance { get; private set; } = 0;
        public List<ReadOrderDto> Orders { get; set; }
    }
}
