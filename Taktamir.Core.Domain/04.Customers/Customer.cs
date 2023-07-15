using Taktamir.Core.Domain._01.Common;
using Taktamir.Core.Domain._01.Jobs;

namespace Taktamir.Core.Domain._4.Customers

{
    public class Customer:IEntity
    {
        public Customer()
        {
            Jobs = new HashSet<Job>();
        }
        public int Id { get; set; }
        public string FullNameCustomer { get; set; }
        public string PhoneNumber { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Identification_code { get; set; }
        public virtual ICollection<Job> Jobs { get; set; }

    }
}
