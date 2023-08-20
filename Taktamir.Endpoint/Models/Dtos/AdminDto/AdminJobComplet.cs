using Taktamir.Core.Domain._06.Wallets;

namespace Taktamir.Endpoint.Models.Dtos.AdminDto
{
    public class UserOrderDto
    {
        public UserOrderDto()
        {
            Orders = new HashSet<OrdersUserDto>();
        }
        public int Userid { get; set; }
        public string Fullname { get; set; }
        public string PhoneNumber { get; set; }
        public virtual ICollection<OrdersUserDto> Orders { get; set; }
    }
    public class OrdersUserDto
    {
        public int OrderId { get; set; }
        public int jobId { get; set; }
        public double Total { get; set; }
        public double Spent { get; set; }
        public string NameCustomer { get; set; }
        public string TitletJob { get; set; }

        public string PaymentStatus { get; set; }
    }
}
