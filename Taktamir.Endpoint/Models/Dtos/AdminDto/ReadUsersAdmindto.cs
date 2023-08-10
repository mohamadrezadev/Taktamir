using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Taktamir.Endpoint.Models.Dtos.CustomerDtos;
using Taktamir.Endpoint.Models.Dtos.JobDtos;
using Taktamir.Endpoint.Models.Dtos.UserDtos;
using Taktamir.Endpoint.Models.Dtos.WalletDtos;

namespace Taktamir.Endpoint.Models.Dtos.AdminDto
{
    public class ReadUsersAdmindto
    {
        public ReadUsersAdmindto()
        {
            this.SpecialtyDtos=new HashSet<SpecialtyDto>();
            this.orders=new HashSet<OrderUserAdmindto>();
        }
        public int id { get; set; }
        public string  FullNameUser { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public ICollection<OrderUserAdmindto> orders { get; set; }
        public ICollection<SpecialtyDto> SpecialtyDtos { get; set; }
    }
    public class OrderUserAdmindto
    {
        public OrderUserAdmindto()
        {
            this.Jobs=new HashSet<jobsAdminDto>();
        }
        public int Id { get; set; }
        public ICollection<jobsAdminDto> Jobs { get; set; }
    }
    public class jobsAdminDto
    {
        public int Id { get; set; }
        public string Name_Device { get; set; }
        public string Problems { get; set; }
        public string StatusJob { get; set; }
        public bool Reservation { get; set; } = false;
        public string ReservationStatusResult { get; set; }
        public ReadCustomerAdminDto Customer { get; set; }
    }
    public class ReadCustomerAdminDto
    {
        public int Id { get; set; }
        public string FullNameCustomer { get; set; }
        public string PhoneNumber { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }

    }
}
