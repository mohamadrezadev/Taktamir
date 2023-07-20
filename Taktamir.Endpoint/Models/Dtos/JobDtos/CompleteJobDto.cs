using Taktamir.Endpoint.Models.Dtos.Suppliess;

namespace Taktamir.Endpoint.Models.Dtos.JobDtos
{
    public class CompleteJobDto
    {
       
        public double spent { get; set; }
        public double Total { get; set; }
        public string idenIdentification_code { get; set; }
        public string Description { get; set; }
        public bool usTagged { get; set; }
        public List<CreateSuppliessDto> SuppliessDto { get; set; }
    }
}
