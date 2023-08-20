using Taktamir.Core.Domain._07.Suppliess;

namespace Taktamir.Endpoint.Models.Dtos.AdminDto
{
    public class OrderDetaillDto
    {
        public OrderDetaillDto()
        {
            Supplies=new HashSet<SuppliesOrderDto>();
        }
        public double Total { get; set; }
        public string NameDevice { get; set; }
        public string Proplem { get; set; }
        public string fullNameCustomer { get; set; }
        public string StatrusPayment { get; set; }
        public ICollection<SuppliesOrderDto> Supplies { get; set; }

    }
    public class SuppliesOrderDto
    {
        public double Price  { get; set; }
        public string  Name  { get; set; }
    }
}
