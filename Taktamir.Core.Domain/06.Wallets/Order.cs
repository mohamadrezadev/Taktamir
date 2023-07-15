using System.ComponentModel.DataAnnotations.Schema;
using Taktamir.Core.Domain._01.Common;
using Taktamir.Core.Domain._01.Jobs;

namespace Taktamir.Core.Domain._06.Wallets
{
    public class Order:IEntity
    {
        public Order()
        {
            this.Jobs = new List<Job>();
        }
        public int Id { get; set; }
        public double Total { get; set; }
        public double spent { get; set; }

        public int JobId { get; set; }
        [ForeignKey("JobId")]
        public ICollection<Job> Jobs { get; set; }
    }
}