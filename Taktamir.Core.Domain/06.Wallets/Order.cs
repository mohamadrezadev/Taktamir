using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Taktamir.Core.Domain._01.Common;
using Taktamir.Core.Domain._01.Jobs;

namespace Taktamir.Core.Domain._06.Wallets
{
    public class Order:IEntity
    {
        public Order()
        {
            
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public double Total { get; set; }
        public double spent { get; set; }

     
        public  Wallet Wallet { get; set; }
        public Job Jobs { get; set; }
    }
}