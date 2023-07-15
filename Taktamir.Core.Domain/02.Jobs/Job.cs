using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taktamir.Core.Domain._01.Common;
using Taktamir.Core.Domain._03.Users;
using Taktamir.Core.Domain._07.Suppliess;
using Taktamir.Core.Domain._4.Customers;

namespace Taktamir.Core.Domain._01.Jobs
{
    public class Job:IEntity
    {
        public Job()
        {
            this.Supplies=new List<Supplies>();
            this.Reservation = false;
        }
        public int Id { get; set; }
        public string Name_Device { get; set; }
        public string Problems { get; set; }

        public int StatusJob { get; set; }
        public string Description { get; set; }
        public bool UsedTokcet { get; set; }
        public bool Reservation { get; set; }
        public int SuppliesId { get; set; }
        [ForeignKey("SuppliesId")]
        public virtual ICollection<Supplies> Supplies { get; set; }

        public virtual Customer Customer { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
