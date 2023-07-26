using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Taktamir.Core.Domain._01.Jobs;
using Taktamir.Core.Domain._06.Wallets;
using Taktamir.Endpoint.Models.Dtos.JobDtos;

namespace Taktamir.Endpoint.Models.Dtos.WalletDtos
{
    public class ReadOrderDto
    {
        
        public int Id { get; set; }
        public double Total { get; set; }
        public double spent { get; set; }


       // public ReadWalletDto Wallet { get; set; }
        public ICollection<ReadJobDto> Jobs { get; set; }
    }
}