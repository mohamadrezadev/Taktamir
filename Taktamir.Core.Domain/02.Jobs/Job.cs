using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taktamir.Core.Domain._01.Common;
using Taktamir.Core.Domain._03.Users;
using Taktamir.Core.Domain._06.Wallets;
using Taktamir.Core.Domain._07.Suppliess;
using Taktamir.Core.Domain._4.Customers;

namespace Taktamir.Core.Domain._01.Jobs
{
    public class Job:IEntity
    {
        public Job()
        {
            this.Supplies = new HashSet<Supplies>();
            this.Reservation = false;
            ReservationStatus = ReservationStatus.WatingforReserve;
            OrderJobs = new HashSet<OrderJob>();
            
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name_Device { get; set; }
        public string Problems { get; set; }
        public int StatusJob { get; set; }
        public string Description { get; set; }
        public bool usTagged { get; set; }
        public bool Reservation { get; set; }
        public ReservationStatus ReservationStatus { get; set; }


        [AllowNull]
        public virtual ICollection<Supplies> Supplies { get; set; }

        public virtual ICollection<OrderJob> OrderJobs { get; set; }

        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

    }
    public enum ReservationStatus
    { 
        WatingforReserve=0,
        ReservedByTec=1,
        Pending=2,       
        ConfirmeByidmin=3,
    }

}
