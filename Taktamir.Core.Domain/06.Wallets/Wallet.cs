using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taktamir.Core.Domain._01.Common;
using Taktamir.Core.Domain._03.Users;

namespace Taktamir.Core.Domain._06.Wallets
{
    public class Wallet:IEntity
    {
        public Wallet()
        {
            this.Orders = new List<Order>();
        }
        public int Id { get; set; }
        public double Balance { get; private set; } = 0;
        public ICollection<Order> Orders { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User  User { get; set; }

        public void Diposit(double amount)
        {
            if (amount > 0){
               this.Balance += amount;
            }
        }

    }
}
