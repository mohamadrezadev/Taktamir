using System.ComponentModel.DataAnnotations.Schema;
using Taktamir.Core.Domain._05.Messages;
using Taktamir.Core.Domain._06.Wallets;
using Taktamir.Endpoint.Models.Dtos.WalletDtos;
using Taktamir.Core.Domain._03.Users;

namespace Taktamir.Endpoint.Models.Dtos.UserDtos
{
    public class ReadUserDto
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string LastName { get; set; }
        public string Profile_url { get; set; }
        public DateTime Create_at { get; set; }
        public DateTime Update_at { get; set; }
        public bool IsActive { get; set; }
        public string SerialNumber { get; set; }
        public bool IsCompleteprofile { get; set; }
        public virtual ReadWalletDto Wallet { get; set; }
    }
}
