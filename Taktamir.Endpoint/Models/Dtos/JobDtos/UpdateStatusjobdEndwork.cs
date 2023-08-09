using Taktamir.Endpoint.Models.Dtos.Suppliess;
using Taktamir.framework.Common.JobsUtill;

namespace Taktamir.Endpoint.Models.Dtos.JobDtos
{
    public class UpdateStatusjobdEndwork
    {
        
        public string DescriptionOrder { get; set; }
        public string CodemeliiCustomer { get; set; }
        public bool UsedTokcet { get; set; }
        public double Spent { get; set; }
        public ICollection<SuppliessDto> suppliessDtos { get; set; }
    }
   
}
