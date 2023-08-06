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
            this.OrderJobs = new List<OrderJob>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public double Total { get; set; }
        public double spent { get; set; }

        public int WalletId { get; set; }
        public  Wallet Wallet { get; set; }

        public virtual ICollection<OrderJob> OrderJobs { get; set; }

    }
    public class OrderJob:IEntity
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int JobId { get; set; }
        public Job Job { get; set; }
    }
}